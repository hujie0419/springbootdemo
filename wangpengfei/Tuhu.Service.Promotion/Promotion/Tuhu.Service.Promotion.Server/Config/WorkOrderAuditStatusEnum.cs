using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tuhu.Service.Promotion.Server.Config
{
    /// <summary>
    /// 工单审核状态
    /// 【0=草稿，1=审核中 ， 2=审核通过，3=审核不通过，4=已撤销,5=紧急审核通过不经过工单系统审核】
    /// </summary>
    enum WorkOrderAuditStatusEnum
    {
        /// <summary>
        /// 草稿
        /// </summary>
        待审批 = 0,
        /// <summary>
        /// 审核中
        /// </summary>
        审批中 = 1,
        /// <summary>
        /// 审核通过
        /// </summary>
        审批通过 = 2,
        /// <summary>
        /// 已终止
        /// </summary>
        已终止 = 4,
        /// <summary>
        /// 紧急审核通过不经过工单系统审核
        /// </summary>
        EmergencyPass = 5,

    }
}
