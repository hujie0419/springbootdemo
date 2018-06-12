using System;
using System.Xml.Serialization;

namespace Top.Api.Response
{
    /// <summary>
    /// TradeSnapshotGetResponse.
    /// </summary>
    public class TradeSnapshotGetResponse : TopResponse
    {
        /// <summary>
        /// 只包含Trade中的tid和snapshot、子订单Order中的oid和snapshot
        /// </summary>
        [XmlElement("trade")]
        public Top.Api.Domain.Trade Trade { get; set; }
    }
}
