using System;

namespace Tuhu.C.Job.UserGrouping.Models
{
    public class UserGroupingModel
    {
        public int Pkid { get; set; }
        public Guid DeviceId { get; set; }

        public Guid UserId { get; set; }

        public string Tag { get; set; }
        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }
    }
}
