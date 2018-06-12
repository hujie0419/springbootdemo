using System;
using Top.Api.Domain;
using System.Collections.Generic;
using System.Xml.Serialization;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.openim.tribe.setmanager
    /// </summary>
    public class OpenimTribeSetmanagerRequest : BaseTopRequest<Top.Api.Response.OpenimTribeSetmanagerResponse>
    {
        /// <summary>
        /// 用户信息
        /// </summary>
        public string Member { get; set; }

        public OpenImUser Member_ { set { this.Member = TopUtils.ObjectToJson(value); } } 

        /// <summary>
        /// 群id
        /// </summary>
        public Nullable<long> Tid { get; set; }

        /// <summary>
        /// 用户信息
        /// </summary>
        public string User { get; set; }

        public OpenImUser User_ { set { this.User = TopUtils.ObjectToJson(value); } } 

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.openim.tribe.setmanager";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("member", this.Member);
            parameters.Add("tid", this.Tid);
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
            RequestValidator.ValidateRequired("tid", this.Tid);
            RequestValidator.ValidateRequired("user", this.User);
        }

        #endregion
    }
}
