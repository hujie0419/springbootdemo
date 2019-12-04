using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.ActivityJob.Models.SalePromotion
{
    public class SalePromotionModel
    {
        public string ActivityId { get; set; }

        public string Name { get; set; }

        public int AuditStatus { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }
    }
}
