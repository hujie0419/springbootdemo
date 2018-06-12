using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// VmarketEticketAuthConsumeResponse.
    /// </summary>
    public class VmarketEticketAuthConsumeResponse : TopResponse
    {
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

    }
}
