using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Promotion.Request
{
    /// <summary>
    /// 请求 - 获取用户优惠券
    /// </summary>
    public class GetCouponByUserIDRequest
    {
        /// <summary>
        /// 用户GUID
        /// </summary>
        public Guid UserID { get; set; }

        /// <summary>
        /// 默认0 = 查询全部，1 = 归档，2 = 未归档
        /// </summary>
        public int IsHistory { get; set; }
    }
}
