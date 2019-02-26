using System;

namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    ///     用户分享 - 请求类
    /// </summary>
    public class GameUserShareRequest : GameObjectRequest
    {
        /// <summary>
        ///     用户ID
        /// </summary>
        public Guid UserId { get; set; }
    }
}
