using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// CainiaoConsignBillPackagemateriallist Data Structure.
    /// </summary>
    [Serializable]
    public class CainiaoConsignBillPackagemateriallist : TopObject
    {
        /// <summary>
        /// 包裹包材信息
        /// </summary>
        [XmlElement("package_material")]
        public Top.Api.Domain.CainiaoConsignBillPackagematerial PackageMaterial { get; set; }
    }
}
