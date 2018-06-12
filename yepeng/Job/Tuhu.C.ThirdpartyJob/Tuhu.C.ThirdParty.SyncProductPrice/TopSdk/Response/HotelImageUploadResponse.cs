using System;
using System.Xml.Serialization;

namespace Top.Api.Response
{
    /// <summary>
    /// HotelImageUploadResponse.
    /// </summary>
    public class HotelImageUploadResponse : TopResponse
    {
        /// <summary>
        /// 酒店图片
        /// </summary>
        [XmlElement("hotel_image")]
        public Top.Api.Domain.HotelImage HotelImage { get; set; }
    }
}
