using System;
using System.Xml.Serialization;

namespace Top.Api.Response
{
    /// <summary>
    /// HotelRoomAddResponse.
    /// </summary>
    public class HotelRoomAddResponse : TopResponse
    {
        /// <summary>
        /// 商品结构
        /// </summary>
        [XmlElement("room")]
        public Top.Api.Domain.Room Room { get; set; }
    }
}
