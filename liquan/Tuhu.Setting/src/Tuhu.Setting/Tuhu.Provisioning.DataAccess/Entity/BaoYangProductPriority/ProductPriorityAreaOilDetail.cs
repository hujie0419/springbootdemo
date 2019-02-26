using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.BaoYangProductPriority
{
    public class ProductPriorityAreaOilDetail
    {
        public int PKID { get; set; }
        /// <summary>
        /// 机油推荐ID
        /// </summary>
        public int AreaOilId { get; set; }
        public string Brand { get; set; }
        public string Series { get; set; }
        public int Seq { get; set; } 
        public string Grade { get; set; }

        public DateTime CreateDateTime { get; set; }

        public DateTime LastUpdateDateTime { get; set; }

    }
}
