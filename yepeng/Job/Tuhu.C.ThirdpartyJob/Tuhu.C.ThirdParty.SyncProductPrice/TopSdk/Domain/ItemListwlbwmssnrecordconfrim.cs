using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// ItemListwlbwmssnrecordconfrim Data Structure.
    /// </summary>
    [Serializable]
    public class ItemListwlbwmssnrecordconfrim : TopObject
    {
        /// <summary>
        /// 商品信息
        /// </summary>
        [XmlElement("sn_item")]
        public Top.Api.Domain.SnItemwlbwmssnrecordconfrim SnItem { get; set; }
    }
}
