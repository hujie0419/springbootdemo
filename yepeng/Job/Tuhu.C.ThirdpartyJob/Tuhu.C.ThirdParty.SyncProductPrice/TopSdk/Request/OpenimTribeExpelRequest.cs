using System;
using Top.Api.Domain;
using System.Collections.Generic;
using System.Xml.Serialization;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.openim.tribe.expel
    /// </summary>
    public class OpenimTribeExpelRequest : BaseTopRequest<Top.Api.Response.OpenimTribeExpelResponse>
    {
        /// <summary>
        /// 用户信息
        /// </summary>
        public string Member { get; set; }

        public OpenImUser Member_ { set { this.Member = TopUtils.ObjectToJson(value); } } 

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
            return "taobao.openim.tribe.expel";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("member", this.Member);
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
            RequestValidator.ValidateRequired("member", this.Member);
            RequestValidator.ValidateRequired("tribe_id", this.TribeId);
            RequestValidator.ValidateRequired("user", this.User);
        }

        #endregion
    }
}
