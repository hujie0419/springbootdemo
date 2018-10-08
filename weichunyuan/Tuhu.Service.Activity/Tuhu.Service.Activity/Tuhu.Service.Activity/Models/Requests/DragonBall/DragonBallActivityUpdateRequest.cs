using System;

namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    ///     七龙珠 - 更新活动
    /// </summary>
    public class DragonBallActivityUpdateRequest
    {
        /// <summary>
        ///     开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        ///     结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
    }
}
