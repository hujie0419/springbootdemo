using System;

namespace Tuhu.Service.Promotion.Request
{
    /// <summary>
    /// 获取获优惠券领取规则审核记录 - 分页
    /// </summary>
    public class GetCouponGetRuleAuditListRequest
    {

        /// <summary>
        /// 使用规则id
        /// </summary>
        public int RuleID { get; set; }

        /// <summary>
        ///领取规则guid
        /// </summary>
        public Guid GetRuleGUID { get; set; }

        /// <summary>
        /// 优惠券领取规则 名称
        /// </summary>
        public string PromtionName { get; set; }


        /// <summary>
        /// 优惠券领取规则 描述
        /// </summary>
        public string Description { get; set; }


        /// <summary>
        ///只是用户范围0全部，1新用户，2老用户
        /// </summary>
        public int SupportUserRange { get; set; }


        /// <summary>
        /// 优惠券领取折扣 下限
        /// </summary>
        public decimal MinDiscount { get; set; }

        /// <summary>
        /// 优惠券领取折扣 上限
        /// </summary>
        public decimal MaxDiscount { get; set; }

        /// <summary>
        /// 优惠券使用门槛价格 下限
        /// </summary>
        public decimal MinMinmoney { get; set; }
        /// <summary>
        /// 优惠券使用门槛价格 上限
        /// </summary>
        public decimal MaxMinmoney { get; set; }

        /// <summary>
        /// 是否允许电销发放
        /// </summary>
        public int AllowChanel { get; set; }


        /// <summary>
        ///部门id
        /// </summary>
        public int DepartmentId { get; set; }

        /// <summary>
        /// 用途ID
        /// </summary>
        public int IntentionId { get; set; }

        /// <summary>
        ///审核状态 【0=草稿，1=审核中 ， 2=审核通过，3=审核不通过，4=已撤销,5=紧急审核通过不经过工单系统审核】
        /// </summary>
        public int AuditStatus { get; set; }


        /// <summary>
        ///  当前页
        /// </summary>
        public int CurrentPage { get; set; }
        /// <summary>
        /// 每页数量
        /// </summary>     
        public int PageSize { get; set; }

       


    }
}
