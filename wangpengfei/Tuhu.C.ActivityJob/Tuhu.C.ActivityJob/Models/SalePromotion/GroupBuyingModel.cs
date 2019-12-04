using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.ActivityJob.Models.SalePromotion
{
    public class GroupBuyingModel
    {
        public string ProductGroupId { get; set; }

        public string ProductName { get; set; }

        public DateTime BeginTime { get; set; }

        public DateTime EndTime { get; set; }
    }
}
