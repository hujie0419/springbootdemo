
using System.Collections.Generic;


namespace Tuhu.Service.BaoYang.Config
{
    public class CacheKey{
		//车型适配数据
		public const string VehicleAdaptation = "VehicleAdaptation/{0}/{1}/{2}";

		//Tid适配数据
		public const string TimerAdaptation = "TimerAdaptation/{0}";

		//养护类产品适配
		public const string MaintainAdaptation = "MaintainAdaptation";

		//特殊车型配置
		public const string PriorityVehicleSetting = "PriorityVehicleSetting/{0}";

		//蓄电池黑名单--GetBatteryBlackPids/ProvinceId/CityId/installType
		public const string GetBatteryBlackPids = "GetBatteryBlackPids/{0}/{1}/{2}";

		//当前地区支持的蓄电池-- GetBatteryBrandAndPidByRegion/ProvinceId/CityId
		public const string GetBatteryBrandAndPidByRegion = "GetBatteryBrandAndPidByRegion/{0}/{1}";

		//产品缓存的前缀
		public const string CacheBaoYangProducts = "CacheALLBaoYangProducts/{0}";

		//当前粘度下的机油ID
		public const string CacheOilViscosityPids = "CacheOilViscosityPids/{0}";

		//当前所有的机油粘度
		public const string AllOilViscosity = "AllOilViscosity";

		//原厂产品缓存
		public const string OriginalProduct = "OriginalProduct/{0}/{1}/{2}";

		//按分类缓存产品ID
		public const string ProductCache = "ProductCache/{0}";

		//当前等级和升数下的机油ID
		public const string AllOil = "AllOil/{0}/{1}";

		//同一系列的机油ID
		public const string SameSeriesOil = "SameSeriesOil/{0}";

		//同一系列的机油产品,0汽油或柴油，1升数
		public const string ChangeJiYouProduct = "ChangeJiYouProduct/{0}/{1}";

		//养护类服务可用地区
		public const string MaintainRegionConfig = "MaintainRegionConfig/{0}";

		//当前车型是否需要额外的安装服务
		public const string IsExistAddition = "IsExistAddition/{0}/{1}/{2}/{3}";

		//3M机油车型配置
		public const string ThreeMVehicles = "BaoYangServiceConfig/ThreeMVehicles";

		//途虎推荐的配置项目和优惠券相关信息
		public const string TuhuRecommendPackageTypes = "BaoYangServiceConfig/TuhuRecommendPackageTypes";

		//保养推荐配置
		public const string BaoYangSuggestConfig = "BaoYangServiceConfig/BaoYangSuggestConfig";

		//保养项目图标
		public const string BaoYangTypeIcons = "BaoYangServiceConfig/BaoYangTypeIcons";

		//App保养项目的分类
		public const string AppBaoYangTypeCategories = "BaoYangServiceConfig/AppBaoYangTypeCategories";

		//Web保养项目的分类
		public const string WebBaoYangTypeCategories = "BaoYangServiceConfig/WebBaoYangTypeCategories";

		//用户保养档案配置文件
		public const string UserBaoYangRecordConfig = "BaoYangServiceConfig/UserBaoYangRecordConfig";

		//产品标签配置
		public const string ProductTagConfig = "BaoYangServiceConfig/ProductTagConfig";

		//保养项目对应的安装服务配置
		public const string BaoYangServiceConfig = "ServiceGlobalConfig/BaoYangServiceConfig";

		//燃油类型不能适配的保养项目配置
		public const string VehicleFuelConfig = "ServiceGlobalConfig/VehicleFuelConfig";

		//TuhuBaoYangParts中的PartName与保养Type的对应关系
		public const string PartTypeNameMap = "ServiceGlobalConfig/PartTypeNameMap";

		//tbl_PartAccessory中的AccessoryName与保养Type的对应关系
		public const string AccessoryTypeNameMap = "ServiceGlobalConfig/AccessoryTypeNameMap";

		//适配优先级配置中Name与保养Type的对应关系
		public const string PrioritySettingMap = "ServiceGlobalConfig/PrioritySettingMap";

		//保养Type的分类信息配置
		public const string BaoYangTypesConfig = "ServiceGlobalConfig/BaoYangTypesConfig";

		//保养Package的描述
		public const string PackageDescriptionConfig = "ServiceGlobalConfig/PackageDescriptionConfig";

		//五级属性描述信息
		public const string GetPropertyDescriptionConfig = "ServiceGlobalConfig/GetPropertyDescriptionConfig";

		//新增保养档案中保养项目的显示顺序
		public const string PackageTypeOrderConfig = "ServiceGlobalConfig/PackageTypeOrderConfig";

		//保养适配优先级配置--通用
		public const string PrioritySettingCommon = "PrioritySetting/Common";

		//保养适配优先级配置--防冻液
		public const string PrioritySettingAntifreeze = "PrioritySetting/Antifreeze";

		//售完即止商品
		public const string QuantityLimitProducts = "QuantityLimitProducts";

		//保养仓库
		public const string WareHouseIds = "WareHouseIds";

		//特殊车型对应的机油粘度配置
		public const string VehicleOilSetting = "BaoYangServiceConfig/VehicleOilSetting";

		//保养公告配置
		public const string BaoYangNoticeSetting = "BaoYangNoticeSetting";

		//所有的小保养套餐
		public const string AllXbyPackages = "AllXbyPackages";

		//车型对应的小保养套餐品牌配置
		public const string XbyPackageSetting = "BaoYangServiceConfig/XbyPackageSetting";

		//单个年卡配置
		public const string YearCardConfig = "YearCardConfig/{0}";

		//所有的年卡配置
		public const string AllYearCardsConfig = "AllYearCardsConfig";

		//所有的年卡ids
		public const string AllYearCardIds = "AllYearCardIds";

		//年卡类型配置
		public const string YearCardTypeConfig = "YearCardTypeConfig";

		//年卡推荐配置
		public const string YearCardRecommendConfig = "YearCardRecommendConfig";

		//年卡订单产品结果
		public const string YearCardOrderItems = "YearCardOrderItems/{0}";

		//年卡订单产品结果
		public const string YearCardPromotionData = "YearCardPromotionData/{0}";

		//保养活动缓存
		public const string Activity = "Activity/{0}";

		//地区缓存
		public const string Region = "Region/{0}";

		//用户当天的订单
		public const string TodayUserOrder = "TodayUserOrder/{0}";

		//App首屏常规推荐,0位UserId，1位VehicleId
		public const string AppBaoYangGeneralSuggest = "AppBaoYangGeneralSuggest/{0}/{1}";

		//用户当前车型的机油推荐
		public const string UserSuggestOil = "UserSuggestOil/{0}/{1}/{2}/{3}";

	
		public static readonly Dictionary<string, Dictionary<string, string>> DirectCacheDic = new Dictionary<string, Dictionary<string, string>>(){
				{"适配类缓存", new Dictionary<string, string>(){
						{"MaintainAdaptation", "养护类产品适配"},
					}},
				{"产品缓存", new Dictionary<string, string>(){
						{"CacheALLBaoYangProducts/{0}", "产品缓存的前缀"},
						{"AllOilViscosity", "当前所有的机油粘度"},
					}},
				{"安装服务缓存", new Dictionary<string, string>(){
					}},
				{"配置类缓存", new Dictionary<string, string>(){
						{"BaoYangServiceConfig/ThreeMVehicles", "3M机油车型配置"},
						{"BaoYangServiceConfig/TuhuRecommendPackageTypes", "途虎推荐的配置项目和优惠券相关信息"},
						{"BaoYangServiceConfig/BaoYangSuggestConfig", "保养推荐配置"},
						{"BaoYangServiceConfig/BaoYangTypeIcons", "保养项目图标"},
						{"BaoYangServiceConfig/AppBaoYangTypeCategories", "App保养项目的分类"},
						{"BaoYangServiceConfig/WebBaoYangTypeCategories", "Web保养项目的分类"},
						{"BaoYangServiceConfig/UserBaoYangRecordConfig", "用户保养档案配置文件"},
						{"BaoYangServiceConfig/ProductTagConfig", "产品标签配置"},
						{"ServiceGlobalConfig/BaoYangServiceConfig", "保养项目对应的安装服务配置"},
						{"ServiceGlobalConfig/VehicleFuelConfig", "燃油类型不能适配的保养项目配置"},
						{"ServiceGlobalConfig/PartTypeNameMap", "TuhuBaoYangParts中的PartName与保养Type的对应关系"},
						{"ServiceGlobalConfig/AccessoryTypeNameMap", "tbl_PartAccessory中的AccessoryName与保养Type的对应关系"},
						{"ServiceGlobalConfig/PrioritySettingMap", "适配优先级配置中Name与保养Type的对应关系"},
						{"ServiceGlobalConfig/BaoYangTypesConfig", "保养Type的分类信息配置"},
						{"ServiceGlobalConfig/PackageDescriptionConfig", "保养Package的描述"},
						{"ServiceGlobalConfig/GetPropertyDescriptionConfig", "五级属性描述信息"},
						{"ServiceGlobalConfig/PackageTypeOrderConfig", "新增保养档案中保养项目的显示顺序"},
						{"PrioritySetting/Common", "保养适配优先级配置--通用"},
						{"PrioritySetting/Antifreeze", "保养适配优先级配置--防冻液"},
						{"QuantityLimitProducts", "售完即止商品"},
						{"WareHouseIds", "保养仓库"},
						{"BaoYangServiceConfig/VehicleOilSetting", "特殊车型对应的机油粘度配置"},
						{"BaoYangNoticeSetting", "保养公告配置"},
					}},
				{"小保养套餐缓存", new Dictionary<string, string>(){
						{"AllXbyPackages", "所有的小保养套餐"},
						{"BaoYangServiceConfig/XbyPackageSetting", "车型对应的小保养套餐品牌配置"},
					}},
				{"保养年卡缓存", new Dictionary<string, string>(){
						{"AllYearCardsConfig", "所有的年卡配置"},
						{"AllYearCardIds", "所有的年卡ids"},
						{"YearCardTypeConfig", "年卡类型配置"},
						{"YearCardRecommendConfig", "年卡推荐配置"},
					}},
				{"活动缓存", new Dictionary<string, string>(){
					}},
				{"其它缓存", new Dictionary<string, string>(){
					}},
				};

		public static readonly Dictionary<string, Dictionary<string, string>> NeedParamCacheDic = new Dictionary<string, Dictionary<string, string>>(){
				{"适配类缓存", new Dictionary<string, string>(){
						{"VehicleAdaptation/{0}/{1}/{2}", "车型适配数据"},
						{"TimerAdaptation/{0}", "Tid适配数据"},
						{"PriorityVehicleSetting/{0}", "特殊车型配置"},
						{"GetBatteryBlackPids/{0}/{1}/{2}", "蓄电池黑名单--GetBatteryBlackPids/ProvinceId/CityId/installType"},
						{"GetBatteryBrandAndPidByRegion/{0}/{1}", "当前地区支持的蓄电池-- GetBatteryBrandAndPidByRegion/ProvinceId/CityId"},
					}},
				{"产品缓存", new Dictionary<string, string>(){
						{"CacheOilViscosityPids/{0}", "当前粘度下的机油ID"},
						{"OriginalProduct/{0}/{1}/{2}", "原厂产品缓存"},
						{"ProductCache/{0}", "按分类缓存产品ID"},
						{"AllOil/{0}/{1}", "当前等级和升数下的机油ID"},
						{"SameSeriesOil/{0}", "同一系列的机油ID"},
						{"ChangeJiYouProduct/{0}/{1}", "同一系列的机油产品,0汽油或柴油，1升数"},
					}},
				{"安装服务缓存", new Dictionary<string, string>(){
						{"MaintainRegionConfig/{0}", "养护类服务可用地区"},
						{"IsExistAddition/{0}/{1}/{2}/{3}", "当前车型是否需要额外的安装服务"},
					}},
				{"配置类缓存", new Dictionary<string, string>(){
					}},
				{"小保养套餐缓存", new Dictionary<string, string>(){
					}},
				{"保养年卡缓存", new Dictionary<string, string>(){
						{"YearCardConfig/{0}", "单个年卡配置"},
						{"YearCardOrderItems/{0}", "年卡订单产品结果"},
						{"YearCardPromotionData/{0}", "年卡订单产品结果"},
					}},
				{"活动缓存", new Dictionary<string, string>(){
						{"Activity/{0}", "保养活动缓存"},
					}},
				{"其它缓存", new Dictionary<string, string>(){
						{"Region/{0}", "地区缓存"},
						{"TodayUserOrder/{0}", "用户当天的订单"},
						{"AppBaoYangGeneralSuggest/{0}/{1}", "App首屏常规推荐,0位UserId，1位VehicleId"},
						{"UserSuggestOil/{0}/{1}/{2}/{3}", "用户当前车型的机油推荐"},
					}},
				};
	}
} 