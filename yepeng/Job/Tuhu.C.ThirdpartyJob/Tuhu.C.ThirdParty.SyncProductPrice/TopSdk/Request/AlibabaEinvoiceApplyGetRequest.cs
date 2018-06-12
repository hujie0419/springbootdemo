using System;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: alibaba.einvoice.apply.get
    /// </summary>
    public class AlibabaEinvoiceApplyGetRequest : BaseTopRequest<Top.Api.Response.AlibabaEinvoiceApplyGetResponse>
    {
        /// <summary>
        /// 开票申请ID，跟消息中的apply_id对应，传入applyId后，只会返回一条开票申请消息
        /// </summary>
        public string ApplyId { get; set; }

        /// <summary>
        /// 平台订单号
        /// </summary>
        public string PlatformTid { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "alibaba.einvoice.apply.get";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("apply_id", this.ApplyId);
            parameters.Add("platform_tid", this.PlatformTid);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("platform_tid", this.PlatformTid);
        }

        #endregion
    }
}
