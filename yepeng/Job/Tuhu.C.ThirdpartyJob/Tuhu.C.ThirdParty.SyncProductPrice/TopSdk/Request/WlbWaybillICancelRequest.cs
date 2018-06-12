using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.wlb.waybill.i.cancel
    /// </summary>
    public class WlbWaybillICancelRequest : BaseTopRequest<Top.Api.Response.WlbWaybillICancelResponse>
    {
        /// <summary>
        /// 取消接口入参
        /// </summary>
        public string WaybillApplyCancelRequest { get; set; }

        public WaybillApplyCancelRequestDomain WaybillApplyCancelRequest_ { set { this.WaybillApplyCancelRequest = TopUtils.ObjectToJson(value); } } 

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.wlb.waybill.i.cancel";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("waybill_apply_cancel_request", this.WaybillApplyCancelRequest);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("waybill_apply_cancel_request", this.WaybillApplyCancelRequest);
        }

	/// <summary>
/// WaybillApplyCancelRequestDomain Data Structure.
/// </summary>
[Serializable]

public class WaybillApplyCancelRequestDomain : TopObject
{
	        /// <summary>
	        /// CP快递公司编码
	        /// </summary>
	        [XmlElement("cp_code")]
	        public string CpCode { get; set; }
	
	        /// <summary>
	        /// ERP订单号或包裹号
	        /// </summary>
	        [XmlElement("package_id")]
	        public string PackageId { get; set; }
	
	        /// <summary>
	        /// 面单使用者编号
	        /// </summary>
	        [XmlElement("real_user_id")]
	        public Nullable<long> RealUserId { get; set; }
	
	        /// <summary>
	        /// 交易订单列表
	        /// </summary>
	        [XmlArray("trade_order_list")]
	        [XmlArrayItem("string")]
	        public List<string> TradeOrderList { get; set; }
	
	        /// <summary>
	        /// 电子面单号码
	        /// </summary>
	        [XmlElement("waybill_code")]
	        public string WaybillCode { get; set; }
}

        #endregion
    }
}
