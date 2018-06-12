using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// EticketMerchantTbmaGetResponse.
    /// </summary>
    public class EticketMerchantTbmaGetResponse : TopResponse
    {
        /// <summary>
        /// respBody
        /// </summary>
        [XmlElement("resp_body")]
        public QueryTbMaCallbackRespDomain RespBody { get; set; }

        /// <summary>
        /// subCode
        /// </summary>
        [XmlElement("ret_code")]
        public string RetCode { get; set; }

        /// <summary>
        /// subMsg
        /// </summary>
        [XmlElement("ret_msg")]
        public string RetMsg { get; set; }

	/// <summary>
/// AttributesDomain Data Structure.
/// </summary>
[Serializable]

public class AttributesDomain : TopObject
{
	        /// <summary>
	        /// itemId
	        /// </summary>
	        [XmlElement("item_id")]
	        public string ItemId { get; set; }
}

	/// <summary>
/// CertificateDtoDomain Data Structure.
/// </summary>
[Serializable]

public class CertificateDtoDomain : TopObject
{
	        /// <summary>
	        /// attributes
	        /// </summary>
	        [XmlElement("attributes")]
	        public AttributesDomain Attributes { get; set; }
	
	        /// <summary>
	        /// availableNum
	        /// </summary>
	        [XmlElement("available_num")]
	        public long AvailableNum { get; set; }
	
	        /// <summary>
	        /// bizType
	        /// </summary>
	        [XmlElement("biz_type")]
	        public long BizType { get; set; }
	
	        /// <summary>
	        /// code
	        /// </summary>
	        [XmlElement("code")]
	        public string Code { get; set; }
	
	        /// <summary>
	        /// codeStatus
	        /// </summary>
	        [XmlElement("code_status")]
	        public long CodeStatus { get; set; }
	
	        /// <summary>
	        /// endTime
	        /// </summary>
	        [XmlElement("end_time")]
	        public string EndTime { get; set; }
	
	        /// <summary>
	        /// initialNum
	        /// </summary>
	        [XmlElement("initial_num")]
	        public long InitialNum { get; set; }
	
	        /// <summary>
	        /// lockedNum
	        /// </summary>
	        [XmlElement("locked_num")]
	        public long LockedNum { get; set; }
	
	        /// <summary>
	        /// outerId
	        /// </summary>
	        [XmlElement("outer_id")]
	        public string OuterId { get; set; }
	
	        /// <summary>
	        /// qrCodeUrl
	        /// </summary>
	        [XmlElement("qr_code_url")]
	        public string QrCodeUrl { get; set; }
	
	        /// <summary>
	        /// startTime
	        /// </summary>
	        [XmlElement("start_time")]
	        public string StartTime { get; set; }
	
	        /// <summary>
	        /// usedNum
	        /// </summary>
	        [XmlElement("used_num")]
	        public long UsedNum { get; set; }
}

	/// <summary>
/// QueryTbMaCallbackRespDomain Data Structure.
/// </summary>
[Serializable]

public class QueryTbMaCallbackRespDomain : TopObject
{
	        /// <summary>
	        /// certificateDTO
	        /// </summary>
	        [XmlElement("certificate")]
	        public CertificateDtoDomain Certificate { get; set; }
}

    }
}
