using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// CainiaoConsignBillTmsitemlist Data Structure.
    /// </summary>
    [Serializable]
    public class CainiaoConsignBillTmsitemlist : TopObject
    {
        /// <summary>
        /// 包裹里面商品
        /// </summary>
        [XmlElement("tms_item")]
        public Top.Api.Domain.CainiaoConsignBillTmsitem TmsItem { get; set; }
    }
}
