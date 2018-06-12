using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class VIPAuthorizationRuleConfig
    {
        public int Id { get; set; }

        public string RuleName { get; set; }

        public string Description { get; set; }

        public int ValidityDay { get; set; }

        public int PrivilegeType { get; set; }

        public DateTime? CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }

    }
}
