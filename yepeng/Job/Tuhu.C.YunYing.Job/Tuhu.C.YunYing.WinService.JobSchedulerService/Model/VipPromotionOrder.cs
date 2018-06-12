using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.Model
{
   public class VipPromotionOrder
    {
        public class VipPromotionOrderModel
        {
            /// <summary>
            /// 订单类型
            /// </summary>
            public string OrderType { get; set; }

            /// <summary>
            /// 订单号
            /// </summary>
            public int OrderId { get; set; }
        }
    }
}
