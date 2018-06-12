using System;
using Top.Api.Domain;
using System.Collections.Generic;
using System.Xml.Serialization;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.open.sms.sendvercode
    /// </summary>
    public class OpenSmsSendvercodeRequest : BaseTopRequest<Top.Api.Response.OpenSmsSendvercodeResponse>
    {
        /// <summary>
        /// 发送验证码请求
        /// </summary>
        public string SendVerCodeRequest { get; set; }

        public SendVerCodeRequest SendVerCodeRequest_ { set { this.SendVerCodeRequest = TopUtils.ObjectToJson(value); } } 

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.open.sms.sendvercode";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("send_ver_code_request", this.SendVerCodeRequest);
            if (this.OtherParams != null)
            {
                parameters.AddAll(this.OtherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("send_ver_code_request", this.SendVerCodeRequest);
        }

        #endregion
    }
}
