using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Domain
{
    /// <summary>
    /// Invoicewlbwmsconsignordernotify Data Structure.
    /// </summary>
    [Serializable]
    public class Invoicewlbwmsconsignordernotify : TopObject
    {
        /// <summary>
        /// 发票金额
        /// </summary>
        [XmlElement("bill_account")]
        public string BillAccount { get; set; }

        /// <summary>
        /// 发票内容
        /// </summary>
        [XmlElement("bill_content")]
        public string BillContent { get; set; }

        /// <summary>
        /// Erp发票ID
        /// </summary>
        [XmlElement("bill_id")]
        public long BillId { get; set; }

        /// <summary>
        /// 发票抬头
        /// </summary>
        [XmlElement("bill_title")]
        public string BillTitle { get; set; }

        /// <summary>
        /// 发票类型：VINVOICE - 增值税普通发票， EVINVOICE - 电子增票
        /// </summary>
        [XmlElement("bill_type")]
        public string BillType { get; set; }

        /// <summary>
        /// 发票明细
        /// </summary>
        [XmlArray("detail_list")]
        [XmlArrayItem("detaillistwlbwmsconsignordernotify")]
        public List<Top.Api.Domain.Detaillistwlbwmsconsignordernotify> DetailList { get; set; }
    }
}
