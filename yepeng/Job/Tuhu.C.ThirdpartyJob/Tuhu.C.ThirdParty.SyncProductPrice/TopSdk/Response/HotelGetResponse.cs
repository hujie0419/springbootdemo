using System;
using System.Xml.Serialization;

namespace Top.Api.Response
{
    /// <summary>
    /// HotelGetResponse.
    /// </summary>
    public class HotelGetResponse : TopResponse
    {
        /// <summary>
        /// 酒店结构
        /// </summary>
        [XmlElement("hotel")]
        public Top.Api.Domain.Hotel Hotel { get; set; }
    }
}
