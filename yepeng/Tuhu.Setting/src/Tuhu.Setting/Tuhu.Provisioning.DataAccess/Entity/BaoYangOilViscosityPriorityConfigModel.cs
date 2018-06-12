using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    /// <summary>
    /// 机油粘度优先级配置
    /// </summary>
    public class BaoYangOilViscosityPriorityConfigModel
    {
        /// <summary>
        /// 自增主键Id
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 原厂机油粘度
        /// </summary>
        public string OriginViscosity { get; set; }

        /// <summary>
        /// 机油粘度优先级
        /// </summary>
        public string ViscosityPriority { get; set; }

        public List<string> ViscosityPriorities
        {
            get
            {
                return string.IsNullOrWhiteSpace(ViscosityPriority) ? new List<string>() :
                        ViscosityPriority.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToList();
            }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDateTime { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime? LastUpdateDateTime { get; set; }

        /// <summary>
        /// 是否已被逻辑删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 类型 User--用户类 Package--套餐类
        /// </summary>
        public string ConfigType { get; set; }
    }

    public enum BaoYangOilViscosityPriorityConfigType
    {
        /// <summary>
        /// 用户类
        /// </summary>
        User,
        /// <summary>
        /// 套餐类
        /// </summary>
        Package
    }

    public class OilViscosityRegionModel
    {
        /// <summary>
        /// 自增主键Id
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 地区Id
        /// </summary>
        public int RegionId { get; set; }

        /// <summary>
        /// RegionId对应的省份名称
        /// </summary>
        public string ProvinceName { get; set; }

        /// <summary>
        /// RegionId对应的城市名称
        /// </summary>
        public string CityName { get; set; }

        /// <summary>
        /// 机油粘度优先级
        /// </summary>
        public string OilViscosity { get; set; }

        public List<string> OilViscosities
        {
            get
            {
                return string.IsNullOrWhiteSpace(OilViscosity) ? new List<string>() :
                        OilViscosity.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToList();
            }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDateTime { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime? LastUpdateDateTime { get; set; }

        /// <summary>
        /// 是否已被逻辑删除
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}
