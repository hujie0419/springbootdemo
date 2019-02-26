using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.BaoYangProductPriority
{
    public class ProductPriorityArea
    {
        public int PKID { get; set; }

        public string PartName { get; set; }

        public int RegionId { get; set; }

        public int AreaId { get; set; }

        public bool IsEnabled { get; set; }

        public DateTime CreateDateTime { get; set; }

        public DateTime LastUpdateDateTime { get; set; }
    }
}
