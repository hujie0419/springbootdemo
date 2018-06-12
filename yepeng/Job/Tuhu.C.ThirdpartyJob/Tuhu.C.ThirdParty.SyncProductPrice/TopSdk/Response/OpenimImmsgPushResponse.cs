using System;
using System.Xml.Serialization;

namespace Top.Api.Response
{
    /// <summary>
    /// OpenimImmsgPushResponse.
    /// </summary>
    public class OpenimImmsgPushResponse : TopResponse
    {
        /// <summary>
        /// 消息id，方便定位问题
        /// </summary>
        [XmlElement("msgid")]
        public long Msgid { get; set; }

    }
}
