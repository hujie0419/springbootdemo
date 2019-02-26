using System;

namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    ///   七龙珠 -  用户领取任务奖励 请求类
    /// </summary>
    public class DragonBallUserMissionRewardRequest
    {
        /// <summary>
        ///     用户ID
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        ///     任务ID
        /// </summary>
        public int MissionId { get; set; }
    }
}
