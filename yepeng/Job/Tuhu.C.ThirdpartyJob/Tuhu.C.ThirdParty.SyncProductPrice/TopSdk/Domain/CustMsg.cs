using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Domain
{
    /// <summary>
    /// CustMsg Data Structure.
    /// </summary>
    [Serializable]
    public class CustMsg : TopObject
    {
        /// <summary>
        /// apns推送的附带数据。
        /// </summary>
        [XmlElement("apns_param")]
        public string ApnsParam { get; set; }

        /// <summary>
        /// apns推送时，里面的aps结构体json字符串，aps.alert为必填字段。本字段为可选，若为空，则表示不进行apns推送
        /// </summary>
        [XmlElement("aps")]
        public string Aps { get; set; }

        /// <summary>
        /// 发送的自定义数据，sdk默认无法解析消息，该数据需要客户端自己解析
        /// </summary>
        [XmlElement("data")]
        public string Data { get; set; }

        /// <summary>
        /// 发送方appkey
        /// </summary>
        [XmlElement("from_appkey")]
        public string FromAppkey { get; set; }

        /// <summary>
        /// 发送方userid
        /// </summary>
        [XmlElement("from_user")]
        public string FromUser { get; set; }

        /// <summary>
        /// 客户端最近消息里面显示的消息摘要
        /// </summary>
        [XmlElement("summary")]
        public string Summary { get; set; }

        /// <summary>
        /// 接收方appkey，默认是发送方appkey，如需跨域发送，需要走审批流程
        /// </summary>
        [XmlElement("to_appkey")]
        public string ToAppkey { get; set; }

        /// <summary>
        /// 接受者userid列表，单次发送用户数小于100
        /// </summary>
        [XmlArray("to_users")]
        [XmlArrayItem("string")]
        public List<string> ToUsers { get; set; }
    }
}
