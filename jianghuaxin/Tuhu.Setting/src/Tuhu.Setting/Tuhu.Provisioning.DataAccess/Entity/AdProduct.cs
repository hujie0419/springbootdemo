using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class AdProduct
    {
        public int AdvertiseID { get; set; }
        public string PID { get; set; }
        public byte Position { get; set; }
        public byte State { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime LastUpdateDateTime { get; set; }
        public decimal PromotionPrice { get; set; }
        public int PromotionNum { get; set; }
    }
}
