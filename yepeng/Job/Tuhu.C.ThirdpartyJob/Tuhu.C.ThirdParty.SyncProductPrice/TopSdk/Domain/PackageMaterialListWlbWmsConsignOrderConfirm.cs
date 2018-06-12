using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// PackageMaterialListWlbWmsConsignOrderConfirm Data Structure.
    /// </summary>
    [Serializable]
    public class PackageMaterialListWlbWmsConsignOrderConfirm : TopObject
    {
        /// <summary>
        /// 包材的数量
        /// </summary>
        [XmlElement("material_quantity")]
        public long MaterialQuantity { get; set; }

        /// <summary>
        /// 包材型号
        /// </summary>
        [XmlElement("material_type")]
        public string MaterialType { get; set; }
    }
}
