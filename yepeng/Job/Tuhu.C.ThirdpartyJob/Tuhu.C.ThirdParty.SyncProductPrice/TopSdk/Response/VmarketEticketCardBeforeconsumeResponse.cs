using System;
using System.Xml.Serialization;

namespace Top.Api.Response
{
    /// <summary>
    /// VmarketEticketCardBeforeconsumeResponse.
    /// </summary>
    public class VmarketEticketCardBeforeconsumeResponse : TopResponse
    {
        /// <summary>
        /// 商品标题
        /// </summary>
        [XmlElement("item_title")]
        public string ItemTitle { get; set; }

        /// <summary>
        /// 当前订单剩余可核销数量
        /// </summary>
        [XmlElement("left_num")]
        public long LeftNum { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        [XmlElement("order_id")]
        public long OrderId { get; set; }

        /// <summary>
        /// 1:可以进行核销码操作
        /// </summary>
        [XmlElement("ret_code")]
        public long RetCode { get; set; }

        /// <summary>
        /// 有效期结束时间
        /// </summary>
        [XmlElement("valid_ends")]
        public string ValidEnds { get; set; }

        /// <summary>
        /// 有效期开始时间
        /// </summary>
        [XmlElement("valid_start")]
        public string ValidStart { get; set; }
    }
}
