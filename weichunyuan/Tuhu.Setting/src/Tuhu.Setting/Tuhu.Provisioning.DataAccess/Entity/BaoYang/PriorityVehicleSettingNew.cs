using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.BaoYang
{
    public class PriorityVehicleSettingNew
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 车型ID
        /// </summary>
        public string VehicleId { get; set; }

        /// <summary>
        /// 配件名称
        /// </summary>
        public string PartName { get; set; }

        /// <summary>
        /// 优先级类型(比如:机油-全合成)
        /// </summary>
        public string PriorityType { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// 系列
        /// </summary>
        public string Series { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        public string PID { get; set; }

        /// <summary>
        /// 优先级别(顺序)
        /// </summary>
        public int Priority { get; set; }

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
        public DateTime LastUpdateDateTime { get; set; }

    }

    public class FullPriorityVehicleSettingNew
    {
        public List<PriorityVehicleSettingNew> Settings { get; set; }

        public string VehicleId { get; set; }

        public string Brand { get; set; }

        public string Vehicle { get; set; }

        public decimal Price { get; set; }

        public List<string> Viscosities { get; set; }

        public List<string> Grades { get; set; }
    }

    public class PriorityVehicleOilModel
    {
        public string VehicleId { get; set; }

        public string Viscosity { get; set; }

        public string Grade { get; set; }
    }

    public class PriorityVehicleSettingNewViewModel
    {
        public string PartName { get; set; }

        public string VehicleId { get; set; }

        public bool IsEnabled { get; set; }

        public List<PriorityInternalModel> Priorities { get; set; }

        public class PriorityInternalModel
        {
            public string PriorityType { get; set; }

            public string Brand { get; set; }

            public string Series { get; set; }

            public string PID { get; set; }

            public int Priority { get; set; }
        }

        private PriorityVehicleSettingNewViewModel() { }

        public static IEnumerable<PriorityVehicleSettingNewViewModel> GetList(IEnumerable<PriorityVehicleSettingNew> settings)
        {
            List<PriorityVehicleSettingNewViewModel> result = null;
            if (settings != null && settings.Any())
            {
                result = settings.GroupBy(setting => new
                {
                    setting.VehicleId,
                    setting.PartName
                }).Select(g => new PriorityVehicleSettingNewViewModel
                {
                    VehicleId = g.Key.VehicleId,
                    PartName = g.Key.PartName,
                    IsEnabled = g.All(setting => setting.IsEnabled),
                    Priorities = g.Select(setting => new PriorityInternalModel
                    {
                        PriorityType = setting.PriorityType,
                        Brand = setting.Brand,
                        Series = setting.Series,
                        PID = setting.PID,
                        Priority = setting.Priority,
                    }).ToList(),
                }).ToList();
            }
            return result ?? new List<PriorityVehicleSettingNewViewModel>();
        }

    }

    public class ProductPrioritySettingNew
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 配件名称
        /// </summary>
        public string PartName { get; set; }

        /// <summary>
        /// 优先级类型
        /// </summary>
        public string PriorityType { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// 系列
        /// </summary>
        public string Series { get; set; }

        /// <summary>
        /// 产品PID
        /// </summary>
        public string PID { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime LastUpdateDateTime { get; set; }
    }

}
