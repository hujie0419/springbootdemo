using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    /// <summary>
    /// 优惠券任务赠券规则
    /// </summary>
    public class PromotionTaskCouponRule
    {
        /// <summary>
        /// 任务优惠券编号
        /// </summary>
        public int? TaskPromotionListId { get; set; }
        /// <summary>
        /// 优惠券任务编号
        /// </summary>
        public int? PromotionTaskId { get; set; }
        /// <summary>
        /// 赠券规则编号
        /// </summary>
        public int? CouponRulesId { get; set; }

        /// <summary>
        /// 赠券规则名称
        /// </summary>
        public string RuleName { get; set; }
        /// <summary>
        /// 优惠券描述
        /// </summary>
        public string PromotionDescription { get; set; }
        /// <summary>
        /// 优惠券使用开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 优惠券使用结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// 使用金额  满多少金额可用
        /// </summary>
        public double? MinMoney { get; set; }
        /// <summary>
        /// 优惠金额 优惠券面值
        /// </summary>
        public double? DiscountMoney { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        /// <summary>
        /// 财务标识名称
        /// </summary>
        public string FinanceMarkName { get; set; }
        /// <summary>
        /// 财务标识code
        /// </summary>
        public int FinanceMarkId { get; set; }
        /// <summary>
        /// 部门id
        /// </summary>
        public int DepartmentId { get; set; }
        /// <summary>
        /// 用途id
        /// </summary>
        public int IntentionId { get; set; }
        /// <summary>
        /// 业务线id
        /// </summary>
        public int BusinessLineId { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string Creater { get; set; }
        /// <summary>
        /// 发放人
        /// </summary>
        public string Issuer { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string DepartmentName { get; set; }
        /// <summary>
        /// 用途
        /// </summary>
        public string IntentionName { get; set; }
        /// <summary>
        /// 业务线名称
        /// </summary>
        public string BusinessLineName { get; set; }
        /// <summary>
        /// 发放数量
        /// </summary>
        public int? Number { get; set; }
        /// <summary>
        /// 发券之后是否立即提醒
        /// </summary>
        public int IsRemind { get; set; }
        /// <summary>
        /// 是否提前通知用户优惠券到期
        /// </summary>
        public int IsPush { get; set; }
        /// <summary>
        /// 提前通知的天数 多个用逗号隔开
        /// </summary>
        public string PushSetting { get; set; }
    }
}