using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Promotion.Response
{
    /// <summary>
    /// 优惠券任务发券规则
    /// </summary>
    public class PromotionTaskPromotionListModel
    {
        /// <summary>
        /// 优惠券2.0 的优惠券id
        /// </summary>		
        public int TaskPromotionListId { get; set; }
        /// <summary>
        /// PromotionTaskId 外键 对应 tbl_PromotionTask  的主键
        /// </summary>		
        public int PromotionTaskId { get; set; }
        /// <summary>
        /// 对应的优惠券主券id(RuleId)
        /// </summary>		
        public int CouponRulesId { get; set; }
        /// <summary>
        /// 优惠券描述
        /// </summary>		
        public string PromotionDescription { get; set; }
        /// <summary>
        /// 优惠券开始时间
        /// </summary>		
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 优惠券结束时间
        /// </summary>		
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 优惠券使用金额
        /// </summary>		
        public decimal MinMoney { get; set; }
        /// <summary>
        /// 优惠券折扣金额
        /// </summary>		
        public decimal DiscountMoney { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>		
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>		
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// FinanceMarkName
        /// </summary>		
        public string FinanceMarkName { get; set; }
        /// <summary>
        /// Issuer
        /// </summary>		
        public string Issuer { get; set; }
        /// <summary>
        /// 渠道
        /// </summary>		
        public string IssueChannle { get; set; }
        /// <summary>
        /// 渠道id
        /// </summary>		
        public string IssueChannleId { get; set; }
        /// <summary>
        /// 部门id
        /// </summary>		
        public int DepartmentId { get; set; }
        /// <summary>
        /// 用途id
        /// </summary>		
        public int IntentionId { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>		
        public string Creater { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>		
        public string DepartmentName { get; set; }
        /// <summary>
        /// 用途名称
        /// </summary>		
        public string IntentionName { get; set; }
        /// <summary>
        /// 发券数目
        /// </summary>		
        public int Number { get; set; }
        /// <summary>
        /// 业务线id
        /// </summary>		
        public int BusinessLineId { get; set; }
        /// <summary>
        /// 业务线名称
        /// </summary>		
        public string BusinessLineName { get; set; }
        /// <summary>
        /// 是否提前提醒
        /// </summary>		
        public int IsPush { get; set; }
        /// <summary>
        /// 提前多少天提醒，多个用逗号隔开
        /// </summary>		
        public string PushSetting { get; set; }
        /// <summary>
        /// 塞券后是否立即提醒 [1 = 推送]
        /// </summary>		
        public string IsRemind { get; set; }
        /// <summary>
        /// 优惠券领取规则id
        /// </summary>		
        public int GetCouponRuleID { get; set; }
    }
}
