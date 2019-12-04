using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tuhu.Service.Promotion.Server.Model
{
    /// <summary>
    /// 提交工单接口 返回的数据
    /// </summary>
    public class CommitWorkOrderForCouponGetRuleAuditResponse
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 返回消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 工单Id
        /// </summary>
        public string ResponseContent { get; set; }
     



    }
}
