using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.BaoYangProductPriority
{
    /// <summary>
    /// 产品推荐模型
    /// </summary>
    public class BaoYangProductPriorityModel
    {

        public BaoYangProductPriorityModel()
        {
            this.Brand = string.Empty;
            this.Series = string.Empty;
        }
        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// 系列
        /// </summary>
        public string Series { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        public int Priority { get; set; }
    }

    /// <summary>
    /// 石油推荐数据库查询模型
    /// </summary>
    public class OilBaoYangProductPriority
    {
        public OilBaoYangProductPriority()
        {
            this.Brand = string.Empty;
            this.Series = string.Empty;
            this.Viscosity = string.Empty;
            this.Grade = string.Empty;
        }
        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// 系列
        /// </summary>
        public string Series { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// 粘度
        /// </summary>
        public string Viscosity { get; set; }

        /// <summary>
        /// 等级
        /// </summary>
        public string Grade { get; set; }
    }
    /// <summary>
    /// 石油产品推荐模型
    /// </summary>
    public class OilBaoYangProductPriorityModel
    {
        /// <summary>
        /// 等级
        /// </summary>
        public string Grade { get; set; }
        /// <summary>
        /// 粘度详细
        /// </summary>
        public List<OilViscosityModel> Viscosities { get; set; }
    }
    /// <summary>
    /// 石油粘度推荐模型
    /// </summary>
    public class OilViscosityModel
    {
        /// <summary>
        /// 粘度
        /// </summary>
        public string Viscosity { get; set; }

        /// <summary>
        /// 产品推荐模型
        /// </summary>
        public List<BaoYangProductPriorityModel> Detaileds { get; set; }
    }

    /// <summary>
    /// 特殊推荐视图模型
    /// </summary>
    public class BaoYangProductPriorityAreaView
    {
        public BaoYangProductPriorityAreaView()
        {
            this.Details = new List<BaoYangProductPriorityAreaDetail>();
        }
        public string partName { get; set; }

        public int AreaId { get; set; }

        public bool IsEnabled { get; set; }

        public List<BaoYangProductPriorityAreaDetail> Details { get; set; }
    }

    public class BaoYangProductPriorityAreaDetail
    {
        public BaoYangProductPriorityAreaDetail()
        {
            this.Citys = new List<City>();
        }
        public int RegionId { get; set; }

        public string RegionName { get; set; }
        public List<City> Citys { get; set; }

    }

    public class City
    {
        public int CityId { get; set; }

        public string CityName { get; set; }
    }

    public class BaseVehicleProductPriorityView
    {
        public string VehicleId { get; set; }
        public string Brand { get; set; }

        public string Vehicle { get; set; }

        public decimal AvgPrice { get; set; }

        public string VehicleBodyType { get; set; }
    }

    /// <summary>
    ///车型推荐视图
    /// </summary>
    public class VehicleProductPriorityView : BaseVehicleProductPriorityView
    {
        public VehicleProductPriorityView()
        {
            Details = new List<ProductPriorityAreaDetail>();
        }

        public bool IsEnabled { get; set; }
        public List<ProductPriorityAreaDetail> Details { get; set; }
    }


    /// <summary>
    /// 车型机油推荐视图
    /// </summary>
    public class VehicleOilProductPriorityView : BaseVehicleProductPriorityView
    {
        public VehicleOilProductPriorityView()
        {
            Details = new List<ProductPriorityAreaOilDetailView>();
        }
        public string NewViscosity { get; set; }

        public int AreaOilId { get; set; }

        public string Viscosity { get; set; }

        public string Grade { get; set; } 

        public bool IsEnabled { get; set; }

        public List<ProductPriorityAreaOilDetailView> Details { get; set; }
    } 

    public class ProductPriorityAreaOilDetailView
    {
        public string Grade { get; set; }
        public int Seq { get; set; }
        public string Brand { get; set; }
        public string Series { get; set; }
    }

    public class ProductPriorityOilDetail
    {

        public int AreaId { get; set; }
        public string VehicleId { get; set; }
        public string Viscosity { get; set; }
        public string Grade { get; set; }
        public string ProductPriorityGrade { get; set; }
        public string NewViscosity { get; set; }
        public int AreaOilId { get; set; }
        public string Brand { get; set; }
        public string Series { get; set; }
        public int Seq { get; set; }
        public bool IsEnabled { get; set; }
    }

    /// <summary>
    /// 批量导入文档实体类型
    /// </summary>
    public class BatchimportExcelModel
    {
        public string VehicleId { get; set; }
        public string PartName { get; set; }
        public int Seq { get; set; }
        public string Brand { get; set; }
        public string Series { get; set; }
        public string Viscosity { get; set; }
        /// <summary>
        /// 新粘度
        /// </summary>
        public string NewViscosity { get; set; }
        /// <summary>
        /// 推荐等级
        /// </summary>
        public string RecommendedGrade { get; set; }
        public string Grade { get; set; }
        public int AreaId { get; set; }
    }
    public enum VehicleProductPriorityRequestStatus
    {
        All = 0,
        Enable = 1,
        Disable = 2,
        No = 3
    }

    public class OfflineRecommendTaskModel
    {
        /// <summary>
        /// 推荐类型--Regular,Special
        /// </summary>
        public string RecommendType { get; set; }

        /// <summary>
        /// 零件名称
        /// </summary>
        public string PartName { get; set; }

        /// <summary>
        /// 地区Id
        /// </summary>
        public int? AreaId { get; set; }

        /// <summary>
        /// 车型Vid
        /// </summary>
        public string VehicleId { get; set; }
    }
}
