using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: alibaba.scm.external.wms.order.cancel.notify
    /// </summary>
    public class AlibabaScmExternalWmsOrderCancelNotifyRequest : BaseTopRequest<Top.Api.Response.AlibabaScmExternalWmsOrderCancelNotifyResponse>
    {
        /// <summary>
        /// 请求参数
        /// </summary>
        public string Request { get; set; }

        public StructDomain Request_ { set { this.Request = TopUtils.ObjectToJson(value); } } 

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "alibaba.scm.external.wms.order.cancel.notify";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("request", this.Request);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("request", this.Request);
        }

	/// <summary>
/// StructDomain Data Structure.
/// </summary>
[Serializable]

public class StructDomain : TopObject
{
	        /// <summary>
	        /// 业务类型
	        /// </summary>
	        [XmlElement("biz_type")]
	        public Nullable<long> BizType { get; set; }
	
	        /// <summary>
	        /// wms对应单据号
	        /// </summary>
	        [XmlElement("order_code")]
	        public string OrderCode { get; set; }
	
	        /// <summary>
	        /// 订单类型：采购入库单（601）、调拨入库单（302）、退供出库单（901）、调拨出库单（301）、发货单（201）、消退单（501）
	        /// </summary>
	        [XmlElement("order_type")]
	        public string OrderType { get; set; }
	
	        /// <summary>
	        /// erp对应单据号
	        /// </summary>
	        [XmlElement("out_order_code")]
	        public string OutOrderCode { get; set; }
	
	        /// <summary>
	        /// 货主ID
	        /// </summary>
	        [XmlElement("owner_user_id")]
	        public string OwnerUserId { get; set; }
	
	        /// <summary>
	        /// 店铺id
	        /// </summary>
	        [XmlElement("user_id")]
	        public string UserId { get; set; }
}

        #endregion
    }
}
