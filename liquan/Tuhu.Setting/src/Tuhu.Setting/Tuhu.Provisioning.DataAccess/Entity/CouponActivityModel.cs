using System;
using System.Collections.Generic;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class CouponActivityModel
    {
        public Guid ActivityID { get; set; }
        public string ActivityName { get; set; }
        public string ActivityChannel { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int CouponNum { get; set; }
        public int GetNum { get; set; }
        public string BannerImg { get; set; }
        public string ActivityRuleImg { get; set; }
        public int IsValid { get; set; }
        public int IsNewUser { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime LastUpdateDateTime { get; set; }

        public IEnumerable<CouponActivity_CouponModel> Coupons { get; set; }
        public CouponActivity_PageModel SuccessPage { get; set; }
        public CouponActivity_PageModel ErrorPage { get; set; }

        /// <summary>
        /// 是否是限时抢购 0非 1是
        /// </summary>
        public int ActivityType { get; set; }

    }

    public class CouponActivity_CouponModel
    {
        public int PKID { get; set; }
        public Guid ActivityID { get; set; }
        public int CouponType { get; set; }
        public decimal Money { get; set; }
        public decimal MinMoney { get; set; }
        public byte DateWay { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? ValidDays { get; set; }
        public string Instructions { get; set; }
        public DateTime CreateDateTime { get; set; }

        /// <summary>
        /// 优惠卷数量
        /// </summary>
        public int CouponNum { get; set; }

        public int CouponId { get; set; }

    }

    public class CouponActivity_PageModel
    {
        public Guid ActivityID { get; set; }
        public byte PageNumber { get; set; }
        public string TopImg { get; set; }
        public string CenterImg { get; set; }
        public string BottomImg { get; set; }
        public string IOSLink { get; set; }
        public string AndroidLink { get; set; }
        public int JumpMillisecond { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime LastUpdateDateTime { get; set; }
    }
}
