using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.Tire
{
    public class InstallFeeConditionModel
    {
        public string Brands { get; set; }

        public string Patterns { get; set; }
        public string TireSizes { get; set; }
        public string Rims { get; set; }
        public string Rof { get; set; }
        public string Winter { get; set; }
        public string PID { get; set; }
        public bool? IsConfig { get; set; }

        public int? OnSale { get; set; }
    }

    public class InstallFeeModel
    {
        public string PID { get; set; }
        public string DisplayName { get; set; }
        public string TireSize { get; set; }

        public decimal? AddPrice { get; set; }
    }

    public class PIDandPrice
    {
        public decimal OldPrice { get; set; }
        public string PID { get; set; }
    }
}
