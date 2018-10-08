using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class BaoYangRecommendServiceConfig
    {
        public int Id { get; set; }

        public string Number { get; set; }

        public string ServiceType { get; set; }

        public string ServiceName { get; set; }

        public string GetRuleGUID { get; set; }

        public string CouponNames { get; set; }

        public bool Recommend { get; set; }

        public bool Status { get; set; }

        public string Tag { get; set; }    

        public DateTime? CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }
    }

    public class CouponExtend
    {
        public string RuleID { get; set; }

        public string Name { get; set; }
    }
}
