using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// InventoryMerchantAdjustResponse.
    /// </summary>
    public class InventoryMerchantAdjustResponse : TopResponse
    {
        /// <summary>
        /// result
        /// </summary>
        [XmlElement("result")]
        public SingleResultDomain Result { get; set; }

	/// <summary>
/// InventoryCheckResultDtoDomain Data Structure.
/// </summary>
[Serializable]

public class InventoryCheckResultDtoDomain : TopObject
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
	        /// 每个货品的调整子单据号，作为业务调整依据，处理时会幂等
	        /// </summary>
	        [XmlElement("sub_order_id")]
	        public string SubOrderId { get; set; }
	
	        /// <summary>
	        /// 每个子调整单据是否成功
	        /// </summary>
	        [XmlElement("success")]
	        public bool Success { get; set; }
}

	/// <summary>
/// SingleResultDomain Data Structure.
/// </summary>
[Serializable]

public class SingleResultDomain : TopObject
{
	        /// <summary>
	        /// data
	        /// </summary>
	        [XmlArray("adjust_results")]
	        [XmlArrayItem("inventory_check_result_dto")]
	        public List<InventoryCheckResultDtoDomain> AdjustResults { get; set; }
	
	        /// <summary>
	        /// 错误码
	        /// </summary>
	        [XmlElement("error_code")]
	        public string ErrorCode { get; set; }
	
	        /// <summary>
	        /// 错误信息
	        /// </summary>
	        [XmlElement("error_message")]
	        public string ErrorMessage { get; set; }
	
	        /// <summary>
	        /// 如果是失败，可能是部分失败。如果是成功，则全部成功
	        /// </summary>
	        [XmlElement("success")]
	        public bool Success { get; set; }
}

    }
}
