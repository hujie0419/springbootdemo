using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Promotion.Response
{
    /// <summary>
    /// 查询 优惠券领取规则  - 分页
    /// </summary>
    public class GetCouponRuleListResponse
    {
        /// <summary>
        /// 领取规则主键
        /// </summary>
        public int GetRuleID { get; set; }

        /// <summary>
        /// 领取规则guid
        /// </summary>
        public Guid GetRuleGUID { get; set; }

        /// <summary>
        /// 优惠券名称
        /// </summary>
        public string PromtionName { get; set; }

        /// <summary>
        /// 优惠券描述
        /// </summary>
        public string Description { get; set; }
    }
}
