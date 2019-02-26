using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class BaoYangPackageBrandPriorityConfig
    {
        public int Id { get; set; }

        public string VehicleID { get; set; }

        public string Brand { get; set; }

        public string Vehicle { get; set; }

        public string PackageBrands { get; set; }

        public string JiYouGrade { get; set; }

        public decimal MinPrice { get; set; }
        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }
    }

    public class Brands
    {
        public string Brand { get; set; }

        public int Sort { get; set; }
    }
}
