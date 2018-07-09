using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class VinSprayPaintRelationship
    {
        public string Mobile { get; set; }

        public string Vin { get; set; }

        public List<VehicleSprayPaintLevel> SprayPaintLevel { get; set; }

    }

    public class VehicleSprayPaintLevel
    {
        public string VehicleLevel { get; set; }

        public List<VehicleInfoSimpleInfo> VehicleInfo { get; set; }
    }

    public class VehicleInfoSimpleInfo
    {
        public string VehicleID { get; set; }

        public string Brand { get; set; }

        public string Vehicle { get; set; }
    }

    public class VehicleInfoDetail
    {
        public string VehicleID { get; set; }

        public string TID { get; set; }

        public string Brand { get; set; }

        public string Vehicle { get; set; }

        public string VehicleLevel { get; set; }
    }

    public class VinSprayPaintModel
    {
        public string Mobile { get; set; }

        public string Vin { get; set; }

        public List<VehicleInfoSimpleInfo> VehicleInfo { get; set; }

        public string VehicleLevel { get; set; }
    }

}
