using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaoYangRefreshCacheService.Model
{
    public class PartsNotifyReportConfig
    {
        public long PKID { get; set; }
        public string ReportType { get; set; }
        public string ReportName { get; set; }
        public string ReportUrl { get; set; }
        public string AuthSecret { get; set; }
        public string NotifyUsers { get; set; }
        public Frequency Frequency { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
    }

    public enum Frequency : byte
    {
        Day = 0,
        Week = 1,
        Month = 2
    }
}
