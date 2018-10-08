using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class ActivitySchedule
    {
        public int id { get; set; }
        public string title { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public string url { get; set; }
        public bool allDay { get; set; }
    }
}
