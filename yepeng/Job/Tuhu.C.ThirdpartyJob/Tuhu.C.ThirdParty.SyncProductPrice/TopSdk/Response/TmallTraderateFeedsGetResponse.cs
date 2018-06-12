using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// TmallTraderateFeedsGetResponse.
    /// </summary>
    public class TmallTraderateFeedsGetResponse : TopResponse
    {
        /// <summary>
        /// 返回评价信息
        /// </summary>
        [XmlElement("tmall_rate_info")]
        public Top.Api.Domain.TmallRateInfo TmallRateInfo { get; set; }

    }
}
