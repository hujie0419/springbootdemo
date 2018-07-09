using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Models
{
    public class PromotionTaskListModel
    {
        public List<SearchPromotionByCondition> PromotionTaskList { get; set; }
        public int SumNum { get; set; }

        public string PagerStr { get; set; }

        public PromotionSearchInfoModel SearchInfo { get; set; }
    }
}