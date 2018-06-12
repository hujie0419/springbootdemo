using System;
using System.Xml.Serialization;

namespace Top.Api.Response
{
    /// <summary>
    /// OpenSmsSendvercodeResponse.
    /// </summary>
    public class OpenSmsSendvercodeResponse : TopResponse
    {
        /// <summary>
        /// 返回结果
        /// </summary>
        [XmlElement("result")]
        public Top.Api.Domain.BmcResult Result { get; set; }

    }
}
