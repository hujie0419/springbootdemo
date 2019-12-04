using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Promotion.Request
{
    /// <summary>
    /// 订单发券前优惠券任务验证
    /// </summary>
    public class CheckSendCouponRequest
    {
        /// <summary>
        /// 订单id
        /// </summary>
        public int OrderID { get; set; }

        /// <summary>
        /// 优惠券任务id
        /// </summary>
        public List<int>  PromotionTaskIdList { get; set; }

    }
}
