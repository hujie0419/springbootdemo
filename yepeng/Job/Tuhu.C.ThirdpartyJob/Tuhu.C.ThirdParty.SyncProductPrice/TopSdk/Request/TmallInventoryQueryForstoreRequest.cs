using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: tmall.inventory.query.forstore
    /// </summary>
    public class TmallInventoryQueryForstoreRequest : BaseTopRequest<Top.Api.Response.TmallInventoryQueryForstoreResponse>
    {
        /// <summary>
        /// 查询列表
        /// </summary>
        public string ParamList { get; set; }

        public List<InventoryQueryForStoreRequestDomain> ParamList_ { set { this.ParamList = TopUtils.ObjectToJson(value); } } 

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "tmall.inventory.query.forstore";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("param_list", this.ParamList);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("param_list", this.ParamList);
            RequestValidator.ValidateObjectMaxListSize("param_list", this.ParamList, 15);
        }

	/// <summary>
/// InventoryQueryForStoreRequestDomain Data Structure.
/// </summary>
[Serializable]

public class InventoryQueryForStoreRequestDomain : TopObject
{
	        /// <summary>
	        /// 实体类型  2：仓库类型， 6：门店类型
	        /// </summary>
	        [XmlElement("inv_store_type")]
	        public Nullable<long> InvStoreType { get; set; }
	
	        /// <summary>
	        /// 后端商品code
	        /// </summary>
	        [XmlElement("sc_item_code")]
	        public string ScItemCode { get; set; }
	
	        /// <summary>
	        /// 后端商品id
	        /// </summary>
	        [XmlElement("sc_item_id")]
	        public Nullable<long> ScItemId { get; set; }
	
	        /// <summary>
	        /// 仓库code
	        /// </summary>
	        [XmlElement("store_code")]
	        public string StoreCode { get; set; }
}

        #endregion
    }
}
