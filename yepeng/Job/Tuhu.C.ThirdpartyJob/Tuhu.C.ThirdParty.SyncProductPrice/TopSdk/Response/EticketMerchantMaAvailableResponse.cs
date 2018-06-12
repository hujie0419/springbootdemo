using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// EticketMerchantMaAvailableResponse.
    /// </summary>
    public class EticketMerchantMaAvailableResponse : TopResponse
    {
        /// <summary>
        /// 回复结果
        /// </summary>
        [XmlElement("resp_body")]
        public ConsumeMaCallbackRespDomain RespBody { get; set; }

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
/// ConsumeMaCallbackRespDomain Data Structure.
/// </summary>
[Serializable]

public class ConsumeMaCallbackRespDomain : TopObject
{
	        /// <summary>
	        /// 业务回复KV
	        /// </summary>
	        [XmlElement("attribute_map")]
	        public string AttributeMap { get; set; }
}

    }
}
