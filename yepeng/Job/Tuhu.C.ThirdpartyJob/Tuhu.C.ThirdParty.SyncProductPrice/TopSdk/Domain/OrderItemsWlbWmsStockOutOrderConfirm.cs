using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// OrderItemsWlbWmsStockOutOrderConfirm Data Structure.
    /// </summary>
    [Serializable]
    public class OrderItemsWlbWmsStockOutOrderConfirm : TopObject
    {
        /// <summary>
        /// 是否完成
        /// </summary>
        [XmlElement("is_completed")]
        public bool IsCompleted { get; set; }

        /// <summary>
        /// 商品码
        /// </summary>
        [XmlElement("item_code")]
        public string ItemCode { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        [XmlElement("item_id")]
        public string ItemId { get; set; }

        /// <summary>
        /// 商品列表
        /// </summary>
        [XmlElement("items")]
        public Top.Api.Domain.ItemsWlbWmsStockOutOrderConfirm Items { get; set; }

        /// <summary>
        /// 单据明细ID
        /// </summary>
        [XmlElement("order_item_id")]
        public string OrderItemId { get; set; }
    }
}
