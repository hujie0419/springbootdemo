using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.BaoYangProductPriority
{
    public class ProductPriorityAreaDetail
    {
        public int PKID { get; set; }
        public string PartName { get; set; }
        /// <summary>
        /// 地区分组ID
        /// </summary>
        public int AreaId { get; set; }
        public string VehicleId { get; set; }

        public string Brand { get; set; }

        public string Series { get; set; }

        public bool IsEnabled { get; set; }

        public bool IsDeleted { get; set; }

        public int Seq { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime LastUpdateDateTime { get; set; }
    }
}
