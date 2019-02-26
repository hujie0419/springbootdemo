using System;

namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    ///    七龙珠 - 用户任务历史 请求类
    /// </summary>
    public class DragonBallUserMissionHistoryListRequest
    {
        /// <summary>
        ///     用户ID
        /// </summary>
        public Guid UserId { get; set; }
    }
}
