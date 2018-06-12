using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// CainiaoConsignBillOrderitemlist Data Structure.
    /// </summary>
    [Serializable]
    public class CainiaoConsignBillOrderitemlist : TopObject
    {
        /// <summary>
        /// 订单商品信息
        /// </summary>
        [XmlElement("order_item")]
        public Top.Api.Domain.CainiaoConsignBillOrderitem OrderItem { get; set; }
    }
}
