using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class SelectPromotionCodeByUserCellPhonePager
    {
        public int PKID { get; set; }
        public string Code { get; set; }
        public string PromtionName { get; set; }
        public int? MinMoney { get; set; }
        public int? Discount { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public string StatusName { get; set; }
        public int Status { get; set; }
    }
}
