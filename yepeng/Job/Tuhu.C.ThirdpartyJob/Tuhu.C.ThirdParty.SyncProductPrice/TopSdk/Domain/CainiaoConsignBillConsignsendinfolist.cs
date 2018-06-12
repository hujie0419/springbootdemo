using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// CainiaoConsignBillConsignsendinfolist Data Structure.
    /// </summary>
    [Serializable]
    public class CainiaoConsignBillConsignsendinfolist : TopObject
    {
        /// <summary>
        /// 物流订单发货信息
        /// </summary>
        [XmlElement("consign_send_info")]
        public Top.Api.Domain.CainiaoConsignBillConsignsendinfo ConsignSendInfo { get; set; }
    }
}
