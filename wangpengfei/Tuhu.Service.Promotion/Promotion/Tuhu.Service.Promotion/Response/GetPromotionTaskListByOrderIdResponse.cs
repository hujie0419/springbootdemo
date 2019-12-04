using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Promotion.Response
{
    public  class GetPromotionTaskListByOrderIdResponse
    {
        /// <summary>
        /// 优惠券任务
        /// </summary>
        public List<PromotionTaskModel> PromotionTaskList { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public string OrderStatus { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public Guid UserID { get; set; }
    }
}
