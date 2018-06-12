using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// InvoinceConfirmsWlbWmsConsignOrderConfirm Data Structure.
    /// </summary>
    [Serializable]
    public class InvoinceConfirmsWlbWmsConsignOrderConfirm : TopObject
    {
        /// <summary>
        /// ERP发票ID
        /// </summary>
        [XmlElement("bill_id")]
        public long BillId { get; set; }

        /// <summary>
        /// 发票代码
        /// </summary>
        [XmlElement("invoice_code")]
        public string InvoiceCode { get; set; }

        /// <summary>
        /// 发票号码
        /// </summary>
        [XmlElement("invoice_number")]
        public string InvoiceNumber { get; set; }

        /// <summary>
        /// 交易号
        /// </summary>
        [XmlElement("trade_number")]
        public string TradeNumber { get; set; }
    }
}
