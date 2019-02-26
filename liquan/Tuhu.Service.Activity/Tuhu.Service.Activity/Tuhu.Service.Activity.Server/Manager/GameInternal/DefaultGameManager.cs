using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using Newtonsoft.Json;
using Tuhu.Nosql;
using Tuhu.Service.Activity.DataAccess;
using Tuhu.Service.Activity.DataAccess.Game;
using Tuhu.Service.Activity.DataAccess.Models.Game;
using Tuhu.Service.Activity.Models.Requests;
using Tuhu.Service.Activity.Models.Response;
using Tuhu.Service.Activity.Server.Config;
using Tuhu.Service.Order;
using Tuhu.ZooKeeper;

namespace Tuhu.Service.Activity.Server.Manager.GameInternal
{
    /// <summary>
    ///     基础游戏业务类
    /// </summary>
    internal abstract class DefaultGameManager
    {
        protected static readonly ILog Logger = LogManager.GetLogger(typeof(DefaultGameManager));

        /// <summary>
        ///     游戏版本
        /// </summary>
        public abstract int GameVersion { get; }

        /// <summary>
        ///     业务名称
        /// </summary>
        public abstract string ManagerName { get; }

        #region 马牌相关

        /// <summary>
        ///     获取 里程碑信息
        /// </summary>
        /// <returns></returns>
        public abstract Task<GetGameMilepostInfoResponse> GetGameMilepostInfoAsync(GetGameMilepostInfoRequest request);

        #endregion

        #region 用户分享

        /// <summary>
        ///     用户分享 会判断是否分享过、每天只会一条记录、可以以后继承重写
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual async Task<(string errCode, string errMsg, GameUserShareResponse response)>
            GameUserShareAsync(
                GameUserShareRequest request)
        {
            try
            {
                var gameVersion = GameVersion;
                var userId = request.UserId;
                // 分布式锁
                using (var zklock =
                    new ZooKeeperLock($"/DefaultGameManager/GameUserShareAsync/{gameVersion}/{userId:N}"))
                {
                    if (await zklock.WaitAsync(5000)) //如果锁释放 会立即执行，正常逻辑不会等待5秒钟
                    {
                        var l = await DalGameUserShare.InsertGameUserShareAsync(new GameUserShareModel
                        {
                            ActivityId = gameVersion,
                            UserId = userId
                        });
                        if (l > 0)
                        {
                            return (null, null, new GameUserShareResponse() { });
                        }

                        return ("-10", Resource.Invalid_Game_AlreadyShare, null);
                    }

                    return ("-6", Resource.Invalid_Game_TryAgain, null);
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

        #region 用户兑换奖品

        /// <summary>
        ///     用户兑换奖品
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public abstract Task<(string errCode, string errMsg, GameUserLootResponse response)> GameUserLootAsync(
            GameUserLootRequest request);

        #endregion

        #region 获取 用户好友助力信息

        /// <summary>
        ///     获取 用户好友助力信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public abstract Task<GetGameUserFriendSupportResponse> GetGameUserFriendSupportAsync(
            GetGameUserFriendSupportRequest request);

        #endregion

        #region 帮忙助力

        /// <summary>
        ///     帮忙助力
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public abstract Task<(string errCode, string errMsg, GameUserFriendSupportResponse response)>
            GameUserFriendSupportAsync(GameUserFriendSupportRequest request);

        #endregion

        #region 用户里程收支明细

        /// <summary>
        ///     用户里程收支明细
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public abstract Task<GetGameUserDistanceInfoResponse> GetGameUserDistanceInfoAsync(
            GetGameUserDistanceInfoRequest request);

        #endregion

        #region 奖励滚动

        /// <summary>
        ///     获取 奖励滚动
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public abstract Task<GetGameUserLootBroadcastResponse> GetGameUserLootBroadcastAsync(
            GetGameUserLootBroadcastRequest request);

        #endregion

        #region 用户助力信息

        /// <summary>
        ///     获取 用户助力信息【剩余助力次数】
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public abstract Task<GetGameUserSupportInfoResponse> GetGameUserSupportInfoAsync(
            GetGameUserSupportInfoRequest request);

        #endregion

        #region 游戏玩家信息

        /// <summary>
        ///     获取 用户信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual async Task<GetGameUserInfoResponse> GetGameUserInfoAsync(GetGameUserInfoRequest request)
        {
            try
            {
                var gameVersion = GameVersion;

                // 获取用户信息
                var taskUserInfo = DalGameUserInfo.GetGameUserInfoAsync(true, gameVersion, request.UserId);

                // 获取用户已领取奖品
                var taskUserPrizeList = DalGameUserPrize.GetGameUserPrizeAsync(true, gameVersion, request.UserId);

                await Task.WhenAll(taskUserInfo, taskUserPrizeList);

                var result = new GetGameUserInfoResponse();

                result.Point = taskUserInfo.Result?.Point ?? 0;

                result.PrizeItems = taskUserPrizeList
                    .Result?
                    .Select(p => new GetGameUserInfoResponsePrizeItems
                    {
                        PrizeName = p.PrizeName,
                        PrizePic = p.PrizePicUrl,
                        PrizeDesc = p.PrizeDesc,
                        PrizeTitle = p.PrizeTitle,
                        PrizeStartTime = p.PrizeStartTime,
                        PrizeEndTime = p.PrizeEndTime
                    })
                    .ToList();


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
        ///     获取或者新增一个玩家
        /// </summary>
        /// <returns></returns>
        protected virtual async Task<GameUserInfoModel> _GetOrSetGameUserInfoModelAsync(int gameVersion, Guid userId)
        {
            // 先获取一下玩家数据
            var userData = await DalGameUserInfo.GetGameUserInfoAsync(true, gameVersion, userId);
            var now = DateTime.Now;
            if (userData == null)
            {
                // 创建一个玩家数据
                var l = await DalGameUserInfo.InsertGameUserInfoAsync(gameVersion, userId, now);
                if (l > 0)
                {
                    return new GameUserInfoModel
                    {
                        PKID = l,
                        ActivityId = gameVersion,
                        UserId = userId,
                        CreateDatetime = now,
                        LastUpdateDateTime = now
                    };
                }

                // 走写库试试
                userData = await DalGameUserInfo.GetGameUserInfoAsync(false, gameVersion, userId);
                if (userData != null)
                {
                    return userData;
                }

            }

            return userData;
        }

        #endregion

        #region 游戏信息

        /// <summary>
        ///     私有 获取 游戏信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<GetGameInfoResponse> _GetGameInfoResponseAsync(GetGameInfoRequest request)
        {
            var dbResult = await DalActivity.GetActivityByTypeId(GameVersion);
            return new GetGameInfoResponse
            {
                ActivityName = dbResult.ActivityName,
                StartTime = dbResult.StartTime,
                EndTime = dbResult.EndTime,
                GameRuleText = dbResult.GameRuleText,
                SupportRuleText = dbResult.SupportRuleText
            };
        }

        /// <summary>
        ///     判断活动是否正在运行
        /// </summary>
        /// <returns></returns>
        protected async Task<(string errCode, string errMsg, bool isOk)> _IsGameActive(GetGameInfoRequest request)
        {
            var getGameInfoResponse = await GetGameInfoResponseAsync(new GetGameInfoRequest()
            {
                GameVersion = request.GameVersion
            });

            // 判断活动是否结束
            if (getGameInfoResponse == null)
            {
                return ("-30", Resource.Invalid_Game_ErrorSetting, false);
            }

            var now = DateTime.Now;
            if (getGameInfoResponse.StartTime > now || getGameInfoResponse.EndTime < now)
            {
                return ("-90", Resource.Invalid_Game_ActivityRunning, false);
            }

            return (null, null, true);
        }

        /// <summary>
        ///     获取第三方渠道
        /// </summary>
        /// <returns></returns>
        protected async Task<List<string>> _GetThirdPartyOrderChannelAsync()
        {
            try
            {
                using (var orderClient = new OrderQueryClient())
                {
                    var fetchResult = await orderClient.Get3rdOrderChannelAsync();
                    fetchResult.ThrowIfException();
                    return fetchResult?.Result?.ToList();
                }
            }
            catch (Exception e)
            {
                Logger.Error($" {nameof(DefaultGameManager)} -> {nameof(_GetThirdPartyOrderChannelAsync)} ");
                throw;
            }
        }

        /// <summary>
        ///     获取 游戏信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual async Task<GetGameInfoResponse> GetGameInfoResponseAsync(GetGameInfoRequest request)
        {
            try
            {
                using (var cacheClient = CacheHelper.CreateCacheClient(GlobalConstant.GameCacheHeader))
                {
                    var result = await cacheClient.GetOrSetAsync(
                        $"{nameof(GetGameInfoResponseAsync)}/{(int) request.GameVersion}"
                        , async () => await _GetGameInfoResponseAsync(request)
                        , GlobalConstant.GameTimeSpan);

                    // 返回数据
                    return result?.Value;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"{ManagerName} -> GetGameInfoResponseAsync -> {GameVersion} ",
                    e.InnerException ?? e);
                throw;
            }
        }


        /// <summary>
        ///     获取 游戏信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual async Task<GetGameInfoResponse> GetGameInfoResponseNoCacheAsync(GetGameInfoRequest request)
        {
            return await _GetGameInfoResponseAsync(request);
        }

        #endregion

        #region 订单跟踪

        /// <summary>
        ///     小游戏 - 订单状态跟踪
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual Task<GameOrderTrackingResponse> GameOrderTrackingAsync(GameOrderTackingRequest request)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region 更新游戏信息

        /// <summary>
        ///     更新游戏信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<UpdateGameInfoResponse> UpdateGameInfoAsync(UpdateGameInfoRequest request)
        {
            try
            {
                var dbResult = await DalActivity.GetActivityByTypeId(GameVersion);

                using (var dbHelper = DbHelper.CreateDbHelper())
                {
                    dbResult.StartTime = request.StartTime;
                    dbResult.EndTime = request.EndTime;
                    await DalActivity.UpdateActivtyAsync(dbHelper, dbResult);
                }

                return new UpdateGameInfoResponse();
            }
            catch (Exception e)
            {
                Logger.Error(
                    $"{ManagerName} -> UpdateGameInfoAsync -> {JsonConvert.SerializeObject(request)} ",
                    e.InnerException ?? e);
                throw;
            }
        }

        #endregion

        #region 小游戏 - 删除游戏的人员数据 - 内部用

        /// <summary>
        ///     小游戏 - 删除游戏的人员数据 - 内部用
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteGameUserDataAsync(DeleteGameUserDataRequest request)
        {
            try
            {
                var gameVersion = GameVersion;
                var userId = request.UserId;
                // 删除用户相关数据
                using (var dbHelper = DbHelper.CreateDbHelper())
                {
                    await Task.WhenAll(
                        DalGameUserInfo.DeleteGameUserInfoAsync(dbHelper, gameVersion, userId),
                        DalGameUserPointDetail.DeleteGameUserPointDetailAsync(dbHelper, gameVersion, userId),
                        DalGameUserPrize.DeleteGameUserPrizeAsync(dbHelper, gameVersion, userId),
                        DalGameUserShare.DeleteGameUserShareAsync(dbHelper, gameVersion, userId)
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

        /// <summary>
        /// 获取游戏实时排行榜
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public abstract Task<OperationResult<GetRankListResponse>> GetRankListAsync(GetRankListAsyncRequest request);

        /// <summary>
        /// 用户进入游戏,初始化数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public abstract Task<OperationResult<UserParticipateGameResponse>> UserParticipateGameAsync(UserParticipateGameRequest request);

        /// <summary>
        /// 获取用户最近获得的奖品
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public abstract Task<OperationResult<GetUserLatestPrizeResponse>> GetUserLatestPrizeAsync(GetUserLatestPrizeRequest request);

        /// <summary>
        /// 获取某天之前的积分排行
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public abstract Task<OperationResult<GetRankListBeforeDayResponse>> GetRankListBeforeDayAsync(GetRankListBeforeDayRequest request);

    }
}
