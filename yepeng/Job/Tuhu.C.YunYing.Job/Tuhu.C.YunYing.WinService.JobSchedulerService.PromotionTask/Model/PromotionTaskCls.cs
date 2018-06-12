using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.PromotionTask.Model
{
    public class PromotionTaskCls
    {
        public int PromotionTaskId { get; set; }
        public string TaskName { get; set; }
        public DateTime TaskStartTime { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public int Tasktype { get; set; }
        public int IsLimitOnce { get; set; }
        public int SelectUserType { get; set; }
        public int SmsId { get; set; }
        public string SmsParam { get; set; }
        public int PromotionTaskActivityId { get; set; }
        public int TaskStatus{ get; set; }
        public DateTime? ExecuteTime { get; set; }
    }
}
