using System;

namespace Tuhu.Service.Activity.Models.Requests
{
    public class ActivityProductUserRemindRequest
    {
        public Guid UserId { get; set; }

        public string ActivityId { get; set; }

        public string Pid { get; set; }

        public string ActivityName { get; set; }

        public string PorductName { get; set; }

        public DateTime StartDateTime { get; set; }
    }
}
