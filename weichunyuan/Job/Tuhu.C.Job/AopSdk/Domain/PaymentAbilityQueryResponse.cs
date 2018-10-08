using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// PaymentAbilityQueryResponse Data Structure.
    /// </summary>
    [Serializable]
    public class PaymentAbilityQueryResponse : AopObject
    {
        /// <summary>
        /// 接口返回的支付能力等级
        /// </summary>
        [XmlElement("level")]
        public string Level { get; set; }

        /// <summary>
        /// 返回的单据号
        /// </summary>
        [XmlElement("order_id")]
        public string OrderId { get; set; }
    }
}
