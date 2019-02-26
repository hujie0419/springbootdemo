using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.ShopDistribution
{
    public class ShopDistributionModel
    {
        public ShopDistributionModel() { }
        public int PKID { get; set; }
        public string FKPID { get; set; }
        public string DisplayName { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateTime { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateTime { get; set; }
        public int IsDelete { get; set; }

    }
}
