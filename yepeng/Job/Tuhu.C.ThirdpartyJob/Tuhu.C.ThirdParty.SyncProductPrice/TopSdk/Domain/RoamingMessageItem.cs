using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// RoamingMessageItem Data Structure.
    /// </summary>
    [Serializable]
    public class RoamingMessageItem : TopObject
    {
        /// <summary>
        /// 节点类型
        /// </summary>
        [XmlElement("type")]
        public string Type { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        [XmlElement("value")]
        public string Value { get; set; }
    }
}
