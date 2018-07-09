using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class ExchangeCenterConfig
    {
        public int ExchangeCenterId { get; set; }

        public int Id { get; set; }

        public int CouponId { get; set; }

        public string CouponName { get; set; }

        public int CouponSum { get; set; }

        public int CouponSurplus { get; set; }

        public int Period { get; set; }

        public int PointsValue { get; set; }

        public DateTime EndTime { get; set; }

        public bool Status { get; set; }

        public string Image { get; set; }

        public string SmallImage { get; set; }

        public string PointsRules { get; set; }

        public string Description { get; set; }

        public DateTime CreateTime { get; set; }

        public int Sort { get; set; }

        public DateTime? CouponEndTime { get; set; }

        public string CouponDuration { get; set; }

        public string Edit { get; set; }

        public string GetRuleGUID { get; set; }


        /// <summary>
        /// 配置位置 :精品通用券、会员商城
        /// </summary>
        public string Postion { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string PID { get; set; }

        /// <summary>
        /// 用户等级 参数：V0、V1、V2、V3、V4

        /// </summary>
        public string UserRank { get; set; }

        /// <summary>
        /// 券后价格
        /// </summary>
        public decimal EndCouponPrice { get; set; }

        /// <summary>
        /// 兑换类型
        /// </summary>
        public bool? ExchangeCenterType { get; set; }

        /// <summary>
        /// 概率大于等于0小于等于100
        /// </summary>
        public int? Chance { get; set; }

        public string StartVersion { get; set; }

        public string EndVersion { get; set; }
    }

    public class Description
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public int Sort { get; set; }
    }

    public class PersonalCenterCouponConfig
    {
        public int Id { get; set; }

        public int ExchangeCenterId { get; set; }

        public int HomePageModuleId { get; set; }

        public int Sort { get; set; }
    }
}
