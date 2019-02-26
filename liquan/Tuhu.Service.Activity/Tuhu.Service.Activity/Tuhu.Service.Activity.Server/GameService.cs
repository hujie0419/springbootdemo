using System.Threading.Tasks;
using Tuhu.Service.Activity.Models.Requests;
using Tuhu.Service.Activity.Models.Response;
using Tuhu.Service.Activity.Server.Manager;

namespace Tuhu.Service.Activity.Server
{
    /// <summary>
    ///     游戏服务
    /// </summary>
    public class GameService : IGameService
    {
        /// <summary>
        ///     获取 游戏信息
        ///     -1 -2     程序异常
        ///     -3        此游戏未实现
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<GetGameInfoResponse>> GetGameInfoAsync(GetGameInfoRequest request)
        {
            return await GameManager.GetGameInfoResponseAsync(request);
        }

        /// <summary>
        ///     获取 里程碑信息
        ///     -1 -2     程序异常
        ///     -3        此游戏未实现
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<GetGameMilepostInfoResponse>> GetGameMilepostInfoAsync(
            GetGameMilepostInfoRequest request)
        {

            return await GameManager.GetGameMilepostInfoAsync(request);
        }

        /// <summary>
        ///     获取 用户信息
        ///     -1 -2   程序异常
        ///     -3      此游戏未实现
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<GetGameUserInfoResponse>> GetGameUserInfoAsync(GetGameUserInfoRequest request)
        {
            return await GameManager.GetGameUserInfoAsync(request);
        }

        /// <summary>
        ///     用户分享
        ///     -1 -2   程序异常
        ///     -3      此游戏未实现
        ///     -6      获取锁失败
        ///     -10     今天已经分享
        ///     -30     配置异常
        ///     -40     用户信息异常
        ///     -90     活动尚未开始或者活动已经结束
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<GameUserShareResponse>> GameUserShareAsync(GameUserShareRequest request)
        {
            return await GameManager.GameUserShareAsync(request);
        }

        /// <summary>
        ///     用户领取奖品
        ///     -1 -2  程序异常
        ///     -9     非法提交
        ///     -10    没有奖品了
        ///     -15    奖品异常
        ///     -20    不足以兑换奖品
        ///     -30    配置异常
        ///     -40    用户信息异常
        ///     -45    您的积分不足
        ///     -50    发放奖品失败
        ///     -60    已领取过
        ///     -90    活动尚未开始或者活动已经结束
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<GameUserLootResponse>> GameUserLootAsync(GameUserLootRequest request)
        {
            return await GameManager.GameUserLootAsync(request);
        }

        /// <summary>
        ///     获取 用户好友助力信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<GetGameUserFriendSupportResponse>> GetGameUserFriendSupportAsync(
            GetGameUserFriendSupportRequest request)
        {
            return await GameManager.GetGameUserFriendSupportAsync(request);
        }

        /// <summary>
        ///     帮忙助力
        ///     -1 -2   程序异常
        ///     -7      参数异常
        ///     -3      此游戏未实现
        ///     -10     不能够助力了
        ///     -20     今天已经助力
        ///     -30     游戏配置异常
        ///     -40     用户信息异常
        ///     -50     助力上限了
        ///     -90     活动尚未开始或者活动已经结束
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<GameUserFriendSupportResponse>> GameUserFriendSupportAsync(
            GameUserFriendSupportRequest request)
        {
            return await GameManager.GameUserFriendSupportAsync(request);
        }

        /// <summary>
        ///     获取 用户里程收支明细
        ///     -3      此游戏未实现
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<GetGameUserDistanceInfoResponse>> GetGameUserDistanceInfoAsync(
            GetGameUserDistanceInfoRequest request)
        {
            return await GameManager.GetGameUserDistanceInfoAsync(request);
        }

        /// <summary>
        ///     获取 奖励滚动
        ///     -3      此游戏未实现
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<GetGameUserLootBroadcastResponse>> GetGameUserLootBroadcastAsync(
            GetGameUserLootBroadcastRequest request)
        {
            return await GameManager.GetGameUserLootBroadcastAsync(request);
        }

        /// <summary>
        ///     获取 用户助力信息【剩余助力次数】
        ///     -3      此游戏未实现
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<GetGameUserSupportInfoResponse>> GetGameUserSupportInfoAsync(
            GetGameUserSupportInfoRequest request)
        {
            return await GameManager.GetGameUserSupportInfoAsync(request);
        }

        /// <summary>
        ///     小游戏 - 订单状态跟踪
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<GameOrderTrackingResponse>> GameOrderTrackingAsync(GameOrderTackingRequest request)
        {
            return await GameManager.GameOrderTrackingAsync(request);
        }

        /// <summary>
        ///     小游戏 - 更新游戏信息 - 内部用
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<UpdateGameInfoResponse>> UpdateGameInfoAsync(UpdateGameInfoRequest request)
        {
            return await GameManager.UpdateGameInfoAsync(request);
        }

        /// <summary>
        ///     小游戏 - 删除游戏的人员数据 - 内部用
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> DeleteGameUserDataAsync(DeleteGameUserDataRequest request)
        {
            return await GameManager.DeleteGameUserDataAsync(request);
        }

        /// <summary>
        ///  获取游戏实时排行榜
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<GetRankListResponse>> GetRankListAsync(GetRankListAsyncRequest request)
        {
            return await GameManager.GetRankListAsync(request);
        }

        /// <summary>
        ///  用户进入游戏
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<UserParticipateGameResponse>> UserParticipateGameAsync(UserParticipateGameRequest request)
        {
            return await GameManager.UserParticipateGameAsync(request);
        }

        /// <summary>
        ///  获取用户最近获得的奖品
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<GetUserLatestPrizeResponse>> GetUserLatestPrizeAsync(GetUserLatestPrizeRequest request)
        {
            return await GameManager.GetUserLatestPrizeAsync(request);
        }

        /// <summary>
        /// 获取某天之前的积分排行
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<GetRankListBeforeDayResponse>> GetRankListBeforeDayAsync(GetRankListBeforeDayRequest request)
        {
            return await GameManager.GetRankListBeforeDayAsync(request); ;
        }

    }
}
