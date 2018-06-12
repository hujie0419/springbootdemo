using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// AlibabaScmExternalWmsOrderTmsUpdateCallbackResponse.
    /// </summary>
    public class AlibabaScmExternalWmsOrderTmsUpdateCallbackResponse : TopResponse
    {
        /// <summary>
        /// success
        /// </summary>
        [XmlElement("boolean_result")]
        public bool BooleanResult { get; set; }

    }
}
