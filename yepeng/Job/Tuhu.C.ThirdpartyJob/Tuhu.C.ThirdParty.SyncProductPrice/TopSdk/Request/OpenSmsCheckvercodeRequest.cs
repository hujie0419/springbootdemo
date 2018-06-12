using System;
using Top.Api.Domain;
using System.Collections.Generic;
using System.Xml.Serialization;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.open.sms.checkvercode
    /// </summary>
    public class OpenSmsCheckvercodeRequest : BaseTopRequest<Top.Api.Response.OpenSmsCheckvercodeResponse>
    {
        /// <summary>
        /// 验证验证码
        /// </summary>
        public string CheckVerCodeRequest { get; set; }

        public CheckVerCodeRequest CheckVerCodeRequest_ { set { this.CheckVerCodeRequest = TopUtils.ObjectToJson(value); } } 

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.open.sms.checkvercode";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("check_ver_code_request", this.CheckVerCodeRequest);
            if (this.OtherParams != null)
            {
                parameters.AddAll(this.OtherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("check_ver_code_request", this.CheckVerCodeRequest);
        }

        #endregion
    }
}
