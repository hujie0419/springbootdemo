using System;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: alibaba.einvoice.apply.test
    /// </summary>
    public class AlibabaEinvoiceApplyTestRequest : BaseTopRequest<Top.Api.Response.AlibabaEinvoiceApplyTestResponse>
    {
        /// <summary>
        /// 0=个人，1=企业
        /// </summary>
        public Nullable<long> BusinessType { get; set; }

        /// <summary>
        /// 买家抬头
        /// </summary>
        public string PayerName { get; set; }

        /// <summary>
        /// 买家税号
        /// </summary>
        public string PayerRegisterNo { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public string PlatformTid { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "alibaba.einvoice.apply.test";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("business_type", this.BusinessType);
            parameters.Add("payer_name", this.PayerName);
            parameters.Add("payer_register_no", this.PayerRegisterNo);
            parameters.Add("platform_tid", this.PlatformTid);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("business_type", this.BusinessType);
            RequestValidator.ValidateRequired("payer_name", this.PayerName);
            RequestValidator.ValidateRequired("platform_tid", this.PlatformTid);
        }

        #endregion
    }
}
