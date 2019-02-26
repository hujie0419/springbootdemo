using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.BeautyService
{
    public class BeautyServicePackageLimitConfig
    {
        public int PKID { get; set; }
        /// <summary>
        /// 服务码配置ID
        /// </summary>
        public int PackageDetailId { get; set; }
        /// <summary>
        /// 周期类型 如month，week,day
        /// </summary>
        public string CycleType { get; set; }
        /// <summary>
        /// 周期限制次数
        /// </summary>
        public int CycleLimit { get; set; }
        /// <summary>
        /// 限购省IDs
        /// </summary>
        public string ProvinceIds { get; set; }
        /// <summary>
        /// 限购市IDs
        /// </summary>
        public string CityIds { get; set; }

        public DateTime? UpdateTime { get; set; }
    }
}
