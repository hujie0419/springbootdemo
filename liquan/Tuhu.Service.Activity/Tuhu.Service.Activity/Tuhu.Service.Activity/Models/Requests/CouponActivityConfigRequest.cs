using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Requests
{
    public class CouponActivityConfigRequest
    {
        /// <summary>
        /// 活动Guid
        /// </summary>
        public string ActivityNum { get; set; }

        /// <summary>
        /// 类型
        /// 1--蓄电池 2--加油卡
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 渠道
        /// </summary>
        public string Channel { get; set; }
    }
}
