using System;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.vmarket.eticket.beforeconsume
    /// </summary>
    public class VmarketEticketBeforeconsumeRequest : BaseTopRequest<Top.Api.Response.VmarketEticketBeforeconsumeResponse>
    {
        /// <summary>
        /// 码商ID,是码商的话必须传递,如果是信任卖家不需要传
        /// </summary>
        public Nullable<long> CodemerchantId { get; set; }

        /// <summary>
        /// 手机号码后四位,没有特殊说明请不要传
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 需要验码的电子凭证订单ID
        /// </summary>
        public Nullable<long> OrderId { get; set; }

        /// <summary>
        /// 操作员身份ID，如果是码商必须传,如果是信任卖家不需要传
        /// </summary>
        public string Posid { get; set; }

        /// <summary>
        /// 安全验证token，需要和发码通知中的token一致
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 需要验的码
        /// </summary>
        public string VerifyCode { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.vmarket.eticket.beforeconsume";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("codemerchant_id", this.CodemerchantId);
            parameters.Add("mobile", this.Mobile);
            parameters.Add("order_id", this.OrderId);
            parameters.Add("posid", this.Posid);
            parameters.Add("token", this.Token);
            parameters.Add("verify_code", this.VerifyCode);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("order_id", this.OrderId);
            RequestValidator.ValidateRequired("token", this.Token);
            RequestValidator.ValidateRequired("verify_code", this.VerifyCode);
        }

        #endregion
    }
}
