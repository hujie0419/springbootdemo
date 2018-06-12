using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// EticketMerchantMaConsumeResponse.
    /// </summary>
    public class EticketMerchantMaConsumeResponse : TopResponse
    {
        /// <summary>
        /// 系统自动生成
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
	        /// 系统自动生成
	        /// </summary>
	        [XmlElement("attribute_map")]
	        public string AttributeMap { get; set; }
}

    }
}
