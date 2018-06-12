using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class SearchPromotionByCondition
    {
        public int PromotionTaskId { get; set; }
        public string TaskName { get; set; }

        public DateTime? TaskStartTime { get; set; }
        public DateTime? TaskEndTime { get; set; }
        public int TaskType { get; set; }
        public int SelectUserType { get; set; }
        public int PromotionTaskActivityId { get; set; }
        public int SmsId { get; set; }
        public string SmsParam { get; set; }
        public string CreateTime { get; set; }
        public int TaskStatus { get; set; }
        public string CouponRulesIds { get; set; }
        public string Creater { get; set; }

        public string Auditor { get; set; }

        public DateTime? AuditTime { get; set; }

        public DateTime? ExecuteTime { get; set; }
        public DateTime? CloseTime { get; set; }
        public int? IsImmediately { get; set; }
    }
}
