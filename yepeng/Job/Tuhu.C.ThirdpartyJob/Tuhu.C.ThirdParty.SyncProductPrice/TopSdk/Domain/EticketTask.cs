using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// EticketTask Data Structure.
    /// </summary>
    [Serializable]
    public class EticketTask : TopObject
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        [XmlElement("order_id")]
        public long OrderId { get; set; }
    }
}
