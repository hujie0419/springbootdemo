using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models
{
    public class FixedPriceActivityRoundResponse
    {
        public Guid ActivityId { get; set; }

        public string ActivityName { get; set; }

        public int UserLimitedQuantity { get; set; }

        public int LimitedQuantity { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int? CurrentPurchaseCount { get; set; }

        public List<string> OrderInfos { get; set; }

    }
}
