using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.PromotionTask.Model
{
    public class PromotionTaskHistoryUsers
    {
        public int PromotionSingleTaskUsersHistoryId { get; set; }
        public int PromotionTaskId { get; set; }
        public string UserCellPhone {get;set;}
        public string UserId { get; set; }
        public string OrderNo { get; set; }
    }
}
