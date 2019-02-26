using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.Models
{
    public class MessagePushConfig
    {
        public string MsgTitle { get; set; }

        public string MsgContent { get; set; }

        public string MsgLink { get; set; }

        public int? TotalDuration { get; set; }

        public int? AheadHour { get; set; }

        public string EnID { get; set; }
    }
}
