using System;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.vmarket.eticket.flow.resend
    /// </summary>
    public class VmarketEticketFlowResendRequest : BaseTopRequest<Top.Api.Response.VmarketEticketFlowResendResponse>
    {
        /// <summary>
        /// 业务类型值，可联系淘宝业务运营取得具体值
        /// </summary>
        public Nullable<long> BizType { get; set; }

        /// <summary>
        /// 业务单号
        /// </summary>
        public string OuterId { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.vmarket.eticket.flow.resend";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("biz_type", this.BizType);
            parameters.Add("outer_id", this.OuterId);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("biz_type", this.BizType);
            RequestValidator.ValidateRequired("outer_id", this.OuterId);
        }

        #endregion
    }
}
