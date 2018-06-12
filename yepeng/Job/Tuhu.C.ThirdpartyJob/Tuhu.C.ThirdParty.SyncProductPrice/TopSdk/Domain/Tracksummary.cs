using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// Tracksummary Data Structure.
    /// </summary>
    [Serializable]
    public class Tracksummary : TopObject
    {
        /// <summary>
        /// 浏览器
        /// </summary>
        [XmlElement("browser")]
        public string Browser { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        [XmlElement("city")]
        public string City { get; set; }

        /// <summary>
        /// 国家
        /// </summary>
        [XmlElement("country")]
        public string Country { get; set; }

        /// <summary>
        /// 设备
        /// </summary>
        [XmlElement("device")]
        public string Device { get; set; }

        /// <summary>
        /// 额外参数
        /// </summary>
        [XmlElement("extra_param")]
        public string ExtraParam { get; set; }

        /// <summary>
        /// UI参数
        /// </summary>
        [XmlElement("extra_ui")]
        public string ExtraUi { get; set; }

        /// <summary>
        /// 渠道
        /// </summary>
        [XmlElement("from_channel")]
        public string FromChannel { get; set; }

        /// <summary>
        /// ip地址
        /// </summary>
        [XmlElement("ip")]
        public string Ip { get; set; }

        /// <summary>
        /// 运营商，如电信，联通
        /// </summary>
        [XmlElement("isp")]
        public string Isp { get; set; }

        /// <summary>
        /// 操作系统
        /// </summary>
        [XmlElement("os")]
        public string Os { get; set; }

        /// <summary>
        /// referer_关键字
        /// </summary>
        [XmlElement("referer_keyword")]
        public string RefererKeyword { get; set; }

        /// <summary>
        /// referer_url
        /// </summary>
        [XmlElement("referer_url")]
        public string RefererUrl { get; set; }

        /// <summary>
        /// 省份，如浙江省
        /// </summary>
        [XmlElement("region")]
        public string Region { get; set; }

        /// <summary>
        /// 访问总时间，以毫秒为单位
        /// </summary>
        [XmlElement("total_time")]
        public long TotalTime { get; set; }

        /// <summary>
        /// 访问总次数
        /// </summary>
        [XmlElement("visit_count")]
        public long VisitCount { get; set; }
    }
}
