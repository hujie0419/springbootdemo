using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// Consignorderitemlistwlbwmsconsignordernotify Data Structure.
    /// </summary>
    [Serializable]
    public class Consignorderitemlistwlbwmsconsignordernotify : TopObject
    {
        /// <summary>
        /// 仓库物流订单信息列表
        /// </summary>
        [XmlElement("consign_order_item")]
        public Top.Api.Domain.Consignorderitemwlbwmsconsignordernotify ConsignOrderItem { get; set; }
    }
}
