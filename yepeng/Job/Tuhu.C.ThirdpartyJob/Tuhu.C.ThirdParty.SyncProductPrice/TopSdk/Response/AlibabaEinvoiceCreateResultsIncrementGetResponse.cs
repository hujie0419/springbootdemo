using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// AlibabaEinvoiceCreateResultsIncrementGetResponse.
    /// </summary>
    public class AlibabaEinvoiceCreateResultsIncrementGetResponse : TopResponse
    {
        /// <summary>
        /// 开票结果返回列表
        /// </summary>
        [XmlArray("invoice_result_list")]
        [XmlArrayItem("invoice_result")]
        public List<Top.Api.Domain.InvoiceResult> InvoiceResultList { get; set; }

        /// <summary>
        /// 符合条件的开票总数
        /// </summary>
        [XmlElement("total_count")]
        public long TotalCount { get; set; }

    }
}
