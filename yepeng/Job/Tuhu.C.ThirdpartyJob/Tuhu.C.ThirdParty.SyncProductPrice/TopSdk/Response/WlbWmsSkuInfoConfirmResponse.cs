using System;
using System.Xml.Serialization;

namespace Top.Api.Response
{
    /// <summary>
    /// WlbWmsSkuInfoConfirmResponse.
    /// </summary>
    public class WlbWmsSkuInfoConfirmResponse : TopResponse
    {
        /// <summary>
        /// 商品回告返回
        /// </summary>
        [XmlElement("result")]
        public Top.Api.Domain.WlbWmsSkuInfoConfirmData Result { get; set; }
    }
}
