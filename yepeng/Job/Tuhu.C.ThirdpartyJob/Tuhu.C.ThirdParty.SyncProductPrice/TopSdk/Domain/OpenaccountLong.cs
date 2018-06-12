using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// OpenaccountLong Data Structure.
    /// </summary>
    [Serializable]
    public class OpenaccountLong : TopObject
    {
        /// <summary>
        /// 返回码
        /// </summary>
        [XmlElement("code")]
        public long Code { get; set; }

        /// <summary>
        /// 返回数据
        /// </summary>
        [XmlElement("data")]
        public long Data { get; set; }

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
