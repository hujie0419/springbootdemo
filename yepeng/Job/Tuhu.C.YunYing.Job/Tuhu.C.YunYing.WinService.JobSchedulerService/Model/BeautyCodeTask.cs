using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.Model
{
    public class BeautyCodeTask
    {
        public long PKID { get; set; }

        public string MobileNumber { get; set; }

        public Guid UserId { get; set; }

        public int Quantity { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int PackageDetailId { get; set; }

        public string Type { get; set; }

        public string ServiceId { get; set; }

        public string ServiceName { get; set; }

        public Guid VipUserId { get; set; }

        public string BatchCode { get; set; }

        public string Source { get; set; }
    }
}
