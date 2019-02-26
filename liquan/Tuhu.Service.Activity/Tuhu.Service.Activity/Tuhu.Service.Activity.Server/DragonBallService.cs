using System;
using System.Threading.Tasks;
using Tuhu.Service.Activity.Models.Requests;
using Tuhu.Service.Activity.Models.Response;
using Tuhu.Service.Activity.Server.Manager;

namespace Tuhu.Service.Activity.Server
{
    public class DragonBallService : IDragonBallService
    {

        /// <summary>
        ///     用户当前龙珠总数/召唤次数
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<DragonBallUserInfoResponse>> DragonBallUserInfoAsync(DragonBallUserInfoRequest request)
        {
            return await DragonBallManager.DragonBallUserInfoAsync(request);
        }

        /// <summary>
        ///     七龙珠 - 获奖轮播
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public async Task<OperationResult<DragonBallBroadcastResponse>> DragonBallBroadcastAsync(int count)
        {
            return await DragonBallManager.DragonBallBroadcastAsync(count);
        }

        /// <summary>
        ///     七龙珠 - 用户获得战利品列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<DragonBallUserLootListResponse>> DragonBallUserLootListAsync(DragonBallUserLootListRequest request)
        {
            return await DragonBallManager.DragonBallUserLootListAsync(request);
        }

        /// <summary>
        ///     七龙珠 - 用户任务列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<DragonBallUserMissionListResponse>> DragonBallUserMissionListAsync(DragonBallUserMissionListRequest request)
        {
            return await DragonBallManager.DragonBallUserMissionListAsync(request);
        }

        /// <summary>
        ///     七龙珠 - 用户获取任务奖励
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<DragonBallUserMissionRewardResponse>> DragonBallUserMissionRewardAsync(DragonBallUserMissionRewardRequest request)
        {
            return await DragonBallManager.DragonBallUserMissionRewardAsync(request);
        }

        /// <summary>
        ///     七龙珠 - 召唤神龙
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<DragonBallSummonResponse>> DragonBallSummonAsync(DragonBallSummonRequest request)
        {
            return await DragonBallManager.DragonBallSummonAsync(request);
        }

        /// <summary>
        ///     七龙珠 - 活动信息
        /// </summary>
        /// <returns></returns>
        public async Task<OperationResult<ActivityResponse>> DragonBallActivityInfoAsync()
        {
            return await DragonBallManager.DragonBallActivityInfoAsync();
        }

        /// <summary>
        ///     七龙珠 - 获取设置
        /// </summary>
        /// <returns></returns>
        public async Task<OperationResult<DragonBallSettingResponse>> DragonBallSettingAsync()
        {
            return await DragonBallManager.DragonBallSettingAsync();
        }

        /// <summary>
        ///     七龙珠 - 用户分享
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> DragonBallUserShareAsync(DragonBallUserShareRequest request)
        {
            return await DragonBallManager.DragonBallUserShareAsync(request);
        }

        /// <summary>
        ///     七龙珠 - 给用户创建一个任务
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> DragonBallCreateUserMissionDetailAsync(DragonBallCreateUserMissionDetailRequest request)
        {
            return await DragonBallManager.DragonBallCreateUserMissionDetailAsync(request);
        }

        /// <summary>
        ///     七龙珠 - 用户任务历史
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<DragonBallUserMissionHistoryListResponse>> DragonBallUserMissionHistoryListAsync(DragonBallUserMissionHistoryListRequest request)
        {
            return await DragonBallManager.DragonBallUserMissionHistoryListAsync(request);
        }

        /// <summary>
        ///     七龙珠 - 初始化用户数据
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="isForce"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> DragonBallUserMissionInitAsync(Guid userId , bool isForce = false)
        {
            return await DragonBallManager.DragonBallUserMissionInitAsync(userId , isForce);
        }

        /// <summary>
        ///     七龙珠 - 更新活动
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> DragonBallActivityUpdateAsync(DragonBallActivityUpdateRequest request)
        {
            return OperationResult.FromResult(await DragonBallManager.DragonBallActivityUpdateAsync(request));
        }

        /// <summary>
        ///     七龙珠 - 更新设置
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> DragonBallSettingUpdateAsync(DragonBallSettingUpdateRequest request)
        {
            return OperationResult.FromResult(await DragonBallManager.DragonBallSettingUpdateAsync(request));
        }

        /// <summary>
        ///     七龙珠 - 删除用户数据
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> DragonBallUserDataDeleteAsync(Guid userId)
        {
            return OperationResult.FromResult(await DragonBallManager.DragonBallUserDataDeleteAsync(userId));
        }

        /// <summary>
        ///     七龙珠 - 增加修改用户龙珠数量
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="dragonBallCount"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> DragonBallUserUpdateAsync(Guid userId, int dragonBallCount)
        {
            return OperationResult.FromResult(await DragonBallManager.DragonBallUserUpdateAsync(userId, dragonBallCount));

        }
    }
}
