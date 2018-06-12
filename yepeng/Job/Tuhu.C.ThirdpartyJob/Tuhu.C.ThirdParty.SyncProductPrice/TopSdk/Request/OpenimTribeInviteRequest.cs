using System;
using Top.Api.Domain;
using System.Collections.Generic;
using System.Xml.Serialization;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.openim.tribe.invite
    /// </summary>
    public class OpenimTribeInviteRequest : BaseTopRequest<Top.Api.Response.OpenimTribeInviteResponse>
    {
        /// <summary>
        /// 用户信息
        /// </summary>
        public string Members { get; set; }

        public List<OpenImUser> Members_ { set { this.Members = TopUtils.ObjectToJson(value); } } 

        /// <summary>
        /// 群id
        /// </summary>
        public Nullable<long> TribeId { get; set; }

        /// <summary>
        /// 用户信息
        /// </summary>
        public string User { get; set; }

        public OpenImUser User_ { set { this.User = TopUtils.ObjectToJson(value); } } 

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.openim.tribe.invite";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("members", this.Members);
            parameters.Add("tribe_id", this.TribeId);
            parameters.Add("user", this.User);
            if (this.OtherParams != null)
            {
                parameters.AddAll(this.OtherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("members", this.Members);
            RequestValidator.ValidateObjectMaxListSize("members", this.Members, 1000);
            RequestValidator.ValidateRequired("tribe_id", this.TribeId);
            RequestValidator.ValidateRequired("user", this.User);
        }

        #endregion
    }
}
