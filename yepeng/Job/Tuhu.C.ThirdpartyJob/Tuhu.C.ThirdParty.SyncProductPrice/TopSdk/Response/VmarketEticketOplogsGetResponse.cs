using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// VmarketEticketOplogsGetResponse.
    /// </summary>
    public class VmarketEticketOplogsGetResponse : TopResponse
    {
        /// <summary>
        /// 操作日志列表
        /// </summary>
        [XmlArray("eticket_op_logs")]
        [XmlArrayItem("eticket_op_log")]
        public List<Top.Api.Domain.EticketOpLog> EticketOpLogs { get; set; }

        /// <summary>
        /// 符合条件的记录总数
        /// </summary>
        [XmlElement("total_results")]
        public long TotalResults { get; set; }

    }
}
