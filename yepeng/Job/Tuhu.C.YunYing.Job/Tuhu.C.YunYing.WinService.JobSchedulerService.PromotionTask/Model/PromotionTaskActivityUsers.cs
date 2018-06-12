using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.PromotionTask.Model
{
    public class PromotionTaskActivityUsers
    {
        public long PKID { get; set; }
        public int PromotionTaskActivityId { get; set; }
        public string UserTel { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
