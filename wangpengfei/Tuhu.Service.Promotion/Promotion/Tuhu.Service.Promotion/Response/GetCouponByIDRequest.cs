using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Promotion.Response
{
    /// <summary>
    /// 根据pkid获取用户的优惠券
    /// </summary>
    public class GetCouponByIDRequest
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int pkid { get; set; }
    }
}
