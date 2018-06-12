using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Domain
{
    /// <summary>
    /// RoamingMessage Data Structure.
    /// </summary>
    [Serializable]
    public class RoamingMessage : TopObject
    {
        /// <summary>
        /// 消息内容
        /// </summary>
        [XmlArray("content_list")]
        [XmlArrayItem("roaming_message_item")]
        public List<Top.Api.Domain.RoamingMessageItem> ContentList { get; set; }

        /// <summary>
        /// 消息方向。user1 -> user2 = 0 , user2->user1 = 1
        /// </summary>
        [XmlElement("direction")]
        public long Direction { get; set; }

        /// <summary>
        /// 消息时间（UTC时间）
        /// </summary>
        [XmlElement("time")]
        public long Time { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        [XmlElement("type")]
        public long Type { get; set; }

        /// <summary>
        /// 消息唯一ID
        /// </summary>
        [XmlElement("uuid")]
        public long Uuid { get; set; }
    }
}
