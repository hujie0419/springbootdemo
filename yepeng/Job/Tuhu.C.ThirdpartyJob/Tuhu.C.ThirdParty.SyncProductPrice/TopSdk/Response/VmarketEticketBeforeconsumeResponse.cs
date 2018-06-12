using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// VmarketEticketBeforeconsumeResponse.
    /// </summary>
    public class VmarketEticketBeforeconsumeResponse : TopResponse
    {
        /// <summary>
        /// 当前码剩余可核销数量
        /// </summary>
        [XmlElement("code_left_num")]
        public long CodeLeftNum { get; set; }

        /// <summary>
        /// 商品标题
        /// </summary>
        [XmlElement("item_title")]
        public string ItemTitle { get; set; }

        /// <summary>
        /// 扩展字段，暂时预留为0，没有任何意义
        /// </summary>
        [XmlElement("item_type")]
        public long ItemType { get; set; }

        /// <summary>
        /// 扩展字段，暂时预留为0，没有任何意义
        /// </summary>
        [XmlElement("left_amount")]
        public string LeftAmount { get; set; }

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
