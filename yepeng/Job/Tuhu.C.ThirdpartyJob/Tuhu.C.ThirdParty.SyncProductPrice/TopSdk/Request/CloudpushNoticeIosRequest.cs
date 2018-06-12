using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.cloudpush.notice.ios
    /// </summary>
    public class CloudpushNoticeIosRequest : BaseTopRequest<Top.Api.Response.CloudpushNoticeIosResponse>
    {
        /// <summary>
        /// iOS的通知是通过APNS中心来发送的，需要填写对应的环境信息.  DEV:表示开发环境, PRODUCT: 表示生产环境.
        /// </summary>
        public string Env { get; set; }

        /// <summary>
        /// 提供给IOS通知的扩展属性，如角标或者声音等,注意：参数值为json
        /// </summary>
        public string Ext { get; set; }

        /// <summary>
        /// 通知摘要
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 推送目标: device:推送给设备; account:推送给指定帐号,all: 推送给全部
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        /// 根据Target来设定，如Target=device, 则对应的值为 设备id1,设备id2. 多个值使用逗号分隔
        /// </summary>
        public string TargetValue { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.cloudpush.notice.ios";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("env", this.Env);
            parameters.Add("ext", this.Ext);
            parameters.Add("summary", this.Summary);
            parameters.Add("target", this.Target);
            parameters.Add("target_value", this.TargetValue);
            if (this.OtherParams != null)
            {
                parameters.AddAll(this.OtherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("env", this.Env);
            RequestValidator.ValidateRequired("summary", this.Summary);
            RequestValidator.ValidateRequired("target", this.Target);
            RequestValidator.ValidateRequired("target_value", this.TargetValue);
        }

        #endregion
    }
}
