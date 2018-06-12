using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// CainiaoConsignBillTmsitem Data Structure.
    /// </summary>
    [Serializable]
    public class CainiaoConsignBillTmsitem : TopObject
    {
        /// <summary>
        /// 商家编码
        /// </summary>
        [XmlElement("item_code")]
        public string ItemCode { get; set; }

        /// <summary>
        /// ERP商品ID
        /// </summary>
        [XmlElement("item_id")]
        public string ItemId { get; set; }

        /// <summary>
        /// 此运单里该商品的数量
        /// </summary>
        [XmlElement("item_qty")]
        public long ItemQty { get; set; }
    }
}
