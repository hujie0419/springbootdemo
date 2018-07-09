using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.PromotionTask.Model
{
    public class PromotionTaskCls
    {
        public string PromotionTaskId { get; set; }
        public string TaskName { get; set; }
        public DateTime TaskStartTime { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public int Tasktype { get; set; }
        public int SelectUserType { get; set; }
    }
    public class CouponRulesModel
    {
        //,,,,,,
        public string Category { get; set; }
        public string ProductId { get; set; }
        public string Brand { get; set; }
        public int ParentId { get; set; }
        public DateTime? CreateDateTime { get; set; }
        public int? ShopId { get; set; }
        public int? ShopType { get; set; }

        public bool PIDType { get; set; }
    }
    public class CouponRulesConfig
    {
        public int PKID { get; set; }
        public int RuleID { get; set; }
        public int Type { get; set; }
        public string ConfigValue { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime LastUpdateDateTime { get; set; }

    }
}
