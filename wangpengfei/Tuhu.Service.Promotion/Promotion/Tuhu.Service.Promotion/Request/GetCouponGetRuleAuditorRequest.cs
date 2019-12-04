using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Promotion.Request
{
    /// <summary>
    /// 请求值 -  获取优惠券领取规则的审核人
    /// </summary>
    public class GetCouponGetRuleAuditorRequest
    {

        /// <summary>
        /// 审核步骤
        /// </summary>
        public int Step { get; set; }

        /// <summary>
        /// 工单号 
        /// </summary>
        public int WorkOrderId { get; set; }

        /// <summary>
        /// 申请人
        /// </summary>
        public string Applicant { get; set; }

        /// <summary>
        /// 成本归属 ID
        /// </summary>
        public int BusinessLineId { get; set; }

        /// <summary>
        /// 部门id
        /// </summary>
        public int DepartmentId { get; set; }
    }
}
