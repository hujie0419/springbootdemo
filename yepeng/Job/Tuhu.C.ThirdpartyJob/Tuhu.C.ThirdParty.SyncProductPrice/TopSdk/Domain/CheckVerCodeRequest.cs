using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// CheckVerCodeRequest Data Structure.
    /// </summary>
    [Serializable]
    public class CheckVerCodeRequest : TopObject
    {
        /// <summary>
        /// app key
        /// </summary>
        [XmlElement("app_key")]
        public string AppKey { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        [XmlElement("biz_type")]
        public Nullable<long> BizType { get; set; }

        /// <summary>
        /// 最多验证错误几次
        /// </summary>
        [XmlElement("check_fail_limit")]
        public Nullable<long> CheckFailLimit { get; set; }

        /// <summary>
        /// 最多验证成功几次
        /// </summary>
        [XmlElement("check_success_limit")]
        public Nullable<long> CheckSuccessLimit { get; set; }

        /// <summary>
        /// 短信验证码域
        /// </summary>
        [XmlElement("domain")]
        public string Domain { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [XmlElement("mobile")]
        public string Mobile { get; set; }

        /// <summary>
        /// isv user id
        /// </summary>
        [XmlElement("user_id")]
        public Nullable<long> UserId { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        [XmlElement("ver_code")]
        public string VerCode { get; set; }
    }
}
