using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// TribeInfo Data Structure.
    /// </summary>
    [Serializable]
    public class TribeInfo : TopObject
    {
        /// <summary>
        /// 群验证模式
        /// </summary>
        [XmlElement("check_mode")]
        public long CheckMode { get; set; }

        /// <summary>
        /// 群头像URL地址
        /// </summary>
        [XmlElement("icon")]
        public string Icon { get; set; }

        /// <summary>
        /// 群名称
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// 群公告
        /// </summary>
        [XmlElement("notice")]
        public string Notice { get; set; }

        /// <summary>
        /// 群接收标记
        /// </summary>
        [XmlElement("recv_flag")]
        public long RecvFlag { get; set; }

        /// <summary>
        /// 群ID
        /// </summary>
        [XmlElement("tribe_id")]
        public long TribeId { get; set; }

        /// <summary>
        /// 群类型
        /// </summary>
        [XmlElement("tribe_type")]
        public long TribeType { get; set; }
    }
}
