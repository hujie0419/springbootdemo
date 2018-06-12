using System;
using System.Xml.Serialization;

namespace Top.Api.Response
{
    /// <summary>
    /// HotelAddResponse.
    /// </summary>
    public class HotelAddResponse : TopResponse
    {
        /// <summary>
        /// 酒店结构
        /// </summary>
        [XmlElement("hotel")]
        public Top.Api.Domain.Hotel Hotel { get; set; }
    }
}
