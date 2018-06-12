using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// LocationRelationQueryResponse.
    /// </summary>
    public class LocationRelationQueryResponse : TopResponse
    {
        /// <summary>
        /// result
        /// </summary>
        [XmlElement("result")]
        public SingleResultDomain Result { get; set; }

	/// <summary>
/// LocationRelationDtoDomain Data Structure.
/// </summary>
[Serializable]

public class LocationRelationDtoDomain : TopObject
{
	        /// <summary>
	        /// 实体类型 2：仓库  6：门店
	        /// </summary>
	        [XmlElement("source_inv_store_type")]
	        public long SourceInvStoreType { get; set; }
	
	        /// <summary>
	        /// 实体code
	        /// </summary>
	        [XmlElement("source_store_code")]
	        public string SourceStoreCode { get; set; }
	
	        /// <summary>
	        /// 状态 0 正常  -1 删除
	        /// </summary>
	        [XmlElement("status")]
	        public long Status { get; set; }
	
	        /// <summary>
	        /// 实体类型 2：仓库  6：门店
	        /// </summary>
	        [XmlElement("target_inv_store_type")]
	        public long TargetInvStoreType { get; set; }
	
	        /// <summary>
	        /// 实体code
	        /// </summary>
	        [XmlElement("target_store_code")]
	        public string TargetStoreCode { get; set; }
}

	/// <summary>
/// SingleResultDomain Data Structure.
/// </summary>
[Serializable]

public class SingleResultDomain : TopObject
{
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
	        /// 地点关系
	        /// </summary>
	        [XmlArray("location_relation_list")]
	        [XmlArrayItem("location_relation_dto")]
	        public List<LocationRelationDtoDomain> LocationRelationList { get; set; }
	
	        /// <summary>
	        /// 是否成功
	        /// </summary>
	        [XmlElement("success")]
	        public bool Success { get; set; }
}

    }
}
