using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.cloudpush.message.ios
    /// </summary>
    public class CloudpushMessageIosRequest : BaseTopRequest<Top.Api.Response.CloudpushMessageIosResponse>
    {
        /// <summary>
        /// 发送的消息内容.
        /// </summary>
        public string Body { get; set; }

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
            return "taobao.cloudpush.message.ios";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("body", this.Body);
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
            RequestValidator.ValidateRequired("body", this.Body);
            RequestValidator.ValidateRequired("target", this.Target);
            RequestValidator.ValidateRequired("target_value", this.TargetValue);
        }

        #endregion
    }
}
