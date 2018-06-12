using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.Model
{
    public class Data
    {
        public string MsgId { get; set; }
        public string TaskId { get; set; }
        public string ErrorCode { get; set; }
        public string ThirdpartyId { get; set; }
    }

    public class PushResultModel
    {
        public string Ret { get; set; }
        public Data Data { get; set; }
    }
}
