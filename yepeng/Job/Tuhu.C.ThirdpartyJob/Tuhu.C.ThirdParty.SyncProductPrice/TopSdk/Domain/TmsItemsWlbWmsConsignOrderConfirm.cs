using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// TmsItemsWlbWmsConsignOrderConfirm Data Structure.
    /// </summary>
    [Serializable]
    public class TmsItemsWlbWmsConsignOrderConfirm : TopObject
    {
        /// <summary>
        /// 批次编号
        /// </summary>
        [XmlElement("batch_code")]
        public string BatchCode { get; set; }

        /// <summary>
        /// 商品过期日期
        /// </summary>
        [XmlElement("due_date")]
        public string DueDate { get; set; }

        /// <summary>
        /// 后端商品ID
        /// </summary>
        [XmlElement("item_id")]
        public string ItemId { get; set; }

        /// <summary>
        /// 商品数量，此包裹内商品的数量
        /// </summary>
        [XmlElement("item_quantity")]
        public long ItemQuantity { get; set; }

        /// <summary>
        /// 订单明细ID
        /// </summary>
        [XmlElement("order_item_id")]
        public string OrderItemId { get; set; }

        /// <summary>
        /// 商品生产批号
        /// </summary>
        [XmlElement("produce_code")]
        public string ProduceCode { get; set; }

        /// <summary>
        /// 商品生产日期
        /// </summary>
        [XmlElement("product_date")]
        public string ProductDate { get; set; }
    }
}
