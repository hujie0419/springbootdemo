using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.eticket.merchant.tbma.get
    /// </summary>
    public class EticketMerchantTbmaGetRequest : BaseTopRequest<Top.Api.Response.EticketMerchantTbmaGetResponse>
    {
        /// <summary>
        /// 查询淘宝码请求
        /// </summary>
        public string QueryTbMaCallbackReq { get; set; }

        public QueryTbMaCallbackReqDomain QueryTbMaCallbackReq_ { set { this.QueryTbMaCallbackReq = TopUtils.ObjectToJson(value); } } 

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.eticket.merchant.tbma.get";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("query_tb_ma_callback_req", this.QueryTbMaCallbackReq);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("query_tb_ma_callback_req", this.QueryTbMaCallbackReq);
        }

	/// <summary>
/// QueryTbMaCallbackReqDomain Data Structure.
/// </summary>
[Serializable]

public class QueryTbMaCallbackReqDomain : TopObject
{
	        /// <summary>
	        /// 淘宝码值
	        /// </summary>
	        [XmlElement("code")]
	        public string Code { get; set; }
}

        #endregion
    }
}
