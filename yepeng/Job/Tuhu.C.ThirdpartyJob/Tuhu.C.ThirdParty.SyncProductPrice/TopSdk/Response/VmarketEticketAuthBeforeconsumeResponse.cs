using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// VmarketEticketAuthBeforeconsumeResponse.
    /// </summary>
    public class VmarketEticketAuthBeforeconsumeResponse : TopResponse
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
        /// 淘宝卖家旺旺名称
        /// </summary>
        [XmlElement("seller_nick")]
        public string SellerNick { get; set; }

        /// <summary>
        /// 淘宝卖家ID
        /// </summary>
        [XmlElement("taobao_sid")]
        public long TaobaoSid { get; set; }

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
