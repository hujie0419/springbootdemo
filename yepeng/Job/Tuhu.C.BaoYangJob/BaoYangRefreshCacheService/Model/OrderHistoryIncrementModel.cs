using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaoYangRefreshCacheService.Model
{
    public class OrderHistoryIncrementModel
    {
        public Guid UserId { get; set; }

        public string VehicleId { get; set; }

        public int OrderId { get; set; }

        public string Pid { get; set; }

        public bool IsDeleted { get; set; }
    }
}
