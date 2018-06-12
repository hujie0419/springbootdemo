using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Domain
{
    /// <summary>
    /// RoamingMessageResult Data Structure.
    /// </summary>
    [Serializable]
    public class RoamingMessageResult : TopObject
    {
        /// <summary>
        /// 消息列表
        /// </summary>
        [XmlArray("messages")]
        [XmlArrayItem("roaming_message")]
        public List<Top.Api.Domain.RoamingMessage> Messages { get; set; }

        /// <summary>
        /// 下次迭代key
        /// </summary>
        [XmlElement("next_key")]
        public string NextKey { get; set; }
    }
}
