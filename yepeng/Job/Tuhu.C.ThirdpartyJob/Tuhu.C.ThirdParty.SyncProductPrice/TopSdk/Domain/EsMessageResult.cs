using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Domain
{
    /// <summary>
    /// EsMessageResult Data Structure.
    /// </summary>
    [Serializable]
    public class EsMessageResult : TopObject
    {
        /// <summary>
        /// 消息序列
        /// </summary>
        [XmlArray("messages")]
        [XmlArrayItem("es_message")]
        public List<Top.Api.Domain.EsMessage> Messages { get; set; }

        /// <summary>
        /// nextkey
        /// </summary>
        [XmlElement("next_key")]
        public string NextKey { get; set; }
    }
}
