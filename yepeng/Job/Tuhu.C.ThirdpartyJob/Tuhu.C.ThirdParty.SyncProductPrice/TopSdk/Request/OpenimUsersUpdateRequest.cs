using System;
using Top.Api.Domain;
using System.Collections.Generic;
using System.Xml.Serialization;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.openim.users.update
    /// </summary>
    public class OpenimUsersUpdateRequest : BaseTopRequest<Top.Api.Response.OpenimUsersUpdateResponse>
    {
        /// <summary>
        /// 用户信息列表
        /// </summary>
        public string Userinfos { get; set; }

        public List<Userinfos> Userinfos_ { set { this.Userinfos = TopUtils.ObjectToJson(value); } } 

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.openim.users.update";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("userinfos", this.Userinfos);
            if (this.OtherParams != null)
            {
                parameters.AddAll(this.OtherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("userinfos", this.Userinfos);
            RequestValidator.ValidateObjectMaxListSize("userinfos", this.Userinfos, 100);
        }

        #endregion
    }
}
