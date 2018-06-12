using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class ShareActivityProductConfig
    {
        public int Id { get; set; }

        public bool Status { get; set; }

        public string ProductId { get; set; }

        public string ProductName { get; set; }

        public DateTime? CreateTime { get; set; }
    }
}
