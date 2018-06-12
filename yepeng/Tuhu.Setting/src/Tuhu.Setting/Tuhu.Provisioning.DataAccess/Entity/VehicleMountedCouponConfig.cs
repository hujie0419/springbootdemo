
using System;
using System.Collections.Generic;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class VehicleMountedCouponConfig
    {
        public int Id { get; set; }

        public string Channel { get; set; }

        public string Versions { get; set; }

        public string Region { get; set; }

        public List<RegionModel> RegionList { get; set; }

        public bool Status { get; set; }

        public string Images { get; set; }

        public string Coupons { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }

        public string CreateName { get; set; }

        public string UpdateName { get; set; }

        public string ActivityId { get; set; }

        public short Location { get; set; }

        public string Name { get; set; }

        public int CouponQuantity { get; set; }

        public short Type { get; set; }
    }
}
