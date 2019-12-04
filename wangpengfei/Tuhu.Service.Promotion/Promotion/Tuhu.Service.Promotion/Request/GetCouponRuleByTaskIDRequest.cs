using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Promotion.Request
{
    /// <summary>
    /// 获取优惠券任务配置发券规则
    /// </summary>
    public class GetCouponRuleByTaskIDRequest
    {
        /// <summary>
        /// 任务id
        /// </summary>
        public int PromotionTaskId { get; set; }
    }
}
