using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// AlibabaEinvoiceApplyTestResponse.
    /// </summary>
    public class AlibabaEinvoiceApplyTestResponse : TopResponse
    {
        /// <summary>
        /// 消息是否发送成功
        /// </summary>
        [XmlElement("is_success")]
        public bool IsSuccess { get; set; }

    }
}
