using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// Invoicelistwlbwmsconsignordernotify Data Structure.
    /// </summary>
    [Serializable]
    public class Invoicelistwlbwmsconsignordernotify : TopObject
    {
        /// <summary>
        /// 发票信息
        /// </summary>
        [XmlElement("invoice_info")]
        public Top.Api.Domain.Invoicewlbwmsconsignordernotify InvoiceInfo { get; set; }
    }
}
