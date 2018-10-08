using System;
namespace Tuhu.C.Job.Activity.Models
{
    public class ActivityMessageRemindModel
    {
        public int Pkid { get; set; }
        public Guid UserId { get; set; }

        public string ActivityId { get; set; }

        public string Pid { get; set; }

        public string ProductName { get; set; }

        public bool IsMultiProducts { get; set; }

        public string ActivityName { get; set; }

        public DateTime StartDtartTime { get; set; }


    }
}
