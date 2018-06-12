using System;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.vmarket.eticket.flow.consume
    /// </summary>
    public class VmarketEticketFlowConsumeRequest : BaseTopRequest<Top.Api.Response.VmarketEticketFlowConsumeResponse>
    {
        /// <summary>
        /// 淘宝业务提供的业务类型值，请联系相关业务运营取得
        /// </summary>
        public Nullable<long> BizType { get; set; }

        /// <summary>
        /// 凭证码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 核销操作人
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// 业务单号
        /// </summary>
        public string OuterId { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.vmarket.eticket.flow.consume";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("biz_type", this.BizType);
            parameters.Add("code", this.Code);
            parameters.Add("operator", this.Operator);
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
            RequestValidator.ValidateRequired("code", this.Code);
            RequestValidator.ValidateRequired("outer_id", this.OuterId);
        }

        #endregion
    }
}
