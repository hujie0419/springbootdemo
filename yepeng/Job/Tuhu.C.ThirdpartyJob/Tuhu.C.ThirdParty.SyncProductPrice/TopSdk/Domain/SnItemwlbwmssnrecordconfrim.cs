using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// SnItemwlbwmssnrecordconfrim Data Structure.
    /// </summary>
    [Serializable]
    public class SnItemwlbwmssnrecordconfrim : TopObject
    {
        /// <summary>
        /// 商家商品编码
        /// </summary>
        [XmlElement("item_code")]
        public string ItemCode { get; set; }

        /// <summary>
        /// 菜鸟商品ID
        /// </summary>
        [XmlElement("item_id")]
        public string ItemId { get; set; }

        /// <summary>
        /// 商品序列号
        /// </summary>
        [XmlElement("sn_code")]
        public string SnCode { get; set; }
    }
}
