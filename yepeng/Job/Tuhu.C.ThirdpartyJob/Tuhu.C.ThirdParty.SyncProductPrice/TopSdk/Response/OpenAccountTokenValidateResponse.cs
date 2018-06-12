using System;
using System.Xml.Serialization;

namespace Top.Api.Response
{
    /// <summary>
    /// OpenAccountTokenValidateResponse.
    /// </summary>
    public class OpenAccountTokenValidateResponse : TopResponse
    {
        /// <summary>
        /// 验证成功返回token中的信息
        /// </summary>
        [XmlElement("data")]
        public Top.Api.Domain.OpenAccountTokenValidateResult Data { get; set; }

    }
}
