using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.open.account.token.validate
    /// </summary>
    public class OpenAccountTokenValidateRequest : BaseTopRequest<Top.Api.Response.OpenAccountTokenValidateResponse>
    {
        /// <summary>
        /// token
        /// </summary>
        public string ParamToken { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.open.account.token.validate";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("param_token", this.ParamToken);
            if (this.OtherParams != null)
            {
                parameters.AddAll(this.OtherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("param_token", this.ParamToken);
        }

        #endregion
    }
}
