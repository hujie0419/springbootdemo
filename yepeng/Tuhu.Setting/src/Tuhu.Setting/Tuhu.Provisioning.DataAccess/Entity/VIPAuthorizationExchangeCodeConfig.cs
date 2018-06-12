using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class VIPAuthorizationExchangeCodeConfig
    {
        public string ExchangeCode { get; set; }

        public string CodeBatch { get; set; }

        public int VIPAuthorizationRuleId { get; set; }

        public string RuleName { get; set; }

        public int Status { get; set; }

        public DateTime? CreateTime { get; set; }

        public DateTime? EndTime { get; set; }

        public int SumNum { get; set; }
    }
}
