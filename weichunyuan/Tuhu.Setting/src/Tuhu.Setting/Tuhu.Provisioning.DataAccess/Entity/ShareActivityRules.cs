using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class ShareActivityRulesConfig
    {
        public int Id { get; set; }

        public string Image { get; set; }

        public bool Status { get; set; }

        public string Rules { get; set; }

        public DateTime? CreateTime { get; set; }
    }
}
