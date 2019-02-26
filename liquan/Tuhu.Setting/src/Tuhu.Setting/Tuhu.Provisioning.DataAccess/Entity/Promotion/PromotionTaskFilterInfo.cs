using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class PromotionTaskFilterInfo
    {
        public DateTime FilterStartTime { get; set; }
        public DateTime FilterEndTime { get; set; }
        public string Brand { get; set; }
        public string Category { get; set; }
        public string Pid { get; set; }
        public double SpendMoney { get; set; }
        public int PurchaseNum { get; set; }
        public string Area { get; set; }
        public string Channel { get; set; }
        public int InstallType { get; set; }
        public string OrderStatus { get; set; }
        public string Vehicle { get; set; }
    }
}
