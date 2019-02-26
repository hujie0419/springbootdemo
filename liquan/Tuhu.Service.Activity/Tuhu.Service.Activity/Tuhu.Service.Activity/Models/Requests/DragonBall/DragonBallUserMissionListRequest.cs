using System;

namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    ///    七龙珠 - 用户任务列表 请求类
    /// </summary>
    public class DragonBallUserMissionListRequest
    {
        /// <summary>
        ///     用户ID
        /// </summary>
        public Guid UserId { get; set; }
    }
}
