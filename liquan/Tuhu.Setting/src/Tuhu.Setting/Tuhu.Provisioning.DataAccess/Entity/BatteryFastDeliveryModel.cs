using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.DAO;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class BatteryFastDeliveryModel
    {
        public int PKID { get; set; }
        public string ServiceTypePid { get; set; }
        public int RegionId { get; set; }
        public string ProvinceName { get; set; }
        public string CityName { get; set; }
        public string DistrictName { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Remark { get; set; }
        public DateTime? CreateDateTime { get; set; }
        public DateTime? LastUpdateDateTime { get; set; }
        public bool IsEnabled { get; set; }
        public Dictionary<string, List<string>> Products { get; set; }
    }

    public class BatteryFastDeliveryProductsModel
    {
        public int FastDeliveryId { get; set; }
        public string Brand { get; set; }
        public string ProductPid { get; set; }
    }

    public class BatteryFastDeliveryForViewModel : SKUPBatteryPID
    {
        public bool IsChecked { get; set; }
    }

    public class BatteryServiceTypeModel
    {
        public string PID { get; set; }
        public string DisplayName { get; set; }
    }

    /// <summary>
    /// 服务类蓄电池产品排序
    /// </summary>
    public class BatteryFastDeliveryPriority
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 省(Id)
        /// </summary>
        public int ProvinceId { get; set; }

        /// <summary>
        /// 省(名称)
        /// </summary>
        public string ProvinceName { get; set; }

        /// <summary>
        /// 市(Id)
        /// </summary>
        public int CityId { get; set; }

        /// <summary>
        /// 市(名称)
        /// </summary>
        public string CityName { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        public List<string> Priorities { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDateTime { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? LastUpdateDateTime { get; set; }
    }

}
