using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// VmarketEticketFlowConsumeResponse.
    /// </summary>
    public class VmarketEticketFlowConsumeResponse : TopResponse
    {
        /// <summary>
        /// 错误提示信息
        /// </summary>
        [XmlElement("error_msg")]
        public string ErrorMsg { get; set; }

        /// <summary>
        /// 执行成功
        /// </summary>
        [XmlElement("ret_code")]
        public long RetCode { get; set; }

    }
}
