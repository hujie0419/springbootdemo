using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// Orderitemlistwlbwmsconsignordernotify Data Structure.
    /// </summary>
    [Serializable]
    public class Orderitemlistwlbwmsconsignordernotify : TopObject
    {
        /// <summary>
        /// 订单商品信息
        /// </summary>
        [XmlElement("order_item")]
        public Top.Api.Domain.Orderitemwlbwmsconsignordernotify OrderItem { get; set; }
    }
}
