using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// CainiaoConsignBillInventoryitemlist Data Structure.
    /// </summary>
    [Serializable]
    public class CainiaoConsignBillInventoryitemlist : TopObject
    {
        /// <summary>
        /// 商品属性列表
        /// </summary>
        [XmlElement("inventory_item")]
        public Top.Api.Domain.CainiaoConsignBillInventoryitem InventoryItem { get; set; }
    }
}
