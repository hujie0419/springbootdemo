using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// AlibabaScmExternalWmsInventoryStockOutNotifyResponse.
    /// </summary>
    public class AlibabaScmExternalWmsInventoryStockOutNotifyResponse : TopResponse
    {
        /// <summary>
        /// 出参响应信息
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
	        /// 错误详细信息
	        /// </summary>
	        [XmlElement("error_msg")]
	        public string ErrorMsg { get; set; }
	
	        /// <summary>
	        /// 单号
	        /// </summary>
	        [XmlElement("order_code")]
	        public string OrderCode { get; set; }
	
	        /// <summary>
	        /// 是否成功标示
	        /// </summary>
	        [XmlElement("success")]
	        public bool Success { get; set; }
}

    }
}
