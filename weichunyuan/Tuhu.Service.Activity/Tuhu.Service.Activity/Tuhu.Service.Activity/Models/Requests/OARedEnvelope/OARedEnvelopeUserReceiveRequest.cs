using System;

namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    ///     公众号领红包 - 用户领取 请求类
    /// </summary>
    public class OARedEnvelopeUserReceiveRequest
    {
        /// <summary>
        ///     用户ID
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        ///     OpenId
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        ///     微信昵称
        /// </summary>
        public string WXNickName { get; set; }

        /// <summary>
        ///     微信头像URL
        /// </summary>
        public string WXHeadPicUrl { get; set; }

        /// <summary>
        ///     公众号类型 1主号
        /// </summary>
        public int OfficialAccountType { get; set; }

        /// <summary>
        ///     推荐人的USERID
        /// </summary>
        public Guid ReferrerUserId { get; set; }
    }
}
