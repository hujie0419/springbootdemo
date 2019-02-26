using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.BeautyService
{
    public class PingAnRegionPackageMap
    {
        /// <summary>
        /// PKID
        /// </summary>
        public int PKID { get; set; }
        /// <summary>
        /// 省ID
        /// </summary>
        public int ProvinceId { get; set; }
        /// <summary>
        /// 省名称
        /// </summary>
        public string ProvinceName { get; set; }
        /// <summary>
        /// 市
        /// </summary>
        public int CityId { get; set; }
        /// <summary>
        /// 市名称
        /// </summary>
        public string CityName { get; set; }
        /// <summary>
        /// 地区ID
        /// </summary>
        public int RegionId { get; set; }
        /// <summary>
        /// 地区名称
        /// </summary>
        public string RegionName { get; set; }
        /// <summary>
        /// 美容包ID
        /// </summary>
        public Guid? PackageId { get; set; }
        /// <summary>
        /// 美容包名称
        /// </summary>
        public string PackageName { get; set; }
        /// <summary>
        /// 保养套餐ID
        /// </summary>
        public string BYPackagePID { get; set; }
        /// <summary>
        /// 保养套餐名称
        /// </summary>
        public string BYPakageName { get; set; }
        /// <summary>
        /// 机构ID
        /// </summary>
        public string BusinessId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedDateTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedDateTime { get; set; }
        /// <summary>
        /// total
        /// </summary>
        public int Total { get; set; }

    }
}
