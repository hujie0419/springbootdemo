using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// AlibabaScmExternalWmsInventoryStockOutCallcackResponse.
    /// </summary>
    public class AlibabaScmExternalWmsInventoryStockOutCallcackResponse : TopResponse
    {
        /// <summary>
        /// callback_error_code
        /// </summary>
        [XmlElement("callback_error_code")]
        public string CallbackErrorCode { get; set; }

        /// <summary>
        /// callback_success
        /// </summary>
        [XmlElement("callback_success")]
        public bool CallbackSuccess { get; set; }

        /// <summary>
        /// error_msg
        /// </summary>
        [XmlElement("error_msg")]
        public string ErrorMsg { get; set; }

    }
}
