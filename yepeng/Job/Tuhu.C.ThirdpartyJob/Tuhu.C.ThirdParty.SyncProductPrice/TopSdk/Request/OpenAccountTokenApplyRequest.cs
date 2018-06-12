using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.open.account.token.apply
    /// </summary>
    public class OpenAccountTokenApplyRequest : BaseTopRequest<Top.Api.Response.OpenAccountTokenApplyResponse>
    {
        /// <summary>
        /// isv自己账号的唯一id
        /// </summary>
        public string IsvAccountId { get; set; }

        /// <summary>
        /// ISV APP的登录态时长单位是秒
        /// </summary>
        public Nullable<long> LoginStateExpireIn { get; set; }

        /// <summary>
        /// open account id
        /// </summary>
        public Nullable<long> OpenAccountId { get; set; }

        /// <summary>
        /// 时间戳单位是毫秒
        /// </summary>
        public Nullable<long> TokenTimestamp { get; set; }

        /// <summary>
        /// 用于防重放的唯一id
        /// </summary>
        public string Uuid { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.open.account.token.apply";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("isv_account_id", this.IsvAccountId);
            parameters.Add("login_state_expire_in", this.LoginStateExpireIn);
            parameters.Add("open_account_id", this.OpenAccountId);
            parameters.Add("token_timestamp", this.TokenTimestamp);
            parameters.Add("uuid", this.Uuid);
            if (this.OtherParams != null)
            {
                parameters.AddAll(this.OtherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
        }

        #endregion
    }
}
