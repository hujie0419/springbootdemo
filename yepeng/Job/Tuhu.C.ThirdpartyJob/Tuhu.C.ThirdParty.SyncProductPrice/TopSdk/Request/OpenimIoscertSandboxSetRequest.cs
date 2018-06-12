using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.openim.ioscert.sandbox.set
    /// </summary>
    public class OpenimIoscertSandboxSetRequest : BaseTopRequest<Top.Api.Response.OpenimIoscertSandboxSetResponse>
    {
        /// <summary>
        /// 证书内容,base64编码
        /// </summary>
        public string Cert { get; set; }

        /// <summary>
        /// 系统自动生成
        /// </summary>
        public string Password { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.openim.ioscert.sandbox.set";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("cert", this.Cert);
            parameters.Add("password", this.Password);
            if (this.OtherParams != null)
            {
                parameters.AddAll(this.OtherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("cert", this.Cert);
        }

        #endregion
    }
}
