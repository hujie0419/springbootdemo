using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.ActivityJob.Models.SalePromotion
{
    public class GiftModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime? ValidTimeBegin { get; set; }

        public DateTime? ValidTimeEnd { get; set; }
    }
}
