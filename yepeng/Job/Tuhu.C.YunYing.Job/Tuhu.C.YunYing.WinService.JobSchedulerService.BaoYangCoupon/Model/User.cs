using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.Model
{
    public class User
    {
        public Guid UserId { get; set; }

        public string Code { get; set; }
        public string UserName { get; set; }
        public string Cellphone { get; set; }
        public string DeviceId { get; set; }
        public string Channel { get; set; }

    }
}
