using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.PromotionTask.Model
{
    public class PromotionCodeModel
    {
        public int PKID { get; set; }
        public Guid UserId { get; set; }
        public double Discount { get; set; }
        public double MinMoney { get; set; }
        public string PromotionName { get; set; }
    }
}
