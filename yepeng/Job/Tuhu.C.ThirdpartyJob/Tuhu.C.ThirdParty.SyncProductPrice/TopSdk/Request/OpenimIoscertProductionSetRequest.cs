using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.openim.ioscert.production.set
    /// </summary>
    public class OpenimIoscertProductionSetRequest : BaseTopRequest<Top.Api.Response.OpenimIoscertProductionSetResponse>
    {
        /// <summary>
        /// 证书文件内容,base64编码
        /// </summary>
        public string Cert { get; set; }

        /// <summary>
        /// 证书密码
        /// </summary>
        public string Password { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.openim.ioscert.production.set";
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
            RequestValidator.ValidateRequired("password", this.Password);
        }

        #endregion
    }
}
