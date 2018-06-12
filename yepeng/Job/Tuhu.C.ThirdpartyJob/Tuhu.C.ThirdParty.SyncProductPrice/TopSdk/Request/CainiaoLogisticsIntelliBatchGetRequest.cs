using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: cainiao.logistics.intelli.batch.get
    /// </summary>
    public class CainiaoLogisticsIntelliBatchGetRequest : BaseTopRequest<Top.Api.Response.CainiaoLogisticsIntelliBatchGetResponse>
    {
        /// <summary>
        /// 批量智选物流请求参数
        /// </summary>
        public string IntelliLogisticsParams { get; set; }

        public List<IntelliLogisticsParamDomain> IntelliLogisticsParams_ { set { this.IntelliLogisticsParams = TopUtils.ObjectToJson(value); } } 

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "cainiao.logistics.intelli.batch.get";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("intelli_logistics_params", this.IntelliLogisticsParams);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("intelli_logistics_params", this.IntelliLogisticsParams);
            RequestValidator.ValidateObjectMaxListSize("intelli_logistics_params", this.IntelliLogisticsParams, 10);
        }

	/// <summary>
/// IntelliLogisticsParamDomain Data Structure.
/// </summary>
[Serializable]

public class IntelliLogisticsParamDomain : TopObject
{
	        /// <summary>
	        /// 发货地城市名称
	        /// </summary>
	        [XmlElement("from_city")]
	        public string FromCity { get; set; }
	
	        /// <summary>
	        /// 发货地的详细地址
	        /// </summary>
	        [XmlElement("from_detail_address")]
	        public string FromDetailAddress { get; set; }
	
	        /// <summary>
	        /// 发货地的区名称
	        /// </summary>
	        [XmlElement("from_district")]
	        public string FromDistrict { get; set; }
	
	        /// <summary>
	        /// 发货地省份名称
	        /// </summary>
	        [XmlElement("from_prov")]
	        public string FromProv { get; set; }
	
	        /// <summary>
	        /// 淘宝交易订单id（为了更好效果，推荐填写）
	        /// </summary>
	        [XmlElement("order_id")]
	        public Nullable<long> OrderId { get; set; }
	
	        /// <summary>
	        /// 商家id(主账号id)
	        /// </summary>
	        [XmlElement("seller_id")]
	        public Nullable<long> SellerId { get; set; }
	
	        /// <summary>
	        /// 到货地城市名称
	        /// </summary>
	        [XmlElement("to_city")]
	        public string ToCity { get; set; }
	
	        /// <summary>
	        /// 到货地的详细地址
	        /// </summary>
	        [XmlElement("to_detail_address")]
	        public string ToDetailAddress { get; set; }
	
	        /// <summary>
	        /// 到货地址的区名称
	        /// </summary>
	        [XmlElement("to_district")]
	        public string ToDistrict { get; set; }
	
	        /// <summary>
	        /// 到货地省份名称
	        /// </summary>
	        [XmlElement("to_prov")]
	        public string ToProv { get; set; }
}

        #endregion
    }
}
