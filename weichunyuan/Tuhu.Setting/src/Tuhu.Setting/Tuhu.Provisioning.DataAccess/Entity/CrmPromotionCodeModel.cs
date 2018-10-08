using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class CrmPromotionCodeModel
    {
        public string Code { get; set; }
        public int Type { get; set; }
        public string Description { get; set; }
        public int MinMoney { get; set; }
        public int Discount { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime UsedTime { get; set; }
        public int OrderId { get; set; }
        public int Status { get; set; }
        public int PkId { get; set; }
    }
}
