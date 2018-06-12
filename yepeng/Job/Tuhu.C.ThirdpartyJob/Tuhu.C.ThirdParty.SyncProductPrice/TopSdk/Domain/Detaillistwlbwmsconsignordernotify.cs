using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// Detaillistwlbwmsconsignordernotify Data Structure.
    /// </summary>
    [Serializable]
    public class Detaillistwlbwmsconsignordernotify : TopObject
    {
        /// <summary>
        /// 商品总金额
        /// </summary>
        [XmlElement("amount")]
        public string Amount { get; set; }

        /// <summary>
        /// 发票信息
        /// </summary>
        [XmlElement("item_detail")]
        public Top.Api.Domain.Itemdetailwlbwmsconsignordernotify ItemDetail { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [XmlElement("item_name")]
        public string ItemName { get; set; }

        /// <summary>
        /// 商品价格
        /// </summary>
        [XmlElement("price")]
        public string Price { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [XmlElement("quantity")]
        public long Quantity { get; set; }

        /// <summary>
        /// 计价单元
        /// </summary>
        [XmlElement("unit")]
        public string Unit { get; set; }
    }
}
