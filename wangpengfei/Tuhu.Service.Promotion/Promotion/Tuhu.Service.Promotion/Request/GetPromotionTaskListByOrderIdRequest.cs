using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Promotion.Response;

namespace Tuhu.Service.Promotion.Request
{
    /// <summary>
    /// 校验符合订单的优惠券任务
    /// </summary>
    public class GetPromotionTaskListByOrderIdRequest
    {
        public int OrderID { get; set; }

        public List<PromotionTaskModel> PromotionTaskList { get; set; }

        #region 计算预算相关

        /// <summary>
        /// true：开启 预算逻辑
        /// </summary>
        public bool Budget { get; set; }

        /// <summary>
        /// 优惠券使用规则id
        /// </summary>
        public List<int> CouponRuleIDList { get; set; }

        /// <summary>
        /// 业务线名称
        /// </summary>
        public string BusinessLineName { get; set; }

        #endregion

    }
}
