using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.PromotionTask.Model
{
    public class PromotionTaskUser
    {
        public int PromotionSingleTaskUsersId { get; set; }
        public int PromotionTaskId { get; set; }
        public string UserCellPhone { get; set; }
        public string OrderNo { get; set; }
        public DateTime CreateTime { get; set; }
    }

    public class PromotionTaskUserObject
    {
        public Guid UserId { get; set; }
        public string Mobile { get; set; }
    }
}
