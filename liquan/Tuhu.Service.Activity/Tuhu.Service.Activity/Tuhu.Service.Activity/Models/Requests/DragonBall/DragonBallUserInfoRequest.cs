using System;

namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    ///   七龙珠 -  用户当前龙珠总数/兑换次数 请求类
    /// </summary>
    public class DragonBallUserInfoRequest
    {
        /// <summary>
        ///     用户ID
        /// </summary>
        public Guid UserId { get; set; }
    }
}
