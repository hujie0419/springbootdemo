using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// TribeUser Data Structure.
    /// </summary>
    [Serializable]
    public class TribeUser : TopObject
    {
        /// <summary>
        /// 账户appkey
        /// </summary>
        [XmlElement("app_key")]
        public string AppKey { get; set; }

        /// <summary>
        /// 群成员角色
        /// </summary>
        [XmlElement("role")]
        public string Role { get; set; }

        /// <summary>
        /// 是否为淘宝账号
        /// </summary>
        [XmlElement("taobao_account")]
        public bool TaobaoAccount { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        [XmlElement("uid")]
        public string Uid { get; set; }
    }
}
