using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// PackageItemItemsWlbWmsStockOutOrderConfirm Data Structure.
    /// </summary>
    [Serializable]
    public class PackageItemItemsWlbWmsStockOutOrderConfirm : TopObject
    {
        /// <summary>
        /// 后端商品ID
        /// </summary>
        [XmlElement("item_id")]
        public string ItemId { get; set; }

        /// <summary>
        /// 此包裹里该商品的数量
        /// </summary>
        [XmlElement("item_quantity")]
        public long ItemQuantity { get; set; }
    }
}
