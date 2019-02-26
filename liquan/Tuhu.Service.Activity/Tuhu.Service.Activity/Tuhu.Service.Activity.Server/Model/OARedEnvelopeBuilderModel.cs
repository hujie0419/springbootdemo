using System;

namespace Tuhu.Service.Activity.Server.Model
{
    /// <summary>
    ///     公众号领红包 - 针对红包 - 建造MODEL
    /// </summary>
    internal class OARedEnvelopeBuilderModel 
    {

        /// <summary>
        ///     当前请求的时间
        /// </summary>
        public DateTime RequestTime { get; set; }

        /// <summary>
        ///     OpenId
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        ///     用户ID
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        ///     推荐人User ID
        /// </summary>
        public Guid ReferrerUserId { get; set; }


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
        ///     是否已经写入到数据库中
        /// </summary>
        public bool IsDbDetailGet { get; set; }

        /// <summary>
        ///     获取到的红包金额
        /// </summary>
        public decimal Money { get; set; }

        /// <summary>
        ///     数据库明细ID
        /// </summary>
        public long DetailId { get; set; }

        /// <summary>
        ///     获取Ticket
        /// </summary>
        public bool TicketGet()
        {
            IsTicketGet = true;
            return true;
        }

        /// <summary>
        ///     获取到红包
        /// </summary>
        /// <returns></returns>
        public bool RedEnvelopeGet(decimal money)
        {
            IsRedEnvelopeGet = true;
            Money = money;

            return true;
        }

        /// <summary>
        ///     已经插入到DB中
        /// </summary>
        /// <returns></returns>
        public bool DbDetailGet(long detailId)
        {
            DetailId = detailId;
            IsDbDetailGet = true;
            return true;
        }
    }
}
