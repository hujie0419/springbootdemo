using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.openim.users.get
    /// </summary>
    public class OpenimUsersGetRequest : BaseTopRequest<Top.Api.Response.OpenimUsersGetResponse>
    {
        /// <summary>
        /// 用户id序列
        /// </summary>
        public string Userids { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.openim.users.get";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("userids", this.Userids);
            if (this.OtherParams != null)
            {
                parameters.AddAll(this.OtherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("userids", this.Userids);
            RequestValidator.ValidateMaxListSize("userids", this.Userids, 100);
        }

        #endregion
    }
}
