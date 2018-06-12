using System;
using System.Xml.Serialization;

namespace Top.Api.Response
{
    /// <summary>
    /// HotelTypeAddResponse.
    /// </summary>
    public class HotelTypeAddResponse : TopResponse
    {
        /// <summary>
        /// 房型结构
        /// </summary>
        [XmlElement("room_type")]
        public Top.Api.Domain.RoomType RoomType { get; set; }
    }
}
