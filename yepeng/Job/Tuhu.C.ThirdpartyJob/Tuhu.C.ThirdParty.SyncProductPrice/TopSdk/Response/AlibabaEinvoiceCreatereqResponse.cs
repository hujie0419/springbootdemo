using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// AlibabaEinvoiceCreatereqResponse.
    /// </summary>
    public class AlibabaEinvoiceCreatereqResponse : TopResponse
    {
        /// <summary>
        /// 开票信息是否成功接受
        /// </summary>
        [XmlElement("is_success")]
        public bool IsSuccess { get; set; }

    }
}
