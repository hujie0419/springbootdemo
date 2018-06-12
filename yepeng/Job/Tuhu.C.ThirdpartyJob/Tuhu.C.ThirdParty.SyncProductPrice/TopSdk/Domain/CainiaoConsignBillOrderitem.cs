using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Domain
{
    /// <summary>
    /// CainiaoConsignBillOrderitem Data Structure.
    /// </summary>
    [Serializable]
    public class CainiaoConsignBillOrderitem : TopObject
    {
        /// <summary>
        /// 商品属性列表
        /// </summary>
        [XmlArray("inventory_item_list")]
        [XmlArrayItem("cainiao_consign_bill_inventoryitemlist")]
        public List<Top.Api.Domain.CainiaoConsignBillInventoryitemlist> InventoryItemList { get; set; }

        /// <summary>
        /// 商家编码
        /// </summary>
        [XmlElement("item_code")]
        public string ItemCode { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        [XmlElement("item_id")]
        public string ItemId { get; set; }

        /// <summary>
        /// 订单明细行编码
        /// </summary>
        [XmlElement("order_item_id")]
        public string OrderItemId { get; set; }
    }
}
