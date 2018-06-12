using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// VmarketEticketCodesGetResponse.
    /// </summary>
    public class VmarketEticketCodesGetResponse : TopResponse
    {
        /// <summary>
        /// 电子凭证码列表
        /// </summary>
        [XmlArray("eticket_codes")]
        [XmlArrayItem("eticket_code")]
        public List<Top.Api.Domain.EticketCode> EticketCodes { get; set; }

        /// <summary>
        /// 记录总数
        /// </summary>
        [XmlElement("total_results")]
        public long TotalResults { get; set; }

    }
}
