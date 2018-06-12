using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// EticketMerchantMaResendResponse.
    /// </summary>
    public class EticketMerchantMaResendResponse : TopResponse
    {
        /// <summary>
        /// 回复参数
        /// </summary>
        [XmlElement("resp_body")]
        public SendMaCallbackRespDomain RespBody { get; set; }

        /// <summary>
        /// 子结果码
        /// </summary>
        [XmlElement("ret_code")]
        public string RetCode { get; set; }

        /// <summary>
        /// 子结果信息
        /// </summary>
        [XmlElement("ret_msg")]
        public string RetMsg { get; set; }

	/// <summary>
/// SendMaCallbackRespDomain Data Structure.
/// </summary>
[Serializable]

public class SendMaCallbackRespDomain : TopObject
{
	        /// <summary>
	        /// 回复业务KV
	        /// </summary>
	        [XmlElement("attribute_map")]
	        public string AttributeMap { get; set; }
}

    }
}
