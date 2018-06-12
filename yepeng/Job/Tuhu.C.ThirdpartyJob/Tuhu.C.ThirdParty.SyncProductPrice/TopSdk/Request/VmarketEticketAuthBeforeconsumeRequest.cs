using System;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.vmarket.eticket.auth.beforeconsume
    /// </summary>
    public class VmarketEticketAuthBeforeconsumeRequest : BaseTopRequest<Top.Api.Response.VmarketEticketAuthBeforeconsumeResponse>
    {
        /// <summary>
        /// 核销方的ID，如果是普通码商必须传入机具ID,如果是私有码商家（即原有的信任商家）可默认传入私有码商ID
        /// </summary>
        public string Operatorid { get; set; }

        /// <summary>
        /// 网点ID,网点授权核销时，必须传入；其他核销方式可不传
        /// </summary>
        public string Storeid { get; set; }

        /// <summary>
        /// 核销的码，只支持单个码，多个码核销需要多次调用
        /// </summary>
        public string VerifyCode { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.vmarket.eticket.auth.beforeconsume";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("operatorid", this.Operatorid);
            parameters.Add("storeid", this.Storeid);
            parameters.Add("verify_code", this.VerifyCode);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("operatorid", this.Operatorid);
            RequestValidator.ValidateRequired("verify_code", this.VerifyCode);
        }

        #endregion
    }
}
