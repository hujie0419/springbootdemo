using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.PromotionTask.Model
{
    public class CouponRuleList
    {
        public List<int> RuleIds = new List<int>();
        public List<int> GetRuleIds = new List<int>();
        public int Days { get; set; }

        public System.Collections.Concurrent.ConcurrentDictionary<Guid, bool> PushedUserIds =
            new System.Collections.Concurrent.ConcurrentDictionary<Guid, bool>(20, 10000);
    }

    public class CouponRule
    {
        public int RuleId { get; set; }
        public int GetRuleId { get; set; }
        public int Day { get; set; }
    }
}
