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
using Tuhu.Service.CreatePromotion.Models;
using Tuhu.Service.Member;
using Tuhu.Service.Order;
using Tuhu.Service.Product;
using Tuhu.ZooKeeper;

namespace Tuhu.Service.Activity.Server.Manager.GameInternal
{
    /// <summary>
    ///     马牌游戏
    /// </summary>
    internal class GameMaPaiManager : DefaultGameManager
    {
        /// <summary>
        ///     游戏
        /// </summary>
        public override int GameVersion { get; } = (int) GameVersionEnum.MAPAI;

        /// <summary>
        ///     马牌里程
        /// </summary>
        public override string ManagerName { get; } = "马牌里程游戏";

        #region 马牌全局设置

        /// <summary>
        ///     获取 查询马牌全局设置
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        protected virtual async Task<GameMaPaiGlobalSettingModel> _GetGameMaPaiGlobalSettingAsync(int activityId)
        {
            try
            {
                var gameMaPaiGlobalSettingModel = await TuhuMemoryCacheNoJson.Instance.GetOrSetAsync(
                    $"GameMaPaiManager/{activityId}",
                    async () => await DalGameMaPaiGlobalSetting.GetGameMaPaiGlobalSettingAsync(activityId),
                    GlobalConstant.GameTimeSpan);

                return gameMaPaiGlobalSettingModel;
            }
            catch (Exception e)
            {
                Logger.Error($"{ManagerName} -> _GetGameMaPaiGlobalSettingAsync -> {activityId} ",
                    e.InnerException ?? e);
                throw e;
            }
        }

        #endregion

        #region 用户信息

        /// <summary>
        ///     获取 【马牌】用户信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public override async Task<GetGameUserInfoResponse> GetGameUserInfoAsync(GetGameUserInfoRequest request)
        {
            try
            {
                var basedata = await base.GetGameUserInfoAsync(request) ?? new GetGameUserInfoResponse();

                basedata.Distance = (int) basedata.Point;


                return basedata;
            }
            catch (Exception e)
            {
                Logger.Error($"{ManagerName} -> GetGameUserInfoAsync -> {JsonConvert.SerializeObject(request)} ",
                    e.InnerException ?? e);
                throw;
            }
        }

        #endregion

        #region 里程碑信息

        /// <summary>
        ///     增加当天库存量
        /// </summary>
        /// <param name="prizeSettingModel"></param>
        /// <param name="incr"></param>
        /// <returns></returns>
        private long _IncrDayLCountByCache(GameMaPaiPrizeSettingModel prizeSettingModel, int incr)
        {
            // 缓存
            using (var cacheClient = CacheHelper.CreateCounterClient(GlobalConstant.GameCacheHeader))
            {
                var result = cacheClient.Increment($":prizedaylcount:city:{prizeSettingModel.PKID}", incr);
                return result.Value;
            }
        }

        /// <summary>
        ///     私有：获取当天剩余数量 走缓存
        /// </summary>
        /// <param name="prizeSettingModel"></param>
        /// <param name="reload"></param>
        /// <returns></returns>
        private async Task<long> _GetDayLCountByCacheAsync(GameMaPaiPrizeSettingModel prizeSettingModel,
            bool reload = false)
        {
            var cacheKey = $":prizedaylcount:{prizeSettingModel.PKID}";

            var isReadOnly = true;

            // 缓存
            using (var cacheClient = CacheHelper.CreateCacheClient(GlobalConstant.GameCacheHeader))
            {
                if (reload)
                {
                    await cacheClient.RemoveAsync(cacheKey);
                    isReadOnly = false;
                }

                var cacheResult = await cacheClient.GetOrSetAsync(cacheKey
                    , async () =>
                    {
                        if (prizeSettingModel.LCount == -1 && prizeSettingModel.Count == -1 && prizeSettingModel.DayCount == -1)
                        {
                            return -1;
                        }

                        if (prizeSettingModel.DayCount > 0)
                        {
                            var dayCount =
                                await DalGameUserPrize.GetGameUserPrizeByPrizeIdDateAsync(isReadOnly,
                                    prizeSettingModel.PKID,
                                    DateTime.Now);
                            var lcount = prizeSettingModel.DayCount - dayCount;

                            return lcount < 0 ? 0 : lcount;
                        }

                        return -1;
                    }
                    , GlobalConstant.GameTimeSpan);

                if (!cacheResult.Success)
                {
                    Logger.Warn(
                        $"{ManagerName} -> _GetDayLCountAsync -> cache error -> {cacheResult.Message} -> {cacheResult.Exception}");
                }

                return cacheResult.Value;
            }
        }

        /// <summary>
        ///     私有：获取 里程碑信息 走缓存
        /// </summary>
        /// <param name="gameVersion"></param>
        /// <param name="reload"></param>
        /// <returns></returns>
        private async Task<GetGameMilepostInfoResponse> _GetGameMilepostInfoByCacheAsync(
            int gameVersion, bool reload = false)
        {
            var cacheKey = $":gamemilepostinfo:{gameVersion}";

            var isReadOnly = true;


            // 缓存
            using (var cacheClient = CacheHelper.CreateCacheClient(GlobalConstant.GameCacheHeader))
            {
                if (reload)
                {
                    isReadOnly = false;
                    await cacheClient.RemoveAsync(cacheKey);
                }

                var cacheResult = await cacheClient.GetOrSetAsync(cacheKey, async () =>
                {
                    var result = new GetGameMilepostInfoResponse();

                    // 马牌城市设置
                    var dbResultTask = DalGameMaPaiMilepostSetting.GetGameMaPaiMilepostSettingAsync(GameVersion);
                    // 马牌奖品设置
                    var prizeDbResultTask =
                        DalGameMaPaiPrizeSetting.GetGameMaPaiPrizeSettingAsync(isReadOnly, GameVersion);

                    await Task.WhenAll(dbResultTask, prizeDbResultTask);

                    var dbResult = dbResultTask.Result;

                    var prizeDbResult = prizeDbResultTask.Result;

                    var getGameMilepostInfoResponseItems = new List<GetGameMilepostInfoResponseItem>();

                    if (dbResult != null)
                    {
                        foreach (var gameMaPaiMilepostSettingModel in dbResult)
                        {
                            var returnValue = new GetGameMilepostInfoResponseItem
                            {
                                Distance = gameMaPaiMilepostSettingModel.Distance,
                                MilepostId = gameMaPaiMilepostSettingModel.PKID,
                                MilepostName = gameMaPaiMilepostSettingModel.MilepostName,
                                MilepostPicUrl = gameMaPaiMilepostSettingModel.MilepostPicUrl
                            };
                            var prize = prizeDbResult?.FirstOrDefault(x =>
                                x.MaPaiMilepostSettingId == gameMaPaiMilepostSettingModel.PKID);
                            if (prize != null)
                            {
                                returnValue.PrizeId = prize.PKID;
                                returnValue.PrizeName = prize.PrizeName;
                                returnValue.PrizePicUrl = prize.PrizePicUrl;
                                returnValue.Count = prize.Count;
                                returnValue.LCount = prize.LCount;
                                returnValue.DayCount = prize.DayCount;
                                // 获取当前剩余数量
                                var dayLCount = await _GetDayLCountByCacheAsync(prize, reload);
                                returnValue.DayLCount = (int) dayLCount;
                            }

                            getGameMilepostInfoResponseItems.Add(returnValue);
                        }
                    }

                    result.Items = getGameMilepostInfoResponseItems;

                    return result;
                }, GlobalConstant.GameTimeSpan);

                if (!cacheResult.Success)
                {
                    Logger.Warn(
                        $"{ManagerName} -> _GetGameMilepostInfoByCacheAsync -> cache error -> {cacheResult.Message} -> {cacheResult.Exception}");
                }

                return cacheResult.Value;
            }
        }

        /// <summary>
        ///     获取 里程碑信息
        /// </summary>
        /// <returns></returns>
        public override async Task<GetGameMilepostInfoResponse> GetGameMilepostInfoAsync(
            GetGameMilepostInfoRequest request)
        {
            try
            {
                // 用户ID
                var userId = request.UserId;

                // 获取数据
                var cacheResult = await _GetGameMilepostInfoByCacheAsync(GameVersion);

                // 当前用户已领取奖品
                var userPrizeTask = DalGameUserPrize.GetGameUserPrizeAsync(true, GameVersion, userId);


                await Task.WhenAll(userPrizeTask);


                var userPrize = userPrizeTask.Result;

                cacheResult?.Items?.ForEach(item =>
                {
                    // 用户已兑换
                    if (userPrize?.Any(x => x.PrizeId == item.PrizeId) ?? false)
                    {
                        item.PrizeStatus = 2;
                    }
                    // 已抢光
                    else if (item.DayLCount == 0 || item.LCount == 0)
                    {
                        item.PrizeStatus = 3;
                    }
                    // 可兑换
                    else
                    {
                        item.PrizeStatus = 1;
                    }
                });

                return cacheResult;
            }
            catch (Exception e)
            {
                Logger.Error($"{ManagerName} -> GetGameMilepostInfoAsync -> {JsonConvert.SerializeObject(request)} ",
                    e.InnerException ?? e);
                throw;
            }
        }

        #endregion

        #region 用户分享

        /// <summary>
        ///     用户分享【马牌】
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public override async Task<(string errCode, string errMsg, GameUserShareResponse response)>
            GameUserShareAsync(GameUserShareRequest request)
        {
            try
            {
                var activityId = GameVersion;
                var userId = request.UserId;

                // 判断活动是否进行中
                var (errCode, errMsg, isOk) = await _IsGameActive(new GetGameInfoRequest()
                    {GameVersion = (GameVersionEnum)GameVersion});

                if (!isOk)
                {
                    return (errCode, errMsg, null);
                }

                // 开启分布式锁
                using (var zklock = new ZooKeeperLock($"/GameMaPaiManager/GameUserShareAsync/{activityId}/{userId:N}"))
                {
                    if (await zklock.WaitAsync(3000)) //如果锁释放 会立即执行
                    {
                        // 先调用父类方法
                        var taskUserShareResult = base.GameUserShareAsync(request);

                        // 获取马牌游戏全局设置
                        var taskGlobalSetting = _GetGameMaPaiGlobalSettingAsync(activityId);

                        // 获取游戏用户信息
                        var taskGameUserInfo = _GetOrSetGameUserInfoModelAsync(activityId, userId);

                        // 等待`异步`方法执行完成
                        await Task.WhenAll(taskGlobalSetting, taskUserShareResult, taskGameUserInfo);

                        var (errorCode, errorMsg, result) = taskUserShareResult.Result;

                        // 已经分享过
                        if (errorCode == "-10")
                        {
                            return (null, null, new GameUserShareResponse() { });
                        }

                        if (result == null)
                        {
                            return (errorCode, errorMsg, null);
                        }


                        var gameUserInfo = taskGameUserInfo.Result;

                        if (taskGlobalSetting.Result == null)
                        {
                            return ("-30", Resource.Invalid_Game_ErrorSetting, null);
                        }

                        if (gameUserInfo == null)
                        {
                            return ("-40", Resource.Invalid_Game_ErrorUser, null);
                        }

                        var globalSetting = taskGlobalSetting.Result;


                        // 分享成功 Todo

                        // 更新用户里程数据
                        var updateUserInfoResult = await DalGameUserInfo.UpdateGameUserInfoPointAsync(
                            gameUserInfo.PKID,
                            globalSetting.ShareDistance);

                        if (updateUserInfoResult)
                        {
                            result.Point = globalSetting.ShareDistance;
                            result.Distance = result.Point;
                            // 更新用户里程明细
                            await DalGameUserPointDetail.InsertGameUserPointDetailAsync(new GameUserPointDetailModel
                            {
                                ActivityId = activityId,
                                Point = globalSetting.ShareDistance,
                                UserId = userId,
                                Status = "每日分享"
                            });

                            return (null, null, response: result);
                        }
                    }

                    return ("-2", Resource.Invalid_Game_TryAgain, null);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"{ManagerName} -> GameUserShareAsync -> {JsonConvert.SerializeObject(request)} ",
                    e.InnerException ?? e);
                throw;
            }
        }

        #endregion

        #region 用户领取奖品

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
            var prizeId = request.PrizeId;

            try
            {
                // 判断活动是否进行中
                var (errCode, errMsg, isOk) = await _IsGameActive(new GetGameInfoRequest()
                    {GameVersion = (GameVersionEnum)gameVersion});

                if (!isOk)
                {
                    return (errCode, errMsg, null);
                }

                // 获取产品信息
                var prizeInfoTask = DalGameMaPaiPrizeSetting.GetGameMaPaiPrizeSettingByPkidAsync(prizeId);

                // 获取产品对应优惠券信息
                var prizeCouponTask = DalGameMaPaiCouponSetting.GetGameMaPaiCouponSettingAsync(prizeId);

                // 获取用户信息
                var userInfoTask = _GetOrSetGameUserInfoModelAsync(gameVersion, userId);

                await Task.WhenAll(prizeInfoTask, userInfoTask, prizeCouponTask);

                var prizeInfo = prizeInfoTask.Result;
                var userInfo = userInfoTask.Result;
                var prizeCouponList = prizeCouponTask.Result;



                if (userInfo == null)
                {
                    return ("-40", Resource.Invalid_Game_ErrorUser, null);
                }

                if (prizeInfo == null)
                {
                    // 返回数据
                    return ("-15", Resource.Invalid_Game_ErrorPrize, null);
                }

                if (prizeInfo.Point > userInfo.Point)
                {
                    return ("-20", Resource.Invalid_Game_ErrorPoint, null);
                }

                // 获取产品对应的城市
                var gameMaPaiMilepostSettingModel = await DalGameMaPaiMilepostSetting.GetGameMaPaiMilepostSettingByIdAsync(prizeInfo.MaPaiMilepostSettingId);

                
                // 判断一下是不是同一个游戏
                if (prizeInfo.ActivityId != userInfo.ActivityId)
                {
                    return ("-9", Resource.Invalid_Game_Illegal, null);
                }

                // 开启分布式锁
                using (var zklock = new ZooKeeperLock($"/GameMaPaiManager/GameUserLootAsync/{gameVersion}/{userId:N}"))
                {
                    if (await zklock.WaitAsync(10000)) //如果锁释放 会立即执行
                    {
                        using (var sqlClient = DbHelper.CreateDbHelper())
                        {
                            bool incrDayLCountFlag = false;


                            var rollBack = new Action(() =>
                            {
                                Logger.Info(
                                    $"GameMaPaiManager -> GameUserLootAsync -> rollback -> {userId} {prizeId} ");
                                if (incrDayLCountFlag)
                                {
                                    _IncrDayLCountByCache(prizeInfo, -1);
                                }

                                sqlClient?.Rollback();
                            });

                            try
                            {
                                // 开启事务
                                sqlClient.BeginTransaction();

                                // 判断用户是否领取过
                                var prizeList =
                                    await DalGameUserPrize.GetGameUserPrizeAsync(false, gameVersion, userId);
                                if (prizeList.Any(p => p.PrizeId == prizeId))
                                {
                                    rollBack();
                                    // 返回数据
                                    return ("-60", Resource.Invalid_Game_AlreadyUserPrizeExists, null);
                                }


                                // 减当天的库存
                                if (prizeInfo.DayCount != -1)
                                {
                                    // 尝试占库存
                                    var lCount = _IncrDayLCountByCache(prizeInfo, 1);

                                    // 没有库存了
                                    if (lCount > prizeInfo.DayCount)
                                    {
                                        return ("-10", Resource.Invalid_Game_NoPrize, null);
                                    }

                                    incrDayLCountFlag = true;
                                    Logger.Info(
                                        $"GameMaPaiManager -> GameUserLootAsync -> _IncrDayLCountByCache -> {userId} {prizeId} -> {lCount} ");
                                }


                                // 减剩余库存
                                bool updatePrizeCountResult;
                                if (prizeInfo.LCount != -1)
                                {
                                    // 减库存
                                    updatePrizeCountResult =
                                        await DalGameMaPaiPrizeSetting.UpdateGameMaPaiPrizeSettingCountAsync(sqlClient,
                                            prizeInfo.PKID, -1);
                                }
                                else
                                {
                                    updatePrizeCountResult = true;
                                }


                                // 减用户数据
                                var updateUserCountResult =
                                    await DalGameUserInfo.UpdateGameUserInfoPointAsync(sqlClient, userInfo.PKID,
                                        -prizeInfo.Point);

                                if (!updatePrizeCountResult)
                                {
                                    rollBack();
                                    // fail
                                    return ("-10", Resource.Invalid_Game_NoPrize, null);
                                }

                                if (!updateUserCountResult)
                                {
                                    rollBack();
                                    // fail
                                    return ("-45", Resource.Invalid_Game_ErrorPoint, null);
                                }

                                var createPromotionModels = prizeCouponList.Select(p => new CreatePromotionModel
                                {
                                    UserID = userId,
                                    GetRuleGUID = p.PromotionGetRuleGuid,
                                    Author = ManagerName,
                                    Channel = ManagerName,
                                    Creater = ManagerName
                                }).ToList();


                                // 尝试发券
                                var createPromotionResult =
                                    await CreatePromotionServiceProxy.CreatePromotionsNewAsync(createPromotionModels);

                                // 用户奖品明细
                                var gameUserPrize = new GameUserPrizeModel
                                {
                                    UserId = userId,
                                    ActivityId = gameVersion,
                                    Point = prizeInfo.Point,
                                    PrizeName = prizeInfo.PrizeName,
                                    PrizeId = prizeId,
                                    PrizePicUrl = prizeInfo.PrizePicUrl,
                                    //                                    PromotionGetRuleGuid = prizeInfo.PromotionGetRuleGuid,
                                    IsBroadCastShow = prizeInfo.IsBroadCastShow
                                };

                                // 校验发券结果
                                if (createPromotionResult.Success && (createPromotionResult.Result?.IsSuccess ?? false))
                                {
                                    gameUserPrize.PromotionCode = createPromotionResult.Result.PromotionCode;
                                    gameUserPrize.PromotionId = createPromotionResult.Result.PromotionId;

                                    if (gameUserPrize.PromotionId == null || gameUserPrize.PromotionId == 0)
                                    {
                                        gameUserPrize.PromotionId =
                                            createPromotionResult.Result?.promotionIds?.FirstOrDefault();
                                    }

                                    using (var client = new PromotionClient())
                                    {
                                        // 优惠券规则
                                        var couponRuleResult = await client.GetCouponRulesAsync(new List<Guid>
                                        {
                                            prizeCouponList.FirstOrDefault()?.PromotionGetRuleGuid ?? Guid.Empty
                                        });

                                        var couponRule = couponRuleResult?.Result?.FirstOrDefault();
                                        if (couponRule != null)
                                        {
                                            if (couponRule.ValiStartDate == null)
                                            {
                                                gameUserPrize.PrizeStartTime = DateTime.Today;
                                                var end = DateTime.Today.AddDays(couponRule.Term ?? 0);
                                                if (end < couponRule.ValiEndDate)
                                                {
                                                    end = couponRule.ValiEndDate.Value;
                                                }

                                                gameUserPrize.PrizeEndTime = end;
                                            }
                                            else
                                            {
                                                gameUserPrize.PrizeStartTime = couponRule.ValiStartDate;
                                                gameUserPrize.PrizeEndTime = couponRule.ValiEndDate;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    Logger.Warn(
                                        $"GameMaPaiManager -> GameUserLootAsync -> CreatePromotionNewAsync -> fail -> {userId} {gameVersion} {prizeId} {createPromotionResult.ErrorCode} {createPromotionResult.ErrorMessage} {createPromotionResult.Result?.ErrorCode} {createPromotionResult.Result?.ErrorMessage}");

                                    // 判断是否被抢光
                                    if ((createPromotionResult.ErrorMessage?.Contains("已抢光") ?? false) ||
                                        (createPromotionResult.Result?.ErrorMessage?.Contains("已抢光") ?? false))
                                    {
                                        rollBack();
                                        return ("-10", Resource.Invalid_Game_NoPrize, null);
                                    }

                                    rollBack();
                                    // fail
                                    return ("-50", Resource.Invalid_Game_ErrorSendPrize, null);
                                }


                                await Task.WhenAll(
                                    // 保存用户已领取记录
                                    DalGameUserPrize.InsertGameUserPrizeAsync(sqlClient, gameUserPrize)

                                    // 保存用户消费记录
                                    , DalGameUserPointDetail.InsertGameUserPointDetailAsync(sqlClient,
                                        new GameUserPointDetailModel
                                        {
                                            ActivityId = gameVersion,
                                            UserId = userId,
                                            Point = -prizeInfo.Point,
                                            Status = gameMaPaiMilepostSettingModel.MilepostName +"奖品"
                                        })
                                );


                                // 提交事务
                                sqlClient.Commit();

                                // 更新每日剩余数量Cache
                                await _GetGameMilepostInfoByCacheAsync(gameVersion, true);

                                return (null, null, new GameUserLootResponse());
                            }
                            catch (Exception e)
                            {
                                rollBack();
                                throw;
                            }
                        }
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

        #endregion

        #region 获取 用户好友助力信息

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

                // 查询助力信息
                var gameGameMaPaiUserSupportModels =
                    await DalGameMaPaiUserSupport.GetGameMaPaiUserSupportAsync(true, activityId, userId);

                // 定义返回值
                var result = new GetGameUserFriendSupportResponse
                {
                    SupportItems = gameGameMaPaiUserSupportModels?
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

        #endregion

        #region 帮忙助力

        /// <summary>
        ///     帮忙助力
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public override async Task<(string errCode, string errMsg, GameUserFriendSupportResponse response)>
            GameUserFriendSupportAsync(
                GameUserFriendSupportRequest request)
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
                    {GameVersion = (GameVersionEnum)gameVersion});

                if (!isOk)
                {
                    return (errCode, errMsg, null);
                }


                if (string.IsNullOrWhiteSpace(openId))
                {
                    return ("-7", Resource.Invalid_Game_ErrorParameter, null);
                }

                var gameMaPaiGlobalSettingModel =
                    await DalGameMaPaiGlobalSetting.GetGameMaPaiGlobalSettingAsync(gameVersion);

                if (gameMaPaiGlobalSettingModel == null)
                {
                    return ("-30", Resource.Invalid_Game_ErrorSetting, null);
                }

                using (var dbHelper = DbHelper.CreateDbHelper())
                    // 开启分布式锁
                using (var zklock =
                    new ZooKeeperLock($"/GameMaPaiManager/GameUserFriendSupportAsync/{gameVersion}/{openId}"))
                {
                    if (await zklock.WaitAsync(10000)) //如果锁释放 会立即执行
                    {
                        // 获取游戏用户
                        var userInfo = await _GetOrSetGameUserInfoModelAsync(gameVersion, targetUserId);
                        if (userInfo == null)
                        {
                            return ("-40", Resource.Invalid_Game_ErrorUser, null);
                        }

                        // 当前OPENID信息 Task
                        var gameGameMaPaiUserSupportOpenIdModelsTask =
                            DalGameMaPaiUserSupport.GetGameMaPaiUserSupportAsync(false, gameVersion,
                                openId);

                        // 当前USERID信 Task
                        var gameGameMaPaiUserSupportUserIdModelsTask =
                            DalGameMaPaiUserSupport.GetGameMaPaiUserSupportAsync(false, gameVersion,
                                targetUserId);

                        await Task.WhenAll(gameGameMaPaiUserSupportOpenIdModelsTask,
                            gameGameMaPaiUserSupportUserIdModelsTask);

                        // 当前OPENID信息
                        var gameGameMaPaiUserSupportOpenIdModels = gameGameMaPaiUserSupportOpenIdModelsTask.Result;

                        // 当前USERID信
                        var gameGameMaPaiUserSupportUserIdModels = gameGameMaPaiUserSupportUserIdModelsTask.Result;


                        // 判断剩余`总`次数
                        if (gameGameMaPaiUserSupportOpenIdModels.Count >= gameMaPaiGlobalSettingModel.UserSupportMax)
//                        if (gameGameMaPaiUserSupportOpenIdModels.Count >= 5)
                        {
                            return ("-50", Resource.Invalid_Game_SupportMax, null);
                        }

                        // 判断当前用户`今天`被助力上限
                        if (gameGameMaPaiUserSupportUserIdModels.Count(p => p.CreateDatetime.Date == nowDate) >=
                            gameMaPaiGlobalSettingModel.DailySupportedUserMax)
//                        if (gameGameMaPaiUserSupportUserIdModels.Count(p => p.CreateDatetime.Date == nowDate) >=
//                                5)
                        {
                            return ("-50", Resource.Invalid_Game_NoSupportedToday, null);
                        }

                        // 判断剩余`今天`次数
                        if (gameGameMaPaiUserSupportOpenIdModels.Count(p => p.CreateDatetime.Date == nowDate && p.UserId == targetUserId) >=
                            gameMaPaiGlobalSettingModel.DailySupportOtherUserMax)
                        {
                            return ("-50", Resource.Invalid_Game_NoSupportToday, null);
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
                                        Distance = gameMaPaiGlobalSettingModel.SupportDistance
                                    });


                            // 增加用户积分
                            var updateUserInfoResult = await DalGameUserInfo.UpdateGameUserInfoPointAsync(dbHelper,
                                userInfo.PKID,
                                gameMaPaiGlobalSettingModel.SupportDistance);

                            // 增加用户积分明细
                            var insertGameUserPointDetailResult =
                                await DalGameUserPointDetail.InsertGameUserPointDetailAsync(dbHelper,
                                    new GameUserPointDetailModel
                                    {
                                        ActivityId = gameVersion,
                                        Point = gameMaPaiGlobalSettingModel.SupportDistance,
                                        Status = "好友助力",
                                        UserId = targetUserId
                                    });


                            // 判断是否成功
                            if (insertGameUserSupportResult > 0
                                && updateUserInfoResult
                                && insertGameUserPointDetailResult > 0)

                            {
                                // 提交事务
                                dbHelper.Commit();

                                // 重新设置用户缓存

                                // 返回
                                return (null, null, new GameUserFriendSupportResponse
                                {
                                    Distance = gameMaPaiGlobalSettingModel.SupportDistance
                                });
                            }

                            dbHelper.Rollback();
                            // fail
                            return ("-2", Resource.Invalid_Game_TryAgain, null);
                        }
                        catch (Exception e)
                        {
                            // 回滚事务
                            dbHelper?.Rollback();
                            throw;
                        }
                    }
                }


                // fail
                return ("-2",Resource.Invalid_Game_TryAgain, null);
            }
            catch (Exception e)
            {
                Logger.Error($"{ManagerName} -> GameUserFriendSupportAsync -> {JsonConvert.SerializeObject(request)} ",
                    e.InnerException ?? e);
                throw;
            }
        }

        #endregion

        #region 用户里程收支明细

        /// <summary>
        ///     用户里程收支明细
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public override async Task<GetGameUserDistanceInfoResponse> GetGameUserDistanceInfoAsync(
            GetGameUserDistanceInfoRequest request)
        {
            try
            {
                var userId = request.UserId;
                var activityId = (int) request.GameVersion;
                // 返回代码
                var result = new GetGameUserDistanceInfoResponse();

                var gameUserPointDetailModels =
                    await DalGameUserPointDetail.GetGameUserPointDetailAsync(true, activityId, userId) ?? new List<GameUserPointDetailModel>();

                {

                    var key = "每日分享";
                    result.Items.Add(new GetGameUserDistanceInfoResponse.GetGameUserDistanceInfoResponseItem()
                    {
                        Key = key,
                        Distance = gameUserPointDetailModels.Where(p=>p.Status==key).Sum(p=>p.Point),
                        Type = 2
                    });

                    key = "购买商品";
                    result.Items.Add(new GetGameUserDistanceInfoResponse.GetGameUserDistanceInfoResponseItem()
                    {
                        Key = key,
                        Distance = gameUserPointDetailModels.Where(p=>p.Status==key).Sum(p=>p.Point),
                        Type = 2
                    });

                    key = "好友助力";
                    result.Items.Add(new GetGameUserDistanceInfoResponse.GetGameUserDistanceInfoResponseItem()
                    {
                        Key = key,
                        Distance = gameUserPointDetailModels.Where(p=>p.Status==key).Sum(p=>p.Point),
                        Type = 2
                    });

                    // 检索小于0
                   var  points = gameUserPointDetailModels
                        .Where(p => p.Point < 0)
                        .OrderByDescending(p => p.PKID)
                        .Select(p => new GetGameUserDistanceInfoResponse.GetGameUserDistanceInfoResponseItem
                        {
                            Key = p.Status,
                            Distance = p.Point,
                            Type = 1
                        }).ToList();

                    // 添加
                    points?.ForEach(result.Items.Add);
                }

                return result;
            }
            catch (Exception e)
            {
                Logger.Error(
                    $"{ManagerName} -> GetGameUserDistanceInfoAsync -> {JsonConvert.SerializeObject(request)} ",
                    e.InnerException ?? e);
                throw;
            }
        }

        #endregion

        #region 奖励滚动

        /// <summary>
        ///     获取 奖励滚动
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public override async Task<GetGameUserLootBroadcastResponse> GetGameUserLootBroadcastAsync(
            GetGameUserLootBroadcastRequest request)
        {
            try
            {
                var activityId = (int) request.GameVersion;
                var pageIndex = request.PageIndex;
                var pageSize = request.PageSize;

                if (pageSize > 100)
                {
                    return new GetGameUserLootBroadcastResponse();
                }

                var cacheKey = $":{activityId}:{pageIndex}:{pageSize}";
                using (var cacheClient = CacheHelper.CreateCacheClient(GlobalConstant.GameCacheHeader))
                {
                    var cacheResult = await cacheClient.GetOrSetAsync(cacheKey
                        , async () =>
                        {
                            var result = new GetGameUserLootBroadcastResponse();

                            // 获取DB数据
                            var gameUserPrizeModels =
                                await DalGameUserPrize.GetGameUserPrizeAsync(activityId, pageIndex, pageSize, 2);


                            gameUserPrizeModels?
                                .Select(p =>
                                {
                                    var userResult = MemberServiceProxy.FetchUserByUserId(p.UserId);

                                    var nickName = userResult.Result?.Nickname;
                                    nickName = NickNameHelper.NickNameDataMasking(nickName);

                                    return new GetGameUserLootBroadcastResponse.GetGameUserLootBroadcastResponseItem
                                    {
                                        NickName = nickName,
                                        Date = p.CreateDatetime.Date,
                                        PrizeName = p.PrizeName
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

                    Logger.Warn(
                        $"{ManagerName} -> GetGameUserLootBroadcastAsync -> cache -> error -> {cacheResult?.Message} {cacheResult?.RealKey} {cacheResult?.Exception}");
                    return new GetGameUserLootBroadcastResponse();
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

        #endregion

        #region 用户助力信息

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
                var activityId = (int) request.GameVersion;
                var openId = request.OpenId;

                var settingTask = _GetGameMaPaiGlobalSettingAsync(activityId);
                var userSupportTask = DalGameMaPaiUserSupport.GetGameMaPaiUserSupportAsync(true, activityId, openId);

                await Task.WhenAll(settingTask, userSupportTask);

                var setting = settingTask.Result;
                var userSupport = userSupportTask.Result;


                if (setting?.UserSupportMax > userSupport.Count)
                {
                    return new GetGameUserSupportInfoResponse
                    {
                        LCount = setting.UserSupportMax - userSupport.Count
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

        #endregion

        #region 订单跟踪

        /// <summary>
        ///     小游戏 - 订单状态跟踪
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public override async Task<GameOrderTrackingResponse> GameOrderTrackingAsync(GameOrderTackingRequest request)
        {
            try
            {
                var orderId = request.OrderId;
                var gameVersion = GameVersion;


                // 验证请求
                if (!ValidateOrder(request))
                {
                    Logger.Info(
                        $"{ManagerName} -> GameOrderTrackingAsync -> orderId:{request.OrderId} {gameVersion} {request.InstallShopId} {request.DeliveryStatus} {request.InstallStatus} -> 不匹配");
                    return null;
                }

                // 判断活动是否进行中
                var gameActiveTask = GetGameInfoResponseAsync(new GetGameInfoRequest()
                    {GameVersion = (GameVersionEnum)gameVersion});

                // 获取配置
                var gameMaPaiGlobalSettingTask = _GetGameMaPaiGlobalSettingAsync(gameVersion);

                // 等待任务完成
                await Task.WhenAll(gameActiveTask, gameMaPaiGlobalSettingTask);

                // 判断活动是否进行中
                var gameInfoResponse = gameActiveTask.Result;

                // 判断活动是否结束
                if (gameInfoResponse == null)
                {
                    return null;
                }

                var now = DateTime.Now;
                if (gameInfoResponse.StartTime > now || gameInfoResponse.EndTime < now)
                {
                    return null;
                }

                // 获取配置
                var gameMaPaiGlobalSettingModel = gameMaPaiGlobalSettingTask.Result;

                if (gameMaPaiGlobalSettingModel == null)
                {
                    return null;
                }


                using (var client = new OrderApiForCClient())
                {
                    // 订单
                    var orderResult = await client.FetchOrderByOrderIdAsync(orderId);
                    if (orderResult?.Result == null)
                    {
                        orderResult = await client.FetchOrderByOrderIdAsync(orderId, false);
                    }

                    var order = orderResult.Result;


                    if (order == null || order.UserId == Guid.Empty || order.SumMoney < 0)
                    {
                        return null;
                    }

                    var userId = order.UserId;

                    // 开启分布式锁
                    using (var zkLock =
                        new ZooKeeperLock($"/GameMaPaiManager/GameOrderTrackingAsync/{gameVersion}/{userId:N}"))
                    {
                        if (await zkLock.WaitAsync(3000)) //如果锁释放 会立即执行
                        {
                            if (order.OrderDatetime < gameInfoResponse.StartTime &&
                                order.OrderDatetime > gameInfoResponse.EndTime)
                            {
                                Logger.Info(
                                    $"{ManagerName} -> GameOrderTrackingAsync -> order date faill {order.OrderDatetime} orderId:{orderId}");

                                // 订单日期不对
                                return null;
                            }


                            // 小游戏 - 用户 Task
                            var gameUserInfoTask = _GetOrSetGameUserInfoModelAsync(gameVersion, order.UserId);

                            // 小游戏 - 用户积分明细 Task
                            var gameUserPointDetailTask =
                                DalGameUserPointDetail.GetGameUserPointDetailAsync(false, gameVersion, userId);

                            // 渠道 Task
                            var thirdPartyOrderChannelTask = _GetThirdPartyOrderChannelAsync();

                            // 订单明细 Task
                            var fetchOrderAndListByOrderIdTask = client.FetchOrderAndListByOrderIdAsync(orderId);

                            await Task.WhenAll(gameUserInfoTask
                                , gameUserPointDetailTask
                                , thirdPartyOrderChannelTask,
                                fetchOrderAndListByOrderIdTask);

                            // 小游戏 - 用户
                            var gameUserInfoModel = gameUserInfoTask.Result;

                            const string buyStatus = "购买商品";

                            // 小游戏 - 用户已经在这获得的积分数量
                            var gameUserPointDetailModels =
                                gameUserPointDetailTask.Result?.Where(p => p.Status == buyStatus).ToList();

                            // 小游戏 - 判断当前订单已经处理过
                            var isOrderExists =
                                gameUserPointDetailTask.Result?.Any(p => p.Memo.Contains(orderId.ToString())) ?? false;

                            if (isOrderExists)
                            {

                                Logger.Info(
                                    $"{ManagerName} -> GameOrderTrackingAsync -> order exists {order.OrderDatetime} orderId:{orderId}");

                                // 订单已经存在
                                return null;
                            }


                            // 订单
                            var orderDetailResult = fetchOrderAndListByOrderIdTask.Result;
                            if (orderDetailResult?.Result?.OrderListModel == null)
                            {
                                orderDetailResult = await client.FetchOrderAndListByOrderIdAsync(orderId, false);
                            }

                            // 渠道
                            var thirdPartyOrderChannel = thirdPartyOrderChannelTask.Result;

                            if (thirdPartyOrderChannel.Contains(order.OrderChannel))
                            {
                                Logger.Info(
                                    $"{ManagerName} -> GameOrderTrackingAsync -> {order.OrderId} {order.OrderChannel} 渠道不对");
                                return null;
                            }

                            var orderDetailList = orderDetailResult
                                .Result?
                                .OrderListModel?
                                //.Where(p => p.ParentId == 0 || p.ParentId == null)
                                .ToList();


                            // 判断产品类型
                            var pidList = orderDetailList?
                                .Select(p => p.Pid)
                                .ToList();

                            if (pidList == null)
                            {
                                Logger.Info(
                                    $"{ManagerName} -> GameOrderTrackingAsync -> Pid -> {order.OrderId} {order.OrderChannel} 订单中没有对应产品");
                                return null;
                            }

                            using (var productClient = new ProductClient())
                            {
                                // 查产品
                                var productQueryResult = await productClient.SelectSkuProductListByPidsAsync(pidList);

                                // 判断产品类型
                                pidList = productQueryResult
                                    .Result?
                                    .Where(p =>
                                        p.Brand == "德国马牌/Continental" &&
                                        // 轮胎
                                        (p.Pid?.ToLower().StartsWith("tr-") ?? false)
                                    )
                                    .Select(p => p.Pid)
                                    .ToList();


                                // 马牌 轮胎 GO ->
                                if (pidList != null && pidList.Count > 0)
                                {
                                    // PID 和 orderDetail Join 出来
                                    orderDetailList = orderDetailList.Where(p => pidList.Contains(p.Pid)).ToList();

                                    // 判断是否是打包产品
                                    var packOrderParentIdList = orderDetailList
                                        .Where(p =>
                                                  orderDetailList.Select(x => x.OrderListId).Contains(p.ParentId ?? 0))
                                        .Select(p=>p.ParentId??0)
                                        .ToList();

                                    // 过滤打包父产品
                                    orderDetailList = orderDetailList
                                        .Where(p => !packOrderParentIdList.Contains(p.OrderListId)).ToList();

                                    // 判断订单里面的轮胎数量
                                    var tireCount = orderDetailList.Sum(p => p.Num);

                                    // 一条轮胎积分
                                    const int tirePerPoint = 1000;

                                    // 用户在轮胎上总积分
                                    const int tireUserMaxPoint = 4000;

                                    // 用户当前积分
                                    var tireUserPoint = gameUserPointDetailModels?.Sum(p => p.Point) ?? 0;


                                    // 计算积分
                                    var point =  tireCount * tirePerPoint;
                                    if (point > tireUserMaxPoint)
                                    {
                                        point = tireUserMaxPoint;
                                    }

                                    if ((tireUserPoint + point) > tireUserMaxPoint)
                                    {
                                        point = tireUserMaxPoint - tireUserPoint;
                                    }


                                    using (var sqlClient = DbHelper.CreateDbHelper())
                                    {
                                        // 开启事务
                                        sqlClient.BeginTransaction();

                                        // 增加积分
                                        var updateGameUserInfoPointResult =
                                            await DalGameUserInfo.UpdateGameUserInfoPointAsync(sqlClient,
                                                gameUserInfoModel.PKID, point);

                                        // 增加积分明细
                                        var insertGameUserPointDetailResult =
                                            await DalGameUserPointDetail.InsertGameUserPointDetailAsync(sqlClient,
                                                new GameUserPointDetailModel()
                                                {
                                                    ActivityId = gameVersion,
                                                    Point = point,
                                                    Status = buyStatus,
                                                    UserId = userId,
                                                    Memo = $"订单 {orderId}"
                                                });

                                        if (updateGameUserInfoPointResult && insertGameUserPointDetailResult > 0)
                                        {
                                            // 提交事务
                                            sqlClient.Commit();
                                        }
                                        else
                                        {
                                            // 回滚
                                            sqlClient.Rollback();
                                        }


                                    }
                                }
                                else
                                {
                                    Logger.Info(
                                        $"{ManagerName} -> GameOrderTrackingAsync -> {order.OrderId} {order.OrderChannel} 订单中没有对应产品");
                                }
                            }
                        }
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

        private bool ValidateOrder(GameOrderTackingRequest order)
        {
            if ((order.InstallShopId > 0 && order.InstallStatus == "2Installed")) //订单完成  到店订单 和 订单已安装
            {
                return true;
            }

            return false;
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
                        DalGameMaPaiUserSupport.DeleteGameMaPaiUserSupportAsync(dbHelper,gameVersion,userId),
                        DalGameMaPaiUserSupport.DeleteGameMaPaiUserSupportAsync(dbHelper,gameVersion,openId)
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
        /// 获取排行榜
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public override Task<OperationResult<GetRankListResponse>> GetRankListAsync(GetRankListAsyncRequest request)
        {
            return null;
        }

        /// <summary>
        /// 用户进入游戏初始化
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public override Task<OperationResult<UserParticipateGameResponse>> UserParticipateGameAsync(UserParticipateGameRequest request)
        {
            return null;
        }

        public override Task<OperationResult<GetUserLatestPrizeResponse>> GetUserLatestPrizeAsync(GetUserLatestPrizeRequest request)
        {
            return null;
        }

        public override Task<OperationResult<GetRankListBeforeDayResponse>> GetRankListBeforeDayAsync(GetRankListBeforeDayRequest request)
        {
            return null;
        }

        #endregion


    }
}
