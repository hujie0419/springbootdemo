using System;
using System.Xml.Serialization;

namespace Top.Api.Response
{
    /// <summary>
    /// HotelFixbookingResponse.
    /// </summary>
    public class HotelFixbookingResponse : TopResponse
    {
        /// <summary>
        /// 返回hid和时间
        /// </summary>
        [XmlElement("hotel")]
        public Top.Api.Domain.Hotel Hotel { get; set; }
    }
}
