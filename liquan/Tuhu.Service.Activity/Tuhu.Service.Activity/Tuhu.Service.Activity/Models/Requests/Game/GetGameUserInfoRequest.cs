using System;

namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    ///     游戏用户信息 - 请求类
    /// </summary>
    public class GetGameUserInfoRequest : GameObjectRequest
    {
        /// <summary>
        ///     用户ID
        /// </summary>
        public Guid UserId { get; set; }
    }
}
