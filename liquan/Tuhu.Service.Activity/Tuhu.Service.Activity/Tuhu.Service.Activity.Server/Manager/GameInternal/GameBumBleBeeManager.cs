using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tuhu.Nosql;
using Tuhu.Service.Activity.DataAccess.Game;
using Tuhu.Service.Activity.DataAccess.Models.Game;
using Tuhu.Service.Activity.Models.Requests;
using Tuhu.Service.Activity.Models.Response;
using Tuhu.Service.Activity.Server.Config;
using Tuhu.Service.Activity.Server.ServiceProxy;
using Tuhu.Service.Activity.Server.Utils;
using Tuhu.Service.Config;
using Tuhu.Service.CreatePromotion.Models;
using Tuhu.Service.Member;
using Tuhu.Service.Order;
using Tuhu.Service.Order.Models;
using Tuhu.Service.Product;
using Tuhu.ZooKeeper;

namespace Tuhu.Service.Activity.Server.Manager.GameInternal
{
    /// <summary>
    ///     大黄蜂游戏
    /// </summary>
    internal class GameBumblebeeManager : DefaultGameManager
    {
        /// <summary>
        ///     游戏
        /// </summary>
        public override int GameVersion { get; } = (int)GameVersionEnum.BUMBLEBEE;

        /// <summary>
        ///     大黄蜂里程
        /// </summary>
        public override string ManagerName { get; } = "大黄蜂集碎片游戏";

        /// <summary>
        /// 用户首次进游戏赠送保养优惠券
        /// </summary>
        public const int FreeMatainCouponID = 1;

        #region 查询

        /// <summary>
        /// 获取排行榜
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async override Task<OperationResult<GetRankListResponse>> GetRankListAsync(GetRankListAsyncRequest request)
        {
            var result = new GetRankListResponse();

            try
            {
                var gameGlobal = await _GetGameGlobalSettingAsync((int)request.GameVersion);

                //分页获取排行榜
                using (var client = CacheHelper.CreateCacheClient(GlobalConstant.GameCacheHeader))
                {
                    var rankCacheKey = $"/{GetRefreshKeyPrefix("GetRankListAsync")}/{GameVersion}/{request.PageIndex}/{ request.PageSize}";

                    var rankListCache = await client.GetOrSetAsync(rankCacheKey,
                        async () => await DalGameUserPointDetail.GetRankListAsync(false, GameVersion,
                                          gameGlobal.RankMinPoint, request.PageIndex, request.PageSize)
                        , TimeSpan.FromMinutes(10));
                    if (rankListCache.Success)
                    {
                        result.RankInfos = rankListCache.Value;
                    }
                    else
                    {
                        result.RankInfos = await DalGameUserPointDetail.GetRankListAsync(true, GameVersion,
                                                 gameGlobal.RankMinPoint, request.PageIndex, request.PageSize);
                        Logger.Warn($"GetRankListAsync => rankListCache 缓存失败:key:{rankListCache.RealKey},Message:{rankListCache.Message}");
                    }
                }

                //用户排名信息
                if (request.UserID != Guid.Empty)
                {
                    result.LoginUserRank = await DalGameUserPointDetail.GetUserRankInfoAsync(GameVersion, request.UserID, true);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"GetRankListAsync异常,userID:{request.UserID.ToString()},activityID:{request.GameVersion}ex:{ex}");
            }

            return OperationResult.FromResult(result);
        }

        /// <summary>
        ///     用户里程收支明细
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public override async Task<GetGameUserDistanceInfoResponse> GetGameUserDistanceInfoAsync(
            GetGameUserDistanceInfoRequest request)
        {
            var result = new GetGameUserDistanceInfoResponse();

            try
            {
                var userId = request.UserId;
                var activityId = (int)request.GameVersion;
                IList<GameUserPointDetailModel> gameUserPointDetailList;

                using (var client = CacheHelper.CreateCacheClient(GlobalConstant.GameCacheHeader))
                {
                    var cacheKey = $"/UserPointDetail/{GetRefreshKeyPrefix($"UserInfo/{request.UserId.ToString().ToUpper()}")}/{request.UserId.ToString().ToUpper()}";
                    var gameUserPointDetailCache = await client.GetOrSetAsync(cacheKey,
                        async () => await DalGameUserPointDetail.GetGameUserPointDetailAsync(false, activityId, userId) ?? new List<GameUserPointDetailModel>(),
                        TimeSpan.FromHours(2));

                    if (gameUserPointDetailCache.Success)
                    {
                        gameUserPointDetailList = gameUserPointDetailCache.Value;
                    }
                    else
                    {
                        gameUserPointDetailList =
                            await DalGameUserPointDetail.GetGameUserPointDetailAsync(true, activityId, userId) ?? new List<GameUserPointDetailModel>();
                        Logger.Warn($"GetGameUserDistanceInfoAsync => gameUserPointDetailCache 缓存失败:key:{gameUserPointDetailCache.RealKey},Message:{gameUserPointDetailCache.Message}");
                    }
                }

                var key = GameCommons.PointTypeDailyShare;
                result.Items.Add(new GetGameUserDistanceInfoResponse.GetGameUserDistanceInfoResponseItem()
                {
                    Key = key,
                    Distance = gameUserPointDetailList.Where(p => p.IsUsed == 0 && p.Status == key).Sum(p => p.Point),
                    Type = 2
                });

                key = GameCommons.PointTypeBuyProduct;
                result.Items.Add(new GetGameUserDistanceInfoResponse.GetGameUserDistanceInfoResponseItem()
                {
                    Key = key,
                    Distance = gameUserPointDetailList.Where(p => p.IsUsed == 0 && p.Status == key).Sum(p => p.Point),
                    Type = 2
                });

                key = GameCommons.PointTypeUserSupport;
                result.Items.Add(new GetGameUserDistanceInfoResponse.GetGameUserDistanceInfoResponseItem()
                {
                    Key = key,
                    Distance = gameUserPointDetailList.Where(p => p.IsUsed == 0 && p.Status == key).Sum(p => p.Point),
                    Type = 2
                });
            }
            catch (Exception e)
            {
                Logger.Error(
                    $"{ManagerName} -> GetGameUserDistanceInfoAsync -> {JsonConvert.SerializeObject(request)} ",
                    e.InnerException ?? e);
            }

            return result;
        }

        /// <summary>
        /// 获取用户游戏信息:碎片 排名 奖品列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public override async Task<GetGameUserInfoResponse> GetGameUserInfoAsync(GetGameUserInfoRequest request)
        {
            var result = new GetGameUserInfoResponse();
            int activityId = GameVersion;

            try
            {
                // 获取用户碎片、排名
                var userRankInfoTask = DalGameUserPointDetail.GetUserRankInfoAsync(GameVersion, request.UserId, true);

                //获取用户奖品列表
                var prizeListTask = GetUserPrizeListAsync(request.UserId, GameVersion);

                await Task.WhenAll(userRankInfoTask, prizeListTask);
                var userRankInfo = userRankInfoTask.Result;
                var prizeList = prizeListTask.Result;

                if (userRankInfo == null)
                {
                    result.Point = 0;
                    result.Rank = 0;
                }
                else
                {
                    result.Point = userRankInfo.Point;
                    result.Rank = userRankInfo.Rank;
                }


                result.PrizeItems = prizeList?.Select(
                    x => new GetGameUserInfoResponsePrizeItems()
                    {
                        PrizeType = x.PrizeType == GameCommons.PrizeTypeCinemaTicket ? GameCommons.PrizeEntityTypeEntity : GameCommons.PrizeEntityTypeCOUPON,
                        PrizeName = x.PrizeName,
                        PrizePic = x.PrizePicUrl,
                        PrizeDesc = x.PrizeDesc,
                        GetTime = x.GetPrizeDate,
                        TodayRank = x.TodayRank
                    }
                    )?.ToList();

                return result;
            }
            catch (Exception e)
            {
                Logger.Error($"{ManagerName} -> GetGameUserInfoAsync -> {JsonConvert.SerializeObject(request)} ",
                    e.InnerException ?? e);
                throw;
            }
        }

        /// <summary>
        /// 获取用户奖品列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="activityId"></param>
        /// <returns></returns>
        private async Task<IList<GameUserPrizeModel>> GetUserPrizeListAsync(Guid userId, int activityId)
        {
            IList<GameUserPrizeModel> prizeList;//奖品列表

            using (var client = CacheHelper.CreateCacheClient(GlobalConstant.GameCacheHeader))
            {
                // 获取用户奖品列表
                var prizeListCacheKey = $"/UserPrizeInfo/{GetRefreshKeyPrefix($"UserInfo/{userId.ToString().ToUpper()}")}/{userId.ToString().ToUpper()}";
                var prizeListCache = await client.GetOrSetAsync(prizeListCacheKey,
                         async () => await DalGameUserPrize.GetGameUserPrizeAsync(false, activityId, userId),
                          TimeSpan.FromHours(1));
                if (prizeListCache.Success)
                {
                    prizeList = prizeListCache.Value;
                }
                else
                {
                    prizeList = await DalGameUserPrize.GetGameUserPrizeAsync(true, activityId, userId);
                    Logger.Warn($"{ManagerName} -> GetUserPrizeListAsync => prizeListCache 缓存失败:key:{prizeListCache.RealKey},Message:{prizeListCache.Message}");
                }
            }

            return prizeList;
        }

        /// <summary>
        ///     获取 用户助力信息【剩余助力次数】
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public override async Task<GetGameUserSupportInfoResponse> GetGameUserSupportInfoAsync(
            GetGameUserSupportInfoRequest request)
        {
            try
            {
                var activityId = (int)request.GameVersion;
                var openId = request.OpenId;

                //获取游戏全局配置
                var settingTask = _GetGameGlobalSettingAsync(activityId);

                //获取openid的助力记录
                var userSupportTask = DalGameMaPaiUserSupport.GetGameMaPaiUserSupportAsync(true, activityId, openId);

                await Task.WhenAll(settingTask, userSupportTask);

                var settingResult = settingTask.Result;
                var userSupportResult = userSupportTask.Result;

                //未超过游戏助力上线
                if (settingResult?.UserSupportMax > userSupportResult.Count)
                {
                    return new GetGameUserSupportInfoResponse
                    {
                        LCount = settingResult.UserSupportMax - userSupportResult.Count
                    };
                }

                return new GetGameUserSupportInfoResponse
                {
                    LCount = 0
                };
            }
            catch (Exception e)
            {
                Logger.Error(
                    $"{ManagerName} -> GetGameUserSupportInfoAsync -> {JsonConvert.SerializeObject(request)} ",
                    e.InnerException ?? e);
                throw;
            }
        }

        /// <summary>
        ///     获取游戏全局设置
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        protected virtual async Task<GameMaPaiGlobalSettingModel> _GetGameGlobalSettingAsync(int activityId)
        {
            try
            {
                var gameMaPaiGlobalSettingModel = await TuhuMemoryCacheNoJson.Instance.GetOrSetAsync(
                    $"GameBumblebeeManager/{activityId}",
                    async () => await DalGameMaPaiGlobalSetting.GetGameMaPaiGlobalSettingAsync(activityId),
                    GlobalConstant.GameTimeSpan);

                return gameMaPaiGlobalSettingModel;
            }
            catch (Exception e)
            {
                Logger.Error($"{ManagerName} -> _GetGameGlobalSettingAsync -> {activityId} ",
                    e.InnerException ?? e);
                throw e;
            }
        }

        /// <summary>
        ///     获取 奖励滚动
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public override async Task<GetGameUserLootBroadcastResponse> GetGameUserLootBroadcastAsync(GetGameUserLootBroadcastRequest request)
        {
            try
            {
                var activityId = (int)request.GameVersion;
                var pageIndex = request.PageIndex;
                var pageSize = request.PageSize;
                var startTime = request.StartTime;

                var cacheKey = $":{activityId}:{pageIndex}:{pageSize}";
                using (var cacheClient = CacheHelper.CreateCacheClient(GlobalConstant.GameCacheHeader))
                {
                    var cacheResult = await cacheClient.GetOrSetAsync(cacheKey
                        , async () =>
                        {
                            var result = new GetGameUserLootBroadcastResponse();

                            // 获取DB数据
                            var gameUserPrizeModels =
                                await DalGameUserPrize.GetGameUserPrizeByTimeAsync(activityId, pageIndex, pageSize, startTime);

                            gameUserPrizeModels?
                                .Select(p =>
                                {
                                    var userResult = MemberServiceProxy.FetchUserByUserId(p.UserId);

                                    var nickName = userResult.Result?.Nickname;
                                    nickName = NickNameHelper.NickNameDataMasking(nickName);

                                    return new GetGameUserLootBroadcastResponse.GetGameUserLootBroadcastResponseItem
                                    {
                                        NickName = nickName,
                                        Date = p.GetPrizeDate.Date,
                                        PrizeName = p.PrizeTitle
                                    };
                                })
                                .ToList()
                                .ForEach(result.Items.Add);

                            return result;
                        }
                        , GlobalConstant.GameTimeSpan);

                    if (cacheResult?.Success ?? false)
                    {
                        return cacheResult?.Value;
                    }
                    else
                    {
                        Logger.Warn(
                        $"{ManagerName} -> GetGameUserLootBroadcastAsync -> cache -> error -> {cacheResult?.Message} {cacheResult?.RealKey} {cacheResult?.Exception}");
                        return new GetGameUserLootBroadcastResponse();
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(
                    $"{ManagerName} -> GetGameUserLootBroadcastAsync -> {JsonConvert.SerializeObject(request)} ",
                    e.InnerException ?? e);
                throw;
            }
        }

        /// <summary>
        ///     获取 用户好友助力信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public override async Task<GetGameUserFriendSupportResponse> GetGameUserFriendSupportAsync(
            GetGameUserFriendSupportRequest request)
        {
            try
            {
                var userId = request.UserId;
                var activityId = GameVersion;

                IList<GameMaPaiUserSupportModel> supportList;

                using (var client = CacheHelper.CreateCacheClient(GlobalConstant.GameCacheHeader))
                {
                    //获取被助力信息
                    var supportListCacheKey = $"/UserSupportList/{GetRefreshKeyPrefix($"UserInfo/{userId.ToString().ToUpper()}")}/{userId.ToString().ToUpper()}";
                    var supportListCache = await client.GetOrSetAsync(supportListCacheKey,
                             async () => await DalGameMaPaiUserSupport.GetGameMaPaiUserSupportAsync(true, activityId, userId),
                              TimeSpan.FromMinutes(10));
                    if (supportListCache.Success)
                    {
                        supportList = supportListCache.Value;
                    }
                    else
                    {
                        supportList = await DalGameMaPaiUserSupport.GetGameMaPaiUserSupportAsync(true, activityId, userId);
                        Logger.Warn($"{ManagerName} -> GetGameUserFriendSupportAsync => supportListCache 缓存失败:key:{supportListCache.RealKey},Message:{supportListCache.Message}");
                    }
                }

                // 定义返回值
                var result = new GetGameUserFriendSupportResponse
                {
                    SupportItems = supportList?
                        .GroupBy(p => p.SupportOpenId)
                        .OrderByDescending(p => p.Count())
                        .Select(p => new GetGameUserFriendSupportResponse.GetGameUserFriendSupportResponseItem
                        {
                            NickName = p.First().SupportNickName,
                            HeadImgURL = p.First().SupportHeadImgURL,
                            SupportCount = p.Count()
                        })
                        .ToList()
                };

                return result;
            }
            catch (Exception e)
            {
                Logger.Error(
                    $"{ManagerName} -> GetGameUserFriendSupportAsync -> {JsonConvert.SerializeObject(request)} ",
                    e.InnerException ?? e);
                throw;
            }
        }

        /// <summary>
        /// 获取用户最近获得的奖品
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public override async Task<OperationResult<GetUserLatestPrizeResponse>> GetUserLatestPrizeAsync(GetUserLatestPrizeRequest request)
        {
            //用户的奖品列表
            var userPrizeLsit = await GetUserPrizeListAsync(request.UserId, (int)request.GameVersion);
            var latestPrize = userPrizeLsit.Where(x => x.PrizeType != GameCommons.PrizeTypeFreeCoupon)?.OrderByDescending(x => x.PKID)?.FirstOrDefault();
            if (latestPrize == null)
            {
                return null;
            }

            var result = new GetUserLatestPrizeResponse()
            {
                PKID = (int)latestPrize.PKID,
                PrizeType = latestPrize.PrizeType == GameCommons.PrizeTypeCinemaTicket ? GameCommons.PrizeEntityTypeEntity : GameCommons.PrizeEntityTypeCOUPON,
                PrizeName = latestPrize.PrizeName,
                PrizePic = latestPrize.PrizePicUrl,
                GetTime = latestPrize.GetPrizeDate,
                TodayRank = latestPrize.TodayRank
            };

            return OperationResult.FromResult(result);
        }

        #endregion

        #region 行为

        /// <summary>
        /// 用户进入游戏初始化 : 记录访问时间、状态 发券
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async override Task<OperationResult<UserParticipateGameResponse>> UserParticipateGameAsync(UserParticipateGameRequest request)
        {
            var result = new UserParticipateGameResponse();
            var couponResult = false;//发券结果
            var initialResult = false;//新建用户信息结果
            var userId = request.UserID;
            int activityId = GameVersion;

            var initialCacheKey = $"/UserParticipateGameAsync/UserInitial/{userId.ToString().ToUpper()}";//初始化用户标识缓存

            try
            {
                // 判断活动是否进行中
                var (errCode, errMsg, isOk) = await _IsGameActive(new GetGameInfoRequest()
                { GameVersion = (GameVersionEnum)GameVersion });
                if (!isOk)
                {
                    return OperationResult.FromResult(result);
                }

                //缓存用户是否进过游戏
                using (var helper = CacheHelper.CreateCacheClient(GlobalConstant.GameCacheHeader))
                {
                    //是否标记进入游戏缓存
                    var cacheInitial = (await helper.GetOrSetAsync(initialCacheKey, async () =>
                    {
                        return (await DalGameUserInfo.GetGameUserInfoAsync(false, GameVersion, userId))?.IsVisit == 1;
                    }, TimeSpan.FromHours(12))
                    ).Value;

                    //获取用户奖品列表
                    var userPrizeList = await GetUserPrizeListAsync(userId, activityId);
                    var hasCoupon = userPrizeList?.Any(x => x.PrizeType == GameCommons.PrizeTypeFreeCoupon);
                    if (cacheInitial && (bool)hasCoupon)
                    {
                        return OperationResult.FromResult(result);
                    }
                }

                using (var zkLock = new ZooKeeperLock($"/GameBumblebeeManager/UserParticipateGameAsync/{userId.ToString().ToUpper()}"))
                {
                    if (await zkLock.WaitAsync(3000))
                    {
                        //查询用户游戏信息
                        var gameUserInfo = await DalGameUserInfo.GetGameUserInfoAsync(false, GameVersion, userId);
                        if (gameUserInfo?.IsVisit != 1)
                        {
                            //初始化用户信息
                            initialResult = (await DalGameUserInfo.InsertGameUserInfoAsync(GameVersion, userId, DateTime.Now)) > 0;
                            if (!initialResult)
                            {
                                Logger.Warn($"UserParticipateGameAsync,大黄蜂初始化用户信息失败:{JsonConvert.SerializeObject(request)}");
                            }
                        }
                        else
                        {
                            initialResult = true;
                        }

                        using (var helper = CacheHelper.CreateCacheClient(GlobalConstant.GameCacheHeader))
                        {
                            var cacheInitial = await helper.SetAsync(initialCacheKey, initialResult, TimeSpan.FromDays(1));
                            if (!cacheInitial.Success)
                            {
                                Logger.Warn($"UserParticipateGameAsync,设置用户初始化标识缓存失败," +
                                    $"cacheInitial:{cacheInitial},message:{cacheInitial.Message}");
                            }
                        }

                        //领取免费券
                        var prizeSetting = await GetTodayPrizeSetting(GameVersion, GameCommons.PrizeTypeFreeCoupon);
                        if (!(prizeSetting?.PKID > 0))
                        {
                            Logger.Warn($"UserParticipateGameAsync => GetTodayPrizeSetting 失败");
                        }

                        //用户奖品列表 :实时获取
                        var userPrizeLsit = await DalGameUserPrize.GetGameUserPrizeAsync(false, activityId, userId);

                        //是否有发送过保养券
                        if (!userPrizeLsit.Any(x => x.PrizeId == prizeSetting?.PKID))
                        {
                            var giveCouponTran = false;

                            using (var dbHelper = DbHelper.CreateDbHelper())
                            {
                                try
                                {
                                    dbHelper.BeginTransaction();

                                    //加奖品记录
                                    var userPrizeModel = new GameUserPrizeModel()
                                    {
                                        UserId = userId,
                                        ActivityId = GameVersion,
                                        Point = 0,
                                        PrizeType = GameCommons.PrizeTypeFreeCoupon,
                                        PrizeName = prizeSetting.PrizeName,
                                        PrizeDesc = prizeSetting.PrizeDesc,
                                        PrizeId = prizeSetting.PKID,
                                        PrizePicUrl = prizeSetting.PrizePicUrl,
                                        IsBroadCastShow = 0,
                                        TodayRank = 0,
                                        GetPrizeDate = DateTime.Now
                                    };
                                    var insertUserPrize = await DalGameUserPrize.InsertGameUserPrizeAsync(dbHelper, userPrizeModel);

                                    if (insertUserPrize > 0)
                                    {
                                        //发券
                                        var createCouponId = await CreatePromotionAsync(userId, activityId, GameCommons.PrizeTypeFreeCoupon, ManagerName);

                                        if (createCouponId > 0)
                                        {
                                            giveCouponTran = true;
                                        }
                                        else
                                        {
                                            Logger.Warn($"UserParticipateGameAsync,CreatePromotionAsync失败:{JsonConvert.SerializeObject(request)}");
                                        }
                                    }
                                    else
                                    {
                                        Logger.Warn($"UserParticipateGameAsync,InsertGameUserPrizeAsync失败:{JsonConvert.SerializeObject(request)}");
                                    }

                                    if (giveCouponTran)
                                    {
                                        dbHelper.Commit();

                                        //刷新缓存
                                        RefreshKeyPrefix($"UserInfo/{userId.ToString().ToUpper()}");
                                        couponResult = true;
                                    }
                                    else
                                    {
                                        dbHelper.Rollback();
                                        Logger.Warn($"UserParticipateGameAsync,Rollback:{JsonConvert.SerializeObject(request)}");
                                    }
                                }
                                catch (Exception)
                                {
                                    dbHelper.Rollback();
                                    couponResult = false;
                                    throw;
                                }
                            }
                        }
                        else
                        {
                            couponResult = true;
                        }

                        if (!couponResult)
                        {
                            Logger.Warn($"UserParticipateGameAsync,UpdateUserIsGetCoupon用户领券失败:{JsonConvert.SerializeObject(request)}");
                        }
                    }
                    else
                    {
                        Logger.Warn($"UserParticipateGameAsync => zcLock超时 ,userID:{userId}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(
                    $"{ManagerName} -> UserParticipateGameAsync -> {JsonConvert.SerializeObject(request)},ex: ",
                    ex.InnerException ?? ex);
            }

            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 发放优惠券
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="activityID"></param>
        /// <param name="prizeType"></param>
        /// <param name="userName"></param>
        private async Task<int> CreatePromotionAsync(Guid userID, int activityID, string prizeType, string userName)
        {
            var result = 0;

            try
            {
                 
                //获取游戏奖品优惠券信息
                var couponInfo = await GetTodayPrizeSetting(activityID, prizeType);

                if (couponInfo == null)
                {
                    Logger.Warn($"CreatePromotionAsync => GetTodayPrizeSetting未获取到游戏奖品信息，userID:{userID}," +
                        $"activityID:{activityID},prizeprizeTypeName:{prizeType}");
                    return result;
                }

                //领取优惠券
                var createPromotionModels = new List<CreatePromotionModel>() {
                            new CreatePromotionModel() {
                                UserID = userID,
                                GetRuleGUID = couponInfo.PrizeId,
                                Author = userName,
                                Channel = userName,
                                Creater = userName
                                }
                         };

                var createPromotionResult =
                    await CreatePromotionServiceProxy.CreatePromotionsNewAsync(createPromotionModels);

                if (!createPromotionResult.Success || !(createPromotionResult.Result?.IsSuccess ?? false))
                {
                    Logger.Warn($"{ManagerName} -> CreatePromotionsNewAsync失败 -> userID:{userID},GameVersion: {GameVersion} ," +
                                $"PrizeId:{couponInfo.PrizeId},ErrorCode:{createPromotionResult.ErrorCode}, {createPromotionResult.ErrorMessage}," +
                                $"{createPromotionResult.Result?.ErrorCode},{createPromotionResult.Result?.ErrorMessage}");
                }
                else
                {
                    result = couponInfo.PKID;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"{ManagerName} -> CreatePromotionAsync异常,userID:{userID},GameVersion:{GameVersion},prizeType:{prizeType},ex:{ex}");
            }

            return result;
        }

        /// <summary>
        ///     用户分享:加碎片记录
        /// </summary>
        /// <param name="request"></param>
        /// <returns>分享获得的积分数</returns>
        public override async Task<(string errCode, string errMsg, GameUserShareResponse response)>
            GameUserShareAsync(GameUserShareRequest request)
        {

            var result = new GameUserShareResponse();

            try
            {
                var activityId = GameVersion;
                var userId = request.UserId;

                // 判断活动是否进行中
                var (errCode, errMsg, isOk) = await _IsGameActive(new GetGameInfoRequest()
                { GameVersion = (GameVersionEnum)GameVersion });

                if (!isOk)
                {
                    return (errCode, errMsg, null);
                }

                // 开启分布式锁
                using (var zklock = new ZooKeeperLock($"/GameBumblebeeManager/GameUserShareAsync/{activityId}/{userId.ToString().ToUpper()}"))
                {
                    if (await zklock.WaitAsync(3000)) //如果锁释放 会立即执行
                    {

                        //用户每天可以通过分享获取一次碎片
                        var getPointResult = await DalGameUserPointDetail.InsertTodayGameUserPointDetailAsync(
                            new GameUserPointDetailModel()
                            {
                                UserId = request.UserId,
                                Status = GameCommons.PointTypeDailyShare,
                                Point = 5,
                                ActivityId = activityId,
                            });
                        if (getPointResult > 0)
                        {
                            result.Point = 5;

                            //刷新排行榜缓存
                            RefreshKeyPrefix("GetRankListAsync");
                            RefreshKeyPrefix($"UserInfo/{request.UserId.ToString().ToUpper()}");
                        }
                        else
                        {
                            result.Point = 0;
                        }
                    }
                    else
                    {
                        Logger.Warn($"{ManagerName} -> GameUserShareAsync,zclock超时,{JsonConvert.SerializeObject(request)}");
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error($"{ManagerName} -> GameUserShareAsync -> {JsonConvert.SerializeObject(request)} ",
                    e.InnerException ?? e);
            }

            return (null, null, result);
        }

        /// <summary>
        ///     帮忙助力:验证是否可助力、助力后加积分、助力记录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public override async Task<(string errCode, string errMsg, GameUserFriendSupportResponse response)>
            GameUserFriendSupportAsync(GameUserFriendSupportRequest request)
        {

            try
            {
                var nowDate = DateTime.Now.Date;
                var openId = request.OpenId;
                var wechatNickName = request.WechatNickName;
                var wechatHeadImg = request.WechatHeadImg;
                var targetUserId = request.TargetUserId;
                var gameVersion = GameVersion;

                // 判断活动是否进行中
                var (errCode, errMsg, isOk) = await _IsGameActive(new GetGameInfoRequest()
                { GameVersion = (GameVersionEnum)gameVersion });

                if (!isOk)
                {
                    return (errCode, errMsg, null);
                }

                if (string.IsNullOrWhiteSpace(openId))
                {
                    return ("-7", Resource.Invalid_Game_ErrorParameter, null);
                }

                //获取游戏配置信息
                var gameGlobalSettingModel = await _GetGameGlobalSettingAsync(gameVersion);

                if (gameGlobalSettingModel == null)
                {
                    return ("-30", Resource.Invalid_Game_ErrorSetting, null);
                }

                using (var dbHelper = DbHelper.CreateDbHelper())
                // 开启分布式锁
                using (var zklock =
                    new ZooKeeperLock($"/GameBumblebeeManager/GameUserFriendSupportAsync/{gameVersion}/{openId}"))
                {
                    if (await zklock.WaitAsync(10000)) //如果锁释放 会立即执行
                    {

                        // 获取openid的助力记录列表
                        var openIDSupportsTask =
                            DalGameMaPaiUserSupport.GetGameMaPaiUserSupportAsync(false, gameVersion, openId);

                        // 获取用户的被助力记录列表
                        var gameUserBeSupportsTask =
                            DalGameMaPaiUserSupport.GetGameMaPaiUserSupportAsync(false, gameVersion, targetUserId);

                        await Task.WhenAll(openIDSupportsTask, gameUserBeSupportsTask);

                        var openIDSupportsResult = openIDSupportsTask.Result;
                        var gameUserBeSupportsResult = gameUserBeSupportsTask.Result;

                        // 判断个人活动期间帮助力总次数
                        if (openIDSupportsResult.Count >= gameGlobalSettingModel.UserSupportMax)
                        {
                            return ("-50", "活动期间助力次数已达上限", null);
                        }

                        // 判断用户今天被助力上限
                        if (gameUserBeSupportsResult.Count(p => p.CreateDatetime.Date == nowDate) >=
                            gameGlobalSettingModel.DailySupportedUserMax)
                        {
                            return ("-60", "该好友今日助力人数已达上线", null);
                        }

                        // 判断openid今日是否助力过该好友
                        if (openIDSupportsResult.Count(p => p.CreateDatetime.Date == nowDate && p.UserId == targetUserId) >=
                            gameGlobalSettingModel.DailySupportOtherUserMax)
                        {
                            return ("-70", "今日已助力过该好友", null);
                        }

                        try
                        {
                            // 开启事务
                            dbHelper.BeginTransaction();

                            // 增加助力记录
                            var insertGameUserSupportResult =
                                await DalGameMaPaiUserSupport.InsertGameMaPaiUserSupportAsync(dbHelper,
                                    new GameMaPaiUserSupportModel
                                    {
                                        ActivityId = gameVersion,
                                        UserId = targetUserId,
                                        SupportOpenId = openId,
                                        SupportNickName = wechatNickName,
                                        SupportHeadImgURL = wechatHeadImg,
                                        Distance = gameGlobalSettingModel.SupportDistance
                                    });

                            // 增加用户积分明细
                            var insertGameUserPointDetailResult =
                                await DalGameUserPointDetail.InsertGameUserPointDetailAsync(dbHelper,
                                    new GameUserPointDetailModel
                                    {
                                        ActivityId = gameVersion,
                                        Point = gameGlobalSettingModel.SupportDistance,
                                        Status = GameCommons.PointTypeUserSupport,
                                        UserId = targetUserId,
                                        IsUsed = 0
                                    });

                            // 判断是否成功
                            if (insertGameUserSupportResult > 0 && insertGameUserPointDetailResult > 0)
                            {
                                // 提交事务
                                dbHelper.Commit();

                                //刷新排行榜缓存\用户数据缓存
                                RefreshKeyPrefix("GetRankListAsync");
                                RefreshKeyPrefix($"UserInfo/{targetUserId.ToString().ToUpper()}");

                                // 返回
                                return (null, null, new GameUserFriendSupportResponse
                                {
                                    Distance = gameGlobalSettingModel.SupportDistance
                                });
                            }
                            else
                            {
                                dbHelper.Rollback();
                                return ("-2", Resource.Invalid_Game_TryAgain, null);
                            }
                        }
                        catch (Exception e)
                        {
                            // 回滚事务
                            dbHelper?.Rollback();
                            throw;
                        }
                    }
                    else
                    {
                        Logger.Warn($"{ManagerName} -> GameUserFriendSupportAsync,zclock超时,{JsonConvert.SerializeObject(request)}");
                    }
                }
                // fail
                return ("-2", Resource.Invalid_Game_TryAgain, null);
            }
            catch (Exception e)
            {
                Logger.Error($"{ManagerName} -> GameUserFriendSupportAsync -> {JsonConvert.SerializeObject(request)} ",
                    e.InnerException ?? e);
                throw;
            }
        }

        /// <summary>
        /// 获取奖品配置
        /// </summary>
        /// <param name="activityID"></param>
        /// <param name="prizeType"></param>
        /// <returns></returns>
        private async Task<GamePrizeSettingModel> GetTodayPrizeSetting(int activityID, string prizeType)
        {

            var gameInfo = await base.GetGameInfoResponseAsync(new GetGameInfoRequest()
            {
                GameVersion = GameVersionEnum.BUMBLEBEE
            });
            var gameStartTime = gameInfo.StartTime;
            //获取是游戏进行的第几天 (当天领昨天的奖励)
            var thisDay = DateTime.Now.Date.Subtract(gameStartTime.Date).Days;

            var couponInfo = new GamePrizeSettingModel();

            using (var client = CacheHelper.CreateCacheClient(GlobalConstant.GameCacheHeader))
            {
                string couponInfoCacheKey = "";
                if (prizeType == GameCommons.PrizeTypeFreeCoupon)
                {
                    couponInfoCacheKey = $"/Bumblebee/CreatePromotionAsync/{activityID}/{prizeType}";
                }
                else
                {
                    couponInfoCacheKey = $"/Bumblebee/CreatePromotionAsync/{activityID}/{prizeType}/{thisDay}";
                }

                var couponInfoCache = await client.GetOrSetAsync(couponInfoCacheKey,
                    async () => await DalGamePrizeSetting.GetGamePrizeSettingAsync(activityID, prizeType, thisDay),
                    TimeSpan.FromMinutes(5));

                if (couponInfoCache.Success)
                {
                    couponInfo = couponInfoCache.Value;
                }
                else
                {
                    couponInfo = await DalGamePrizeSetting.GetGamePrizeSettingAsync(activityID, prizeType, thisDay);
                    Logger.Warn($"{ManagerName} -> GetTodayPrizeSetting => couponInfoCache缓存失败,Message:{couponInfoCache.Message}");
                }
            }

            return couponInfo;
        }

        /// <summary>
        /// 开关
        /// </summary>
        /// <param name="switchStr"></param>
        /// <returns></returns>
        private static bool CheckSwitch(string switchStr)
        {
            var switchFlag = false;
            try
            {
                using (var switchClient = new ConfigClient())
                {
                    var switchResult = switchClient.GetOrSetRuntimeSwitch(switchStr);

                    if (!switchResult.Success)
                    {
                        Logger.Warn($"BumBlebee => CheckIgnoreNewUserSwitch服务调用失败");
                    }
                    else
                    {
                        if (switchResult.Result != null)
                        {
                            switchFlag = (bool)switchResult.Result?.Value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                switchFlag = false;
                Logger.Error($"CheckIgnoreNewUserSwitch服务调用异常,ex:{ex}");
            }
            return switchFlag;
        }

        #endregion

        #region Job、MQ

        /// <summary>
        ///     用户领取奖品
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public override async Task<(string errCode, string errMsg, GameUserLootResponse response)> GameUserLootAsync(
            GameUserLootRequest request)
        {
            var gameVersion = GameVersion;
            var userId = request.UserId;
            var rank = request.PrizeId;//排名 
            var getDate = DateTime.Now.AddDays(-1);//获奖时间

            Logger.Info($"{ManagerName} -> GameUserLootAsync => 用户开始领奖,request:{JsonConvert.SerializeObject(request)}");

            var prizeType = "";//CARMODEL-车模;ENGINEILOIL-机油;CINEMATICKET-电影票;FREECOUPON-免费券

            if (rank >= 1 && rank <= 30)
            {
                prizeType = GameCommons.PrizeTypeCarModel;
            }
            else if (rank >= 31 && rank <= 60)
            {
                prizeType = GameCommons.PrizeTypeCinemaTicket;
            }
            else if (rank >= 61 && rank <= 150)
            {
                prizeType = GameCommons.PrizeTypeEngineOil;
            }

            if (string.IsNullOrWhiteSpace(prizeType))
            {
                Logger.Warn($"{ManagerName} -> GameUserLootAsync =>未获取到奖品类型 {JsonConvert.SerializeObject(request)}");
                return ("-6", "未获取到奖品类型", null);
            }

            try
            {
                var gameSetting = await base.GetGameInfoResponseAsync(new GetGameInfoRequest()
                {
                    GameVersion = GameVersionEnum.BUMBLEBEE
                });

                if (gameSetting == null)
                {
                    Logger.Warn($"{ManagerName} -> GameUserLootAsync => GetGameInfoResponseAsync获取游戏信息失败{JsonConvert.SerializeObject(request)}");
                    return ("-94", "未获取到游戏信息", null);
                }

                if (getDate < gameSetting.StartTime || getDate > gameSetting.EndTime)
                {
                    Logger.Warn($"{ManagerName} -> GameUserLootAsync =>游戏未开始或已结束{JsonConvert.SerializeObject(request)}");
                    return ("-95", "游戏未开始或已结束", null);
                }

                using (var zcLock = new ZooKeeperLock($"/GameBumblebeeManager/GameUserLootAsync/{request.UserId.ToString().ToUpper()}"))
                {
                    if (await zcLock.WaitAsync(6000))
                    {
                        //1.验证用户当日是否领取过奖品
                        var gotPrizeList = await DalGameUserPrize.GetGameUserPrizeAsync(false, gameVersion, userId);
                        if ((bool)gotPrizeList?.Any(x => x.ActivityId == gameVersion && x.PrizeType != GameCommons.PrizeTypeFreeCoupon
                                                    && x.GetPrizeDate.Date == getDate.Date))
                        {
                            //当天领取过奖品
                            Logger.Warn($"{ManagerName} -> GameUserLootAsync,当日领取过奖品,userId:{userId},date:{getDate},rank:{rank}");
                            return (null, null, null);
                        }

                        //2.获取奖品信息
                        var prizeInfo = await GetTodayPrizeSetting(gameVersion, prizeType);

                        if (prizeInfo == null)
                        {
                            Logger.Warn($"{ManagerName} -> GameUserLootAsync => GetTodayPrizeSetting,未获取到奖品信息,userId:{userId},date:{getDate},prizeType:{prizeType}, {JsonConvert.SerializeObject(request)}");
                            return (null, null, null);
                        }

                        //获取积分中奖日及之前的积分数
                        var userPointDetails = await DalGameUserPointDetail.GetGameUserPointDetailAsync(false, gameVersion, userId);
                        var userPoints = userPointDetails?.Where(x => x.IsUsed == 0 && x.CreateDatetime.Date < DateTime.Now.Date)?.Sum(x => x.Point);

                        var emptyPoint = false;//扣积分成功标识

                        //加奖品记录 扣除积分
                        using (var dbHelper = DbHelper.CreateDbHelper())
                        {
                            //开启事务
                            dbHelper.BeginTransaction();

                            try
                            {
                                //加奖品记录
                                var userPrizeModel = new GameUserPrizeModel()
                                {
                                    ActivityId = gameVersion,
                                    UserId = userId,
                                    Point = (int)userPoints,
                                    PrizeId = prizeInfo.PKID,
                                    PrizeType = prizeInfo.PrizeType,
                                    PrizeName = prizeInfo.PrizeName,
                                    PrizePicUrl = prizeInfo.PrizePicUrl,
                                    PrizeTitle = prizeInfo.BroadCastTitle,
                                    PrizeDesc = prizeInfo.PrizeDesc,
                                    PromotionGetRuleGuid = prizeInfo.PrizeId,
                                    IsBroadCastShow = prizeInfo.IsBroadCastShow,
                                    TodayRank = rank,
                                    GetPrizeDate = getDate
                                };
                                var insertUserPrizeTask = DalGameUserPrize.InsertGameUserPrizeAsync(dbHelper, userPrizeModel);

                                //扣除积分
                                var updatePointIsUsedTask = DalGameUserPointDetail.UpdateUserPointIsUsedAsync(dbHelper, gameVersion, userId);

                                //减奖品的当日库存和总库存

                                await Task.WhenAll(insertUserPrizeTask, updatePointIsUsedTask);
                                var insertUserPrize = insertUserPrizeTask.Result;
                                var updatePointIsUsed = updatePointIsUsedTask.Result;

                                if (insertUserPrize > 0 && updatePointIsUsed > 0)
                                {
                                    dbHelper.Commit();
                                    emptyPoint = true;

                                    Logger.Info($"{ManagerName} -> GameUserLootAsync => 用户领奖成功,request:{JsonConvert.SerializeObject(request)},奖品:{JsonConvert.SerializeObject(userPrizeModel)}");
                                    //刷新排行榜\用户信息缓存
                                    RefreshKeyPrefix("GetRankListAsync");
                                    RefreshKeyPrefix($"UserInfo/{userId.ToString().ToUpper()}");
                                }
                                else
                                {
                                    dbHelper.Rollback();
                                    emptyPoint = false;
                                    Logger.Warn($"{ManagerName} -> GameUserLootAsync=> 奖品数据更新失败,insertUserPrize:{insertUserPrize},updatePointIsUsed:{updatePointIsUsed}," +
                                        $" prizeType:{prizeType}, {JsonConvert.SerializeObject(request)}");
                                }
                            }
                            catch (Exception)
                            {
                                dbHelper.Rollback();
                                emptyPoint = false;
                                Logger.Warn($"{ManagerName} -> GameUserLootAsync=> Rollback, prizeType:{prizeType},request: {JsonConvert.SerializeObject(request)}");
                                throw;
                            }
                        }

                        //发券
                        if (emptyPoint && (prizeType == GameCommons.PrizeTypeCarModel || prizeType == GameCommons.PrizeTypeEngineOil))
                        {

                            //如果开关打开 并且不是免费保养券 就发垃圾券
                            var switchResult = CheckSwitch("BumBlebeeNotGiveRealPrize");
                            if (switchResult)
                            {
                                Logger.Info($"{ManagerName} -> GameUserLootAsync 发送垃圾券,{JsonConvert.SerializeObject(request)}");
                                prizeType = GameCommons.PrizeTypeUTTestCoupon;
                            }

                            var createCoupon = await CreatePromotionAsync(userId, GameVersion, prizeType, ManagerName);
                            if (createCoupon <= 0)
                            {
                                //发券失败
                                return ("-5", "奖品发券失败", null);
                            }
                        }
                        if (emptyPoint)
                        {
                            return (null, null, new GameUserLootResponse());
                        }
                        else
                        {
                            return ("-4", "用户领取奖品失败", null);
                        }
                    }
                    else
                    {
                        Logger.Warn($"{ManagerName} -> GameUserLootAsync,zclock超时,领取奖品失败,userId:{userId},date:{getDate},prizeType:{prizeType}");
                    }
                }

                // fail
                return ("-2", Resource.Invalid_Game_TryAgain, null);
            }
            catch (Exception e)
            {
                Logger.Error($"{ManagerName} -> GameUserLootAsync -> {JsonConvert.SerializeObject(request)} ",
                    e.InnerException ?? e);
                throw;
            }
        }

        /// <summary>
        ///    已支付订单跟踪:发放积分
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public override async Task<GameOrderTrackingResponse> GameOrderTrackingAsync(GameOrderTackingRequest request)
        {
            try
            {
                var orderId = request.OrderId;
                var gameVersion = GameVersion;
                var activityId = gameVersion;

                // 活动游戏信息
                var gameActiveTask = GetGameInfoResponseAsync(new GetGameInfoRequest()
                { GameVersion = (GameVersionEnum)gameVersion });

                // 获取游戏配置
                var gameGlobalSettingTask = _GetGameGlobalSettingAsync(gameVersion);

                await Task.WhenAll(gameActiveTask, gameGlobalSettingTask);

                // 判断游戏是否进行中
                var gameInfoResponse = gameActiveTask.Result;

                if (gameInfoResponse == null)
                {
                    return null;
                }

                var now = DateTime.Now;
                if (gameInfoResponse.StartTime > now || gameInfoResponse.EndTime < now)
                {
                    return null;
                }

                // 游戏配置
                var gameGlobalSettingModel = gameGlobalSettingTask.Result;

                if (gameGlobalSettingModel == null)
                {
                    return null;
                }

                //订单信息
                OrderModel order;
                using (var client = new OrderApiForCClient())
                {
                    // 订单信息
                    var orderResult = await client.FetchOrderAndListByOrderIdAsync(orderId);
                    if (orderResult?.Result == null)
                    {
                        orderResult = await client.FetchOrderAndListByOrderIdAsync(orderId, false);
                    }

                    order = orderResult.Result;
                    if (order == null || order.UserId == Guid.Empty || order.SumMoney < 0)
                    {
                        Logger.Warn($"{ManagerName} -> GameOrderTrackingAsync => FetchOrderByOrderIdAsync not found order,orderID:{orderId}");
                        return null;
                    }
                }

                var userId = order.UserId;

                if (order.OrderDatetime < gameInfoResponse.StartTime && order.OrderDatetime > gameInfoResponse.EndTime)
                {
                    Logger.Info($"{ManagerName} -> GameOrderTrackingAsync -> order date fail {order.OrderDatetime} orderId:{orderId}");
                    // 订单日期不对
                    return null;
                }

                //验证订单商品可以获得的积分
                var (points, shellAmount) = await GetBuyProductPoint(order, gameGlobalSettingModel.PayAmountPointRate);
                if (points <= 0)
                {
                    return null;
                }

                // 开启分布式锁
                using (var zkLock = new ZooKeeperLock($"/GameBumblebeeManager/GameOrderTrackingAsync/{gameVersion}/{userId:N}"))
                {
                    if (await zkLock.WaitAsync(3000)) //如果锁释放 会立即执行
                    {

                        //获取用户信息,访问过游戏购买商品才有积分
                        //var userGameInfo = await DalGameUserInfo.GetGameUserInfoAsync(false, activityId, userId);
                        //if (!(userGameInfo?.IsVisit == 1))
                        //{
                        //    Logger.Info($"GameOrderTrackingAsync => 下单前用户未访问游戏 :{orderId},point:{points}");
                        //    return null;
                        //}

                        // 用户积分明细 
                        var gameUserPointDetail =
                            await DalGameUserPointDetail.GetGameUserPointDetailAsync(false, gameVersion, userId);

                        // 判断当前订单已经处理过
                        var isOrderExists =
                            gameUserPointDetail?.Any(p => p.Memo.Contains(orderId.ToString())) ?? false;

                        if (isOrderExists)
                        {
                            Logger.Info(
                                $"BumblebeeGameOrderTrackingAsync -> order exists {order.OrderDatetime} orderId:{orderId}");

                            // 订单已经加过积分
                            return null;
                        }

                        //加积分
                        var addPoiontDetailModel = new GameUserPointDetailModel()
                        {
                            UserId = userId,
                            ActivityId = activityId,
                            Point = points,
                            Status = "商品购买",
                            Memo = $"OrderId:{orderId},shellAmount:{shellAmount}"
                        };

                        var addPointDetailResult = await DalGameUserPointDetail.InsertGameUserPointDetailAsync(addPoiontDetailModel);
                        if (addPointDetailResult <= 0)
                        {
                            Logger.Warn($"{ManagerName} -> GameOrderTrackingAsync => AddPointDetail Fail。userId:{userId},orderId:{orderId},point:{points}");
                        }
                        else
                        {
                            //刷新排行榜缓存
                            RefreshKeyPrefix("GetRankListAsync");
                            RefreshKeyPrefix($"UserInfo/{userId.ToString().ToUpper()}");
                        }
                    }
                    else
                    {
                        Logger.Warn($"{ManagerName} -> GameOrderTrackingAsync -> {JsonConvert.SerializeObject(request)} ");
                    }
                }

                return new GameOrderTrackingResponse();
            }
            catch (Exception e)
            {
                Logger.Error(
                    $"{ManagerName} -> GameOrderTrackingAsync -> {JsonConvert.SerializeObject(request)} ",
                    e.InnerException ?? e);
                throw;
            }
        }

        /// <summary>
        /// 计算订单商品可获取的积分数
        /// </summary>
        /// <param name="order"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        private async Task<(int, decimal)> GetBuyProductPoint(OrderModel order, decimal rate)
        {
            var resultPoint = 0;
            var shellPids = new List<string>();
            int activityId = GameVersion;

            if (rate == 0)
            {
                Logger.Info($"GetBuyProductPoint OrderId:{order.OrderId} ,{order.OrderChannel} rate==0");
                return (0, 0);
            }

            if (order.PayStatus != "2Paid")
            {
                Logger.Info($"GetBuyProductPoint OrderId:{order.OrderId} ,{order.PayStatus} 订单支付状态不对");
                return (0, 0);
            }

            //验证订单渠道
            var thirdChannels = await base._GetThirdPartyOrderChannelAsync();
            if (thirdChannels.Contains(order.OrderChannel))
            {
                Logger.Info($"GetBuyProductPoint OrderId:{order.OrderId} ,{order.OrderChannel} 订单渠道不对");
                return (0, 0);
            }

            //去掉套装商品
            var orderItems = order.OrderListModel?.Where(x => x.PayPrice > 0 && (x.ParentId == null || x.ParentId == 0))?.ToList();
            if (!(orderItems?.Count() > 0))
            {
                Logger.Info($"GetBuyProductPoint OrderId:{order.OrderId} , 未查询到非套装商品");
                return (0, 0);
            }

            //判断是否壳牌商品
            using (var productClient = new ProductClient())
            {
                // 查产品
                var orderPids = orderItems.Select(x => x.Pid).ToList();
                var productQueryResult = await productClient.SelectSkuProductListByPidsAsync(orderPids);

                shellPids = productQueryResult?.Result?.Where(x => x.Brand == "壳牌/Shell")?.Select(x => x.Pid).ToList();
                if (shellPids == null || shellPids.Count == 0)
                {
                    Logger.Info($"GetBuyProductPoint OrderId:{order.OrderId} , 订单中没有壳牌/Shell产品");
                    return (0, 0);
                }
            }

            //可兑换积分的订单商品
            var orderPointItems = orderItems.Where(x => (bool)shellPids?.Contains(x.Pid));

            if (!(orderPointItems?.Count() > 0))
            {
                Logger.Info($"GetBuyProductPoint OrderId:{order.OrderId} , 订单不包含非套装壳牌商品");
                return (0, 0);
            }

            //订单可获取积分的商品
            var orderPointAmount = orderPointItems.Sum(x => x.PayPrice * x.Num);

            //获取的碎片四舍五入
            resultPoint = (int)(orderPointAmount / rate + (decimal)0.5);
            return (resultPoint, orderPointAmount);
        }

        /// <summary>
        /// 获取当日之前的积分排行榜
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public override async Task<OperationResult<GetRankListBeforeDayResponse>> GetRankListBeforeDayAsync(GetRankListBeforeDayRequest request)
        {
            var result = new GetRankListBeforeDayResponse();
            DateTime.TryParse((request.DayTime == null ? DateTime.Now.Date : request.DayTime).ToString(), out DateTime getDate);
            try
            {
                var gameGlobal = await _GetGameGlobalSettingAsync((int)request.GameVersion);

                //分页获取排行榜
                using (var client = CacheHelper.CreateCacheClient(GlobalConstant.GameCacheHeader))
                {
                    var rankCacheKey = $"/{GetRefreshKeyPrefix("LastDayRank")}/{GameVersion}/{getDate}";

                    var rankListCache = await client.GetOrSetAsync(rankCacheKey,
                        async () => await DalGameUserPointDetail.GetRankListBeforeDayAsync(false, GameVersion,
                                          gameGlobal.RankMinPoint, DateTime.Now, 1, 150)
                        , TimeSpan.FromMinutes(10));
                    if (rankListCache.Success)
                    {
                        result.RankList = rankListCache.Value;
                    }
                    else
                    {
                        result.RankList = await DalGameUserPointDetail.GetRankListBeforeDayAsync(true, GameVersion,
                                                 gameGlobal.RankMinPoint, DateTime.Now, 1, 150);
                        Logger.Warn($"GetRankListBeforeDayAsync => rankListCache 缓存失败:key:{rankListCache.RealKey},Message:{rankListCache.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"GetRankListBeforeDayAsync异常,ex:{ex}");
            }

            return OperationResult.FromResult(result);
        }

        #endregion

        #region 小游戏 - 删除游戏的人员数据 - 内部用

        /// <summary>
        ///     小游戏 - 删除游戏的人员数据 - 内部用
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public override async Task<bool> DeleteGameUserDataAsync(DeleteGameUserDataRequest request)
        {
            try
            {
                await base.DeleteGameUserDataAsync(request);
                var gameVersion = GameVersion;
                var userId = request.UserId;
                var openId = request.OpenId;
                // 删除用户相关数据
                using (var dbHelper = DbHelper.CreateDbHelper())
                {
                    await Task.WhenAll(
                        DalGameMaPaiUserSupport.DeleteGameMaPaiUserSupportAsync(dbHelper, gameVersion, userId),
                        DalGameMaPaiUserSupport.DeleteGameMaPaiUserSupportAsync(dbHelper, gameVersion, openId)
                    );
                }

                return true;
            }
            catch (Exception e)
            {
                Logger.Error(
                    $"{ManagerName} -> DeleteGameUserDataAsync -> {JsonConvert.SerializeObject(request)} ",
                    e.InnerException ?? e);
                throw;
            }
        }

        #endregion

        #region 不需实现

        /// <summary>
        ///     获取 里程碑信息
        /// </summary>
        /// <returns></returns>
        public override async Task<GetGameMilepostInfoResponse> GetGameMilepostInfoAsync(GetGameMilepostInfoRequest request)
        {
            await Task.Yield();
            return null;
        }

        #endregion


        /// <summary>
        /// 刷新业务缓存Key前缀
        /// </summary>
        public async void RefreshKeyPrefix(string redisKey)
        {
            try
            {
                redisKey = $"/GameBumblebeeManagerCachePrefix/{redisKey}";

                using (var client = CacheHelper.CreateCacheClient(GlobalConstant.GameCacheHeader))
                {
                    var result = await client.SetAsync(redisKey, DateTime.Now.Ticks + redisKey, TimeSpan.FromDays(1));
                    if (!result.Success)
                    {
                        Logger.Warn($"更新{redisKey}缓存失败", result.Exception);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn($"刷新缓存Key异常;Key={redisKey}", ex);
            }
        }

        /// <summary>
        /// 获取缓存key前缀
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public string GetRefreshKeyPrefix(string redisKey)
        {
            var key = $"/GameBumblebeeManagerCachePrefix/{redisKey}";
            try
            {
                using (var client = CacheHelper.CreateCacheClient(GlobalConstant.GameCacheHeader))
                {
                    var result = client.GetOrSet(key, () => { return DateTime.Now.Ticks + key; }
                    , TimeSpan.FromDays(1));
                    if (!result.Success || string.IsNullOrWhiteSpace(result.Value))
                    {
                        return DateTime.Now.Ticks + redisKey;
                    }
                    else
                    {
                        return result.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn($"GetRefreshKeyPrefix异常;Key={redisKey}", ex);
                return DateTime.Now.Ticks + redisKey;
            }

        }
    }
}
