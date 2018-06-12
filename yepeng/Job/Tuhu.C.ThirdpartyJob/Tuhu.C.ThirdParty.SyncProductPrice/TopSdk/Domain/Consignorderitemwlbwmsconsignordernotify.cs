using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// Consignorderitemwlbwmsconsignordernotify Data Structure.
    /// </summary>
    [Serializable]
    public class Consignorderitemwlbwmsconsignordernotify : TopObject
    {
        /// <summary>
        /// 商品编码
        /// </summary>
        [XmlElement("item_code")]
        public string ItemCode { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        [XmlElement("item_id")]
        public string ItemId { get; set; }

        /// <summary>
        /// 商品数量
        /// </summary>
        [XmlElement("item_quantity")]
        public long ItemQuantity { get; set; }

        /// <summary>
        /// ERP订单明细行号ID
        /// </summary>
        [XmlElement("order_item_id")]
        public string OrderItemId { get; set; }
    }
}
