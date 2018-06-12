using System;
using System.Collections;
using System.Collections.Generic;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class BaoYangActivitySetting
    {
        public int Id { get; set; }
        public string ActivityNum { get; set; }
        public string ActivityName { get; set; }
        public bool ActivityStatus { get; set; }
        public bool CheckStatus { get; set; }
        public bool LayerStatus { get; set; }
        public string LayerImage { get; set; }
        public string LayerImage2 { get; set; }
        public string ActivityImage { get; set; }
        public string CouponId { get; set; }
        public string GetRuleGUID { get; set; }
        public string ButtonChar { get; set; }
        public string RelevanceServicesTypes { get; set; }
        public string RelevanceServicesCatalogs { get; set; }
        public string RelevanceBrands { get; set; }
        public string RelevanceProducts { get; set; }
        public string RelevanceSeries { get; set; }
        public int StoreAuthentication { get; set; }
        public string StoreAuthenticationName { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
    }


    public class BaoYangActivityRelevance
    {
        public string Brand { get; set; }

        public string Series { get; set; }

        public string ShopCertification { get; set; }
    }

    public class BaoYangPackage
    {
        public string Type { get; set; }
        public string Items { get; set; }
        public string Catalog { get; set; }
        public string Name { get; set; }
    }

    public class BaoYangType
    {
        public BaoYangPackage Package { get; set; }
        public string Type { get; set; }
        public string CatalogName { get; set; }
        public string Name { get; set; }
        public object BrandList { get; set; }
        public object SerieList { get; set; }
        public string Brand { get; set; }
        public string Serie { get; set; }
    }

    public class BaoYangActivitySettingItem
    {
        public int Id { get; set; }

        public int BaoYangActivityId { get; set; }

        public string ServicePackagesType { get; set; }

        public string ServicePackagesItems { get; set; }

        public string ServicePackagesName { get; set; }

        public string ServiceType { get; set; }

        public string ServiceCatalog { get; set; }

        public string ServiceCatalogName { get; set; }

        public string Brands { get; set; }

        public string Series { get; set; }

        public string Products { get; set; }

        public string InAdapteTipPrefix { get; set; }

        public DateTime? CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }
    }

    public class OilLevelModel
    {
        /// <summary>
        /// 机油等级
        /// </summary>
        public string OilLevel { get; set; }
        /// <summary>
        /// 机油类型
        /// </summary>
        public string OilType { get; set; }
    }

    public class BaoYangVehicleSearchModel
    {
        /// <summary>
        /// 车系
        /// </summary>
        public IEnumerable<string> BrandCategories { get; set; }
       /// <summary>
       /// 二级车型价格AvgPrice
       /// </summary>
        public decimal MinPrice { get; set; }
        /// <summary>
        /// 二级车型价格AvgPrice
        /// </summary>
        public decimal MaxPrice { get; set; }
        /// <summary>
        /// 车型品牌
        /// </summary>
        public Dictionary<string, Dictionary<string, List<VehicleModel>>> Brands { get; set; }
        /// <summary>
        /// 机油粘度
        /// </summary>
        public IEnumerable<string> Viscosities { get; set; }
        /// <summary>
        /// 机油等级
        /// </summary>
        public IEnumerable<string> OilLevels { get; set; }

    }

    public class BaoYangActivityVehicleSearchModel
    {
        public List<string> Brands { get; set; }
        public List<string> BrandCategories { get; set; }
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
        public string OilViscosity { get; set; }
        public List<string> OilLevels { get; set; }
    }

    public class BaoYangActivityVehicleViewModel
    {
        /// <summary>
        /// 保养活动分车型分地区配置PKID
        /// </summary>
        public int PKID { get; set; }
        /// <summary>
        /// 二级车型Id
        /// </summary>
        public string VehicleId { get; set; }
        /// <summary>
        /// 车型品牌
        /// </summary>
        public string Brand { get; set; }
        /// <summary>
        /// 车系
        /// </summary>
        public string VehicleSeries { get; set; }
        /// <summary>
        /// 二级车型均价
        /// </summary>
        public double AvgPrice { get; set; }
        /// <summary>
        /// 机油粘度
        /// </summary>
        public string OilViscosity { get; set; }
        /// <summary>
        /// 机油等级
        /// </summary>
        public string OilLevel { get; set; }
        /// <summary>
        /// 保养活动Id
        /// </summary>
        public string ActivityId { get; set; }
        /// <summary>
        /// 保养活动名称
        /// </summary>
        public string ActivityName { get; set; }
    }

    public class BaoYangActivityVehicleAndRegionModel
    {
        /// <summary>
        /// 保养活动分车型分地区PKID
        /// </summary>
        public int PKID { get; set; }
        /// <summary>
        /// 二级车型Id
        /// </summary>
        public string VehicleId { get; set; }
        /// <summary>
        /// 地区Id(市一级)
        /// </summary>
        public int RegionId { get; set; }
        /// <summary>
        /// 类型： 车型/地区
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 保养活动配置活动Id
        /// </summary>
        public string ActivityId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDateTime { get; set; }
        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime? LastUpdateDateTime { get; set; }
    }

    public class BaoYangActivityRegionViewModel
    {
        /// <summary>
        /// 分地区PKID
        /// </summary>
        public int PKID { get; set; }
        /// <summary>
        /// 省份名称
        /// </summary>
        public string ProvinceName { get; set; }
        /// <summary>
        /// 城市名称
        /// </summary>
        public string CityName { get; set; }
        /// <summary>
        /// 地区Id(市一级)
        /// </summary>
        public int RegionId { get; set; }
        /// <summary>
        /// 保养活动Id
        /// </summary>
        public string ActivityId { get; set; }
        /// <summary>
        /// 活动名称
        /// </summary>
        public string ActivityName { get; set; }
    }
}
