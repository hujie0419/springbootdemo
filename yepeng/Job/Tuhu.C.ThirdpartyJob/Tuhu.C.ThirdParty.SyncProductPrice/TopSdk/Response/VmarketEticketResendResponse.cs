using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// VmarketEticketResendResponse.
    /// </summary>
    public class VmarketEticketResendResponse : TopResponse
    {
        /// <summary>
        /// 0:失败，1:成功
        /// </summary>
        [XmlElement("ret_code")]
        public long RetCode { get; set; }

    }
}
