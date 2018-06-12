using System;
using Top.Api.Domain;
using System.Collections.Generic;
using System.Xml.Serialization;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.openim.immsg.push
    /// </summary>
    public class OpenimImmsgPushRequest : BaseTopRequest<Top.Api.Response.OpenimImmsgPushResponse>
    {
        /// <summary>
        /// openim消息结构体
        /// </summary>
        public string Immsg { get; set; }

        public ImMsg Immsg_ { set { this.Immsg = TopUtils.ObjectToJson(value); } } 

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.openim.immsg.push";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("immsg", this.Immsg);
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
