using System;

namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    ///     帮忙助力 - 请求类
    /// </summary>
    public class GameUserFriendSupportRequest : GameObjectRequest
    {
        /// <summary>
        ///     助力人的OPENID【马牌】
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        ///     被助力人的UserId【马牌】
        /// </summary>
        public Guid TargetUserId { get; set; }

        /// <summary>
        ///     助力人的微信昵称【马牌】
        /// </summary>
        public string WechatNickName { get; set; }

        /// <summary>
        ///     助力人的微信头像URL【马牌】
        /// </summary>
        public string WechatHeadImg { get; set; }
    }
}
