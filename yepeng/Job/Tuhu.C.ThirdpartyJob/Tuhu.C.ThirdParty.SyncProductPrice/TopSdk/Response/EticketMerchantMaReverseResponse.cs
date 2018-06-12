using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// EticketMerchantMaReverseResponse.
    /// </summary>
    public class EticketMerchantMaReverseResponse : TopResponse
    {
        /// <summary>
        /// 回复结果
        /// </summary>
        [XmlElement("resp_body")]
        public ReverseMaCallbackRespDomain RespBody { get; set; }

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
/// ReverseMaCallbackRespDomain Data Structure.
/// </summary>
[Serializable]

public class ReverseMaCallbackRespDomain : TopObject
{
	        /// <summary>
	        /// 业务参数KV
	        /// </summary>
	        [XmlElement("attribute_map")]
	        public string AttributeMap { get; set; }
}

    }
}
