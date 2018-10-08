using System;

namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    ///    七龙珠 - 用户获取的奖励 请求类
    /// </summary>
    public class DragonBallUserLootListRequest
    {
        /// <summary>
        ///     用户ID
        /// </summary>
        public Guid UserId { get; set; }
    }
}
