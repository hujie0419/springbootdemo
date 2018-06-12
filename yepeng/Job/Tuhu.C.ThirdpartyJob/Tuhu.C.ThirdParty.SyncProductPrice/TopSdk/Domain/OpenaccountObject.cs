using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// OpenaccountObject Data Structure.
    /// </summary>
    [Serializable]
    public class OpenaccountObject : TopObject
    {
        /// <summary>
        /// 错误码
        /// </summary>
        [XmlElement("code")]
        public long Code { get; set; }

        /// <summary>
        /// 返回数据
        /// </summary>
        [XmlElement("data")]
        public Top.Api.Domain.OpenAccount Data { get; set; }

        /// <summary>
        /// 返回信息
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
