using System;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: alibaba.einvoice.apply.update
    /// </summary>
    public class AlibabaEinvoiceApplyUpdateRequest : BaseTopRequest<Top.Api.Response.AlibabaEinvoiceApplyUpdateResponse>
    {
        /// <summary>
        /// 开票申请ID
        /// </summary>
        public string ApplyId { get; set; }

        /// <summary>
        /// 订单所属平台
        /// </summary>
        public string PlatformCode { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public string PlatformTid { get; set; }

        /// <summary>
        /// 拒绝申请的原因
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// 审核结果，0-拒绝，2-同意
        /// </summary>
        public Nullable<long> Status { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "alibaba.einvoice.apply.update";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("apply_id", this.ApplyId);
            parameters.Add("platform_code", this.PlatformCode);
            parameters.Add("platform_tid", this.PlatformTid);
            parameters.Add("reason", this.Reason);
            parameters.Add("status", this.Status);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("apply_id", this.ApplyId);
            RequestValidator.ValidateRequired("platform_code", this.PlatformCode);
            RequestValidator.ValidateRequired("platform_tid", this.PlatformTid);
            RequestValidator.ValidateRequired("status", this.Status);
        }

        #endregion
    }
}
