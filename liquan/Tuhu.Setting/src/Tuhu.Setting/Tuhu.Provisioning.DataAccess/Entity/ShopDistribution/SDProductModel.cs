using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.ShopDistribution
{
    public class SDProductModel
    {
        public string PID { get; set; }
        public string DisplayName { get; set; }
        public string Category { get; set; }
        public int Onsale { get; set; }
    }
}
