using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// VmarketEticketTasksGetResponse.
    /// </summary>
    public class VmarketEticketTasksGetResponse : TopResponse
    {
        /// <summary>
        /// 任务列表查询结果信息
        /// </summary>
        [XmlArray("eticket_tasks")]
        [XmlArrayItem("eticket_task")]
        public List<Top.Api.Domain.EticketTask> EticketTasks { get; set; }

        /// <summary>
        /// 任务列表查询结果的总数
        /// </summary>
        [XmlElement("total_results")]
        public long TotalResults { get; set; }

    }
}
