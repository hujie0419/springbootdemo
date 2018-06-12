using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// SendMessageResult Data Structure.
    /// </summary>
    [Serializable]
    public class SendMessageResult : TopObject
    {
        /// <summary>
        /// 发送的唯一号
        /// </summary>
        [XmlElement("task_id")]
        public long TaskId { get; set; }
    }
}
