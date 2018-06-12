using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// AlibabaScmExternalWmsInventoryStockCheckCallcackResponse.
    /// </summary>
    public class AlibabaScmExternalWmsInventoryStockCheckCallcackResponse : TopResponse
    {
        /// <summary>
        /// errorCode
        /// </summary>
        [XmlElement("callback_error_code")]
        public string CallbackErrorCode { get; set; }

        /// <summary>
        /// success
        /// </summary>
        [XmlElement("callback_success")]
        public bool CallbackSuccess { get; set; }

        /// <summary>
        /// errorMsg
        /// </summary>
        [XmlElement("error_msg")]
        public string ErrorMsg { get; set; }

    }
}
