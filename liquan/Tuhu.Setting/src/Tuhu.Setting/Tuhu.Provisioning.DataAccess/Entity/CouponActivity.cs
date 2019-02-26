using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class CouponActivity
    {
        public Guid? ActivityKey { get; set; }
        public int ActivityId { get; set; }
        public string SmallImageUrl { get; set; }
        public string BigImageUrl { get; set; }
        public string ButtonBackGroundColor { get; set; }
        public string ButtonTextColor { get; set; }
        public string ButtonText { get; set; }
        public string Url { get; set; }
        public string IosJson { get; set; }
        public string AndroidJson { get; set; }
        public bool IsSendCoupon { get; set; }
        public string LayerButtonText { get; set; }
        public string LayerButtonBackGroundColor { get; set; }
        public string LayerButtonTextColor { get; set; }
        //public int PromotionType { get; set; }
        public int PromotionRuleId { get; set; }
        public int PromotionMinMoney { get; set; }
        public int PromotionDiscount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PromotionDescription { get; set; }
        public string PromotionCodeChannel { get; set; }
    }

    public class WebCouponActivity
    {
        public Guid? ActivityKey { get; set; }
        public int ActivityId { get; set; }
        public string SmallBannerImageUrl { get; set; }
        public string BigBannerImageUrl { get; set; }
        public string SmallContentImageUrl { get; set; }
        public string BigContentImageUrl { get; set; }
        public string ButtonBackGroundColor { get; set; }
        public string ButtonTextColor { get; set; }
        public string ButtonText { get; set; }
        public string Url { get; set; }
        public bool IsSendCoupon { get; set; }
        public string LayerButtonText { get; set; }
        public string LayerButtonBackGroundColor { get; set; }
        public string LayerButtonTextColor { get; set; }
      //  public int PromotionType { get; set; }
        public int PromotionRuleId { get; set; }
        public int PromotionMinMoney { get; set; }
        public int PromotionDiscount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PromotionDescription { get; set; }
        public string PromotionCodeChannel { get; set; }

        /// <summary>
        /// 优惠券GetRuleGUID
        /// </summary>
        public Guid? PromotionRuleGUID { get; set; }
		
    }

    public class WebCouponActivityRuleModel
    {
        //
        // 摘要:
        //     用途名称
        public string IntentionName { get; set; }
        //
        // 摘要:
        //     部门名称
        public string DepartmentName { get; set; }
        //
        // 摘要:
        //     创建人
        public string Creater { get; set; }
        public Guid GetRuleGUID { get; set; }
        //
        // 摘要:
        //     截止日期///
        public DateTime? ValiEndDate { get; set; }
        //
        // 摘要:
        //     开始日期///
        public DateTime? ValiStartDate { get; set; }
        //
        // 摘要:
        //     最终有效期
        public DateTime? DeadLineDate { get; set; }
        public int? Term { get; set; }
        //
        // 摘要:
        //     满足最低价///
        public decimal MinMoney { get; set; }
        //
        // 摘要:
        //     优惠金额///
        public decimal Discount { get; set; }
        //
        // 摘要:
        //     优惠券名称///
        public string PromotionName { get; set; }
        public string Description { get; set; }
        //
        // 摘要:
        //     优惠券规则Id///
        public int RuleID { get; set; }

        public string Channel { get; set; }
    }
}
