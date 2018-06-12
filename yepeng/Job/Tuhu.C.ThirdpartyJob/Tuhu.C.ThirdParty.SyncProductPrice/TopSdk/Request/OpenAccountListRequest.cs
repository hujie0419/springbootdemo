using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.open.account.list
    /// </summary>
    public class OpenAccountListRequest : BaseTopRequest<Top.Api.Response.OpenAccountListResponse>
    {
        /// <summary>
        /// ISV自己账号的id列表，isvAccountId和openAccountId二选一必填
        /// </summary>
        public string IsvAccountIds { get; set; }

        /// <summary>
        /// Open Account的id列表
        /// </summary>
        public string OpenAccountIds { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.open.account.list";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("isv_account_ids", this.IsvAccountIds);
            parameters.Add("open_account_ids", this.OpenAccountIds);
            if (this.OtherParams != null)
            {
                parameters.AddAll(this.OtherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateMaxListSize("isv_account_ids", this.IsvAccountIds, 20);
            RequestValidator.ValidateMaxListSize("open_account_ids", this.OpenAccountIds, 20);
        }

        #endregion
    }
}
