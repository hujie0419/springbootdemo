using System;

namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    ///    七龙珠 - 创建一个用户任务 请求类
    /// </summary>
    public class DragonBallCreateUserMissionDetailRequest
    {
        /// <summary>
        ///     用户ID
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        ///     任务ID
        /// </summary>
        public int MissionId { get; set; }

        /// <summary>
        ///     无视活动时间
        /// </summary>
        public bool ForceActivityTime { get; set; }
    }
}
