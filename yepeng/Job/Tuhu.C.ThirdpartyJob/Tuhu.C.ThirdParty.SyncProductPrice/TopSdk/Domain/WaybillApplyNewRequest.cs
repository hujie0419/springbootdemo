using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Domain
{
    /// <summary>
    /// WaybillApplyNewRequest Data Structure.
    /// </summary>
    [Serializable]
    public class WaybillApplyNewRequest : TopObject
    {
        /// <summary>
        /// TOP  appkey
        /// </summary>
        [XmlElement("app_key")]
        public string AppKey { get; set; }

        /// <summary>
        /// 物流服务商编码
        /// </summary>
        [XmlElement("cp_code")]
        public string CpCode { get; set; }

        /// <summary>
        /// --
        /// </summary>
        [XmlElement("cp_id")]
        public long CpId { get; set; }

        /// <summary>
        /// 使用者ID
        /// </summary>
        [XmlElement("real_user_id")]
        public long RealUserId { get; set; }

        /// <summary>
        /// 商家ID
        /// </summary>
        [XmlElement("seller_id")]
        public long SellerId { get; set; }

        /// <summary>
        /// 发货地址
        /// </summary>
        [XmlElement("shipping_address")]
        public Top.Api.Domain.WaybillAddress ShippingAddress { get; set; }

        /// <summary>
        /// 面单详细信息
        /// </summary>
        [XmlArray("trade_order_info_cols")]
        [XmlArrayItem("trade_order_info")]
        public List<Top.Api.Domain.TradeOrderInfo> TradeOrderInfoCols { get; set; }
    }
}
