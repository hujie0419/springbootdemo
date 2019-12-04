using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Promotion.Request
{
    /// <summary>
    /// 请求值 - 更新优惠券审核状态
    /// </summary>
    public class UpdateCouponGetRuleAuditRequest
    {
        /// <summary>
        /// 优惠券领取规则审核的PKID
        /// </summary>
        ///public int GetCouponRuleAuditID { get; set; }
        /// <summary>
        /// 工单号 
        /// </summary>
        public int WorkOrderId { get; set; }
        /// <summary>
        /// 审核人
        /// </summary>
        public string Auditor { get; set; }
        /// <summary>
        /// 审核状态 [参照 WorkOrderAuditStatusEnum 枚举]
        /// </summary>
        public string AuditStatus { get; set; }
        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime AuditDateTime { get; set; }
        /// <summary>
        /// 审核信息 【例如审核不通过原因】
        /// </summary>
        public string AuditMessage { get; set; }
    }
}
