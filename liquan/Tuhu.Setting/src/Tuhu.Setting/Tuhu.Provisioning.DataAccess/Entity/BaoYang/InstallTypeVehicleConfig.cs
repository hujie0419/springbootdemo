using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.BaoYang
{
    public class InstallTypeVehicleConfig
    {
        public string PackageType { get; set; }

        public string InstallType { get; set; }

        public string VehicleId { get; set; }

        public string Brand { get; set; }

        public string Series { get; set; }

        public bool IsRecommend { get; set; }
    }
}
