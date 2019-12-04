using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Promotion.Request
{
    /// <summary>
    /// 关闭优惠券任务
    /// </summary>
    public class ClosePromotionTaskByPkidRequest
    {
        /// <summary>
        /// 优惠券任务id
        /// </summary>
        public int PromotionTaskId { get; set; }
    }
}
