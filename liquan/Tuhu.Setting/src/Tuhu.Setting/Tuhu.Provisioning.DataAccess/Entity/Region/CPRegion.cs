using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class CPRegion
    {
        public int PKID { get; set; }
        public int ShopId { get; set; }
        public int RegionId { get; set; }
        public bool IsOnlyShop { get; set; }
        public DateTime LastUpdateTime { get; set; }
    }
}
