using System;
using System.Xml.Serialization;

namespace Top.Api.Response
{
    /// <summary>
    /// VmarketEticketQrcodeImguploadResponse.
    /// </summary>
    public class VmarketEticketQrcodeImguploadResponse : TopResponse
    {
        /// <summary>
        /// 1:成功  其它为失败
        /// </summary>
        [XmlElement("ret_code")]
        public long RetCode { get; set; }
    }
}
