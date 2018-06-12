using System;
using System.Xml.Serialization;

namespace Top.Api.Response
{
    /// <summary>
    /// HotelRoomUpdateResponse.
    /// </summary>
    public class HotelRoomUpdateResponse : TopResponse
    {
        /// <summary>
        /// 商品结构
        /// </summary>
        [XmlElement("room")]
        public Top.Api.Domain.Room Room { get; set; }
    }
}
