using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// TokenInfo Data Structure.
    /// </summary>
    [Serializable]
    public class TokenInfo : TopObject
    {
        /// <summary>
        /// token info扩展信息
        /// </summary>
        [XmlElement("ext")]
        public Top.Api.Domain.TokenInfoExt Ext { get; set; }

        /// <summary>
        /// isv自己账号的唯一id
        /// </summary>
        [XmlElement("isv_account_id")]
        public string IsvAccountId { get; set; }

        /// <summary>
        /// ISV APP的登录态时长
        /// </summary>
        [XmlElement("login_state_expire_in")]
        public long LoginStateExpireIn { get; set; }

        /// <summary>
        /// open account id
        /// </summary>
        [XmlElement("open_account_id")]
        public long OpenAccountId { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        [XmlElement("timestamp")]
        public long Timestamp { get; set; }

        /// <summary>
        /// 用于防重放的唯一id
        /// </summary>
        [XmlElement("uuid")]
        public string Uuid { get; set; }
    }
}
