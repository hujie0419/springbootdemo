using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class RegionVehicleIdActivityUrlConfig
    {
        public int PKID { set; get; }

        public string UrlTitle { set; get; }
        public Guid ActivityId { set; get; }

        /// <summary>
        /// H5活动页地址
        /// </summary>
        public string TargetUrl { set; get; }

        public int IsDefault { set; get; }

        public string VehicleId { set; get; }

        public int RegionId { set; get; }

        /// <summary>
        /// 小程序活动页链接
        /// </summary>
        public string WxappUrl { get; set; }

        /// <summary>
        /// 小程序活动页标题
        /// </summary>
        public string WxappUrlTitle { get; set; }
    }
}
