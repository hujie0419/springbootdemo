using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.Model
{
    public class CouponLogModel
    {
        public Guid UserId { get; set; }

        public string UserName { get; set; }

        public string Channel { get; set; }

        public string Subject { get; set; }

        public bool Status { get; set; }

        public string PhoneNum { get; set; }
    }
}
