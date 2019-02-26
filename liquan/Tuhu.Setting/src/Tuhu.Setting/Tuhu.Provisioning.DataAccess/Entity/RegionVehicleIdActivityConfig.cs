using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class RegionVehicleIdActivityConfig
    {
        public int PKID { set; get; }

        public Guid ActivityId { set; get; }

        public string ActivityName { set; get; }

        public string ActivityType { set; get; }

        public DateTime? StartTime { set; get; }

        public DateTime? EndTime { set; get; }

        public int IsEnabled { set; get; }

        public string CreateUser { set; get; }

        public string UpdateUser { set; get; }

        public string Status { set; get; }
    }


    public class ActivityTitleId
    {
        public string ID { set; get; }

        public string Title { set; get; }

        public int ActivityType { set; get; }
    }
}
