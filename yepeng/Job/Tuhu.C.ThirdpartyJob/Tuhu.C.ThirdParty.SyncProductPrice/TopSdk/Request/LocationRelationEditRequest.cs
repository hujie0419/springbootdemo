using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.location.relation.edit
    /// </summary>
    public class LocationRelationEditRequest : BaseTopRequest<Top.Api.Response.LocationRelationEditResponse>
    {
        /// <summary>
        /// 关系对象列表
        /// </summary>
        public string LocationRelationList { get; set; }

        public List<LocationRelationDtoDomain> LocationRelationList_ { set { this.LocationRelationList = TopUtils.ObjectToJson(value); } } 

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.location.relation.edit";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("location_relation_list", this.LocationRelationList);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("location_relation_list", this.LocationRelationList);
            RequestValidator.ValidateObjectMaxListSize("location_relation_list", this.LocationRelationList, 20);
        }

	/// <summary>
/// LocationRelationDtoDomain Data Structure.
/// </summary>
[Serializable]

public class LocationRelationDtoDomain : TopObject
{
	        /// <summary>
	        /// 实体类型 2：仓库 6：门店
	        /// </summary>
	        [XmlElement("source_inv_store_type")]
	        public Nullable<long> SourceInvStoreType { get; set; }
	
	        /// <summary>
	        /// 实体code
	        /// </summary>
	        [XmlElement("source_store_code")]
	        public string SourceStoreCode { get; set; }
	
	        /// <summary>
	        /// 状态  0 正常  -1 删除
	        /// </summary>
	        [XmlElement("status")]
	        public Nullable<long> Status { get; set; }
	
	        /// <summary>
	        /// 实体类型 2：仓库 6：门店
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
