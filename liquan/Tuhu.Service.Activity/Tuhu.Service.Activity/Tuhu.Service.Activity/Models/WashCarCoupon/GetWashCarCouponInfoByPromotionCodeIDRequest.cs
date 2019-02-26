using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.WashCarCoupon
{
    /// <summary>
    /// 根据优惠券id获取  一分钱洗车优惠券领取记录
    /// </summary>
    public class GetWashCarCouponInfoByPromotionCodeIDRequest
    {
        /// <summary>
        /// 优惠券id
        /// </summary>
        public int PromotionCodeID { get; set; }
    }
}
