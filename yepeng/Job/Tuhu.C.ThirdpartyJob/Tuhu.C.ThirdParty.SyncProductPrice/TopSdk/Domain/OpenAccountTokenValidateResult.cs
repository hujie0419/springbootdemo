using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// OpenAccountTokenValidateResult Data Structure.
    /// </summary>
    [Serializable]
    public class OpenAccountTokenValidateResult : TopObject
    {
        /// <summary>
        /// 错误码
        /// </summary>
        [XmlElement("code")]
        public long Code { get; set; }

        /// <summary>
        /// token中的数据
        /// </summary>
        [XmlElement("data")]
        public Top.Api.Domain.TokenInfo Data { get; set; }

        /// <summary>
        /// 错误信息
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
