using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// AlibabaScmExternalWmsOrderNotifyResponse.
    /// </summary>
    public class AlibabaScmExternalWmsOrderNotifyResponse : TopResponse
    {
        /// <summary>
        /// 返回对象
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
	        /// 仓库订单编号
	        /// </summary>
	        [XmlElement("order_code")]
	        public string OrderCode { get; set; }
	
	        /// <summary>
	        /// 返回结果
	        /// </summary>
	        [XmlElement("success")]
	        public bool Success { get; set; }
}

    }
}
