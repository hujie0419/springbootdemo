using System;
using System.Xml.Serialization;

namespace Top.Api.Response
{
    /// <summary>
    /// VmarketEticketCardSendcardResponse.
    /// </summary>
    public class VmarketEticketCardSendcardResponse : TopResponse
    {
        /// <summary>
        /// 调用成功
        /// </summary>
        [XmlElement("resultcode")]
        public long Resultcode { get; set; }
    }
}
