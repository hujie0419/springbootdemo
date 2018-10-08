using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class ThirdPartyCouponConfigModel
    {
        public int PKID { get; set; }

        public string ThirdPartyCouponPatch { get; set; }

        public string PatchDescription { get; set; }

        public string ThirdPartyChannel { get; set; }

        public Guid CouponGetRuleId { get; set; }

        public string CouponName { get; set; }

        public string CouponDescription { get; set; }

        public decimal? CouponDiscount { get; set; }

        public decimal? CouponMinMoney { get; set; }

        public int? CouponEffectDuration { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime UpDateTime { get; set; }

        public int TotalCount { get; set; }
    }
}
