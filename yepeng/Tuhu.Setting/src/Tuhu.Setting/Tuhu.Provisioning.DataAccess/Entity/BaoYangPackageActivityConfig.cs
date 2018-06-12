using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class CouponActivityConfig
    {
        public int Id { get; set; }
        public string ActivityNum { get; set; }
        public string ActivityName { get; set; }
        public bool ActivityStatus { get; set; }
        public bool CheckStatus { get; set; }
        public string LayerImage { get; set; }
        public string ActivityImage { get; set; }
        public string CouponId { get; set; }
        public string GetRuleGUID { get; set; }
        public string ButtonChar { get; set; }

        /// <summary>
        /// 1 蓄电池活动；
        /// </summary>
        public int Type { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
    public class BaoYangPackageActivityConfig
    {
        public int Id { get; set; }
        public string ActivityNum { get; set; }
        public string ActivityName { get; set; }
        public bool ActivityStatus { get; set; }
        public bool CheckStatus { get; set; }
        public string LayerImage { get; set; }
        public string ActivityImage { get; set; }
        public string CouponId { get; set; }
        public string GetRuleGUID { get; set; }
        public string ButtonChar { get; set; }

        public DateTime? CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
    }

    public class BaoYangPackageImageConfig
    {
        public int Id { get; set; }
        public string PID { get; set; }
        public string Image { get; set; }
        public string Image2 { get; set; }
        public string DisplayName { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
