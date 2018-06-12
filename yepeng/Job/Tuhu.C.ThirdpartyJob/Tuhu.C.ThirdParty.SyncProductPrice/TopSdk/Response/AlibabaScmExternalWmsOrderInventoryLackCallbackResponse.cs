using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// AlibabaScmExternalWmsOrderInventoryLackCallbackResponse.
    /// </summary>
    public class AlibabaScmExternalWmsOrderInventoryLackCallbackResponse : TopResponse
    {
        /// <summary>
        /// success
        /// </summary>
        [XmlElement("result_boolean")]
        public bool ResultBoolean { get; set; }

    }
}
