using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.Model
{
    public class SingleBaoYangPackage
    {
        public Guid VipUserId { get; set; }

        public string PackageName { get; set; }

        public decimal Price { get; set; }

        public string PID { get; set; }

        public string SettlementMethod { get; set; }

        public string RedemptionCode { get; set; }
    }
}
