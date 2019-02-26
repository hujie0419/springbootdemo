using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class VehicleOilSetting
    {
        public string Brand { get; set; }
        public string VehicleId { get; set; }
        public string Vehicle { get; set; }
        public string PaiLiang { get; set; }

        public string RecommendViscosity { get; set; }
        public string Viscosity { get; set; }
    }
}
