using System;
using System.Xml.Serialization;

namespace Top.Api.Response
{
    /// <summary>
    /// WlbWmsInventoryStatusUploadResponse.
    /// </summary>
    public class WlbWmsInventoryStatusUploadResponse : TopResponse
    {
        /// <summary>
        /// 库存状态调整回告
        /// </summary>
        [XmlElement("result")]
        public Top.Api.Domain.WlbWmsInventoryStatusUploadResp Result { get; set; }
    }
}
