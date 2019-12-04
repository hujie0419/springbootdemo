using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Promotion.Response
{
    /// <summary>
    /// 返回值 -  获取优惠券领取规则的审核人
    /// </summary>
    public class GetCouponGetRuleAuditorResponse
    {
        /// <summary>
        /// 审核人[多个使用,连接]
        /// </summary>
        public string Auditors { get; set; }
    }
}
