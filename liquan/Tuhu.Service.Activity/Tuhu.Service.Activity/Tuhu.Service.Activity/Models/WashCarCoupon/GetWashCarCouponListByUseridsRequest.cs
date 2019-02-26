using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.WashCarCoupon
{
    /// <summary>
    /// 根据用户userid 获取 一分钱的优惠券 记录 
    /// </summary>
    public class GetWashCarCouponListByUseridsRequest
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public Guid UserID { get; set; }
    }
}
