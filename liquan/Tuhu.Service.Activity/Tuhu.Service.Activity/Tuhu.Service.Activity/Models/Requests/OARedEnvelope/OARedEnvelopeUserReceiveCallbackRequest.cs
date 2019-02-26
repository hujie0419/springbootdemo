using System;

namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    ///     公众号领红包 - 用户领取 回调 请求类
    /// </summary>
    public class OARedEnvelopeUserReceiveCallbackRequest
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
        ///     是否获取到了 Ticket
        /// </summary>
        public bool IsTicketGet { get; set; }

        /// <summary>
        ///     是否获取到了红包
        /// </summary>
        public bool IsRedEnvelopeGet { get; set; }

        /// <summary>
        ///     获取到的红包金额
        /// </summary>
        public decimal Money { get; set; }

        /// <summary>
        ///     当前请求的时间
        /// </summary>
        public DateTime RequestTime { get; set; }

        /// <summary>
        ///     推荐人的USERID
        /// </summary>
        public Guid ReferrerUserId { get; set; }
    }
}
