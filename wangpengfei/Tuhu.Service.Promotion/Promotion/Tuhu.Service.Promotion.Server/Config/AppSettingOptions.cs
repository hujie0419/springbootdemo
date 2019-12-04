using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tuhu.Service.Promotion.Server.Config
{
    /// <summary>
    /// 配置文件选项  Apollo
    /// </summary>
    public class AppSettingOptions
    {
        /// <summary>
        /// 环境配置
        /// </summary>
        public string Environment { get; set; }


        /// <summary>
        /// 提交工单url
        /// </summary>
        public string CommitWorkOrderApiURL { get; set; }
        /// <summary>
        /// 优惠券审核工单号
        /// </summary>
        public string WorkOrderTypeIdForCouponGetRuleAudit { get; set; }
        /// <summary>
        /// 优惠券使用规则地址 - 域名
        /// </summary>
        public string SettingHost { get; set; }

        /// <summary>
        /// 最大执行的任务数
        /// </summary>
        public int CouponTaskExecutPageSize { get; set; }

        /// <summary>
        /// 并发执行的任务数
        /// </summary>
        public int CouponTaskMaxDegreeOfParallelism { get; set; }


        /// <summary>
        /// 优惠券任务验证等待时间 毫秒
        /// </summary>
        public int CouponTaskCheckAwaitTime { get; set; }
    }
}
