using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// Tracks Data Structure.
    /// </summary>
    [Serializable]
    public class Tracks : TopObject
    {
        /// <summary>
        /// 轨迹发生的时间，即用户进入页面的时间
        /// </summary>
        [XmlElement("enter_time")]
        public string EnterTime { get; set; }

        /// <summary>
        /// referer_keyword
        /// </summary>
        [XmlElement("referer_keyword")]
        public string RefererKeyword { get; set; }

        /// <summary>
        /// referer_url
        /// </summary>
        [XmlElement("referer_url")]
        public string RefererUrl { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [XmlElement("title")]
        public string Title { get; set; }

        /// <summary>
        /// url
        /// </summary>
        [XmlElement("url")]
        public string Url { get; set; }
    }
}
