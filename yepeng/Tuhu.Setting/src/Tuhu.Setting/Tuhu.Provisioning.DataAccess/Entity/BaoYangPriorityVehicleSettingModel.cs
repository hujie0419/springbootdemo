using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class BaoYangPriorityVehicleSettingModel
    {
        public string VehicleID { get; set; }
        public string Brand { get; set; }
        public string VehicleSeries { get; set; }
        public string PartName { get; set; }
        public string PriorityType { get; set; }
        public string FirstPriority { get; set; }
        public string SecondPriority { get; set; }
        public double MinPrice { get; set; }
        public string Viscosity { get; set; }
    }
}
