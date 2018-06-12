using System;
using System.Xml.Serialization;

namespace Top.Api.Response
{
    /// <summary>
    /// HotelRoomGetResponse.
    /// </summary>
    public class HotelRoomGetResponse : TopResponse
    {
        /// <summary>
        /// 商品结构。是否返回酒店信息、房型信息、房态列表、宝贝描述根据参数决定
        /// </summary>
        [XmlElement("room")]
        public Top.Api.Domain.Room Room { get; set; }
    }
}
