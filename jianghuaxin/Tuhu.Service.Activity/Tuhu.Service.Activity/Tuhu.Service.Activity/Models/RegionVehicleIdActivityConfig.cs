using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models
{
    public class RegionVehicleIdActivityConfig
    {
        public Guid ActivityId { get; set; }

        public string ActivityType { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public List<RegionVehicleIdActivityUrlConfig> UrlConfigs { get; set; }
    }

    public class RegionVehicleIdActivityUrlConfig
    {
        public string TargetUrl { get; set; }

        public bool IsDefault { get; set; }

        public string VehicleId { get; set; }

        public int? RegionId { get; set; }

        /// <summary>
        /// 小程序活动页链接
        /// </summary>
        public string WxappUrl { get; set; }
    }

}
