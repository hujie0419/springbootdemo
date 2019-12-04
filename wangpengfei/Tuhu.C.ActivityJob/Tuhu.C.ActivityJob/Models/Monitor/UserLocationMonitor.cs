using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.ActivityJob.Models.Monitor
{
    public class UserLocationMonitor
    {
        public int Pkid { get; set; }

        public string Mobile { get; set; }

        public string MobileLocation { get; set; }

        public string Gps { get; set; }

        public string GpsLocation { get; set; }

        public string Ip { get; set; }

        public string IpLocation { get; set; }
    }
}
