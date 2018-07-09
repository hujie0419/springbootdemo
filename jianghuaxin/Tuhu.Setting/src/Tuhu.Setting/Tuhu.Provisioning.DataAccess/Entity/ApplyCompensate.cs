using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class ApplyCompensate
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string PhoneNumber { get; set; }

        public string OrderId { get; set; }

        public string ProductName { get; set; }

        public string Link { get; set; }

        public decimal DifferencePrice { get; set; }

        public string Images { get; set; }

        public DateTime? ApplyTime { get; set; }
        public DateTime? StartApplyTime { get; set; }
        public DateTime? EndApplyTime { get; set; }

        public DateTime? AuditTime { get; set; }
        public DateTime? StartAuditTime { get; set; }
        public DateTime? EndAuditTime { get; set; }

        public int Status { get; set; }

        public string OrderChannel { get; set; }
    }
}
