using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// OpenImUser Data Structure.
    /// </summary>
    [Serializable]
    public class OpenImUser : TopObject
    {
        /// <summary>
        /// 账户appkey
        /// </summary>
        [XmlElement("app_key")]
        public string AppKey { get; set; }

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
