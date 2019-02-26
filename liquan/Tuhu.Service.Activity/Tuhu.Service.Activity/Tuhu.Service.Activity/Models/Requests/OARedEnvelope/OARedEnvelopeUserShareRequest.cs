using System;

namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    ///     公众号领红包 - 分享
    /// </summary>
    public class OARedEnvelopeUserShareRequest
    {
        /// <summary>
        ///     分享人ID
        /// </summary>
        public Guid ShareingUserId { get; set; }

        /// <summary>
        ///     被分享人ID
        /// </summary>
        public Guid SharedUserId { get; set; }

        /// <summary>
        ///     分享人OPENID
        /// </summary>
        public string ShareingOpenId { get; set; }

        /// <summary>
        ///     被分享人OPENID
        /// </summary>
        public string SharedOpenId { get; set; }

        /// <summary>
        ///     公众号类型 1主号
        /// </summary>
        public int OfficialAccountType { get; set; }
    }
}
