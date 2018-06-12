using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Domain
{
    /// <summary>
    /// Consignorder Data Structure.
    /// </summary>
    [Serializable]
    public class Consignorder : TopObject
    {
        /// <summary>
        /// 仓库物流订单信息列表
        /// </summary>
        [XmlArray("consign_order_item_list")]
        [XmlArrayItem("consignorderitemlistwlbwmsconsignordernotify")]
        public List<Top.Api.Domain.Consignorderitemlistwlbwmsconsignordernotify> ConsignOrderItemList { get; set; }

        /// <summary>
        /// 错误编码
        /// </summary>
        [XmlElement("error_code")]
        public string ErrorCode { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        [XmlElement("error_msg")]
        public string ErrorMsg { get; set; }

        /// <summary>
        /// 仓库编码
        /// </summary>
        [XmlElement("store_code")]
        public string StoreCode { get; set; }

        /// <summary>
        /// 仓库订单编码
        /// </summary>
        [XmlElement("store_order_code")]
        public string StoreOrderCode { get; set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        [XmlElement("success")]
        public bool Success { get; set; }

        /// <summary>
        /// 配送编码
        /// </summary>
        [XmlElement("tms_code")]
        public string TmsCode { get; set; }
    }
}
