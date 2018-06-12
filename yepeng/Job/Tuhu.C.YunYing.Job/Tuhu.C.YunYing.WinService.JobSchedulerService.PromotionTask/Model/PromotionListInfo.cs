using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.PromotionTask.Model
{
    public class PromotionListInfo
    {
        public int TaskPromotionListId { get; set; }
        public int PromotionTaskId { get; set; }
        public int CouponRulesId { get; set; }
        public string PromotionName { get; set; }
        public int Type { get; set; }
        public string PromotionDescription { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal MinMoney { get; set; }
        public decimal DiscountMoney { get; set; }
        public DateTime CreateTime { get; set; }
        public string FinanceMarkName { get; set; }
        public string Issuer { get; set; }
        public string IssueChannle { get; set; }
        public string IssueChannleId { get; set; }
        public int DepartmentId { get; set; }
        public int IntentionId { get; set; }
        public string Creater { get; set; }
        public string DepartmentName { get; set; }
        public string IntentionName { get; set; }
        public string BusinessLineName { get; set; }
        public int Number { get; set; }
        public int IsPush { get; set; }
        public string PushSetting { get; set; }
        public int IsRemind { get; set; }
    }

    public class CouponRules {
        public int PKID { get; set; }
        public int Type { get; set; }
        public string Name { get; set; }
    }
}
