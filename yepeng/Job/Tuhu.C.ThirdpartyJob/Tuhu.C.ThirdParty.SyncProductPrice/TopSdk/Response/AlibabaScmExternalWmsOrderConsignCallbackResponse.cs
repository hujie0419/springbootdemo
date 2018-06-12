using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// AlibabaScmExternalWmsOrderConsignCallbackResponse.
    /// </summary>
    public class AlibabaScmExternalWmsOrderConsignCallbackResponse : TopResponse
    {
        /// <summary>
        /// checkTime
        /// </summary>
        [XmlElement("check_time")]
        public string CheckTime { get; set; }

    }
}
