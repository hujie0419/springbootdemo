using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// AlibabaEinvoiceSerialnoGenerateResponse.
    /// </summary>
    public class AlibabaEinvoiceSerialnoGenerateResponse : TopResponse
    {
        /// <summary>
        /// result
        /// </summary>
        [XmlElement("serial_no")]
        public string SerialNo { get; set; }

    }
}
