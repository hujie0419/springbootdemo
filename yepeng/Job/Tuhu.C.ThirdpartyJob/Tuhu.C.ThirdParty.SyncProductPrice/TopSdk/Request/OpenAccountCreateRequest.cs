using System;
using Top.Api.Domain;
using System.Collections.Generic;
using System.Xml.Serialization;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.open.account.create
    /// </summary>
    public class OpenAccountCreateRequest : BaseTopRequest<Top.Api.Response.OpenAccountCreateResponse>
    {
        /// <summary>
        /// Open Account的列表
        /// </summary>
        public string ParamList { get; set; }

        public List<OpenAccount> ParamList_ { set { this.ParamList = TopUtils.ObjectToJson(value); } } 

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.open.account.create";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("param_list", this.ParamList);
            if (this.OtherParams != null)
            {
                parameters.AddAll(this.OtherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateObjectMaxListSize("param_list", this.ParamList, 20);
        }

        #endregion
    }
}
