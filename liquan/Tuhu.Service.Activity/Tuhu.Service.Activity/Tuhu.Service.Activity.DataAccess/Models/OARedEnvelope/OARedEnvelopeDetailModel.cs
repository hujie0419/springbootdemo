using System;

namespace Tuhu.Service.Activity.DataAccess.Models
{
    /// <summary>
    ///     公众号领红包明细表
    /// </summary>
    public class OARedEnvelopeDetailModel
    {
        /// <summary>
        ///     主键
        /// </summary>
        public long PKID { get; set; }

        /// <summary>
        ///     用户ID
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        ///     用户昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        ///     微信头像URL
        /// </summary>
        public string WXHeadImgUrl { get; set; }


        /// <summary>
        ///     微信OpenId
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        ///     推荐人User ID
        /// </summary>
        public Guid ReferrerUserId { get; set; }

        /// <summary>
        ///     获取的钱
        /// </summary>
        public decimal GetMoney { get; set; }

        /// <summary>
        ///     领取日期 - 只有日期
        /// </summary>
        public DateTime GetDate { get; set; }

        /// <summary>
        ///     公众号类型 1主号
        /// </summary>
        public int OfficialAccountType { get; set; }

        /// <summary>
        ///     创建时间
        /// </summary>
        public DateTime CreateDatetime { get; set; }

        /// <summary>
        ///     最后一次更新时间
        /// </summary>
        public DateTime LastUpdateDateTime { get; set; }

        /// <summary>
        ///     行驶证
        /// </summary>
        public string DrivingLicense { get; set; }
    }
}
