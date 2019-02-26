using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class VendorProductPriorityConfigModel
    {
        /// <summary>
        /// 自增主键
        /// </summary>
        public long PKID { get; set; }

        /// <summary>
        /// 产品类型 蓄电池Battery/玻璃Glass
        /// </summary>
        public string ProductType { get; set; }

        /// <summary>
        /// 配置类型 车型Vehicle/地区Region
        /// </summary>
        public string ConfigType { get; set; }

        /// <summary>
        /// 车型Vid
        /// </summary>
        public string VehicleId { get; set; }

        /// <summary>
        /// 地区Id
        /// </summary>
        public int RegionId { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        public string Priority { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDateTime { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime? LastUpdateDateTime { get; set; }
    }
}
