using System;

namespace Tuhu.Service.Activity.DataAccess.Models
{
    /// <summary>
    ///     公众号领红包分享表
    /// </summary>
    public class OARedEnvelopeShareModel
    {
        /// <summary>
        ///     主键
        /// </summary>
        public long PKID { get; set; }

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

        /// <summary>
        ///     创建时间
        /// </summary>
        public DateTime CreateDatetime { get; set; }

        /// <summary>
        ///     最后一次修改时间
        /// </summary>
        public DateTime LastUpdateDateTime { get; set; }
    }
}
