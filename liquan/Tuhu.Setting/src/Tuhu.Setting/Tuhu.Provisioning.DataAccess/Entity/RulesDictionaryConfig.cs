using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class RulesDictionaryConfig
    {
        public int Id { get; set; }

        public int Type { get; set; }

        public string Content { get; set; }

        public bool Status { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
