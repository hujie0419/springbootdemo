using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// CainiaoBmsOrderConsignConfirmResponse.
    /// </summary>
    public class CainiaoBmsOrderConsignConfirmResponse : TopResponse
    {
        /// <summary>
        /// 返回值
        /// </summary>
        [XmlElement("result")]
        public ResultDODomain Result { get; set; }

	/// <summary>
/// ResultDODomain Data Structure.
/// </summary>
[Serializable]

public class ResultDODomain : TopObject
{
	        /// <summary>
	        /// 01
	        /// </summary>
	        [XmlElement("error_code")]
	        public string ErrorCode { get; set; }
	
	        /// <summary>
	        /// 网络延时
	        /// </summary>
	        [XmlElement("error_msg")]
	        public string ErrorMsg { get; set; }
	
	        /// <summary>
	        /// 成功、失败
	        /// </summary>
	        [XmlElement("success")]
	        public bool Success { get; set; }
}

    }
}
