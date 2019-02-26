using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class PointsTransactionDescriptionConfig
    {
        public string IntegralRuleID { get; set; }

        public int IntegralType { get; set; }

        public string IntegralConditions { get; set; }

        public string IntegralDescribe { get; set; }

        public string Remark { get; set; }

        public string NeedeKeys { get; set; }

        public bool IsActive { get; set; }
    }
}
