using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Promotion.Response
{
    /// <summary>
    /// 优惠券领取审核记录
    /// </summary>
    public class GetCouponRuleAuditModel
    {

        public int PKID { get; set; }
        /// <summary>
        ///  领取规则的pkid
        /// </summary>
        public int GetCouponRulePKID { get; set; }


        /// <summary>
        /// 使用规则id
        /// </summary>
        public int RuleID { get; set; }



        /// <summary>
        ///  优惠券领取规则描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 优惠券领取规则 名称
        /// </summary>
        public string PromtionName { get; set; }


        /// <summary>
        /// 优惠券领取折扣
        /// </summary>
        public decimal? Discount { get; set; }

        /// <summary>
        /// 优惠券使用门槛价格
        /// </summary>
        public decimal? Minmoney { get; set; }


        /// <summary>
        /// 自领取后多少天有效
        /// </summary>
        public int? Term { get; set; }

        /// <summary>
        /// 固定有效期 开始时间
        /// </summary>
        public DateTime? ValiStartDate { get; set; }

        /// <summary>
        /// 固定有效期 结束时间
        /// </summary>
        public DateTime? ValiEndDate { get; set; }


        /// <summary>
        /// 发行量
        /// </summary>
        public int? Quantity { get; set; }


        /// <summary>
        /// 已领取数目
        /// </summary>
        public int GetQuantity { get; set; }

        /// <summary>
        ///单个人限制领取数量
        /// </summary>
        public int SingleQuantity { get; set; }

        /// <summary>
        /// 是否允许电销发放
        /// </summary>
        public int AllowChanel { get; set; }

        /// <summary>
        ///电销 固定有效期 开始时间
        /// </summary>
        public DateTime? DXStartDate { get; set; }

        /// <summary>
        ///电销 固定有效期 结束时间
        /// </summary>
        public DateTime? DXEndDate { get; set; }

        /// <summary>
        ///领取规则guid
        /// </summary>
        public Guid GetRuleGUID { get; set; }

        /// <summary>
        ///发放渠道
        /// </summary>
        public string Channel { get; set; }


        /// <summary>
        ///只是用户范围0全部，1新用户，2老用户
        /// </summary>
        public int SupportUserRange { get; set; }

        /// <summary>
        ///详情页展示开始时间
        /// </summary>
        public DateTime? DetailShowStartDate { get; set; }

        /// <summary>
        ///详情页展示结束时间
        /// </summary>
        public DateTime? DetailShowEndDate { get; set; }

        /// <summary>
        ///部门id
        /// </summary>
        public int DepartmentId { get; set; }

        /// <summary>
        ///部门名称
        /// </summary>
        public string DepartmentName { get; set; }

        /// <summary>
        /// 用途ID
        /// </summary>
        public int IntentionId { get; set; }

        /// <summary>
        ///部门名称
        /// </summary>
        public string IntentionName { get; set; }

        /// <summary>
        /// 	创建者email
        /// </summary>
        public string Creater { get; set; }

        /// <summary>
        /// 	规则类型
        /// </summary>
        public int CouponType { get; set; }

        /// <summary>
        /// 最终有效期
        /// </summary>
        public DateTime? DeadLineDate { get; set; }



        /// <summary>
        ///到期前是否提醒
        /// </summary>
        public bool IsPush { get; set; }


        /// <summary>
        ///到期前是否提醒
        /// </summary>
        public string PushSetting { get; set; }

        /// <summary>
        ///业务线id
        /// </summary>
        public int BusinessLineId { get; set; }

        /// <summary>
        ///业务线名称
        /// </summary>
        public string BusinessLineName { get; set; }

        /// <summary>
        ///抢光警戒线
        /// </summary>
        public int? RemindQuantity { get; set; }

        /// <summary>
        ///抢光警戒线
        /// </summary>
        public string RemindEmails { get; set; }

        /// <summary>
        ///申请人账号或者邮箱
        /// </summary>
        public string Applicant { get; set; }

        /// <summary>
        /// 审核后填写最终审核人
        /// </summary>
        public string Auditor { get; set; }

        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime AuditDateTime { get; set; }

        /// <summary>
        ///审核状态 【0=草稿，1=审核中 ， 2=审核通过，3=审核不通过，4=已撤销,5=紧急审核通过不经过工单系统审核】
        /// </summary>
        public int AuditStatus { get; set; }

        /// <summary>
        ///审核信息 【备注，如审核不通过原因】
        /// </summary>
        public string AuditMessage { get; set; }

        /// <summary>
        ///工单号
        /// </summary>
        public int WorkOrderId { get; set; }

        public DateTime CreateDateTime { get; set; }
        public DateTime LastUpdateDateTime { get; set; }

    }
}
