using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tuhu.Provisioning.Models
{
    public class PromotionTaskCouponRuleModel
    {
        ///// <summary>
        ///// 任务优惠券编号
        ///// </summary>
        //public int? TaskPromotionListId { get; set; }
        ///// <summary>
        ///// 优惠券任务编号
        ///// </summary>
        //public int? PromotionTaskId { get; set; }
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
        public string StartTime { get; set; }

        /// <summary>
        /// 优惠券使用结束时间
        /// </summary>
        public string EndTime { get; set; }
        /// <summary>
        /// 使用金额  满多少金额可用
        /// </summary>
        public double? MinMoney { get; set; }
        /// <summary>
        /// 优惠金额 优惠券面值
        /// </summary>
        public double? DiscountMoney { get; set; }
        public string FinanceMarkName { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string DepartmentName { get; set; }
        /// <summary>
        /// 用途
        /// </summary>
        public string IntentionName { get; set; }
        /// <summary>
        /// 业务线
        /// </summary>
        public string BusinessLineName { get; set; }
        /// <summary>
        /// 发放数量
        /// </summary>
        public int? Number { get; set; }
        /// <summary>
        /// 发券后是否提醒
        /// </summary>
        public int IsRemind { get; set; }
        /// <summary>
        /// 到期前是否提醒
        /// </summary>
        public int IsPush { get; set; }
        /// <summary>
        /// 提前多少天提醒 逗号隔开
        /// </summary>
        public string PushSetting { get; set; }
    }
}