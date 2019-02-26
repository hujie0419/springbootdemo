using System;

namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    ///     获取用户好友助力 - 请求类
    /// </summary>
    public class GetGameUserFriendSupportRequest : GameObjectRequest
    {
        /// <summary>
        ///     用户ID
        /// </summary>
        public Guid UserId { get; set; }
    }
}
