using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.location.relation.query
    /// </summary>
    public class LocationRelationQueryRequest : BaseTopRequest<Top.Api.Response.LocationRelationQueryResponse>
    {
        /// <summary>
        /// 关系查询
        /// </summary>
        public string LocationRelation { get; set; }

        public LocationRelationDtoDomain LocationRelation_ { set { this.LocationRelation = TopUtils.ObjectToJson(value); } } 

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.location.relation.query";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("location_relation", this.LocationRelation);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("location_relation", this.LocationRelation);
        }

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
	        public Nullable<long> SourceInvStoreType { get; set; }
	
	        /// <summary>
	        /// 实体code
	        /// </summary>
	        [XmlElement("source_store_code")]
	        public string SourceStoreCode { get; set; }
	
	        /// <summary>
	        /// 实体类型 2：仓库  6：门店 （target,sorce 二选一填写，都填写报错）
	        /// </summary>
	        [XmlElement("target_inv_store_type")]
	        public Nullable<long> TargetInvStoreType { get; set; }
	
	        /// <summary>
	        /// 实体code
	        /// </summary>
	        [XmlElement("target_store_code")]
	        public string TargetStoreCode { get; set; }
}

        #endregion
    }
}
