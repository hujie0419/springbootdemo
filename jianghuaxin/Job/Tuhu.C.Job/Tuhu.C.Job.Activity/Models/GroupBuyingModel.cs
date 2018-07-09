using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.Activity.Models
{
    public class ExpiringGroupInfo
    {
        public Guid GroupId { get; set; }
        public DateTime EndTime { get; set; }
        public int GroupStatus { get; set; }
        public int CurrentMemberCount { get; set; }
    }

    public class ExpiringUserInfo
    {
        public Guid UserId { get; set;}
        public string PID { get; set; }
        public int OrderId { get; set; }
        public string ProductName { get; set; }
        public int CurrentMemberCount { get; set;}
        public int RequiredMemberCount { get; set; }
    }
    public class UserOrderJobModel
    {
        public Guid UserId { get; set; }
        public int OrderId { get; set; }
    }

}
