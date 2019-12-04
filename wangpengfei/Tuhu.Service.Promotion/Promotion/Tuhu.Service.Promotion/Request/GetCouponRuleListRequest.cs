using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Promotion.Request
{
    /// <summary>
    /// 获取 领取规则
    /// </summary>
    public class GetCouponRuleListRequest
    {
        /// <summary>
        /// 领取规则名称
        /// </summary>
        public string PromtionName { get; set; }

        /// <summary>
        /// 是否允许电销发放  1是，0全部
        /// </summary>
        public int AllowChanel { get; set; }


        /// <summary>
        /// 规则GUID
        /// </summary>
        public Guid GetRuleGUID { get; set; }

        /// <summary>
        /// 只是用户范围0全部，1新用户，2老用户
        /// </summary>
        public int SupportUserRange { get; set; }


        /// <summary>
        ///  当前页
        /// </summary>
        public int CurrentPage = 1;
        /// <summary>
        /// 每页数量
        /// </summary>     
        public int PageSize = 10;

        /// <summary>
        /// 电销有效区间必须包含的时间
        /// </summary>
        public DateTime? DXDateTime { get; set; }

        /// <summary>
        /// 使用规则id
        /// </summary>
        public int RuleID { get; set; }
    }
}
