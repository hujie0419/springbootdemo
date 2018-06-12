using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// CainiaoConsignBillInvoinceconfirm Data Structure.
    /// </summary>
    [Serializable]
    public class CainiaoConsignBillInvoinceconfirm : TopObject
    {
        /// <summary>
        /// Erp发票ID
        /// </summary>
        [XmlElement("bill_id")]
        public string BillId { get; set; }

        /// <summary>
        /// 发票代码
        /// </summary>
        [XmlElement("invoice_code")]
        public string InvoiceCode { get; set; }

        /// <summary>
        /// 发票号
        /// </summary>
        [XmlElement("invoice_number")]
        public string InvoiceNumber { get; set; }
    }
}
