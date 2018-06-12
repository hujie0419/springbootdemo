using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// OpenaccountVoid Data Structure.
    /// </summary>
    [Serializable]
    public class OpenaccountVoid : TopObject
    {
        /// <summary>
        /// 错误码
        /// </summary>
        [XmlElement("code")]
        public long Code { get; set; }

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
