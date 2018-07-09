using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.DailyShopSyncJob.Models
{
    public class ShopChange
    {
        public int ShopId { get; set; }

        public bool ShopService { get; set; }

        public bool ShopHoliday { get; set; }

        public bool ShopIsInActive { get; set; }

        public bool ShopDetail { get; set; }

        public DateTime CreateDateTime { get; set; }
    }
}
