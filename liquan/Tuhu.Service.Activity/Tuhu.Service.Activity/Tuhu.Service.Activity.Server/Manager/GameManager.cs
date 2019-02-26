using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using log4net.Repository.Hierarchy;
using Newtonsoft.Json;
using Tuhu.Service.Activity.Models.Requests;
using Tuhu.Service.Activity.Models.Response;
using Tuhu.Service.Activity.Server.Manager.GameInternal;

namespace Tuhu.Service.Activity.Server.Manager
{
    /// <summary>
    ///     小游戏业务类
    /// </summary>
    public static class GameManager
    {
        /// <summary>
        ///     获取 游戏信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<OperationResult<GetGameInfoResponse>> GetGameInfoResponseAsync(
            GetGameInfoRequest request)
        {
            var manager = GameManagerFactory.GetGameManager(request.GameVersion);
            if (manager == null)
            {
                // 未实现
                return OperationResult.FromError<GetGameInfoResponse>("-3", Resource.Invalid_Game_NotImpl);
            }

            return OperationResult.FromResult(await manager.GetGameInfoResponseAsync(request));
        }


        /// <summary>
        ///     获取 里程碑信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<OperationResult<GetGameMilepostInfoResponse>> GetGameMilepostInfoAsync(
            GetGameMilepostInfoRequest request)
        {
            var manager = GameManagerFactory.GetGameManager(request.GameVersion);
            if (manager == null)
            {
                // 未实现
                return OperationResult.FromError<GetGameMilepostInfoResponse>("-3", Resource.Invalid_Game_NotImpl);
            }

            return OperationResult.FromResult(await manager.GetGameMilepostInfoAsync(request));
        }


        /// <summary>
        ///     获取 用户信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<OperationResult<GetGameUserInfoResponse>> GetGameUserInfoAsync(
            GetGameUserInfoRequest request)
        {
            try
            {
                var manager = GameManagerFactory.GetGameManager(request.GameVersion);
                if (manager == null)
                {
                    // 未实现
                    return OperationResult.FromError<GetGameUserInfoResponse>("-3", Resource.Invalid_Game_NotImpl);
                }

                return OperationResult.FromResult(await manager.GetGameUserInfoAsync(request));
            }
            catch (NotImplementedException e)
            {
                // 未实现
                return OperationResult.FromError<GetGameUserInfoResponse>("-3", Resource.Invalid_Game_NotImpl);
            }
        }

        /// <summary>
        ///     用户分享
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<OperationResult<GameUserShareResponse>> GameUserShareAsync(
            GameUserShareRequest request)
        {
            try
            {
                var manager = GameManagerFactory.GetGameManager(request.GameVersion);
                if (manager == null)
                {
                    // 未实现
                    return OperationResult.FromError<GameUserShareResponse>("-3", Resource.Invalid_Game_NotImpl);
                }

                var result = await manager.GameUserShareAsync(request);
                if (string.IsNullOrWhiteSpace(result.errCode) && result.response != null)
                {
                    return OperationResult.FromResult(result.response);
                }

                return OperationResult.FromError<GameUserShareResponse>(result.errCode, result.errMsg);
            }
            catch (NotImplementedException e)
            {
                // 未实现
                return OperationResult.FromError<GameUserShareResponse>("-3", Resource.Invalid_Game_NotImpl);
            }
        }

        /// <summary>
        ///     用户领取奖品
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<OperationResult<GameUserLootResponse>> GameUserLootAsync(GameUserLootRequest request)
        {
            try
            {
                var manager = GameManagerFactory.GetGameManager(request.GameVersion);
                if (manager == null)
                {
                    // 未实现
                    return OperationResult.FromError<GameUserLootResponse>("-3", Resource.Invalid_Game_NotImpl);
                }

                var result = await manager.GameUserLootAsync(request);
                if (string.IsNullOrWhiteSpace(result.errCode) && result.response != null)
                {
                    return OperationResult.FromResult(result.response);
                }

                return OperationResult.FromError<GameUserLootResponse>(result.errCode, result.errMsg);
            }
            catch (NotImplementedException e)
            {
                // 未实现
                return OperationResult.FromError<GameUserLootResponse>("-3", Resource.Invalid_Game_NotImpl);
            }
        }

        /// <summary>
        ///     帮忙助力
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<OperationResult<GetGameUserFriendSupportResponse>> GetGameUserFriendSupportAsync(
            GetGameUserFriendSupportRequest request)
        {
            try
            {
                var manager = GameManagerFactory.GetGameManager(request.GameVersion);
                if (manager == null)
                {
                    // 未实现
                    return OperationResult.FromError<GetGameUserFriendSupportResponse>("-3", Resource.Invalid_Game_NotImpl);
                }

                var result = await manager.GetGameUserFriendSupportAsync(request);
                return OperationResult.FromResult(result);
            }
            catch (NotImplementedException e)
            {
                // 未实现
                return OperationResult.FromError<GetGameUserFriendSupportResponse>("-3", Resource.Invalid_Game_NotImpl);
            }
        }


        /// <summary>
        ///     帮忙助力
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<OperationResult<GameUserFriendSupportResponse>> GameUserFriendSupportAsync(
            GameUserFriendSupportRequest request)
        {
            try
            {
                var manager = GameManagerFactory.GetGameManager(request.GameVersion);
                if (manager == null)
                {
                    // 未实现
                    return OperationResult.FromError<GameUserFriendSupportResponse>("-3", Resource.Invalid_Game_NotImpl);
                }

                var result = await manager.GameUserFriendSupportAsync(request);
                if (string.IsNullOrWhiteSpace(result.errCode) && result.response != null)
                {
                    return OperationResult.FromResult(result.response);
                }

                return OperationResult.FromError<GameUserFriendSupportResponse>(result.errCode, result.errMsg);
            }
            catch (NotImplementedException e)
            {
                // 未实现
                return OperationResult.FromError<GameUserFriendSupportResponse>("-3", Resource.Invalid_Game_NotImpl);
            }
        }

        /// <summary>
        ///     获取 用户里程收支明细
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<OperationResult<GetGameUserDistanceInfoResponse>> GetGameUserDistanceInfoAsync(
            GetGameUserDistanceInfoRequest request)
        {
            try
            {
                var manager = GameManagerFactory.GetGameManager(request.GameVersion);
                if (manager == null)
                {
                    // 未实现
                    return OperationResult.FromError<GetGameUserDistanceInfoResponse>("-3", Resource.Invalid_Game_NotImpl);
                }

                var result = await manager.GetGameUserDistanceInfoAsync(request);
                return OperationResult.FromResult(result);
            }
            catch (NotImplementedException e)
            {
                // 未实现
                return OperationResult.FromError<GetGameUserDistanceInfoResponse>("-3", Resource.Invalid_Game_NotImpl);
            }
        }


        /// <summary>
        ///     获取 奖励滚动
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<OperationResult<GetGameUserLootBroadcastResponse>> GetGameUserLootBroadcastAsync(
            GetGameUserLootBroadcastRequest request)
        {
            try
            {
                var manager = GameManagerFactory.GetGameManager(request.GameVersion);
                if (manager == null)
                {
                    // 未实现
                    return OperationResult.FromError<GetGameUserLootBroadcastResponse>("-3", Resource.Invalid_Game_NotImpl);
                }

                var result = await manager.GetGameUserLootBroadcastAsync(request);
                return OperationResult.FromResult(result);
            }
            catch (NotImplementedException e)
            {
                // 未实现
                return OperationResult.FromError<GetGameUserLootBroadcastResponse>("-3", Resource.Invalid_Game_NotImpl);
            }
        }


        /// <summary>
        ///     获取 用户助力信息【剩余助力次数】
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<OperationResult<GetGameUserSupportInfoResponse>> GetGameUserSupportInfoAsync(
            GetGameUserSupportInfoRequest request)
        {
            try
            {
                var manager = GameManagerFactory.GetGameManager(request.GameVersion);
                if (manager == null)
                {
                    // 未实现
                    return OperationResult.FromError<GetGameUserSupportInfoResponse>("-3", Resource.Invalid_Game_NotImpl);
                }

                var result = await manager.GetGameUserSupportInfoAsync(request);
                return OperationResult.FromResult(result);
            }
            catch (NotImplementedException e)
            {
                // 未实现
                return OperationResult.FromError<GetGameUserSupportInfoResponse>("-3", Resource.Invalid_Game_NotImpl);
            }
        }

        /// <summary>
        ///     小游戏 - 订单状态跟踪
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<OperationResult<GameOrderTrackingResponse>> GameOrderTrackingAsync(GameOrderTackingRequest request)
        {
            try
            {
                var gameVersion = request.GameVersion;

                IList<DefaultGameManager> gameManagers;
                if (gameVersion == GameVersionEnum.None)
                {
                    gameManagers = GameManagerFactory.GetAllGameManagers();
                }
                else
                {
                    gameManagers = new List<DefaultGameManager>()
                    {
                        GameManagerFactory.GetGameManager(gameVersion)
                    };
                }

                foreach (var defaultGameManager in gameManagers)
                {
                    try
                    {
                        await defaultGameManager.GameOrderTrackingAsync(request);
                    }
                    catch (NotImplementedException e)
                    {

                    }
                }

                return OperationResult.FromResult(new GameOrderTrackingResponse());
            }
            catch (NotImplementedException e)
            {
                // 未实现
                return OperationResult.FromError<GameOrderTrackingResponse>("-3", Resource.Invalid_Game_NotImpl);
            }
        }


        /// <summary>
        ///    更新游戏信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static async Task<OperationResult<UpdateGameInfoResponse>> UpdateGameInfoAsync(UpdateGameInfoRequest request)
        {
            try
            {
                var manager = GameManagerFactory.GetGameManager(request.GameVersion);
                if (manager == null)
                {
                    // 未实现
                    return OperationResult.FromError<UpdateGameInfoResponse>("-3", Resource.Invalid_Game_NotImpl);
                }

                var result = await manager.UpdateGameInfoAsync(request);
                return OperationResult.FromResult(result);
            }
            catch (NotImplementedException e)
            {
                // 未实现
                return OperationResult.FromError<UpdateGameInfoResponse>("-3", Resource.Invalid_Game_NotImpl);
            }
        }

        /// <summary>
        ///    小游戏 - 删除游戏的人员数据 - 内部用
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static async Task<OperationResult<bool>> DeleteGameUserDataAsync(DeleteGameUserDataRequest request)
        {
            try
            {
                var manager = GameManagerFactory.GetGameManager(request.GameVersion);
                if (manager == null)
                {
                    // 未实现
                    return OperationResult.FromError<bool>("-3", Resource.Invalid_Game_NotImpl);
                }

                var result = await manager.DeleteGameUserDataAsync(request);
                return OperationResult.FromResult(result);
            }
            catch (NotImplementedException e)
            {
                // 未实现
                return OperationResult.FromError<bool>("-3", Resource.Invalid_Game_NotImpl);
            }
        }

        /// <summary>
        ///  获取游戏实时排行榜
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<OperationResult<GetRankListResponse>> GetRankListAsync(GetRankListAsyncRequest request)
        {
            try
            {
                var manager = GameManagerFactory.GetGameManager(request.GameVersion);
                if (manager == null)
                {
                    // 未实现
                    return OperationResult.FromError<GetRankListResponse>("-3", Resource.Invalid_Game_NotImpl);
                }

                return await manager.GetRankListAsync(request);
            }
            catch (NotImplementedException e)
            {
                // 未实现
                return OperationResult.FromError<GetRankListResponse>("-3", Resource.Invalid_Game_NotImpl);
            }
        }

        /// <summary>
        ///  用户进入游戏
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<OperationResult<UserParticipateGameResponse>> UserParticipateGameAsync(UserParticipateGameRequest request)
        {
            try
            {
                var manager = GameManagerFactory.GetGameManager(request.GameVersion);
                if (manager == null)
                {
                    // 未实现
                    return OperationResult.FromError<UserParticipateGameResponse>("-3", Resource.Invalid_Game_NotImpl);
                }

                return await manager.UserParticipateGameAsync(request);
            }
            catch (NotImplementedException e)
            {
                // 未实现
                return OperationResult.FromError<UserParticipateGameResponse>("-3", Resource.Invalid_Game_NotImpl);
            }
        }

        /// <summary>
        ///  获取用户最近获得的奖品
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<OperationResult<GetUserLatestPrizeResponse>> GetUserLatestPrizeAsync(GetUserLatestPrizeRequest request)
        {
            try
            {
                var manager = GameManagerFactory.GetGameManager(request.GameVersion);
                if (manager == null)
                {
                    // 未实现
                    return OperationResult.FromError<GetUserLatestPrizeResponse>("-3", Resource.Invalid_Game_NotImpl);
                }

                return await manager.GetUserLatestPrizeAsync(request);
            }
            catch (NotImplementedException e)
            {
                // 未实现
                return OperationResult.FromError<GetUserLatestPrizeResponse>("-3", Resource.Invalid_Game_NotImpl);
            }
        }

        /// <summary>
        /// 获取某天之前的积分排行
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<OperationResult<GetRankListBeforeDayResponse>> GetRankListBeforeDayAsync(GetRankListBeforeDayRequest request)
        {
            try
            {
                var manager = GameManagerFactory.GetGameManager(request.GameVersion);
                if (manager == null)
                {
                    // 未实现
                    return OperationResult.FromError<GetRankListBeforeDayResponse>("-3", Resource.Invalid_Game_NotImpl);
                }

                return await manager.GetRankListBeforeDayAsync(request);
            }
            catch (NotImplementedException e)
            {
                // 未实现
                return OperationResult.FromError<GetRankListBeforeDayResponse>("-3", Resource.Invalid_Game_NotImpl);
            }
        }
    }
}
