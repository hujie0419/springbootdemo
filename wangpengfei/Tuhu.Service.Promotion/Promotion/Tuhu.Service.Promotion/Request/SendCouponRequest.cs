using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Promotion.Request
{
    /// <summary>
    /// 发放优惠券
    /// </summary>
    public class SendCouponRequest
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public int OrderID { get; set; }

        //<summary>
        // 订单状态。0New；1Booked；2Shipped；3Installed；4Paied；6Complete；7Canceled；Ongoing  [此处防并发使用]
        //</summary>  
        public string Status { get; set; }

        /// <summary>
        /// 用户账号
        /// </summary>
        public Guid UserID { get; set; }

        /// <summary>
        /// 优惠券任务id
        /// </summary>
        public int PromotionTaskId { get; set; }

        /// <summary>
        /// 每人限塞几次
        /// </summary>		
        public bool IsLimitOnce { get; set; }

        /// <summary>
        /// 如果要发短信 ，需要短信模板id
        /// </summary>		
        public int SmsId { get; set; }

        /// <summary>
        /// 短信模板对应的参数格式为json数组
        /// </summary>		
        public string SmsParam { get; set; }

        /// <summary>
        /// 创建者email
        /// </summary>		
        public string Creater { get; set; }

    }
}
