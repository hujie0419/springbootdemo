using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// CainiaoConsignBillInvoinceconfirmlist Data Structure.
    /// </summary>
    [Serializable]
    public class CainiaoConsignBillInvoinceconfirmlist : TopObject
    {
        /// <summary>
        /// 发票确认信息
        /// </summary>
        [XmlElement("invoince_confirm")]
        public Top.Api.Domain.CainiaoConsignBillInvoinceconfirm InvoinceConfirm { get; set; }
    }
}
