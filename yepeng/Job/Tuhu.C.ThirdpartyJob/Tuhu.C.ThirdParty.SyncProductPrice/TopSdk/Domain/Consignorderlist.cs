using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// Consignorderlist Data Structure.
    /// </summary>
    [Serializable]
    public class Consignorderlist : TopObject
    {
        /// <summary>
        /// 发货订单信息
        /// </summary>
        [XmlElement("consign_order")]
        public Top.Api.Domain.Consignorder ConsignOrder { get; set; }
    }
}
