using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tuhu.Provisioning.Models
{
    public class PromotionCellphoneSendModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int RuleID { get; set; }
        public string Description { get; set; }
        public decimal? Discount { get; set; }
        public decimal? MinMoney { get; set; }
        public bool AllUser { get; set; }
    }
}