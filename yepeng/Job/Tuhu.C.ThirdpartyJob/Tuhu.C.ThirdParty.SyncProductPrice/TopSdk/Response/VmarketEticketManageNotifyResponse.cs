using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// VmarketEticketManageNotifyResponse.
    /// </summary>
    public class VmarketEticketManageNotifyResponse : TopResponse
    {
        /// <summary>
        /// 1:成功
        /// </summary>
        [XmlElement("ret_code")]
        public long RetCode { get; set; }

    }
}
