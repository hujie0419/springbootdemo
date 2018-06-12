using System;
using System.Xml.Serialization;
using Top.Api.Util;

namespace Top.Api.Domain
{
    /// <summary>
    /// SendVerCodeRequest Data Structure.
    /// </summary>
    [Serializable]
    public class SendVerCodeRequest : TopObject
    {
        /// <summary>
        /// appKey
        /// </summary>
        [XmlElement("app_key")]
        public string AppKey { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        [XmlElement("biz_type")]
        public Nullable<long> BizType { get; set; }

        /// <summary>
        /// 短信内容替换上下文
        /// </summary>
        [XmlElement("context")]
        public object Context { get; set; }
        public string Context_ { set { this.Context = TopUtils.JsonToObject(value); } }

        /// <summary>
        /// 设备id
        /// </summary>
        [XmlElement("device_id")]
        public string DeviceId { get; set; }

        /// <summary>
        /// 设备级别的发送次数限制
        /// </summary>
        [XmlElement("device_limit")]
        public Nullable<long> DeviceLimit { get; set; }

        /// <summary>
        /// 发送次数限制的时间，单位为秒
        /// </summary>
        [XmlElement("device_limit_in_time")]
        public Nullable<long> DeviceLimitInTime { get; set; }

        /// <summary>
        /// 场景域，比如登录的验证码不能用于注册
        /// </summary>
        [XmlElement("domain")]
        public string Domain { get; set; }

        /// <summary>
        /// 验证码失效时间，单位为秒
        /// </summary>
        [XmlElement("expire_time")]
        public Nullable<long> ExpireTime { get; set; }

        /// <summary>
        /// 外部的id
        /// </summary>
        [XmlElement("external_id")]
        public string ExternalId { get; set; }

        /// <summary>
        /// long型模板id
        /// </summary>
        [XmlElement("long_template_id")]
        public Nullable<long> LongTemplateId { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [XmlElement("mobile")]
        public string Mobile { get; set; }

        /// <summary>
        /// 手机号的次数限制
        /// </summary>
        [XmlElement("mobile_limit")]
        public Nullable<long> MobileLimit { get; set; }

        /// <summary>
        /// 手机号的次数限制的时间
        /// </summary>
        [XmlElement("mobile_limit_in_time")]
        public Nullable<long> MobileLimitInTime { get; set; }

        /// <summary>
        /// session id
        /// </summary>
        [XmlElement("session_id")]
        public string SessionId { get; set; }

        /// <summary>
        /// session级别的发送次数限制
        /// </summary>
        [XmlElement("session_limit")]
        public Nullable<long> SessionLimit { get; set; }

        /// <summary>
        /// 发送次数限制的时间，单位为秒
        /// </summary>
        [XmlElement("session_limit_in_time")]
        public Nullable<long> SessionLimitInTime { get; set; }

        /// <summary>
        /// 签名id
        /// </summary>
        [XmlElement("signature_id")]
        public Nullable<long> SignatureId { get; set; }

        /// <summary>
        /// 模板id
        /// </summary>
        [XmlElement("template_id")]
        public Nullable<long> TemplateId { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        [XmlElement("user_id")]
        public Nullable<long> UserId { get; set; }

        /// <summary>
        /// 验证码长度
        /// </summary>
        [XmlElement("ver_code_length")]
        public Nullable<long> VerCodeLength { get; set; }
    }
}
