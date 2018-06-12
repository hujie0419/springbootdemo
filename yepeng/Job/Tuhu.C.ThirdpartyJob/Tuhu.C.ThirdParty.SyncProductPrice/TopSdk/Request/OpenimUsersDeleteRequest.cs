using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.openim.users.delete
    /// </summary>
    public class OpenimUsersDeleteRequest : BaseTopRequest<Top.Api.Response.OpenimUsersDeleteResponse>
    {
        /// <summary>
        /// 需要删除的用户列表，多个用户用半角逗号分隔，最多一次可以删除100个用户
        /// </summary>
        public string Userids { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.openim.users.delete";
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
