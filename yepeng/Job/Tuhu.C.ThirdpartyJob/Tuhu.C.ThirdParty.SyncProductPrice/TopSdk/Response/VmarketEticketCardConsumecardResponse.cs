using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// VmarketEticketCardConsumecardResponse.
    /// </summary>
    public class VmarketEticketCardConsumecardResponse : TopResponse
    {
        /// <summary>
        /// 正确返回值
        /// </summary>
        [XmlElement("resultcode")]
        public long Resultcode { get; set; }

    }
}
