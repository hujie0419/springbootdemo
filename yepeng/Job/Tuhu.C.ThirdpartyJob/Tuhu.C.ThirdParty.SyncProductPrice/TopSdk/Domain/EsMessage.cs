using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Domain
{
    /// <summary>
    /// EsMessage Data Structure.
    /// </summary>
    [Serializable]
    public class EsMessage : TopObject
    {
        /// <summary>
        /// 消息内容
        /// </summary>
        [XmlArray("content")]
        [XmlArrayItem("roaming_message_item")]
        public List<Top.Api.Domain.RoamingMessageItem> Content { get; set; }

        /// <summary>
        /// 发送方
        /// </summary>
        [XmlElement("from_id")]
        public Top.Api.Domain.OpenImUser FromId { get; set; }

        /// <summary>
        /// 消息时间，UTC时间
        /// </summary>
        [XmlElement("time")]
        public long Time { get; set; }

        /// <summary>
        /// 接收方
        /// </summary>
        [XmlElement("to_id")]
        public Top.Api.Domain.OpenImUser ToId { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        [XmlElement("type")]
        public long Type { get; set; }

        /// <summary>
        /// 消息UUID
        /// </summary>
        [XmlElement("uuid")]
        public long Uuid { get; set; }
    }
}
