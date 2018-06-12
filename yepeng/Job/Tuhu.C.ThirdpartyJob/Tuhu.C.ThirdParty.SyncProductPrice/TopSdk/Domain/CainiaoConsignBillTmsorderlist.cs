using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// CainiaoConsignBillTmsorderlist Data Structure.
    /// </summary>
    [Serializable]
    public class CainiaoConsignBillTmsorderlist : TopObject
    {
        /// <summary>
        /// 运单信息列表
        /// </summary>
        [XmlElement("tms_order")]
        public Top.Api.Domain.CainiaoConsignBillTmsorder TmsOrder { get; set; }
    }
}
