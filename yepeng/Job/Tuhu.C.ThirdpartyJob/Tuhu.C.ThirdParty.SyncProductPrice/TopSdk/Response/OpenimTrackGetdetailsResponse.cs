using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// OpenimTrackGetdetailsResponse.
    /// </summary>
    public class OpenimTrackGetdetailsResponse : TopResponse
    {
        /// <summary>
        /// 用户轨迹
        /// </summary>
        [XmlArray("tracks")]
        [XmlArrayItem("tracks")]
        public List<Top.Api.Domain.Tracks> Tracks { get; set; }

    }
}
