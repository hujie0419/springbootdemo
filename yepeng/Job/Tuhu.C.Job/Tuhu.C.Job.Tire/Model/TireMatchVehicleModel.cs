using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.Tire.Model
{
    public class VehicleMatchTireModel
    {
        public string ProductID { get; set; }
        public string TiresMatch { get; set; }
    }

    public class TireMatchVehicleModel {
        public string Tire { get; set; }
        public string Vehicle { get; set; }
    }

    public class TireMatchVehicleCount
    {
        public string PID { get; set; }
        public int VehicleCount { get; set; }
    }
}
