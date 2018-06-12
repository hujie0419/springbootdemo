using System;
using Top.Api.Domain;
using System.Collections.Generic;
using System.Xml.Serialization;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.openim.custmsg.push
    /// </summary>
    public class OpenimCustmsgPushRequest : BaseTopRequest<Top.Api.Response.OpenimCustmsgPushResponse>
    {
        /// <summary>
        /// 自定义消息内容
        /// </summary>
        public string Custmsg { get; set; }

        public CustMsg Custmsg_ { set { this.Custmsg = TopUtils.ObjectToJson(value); } } 

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.openim.custmsg.push";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("custmsg", this.Custmsg);
            if (this.OtherParams != null)
            {
                parameters.AddAll(this.OtherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("custmsg", this.Custmsg);
        }

        #endregion
    }
}
