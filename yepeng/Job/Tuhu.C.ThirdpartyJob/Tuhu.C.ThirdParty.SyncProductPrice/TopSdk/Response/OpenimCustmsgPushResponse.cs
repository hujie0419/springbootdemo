using System;
using System.Xml.Serialization;

namespace Top.Api.Response
{
    /// <summary>
    /// OpenimCustmsgPushResponse.
    /// </summary>
    public class OpenimCustmsgPushResponse : TopResponse
    {
        /// <summary>
        /// 消息id，用于定位问题
        /// </summary>
        [XmlElement("msgid")]
        public string Msgid { get; set; }

    }
}
