using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// BmcResult Data Structure.
    /// </summary>
    [Serializable]
    public class BmcResult : TopObject
    {
        /// <summary>
        /// 结果code
        /// </summary>
        [XmlElement("code")]
        public long Code { get; set; }

        /// <summary>
        /// 发送结果
        /// </summary>
        [XmlElement("datas")]
        public Top.Api.Domain.SendMessageResult Datas { get; set; }

        /// <summary>
        /// 信息
        /// </summary>
        [XmlElement("message")]
        public string Message { get; set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        [XmlElement("successful")]
        public bool Successful { get; set; }
    }
}
