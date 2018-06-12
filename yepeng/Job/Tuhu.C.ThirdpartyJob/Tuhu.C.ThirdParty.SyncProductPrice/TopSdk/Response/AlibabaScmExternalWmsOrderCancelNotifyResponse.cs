using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// AlibabaScmExternalWmsOrderCancelNotifyResponse.
    /// </summary>
    public class AlibabaScmExternalWmsOrderCancelNotifyResponse : TopResponse
    {
        /// <summary>
        /// 返回参数
        /// </summary>
        [XmlElement("response")]
        public StructDomain Response { get; set; }

	/// <summary>
/// StructDomain Data Structure.
/// </summary>
[Serializable]

public class StructDomain : TopObject
{
	        /// <summary>
	        /// 错误码
	        /// </summary>
	        [XmlElement("error_code")]
	        public string ErrorCode { get; set; }
	
	        /// <summary>
	        /// 错误信息
	        /// </summary>
	        [XmlElement("error_msg")]
	        public string ErrorMsg { get; set; }
	
	        /// <summary>
	        /// success
	        /// </summary>
	        [XmlElement("success")]
	        public bool Success { get; set; }
}

    }
}
