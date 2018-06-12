using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// VmarketEticketFlowResendResponse.
    /// </summary>
    public class VmarketEticketFlowResendResponse : TopResponse
    {
        /// <summary>
        /// 错误提示信息
        /// </summary>
        [XmlElement("error_msg")]
        public string ErrorMsg { get; set; }

        /// <summary>
        /// 1成功;0失败
        /// </summary>
        [XmlElement("ret_code")]
        public long RetCode { get; set; }

    }
}
