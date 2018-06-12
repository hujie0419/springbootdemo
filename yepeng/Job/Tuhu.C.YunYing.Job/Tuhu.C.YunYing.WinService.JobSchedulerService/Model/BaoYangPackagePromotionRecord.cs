using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.Model
{
    public class BaoYangPackagePromotionRecord
    {
        public string BatchCode { get; set; }

        public Guid RulesGUID { get; set; }

        public string CreateUser { get; set; }

        public bool IsSendSms { get; set; }

        public string SettlementMethod { get; set; }
    }

    public class BaoYangPackagePromotionDetail
    {
        public long PKID { get; set; }

        public string BatchCode { get; set; }

        public string MobileNumber { get; set; }

        public string Carno { get; set; }

        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }

    public class UploadDetails
    {
        public string MobileNumber { get; set; }

        public string Carno { get; set; }

        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }

    public enum Status
    {
        NONE,
        WAIT,
        Running,
        SUCCESS,
        FAIL
    }
}
