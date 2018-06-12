using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// VmarketEticketFailsendResponse.
    /// </summary>
    public class VmarketEticketFailsendResponse : TopResponse
    {
        /// <summary>
        /// 成功
        /// </summary>
        [XmlElement("ret_code")]
        public long RetCode { get; set; }

    }
}
