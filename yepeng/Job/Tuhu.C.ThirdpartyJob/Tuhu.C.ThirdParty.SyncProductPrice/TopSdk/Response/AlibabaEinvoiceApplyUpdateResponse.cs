using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// AlibabaEinvoiceApplyUpdateResponse.
    /// </summary>
    public class AlibabaEinvoiceApplyUpdateResponse : TopResponse
    {
        /// <summary>
        /// 开票审核是否成功
        /// </summary>
        [XmlElement("is_success")]
        public bool IsSuccess { get; set; }

    }
}
