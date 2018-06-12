using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.MMS.Web.Domain.UserFilter;

namespace Tuhu.MMS.Web.Service
{
    public partial class UserFilterService
    {
        public static Dictionary<string, string> FirstCategoryNameMap => new Dictionary<string, string>()
        {
            { "ShowStaticProperty", "静态属性统计" } ,
            { "ShowVehicle", "车辆统计" },
            { "ShowViews", "浏览统计" },
            { "ShowOrders", "订单统计" }
        };

        public static Dictionary<string, string> SecondCategoryNameMap => new Dictionary<string, string>()
        {
            { "RegisterTime", "注册时间" } ,
            { "FirstLoginTime", "首登APP时间" },
            { "Sex", "性别" },
            { "Area", "所在地区" },
            { "DefaultVehicle", "默认车型" },
            { "DefaultTire", "默认车型轮胎规格" },
            { "DefaultTwoLevelPrice", "默认二级车型均价" },
            { "DefaultFiveLevelPrice", "默认五级车型均价" },
            { "DefaultVehicleKM", "默认车辆公里数" },
            { "DefaultVehicleRoadTime", "默认车辆上路时间" },
            { "ListPagePV", "列表页PV" },
            { "ProductDetailByCategoryPV", "商品详情页(按品类)PV" },
            { "ProductDetailByPIDPV", "商品详情页(按PID)PV" },
            { "PageViewByTireSizePV", "页面浏览统计(按轮胎规格)PV" },
            { "OrderByOrderChannel", "订单统计(按订单渠道)" },
            { "OrderByCategoryYewu", " 订单统计(按品类/业务线)" },
            { "OrderByPID", "订单统计(按PID)" },
            { "OrderByOrderType", "订单统计(活动类型)" },
        };

        public static IEnumerable<UserFilterColConfig> UserFilterColConfigs => new List<UserFilterColConfig>()
        {
            new UserFilterColConfig()
            {
                BaseCategory = "ShowStaticProperty",
                BaseCategoryName = FirstCategoryNameMap["ShowStaticProperty"],
                SecnodCategory = "RegisterTime",
                SecondCategoryName = SecondCategoryNameMap["RegisterTime"],
                TableName = "dm_userdetail",
                TableColConfig = new List<TableColConfig>()
                {
                    new TableColConfig()
                    {
                        ColName = "registrationtime",
                        TableColName = "注册时间"
                    }
                }
            },
            new UserFilterColConfig()
            {
                BaseCategory = "ShowStaticProperty",
                BaseCategoryName = FirstCategoryNameMap["ShowStaticProperty"],
                SecnodCategory = "FirstLoginTime",
                SecondCategoryName = SecondCategoryNameMap["FirstLoginTime"],
                TableName = "dm_userdetail",
                TableColConfig = new List<TableColConfig>()
                {
                    new TableColConfig()
                    {
                        ColName = "appbindingtime",
                        TableColName = "首登APP时间"
                    }
                }
            },
            new UserFilterColConfig()
            {
                BaseCategory = "ShowStaticProperty",
                BaseCategoryName = FirstCategoryNameMap["ShowStaticProperty"],
                SecnodCategory = "Sex",
                SecondCategoryName = SecondCategoryNameMap["Sex"],
                TableName = "dm_userdetail",
                TableColConfig = new List<TableColConfig>()
                {
                    new TableColConfig()
                    {
                        ColName = "gender",
                        TableColName = "性别"
                    }
                }
            },
            new UserFilterColConfig()
            {
                BaseCategory = "ShowStaticProperty",
                BaseCategoryName = FirstCategoryNameMap["ShowStaticProperty"],
                SecnodCategory = "Area",
                SecondCategoryName = SecondCategoryNameMap["Area"],
                TableName = "dm_userdetail",
                TableColConfig = new List<TableColConfig>()
                {
                    new TableColConfig()
                    {
                        ColName = "province",
                        TableColName = "所在省份"
                    },
                    new TableColConfig()
                    {
                        ColName = "city",
                        TableColName = "所在城市"
                    },
                }
            },
            new UserFilterColConfig()
            {
                BaseCategory = "ShowVehicle",
                BaseCategoryName = FirstCategoryNameMap["ShowVehicle"],
                SecnodCategory = "DefaultVehicle",
                SecondCategoryName = SecondCategoryNameMap["DefaultVehicle"],
                TableName = "dm_userdetail",
                TableColConfig = new List<TableColConfig>()
                {
                    new TableColConfig()
                    {
                        ColName = "vehiclebrand",
                        TableColName = "默认车型品牌"
                    },
                    new TableColConfig()
                    {
                        ColName = "vehiclename",
                        TableColName = "默认车型车系"
                    },
                    new TableColConfig()
                    {
                        ColName = "vehiclepailiang",
                        TableColName = "默认车型排量"
                    },
                    new TableColConfig()
                    {
                        ColName = "vehiclenian",
                        TableColName = "默认车型生产年份"
                    },
                    new TableColConfig()
                    {
                        ColName = "vehiclesalesname",
                        TableColName = "默认车型配置"
                    },
                }
            },
            new UserFilterColConfig()
            {
                BaseCategory = "ShowVehicle",
                BaseCategoryName = FirstCategoryNameMap["ShowVehicle"],
                SecnodCategory = "DefaultTire",
                SecondCategoryName = SecondCategoryNameMap["DefaultTire"],
                TableName = "dm_userdetail",
                TableColConfig = new List<TableColConfig>()
                {
                    new TableColConfig()
                    {
                        ColName = "defaultspecifications",
                        TableColName = "默认车型轮胎规格"
                    }
                }
            },
            new UserFilterColConfig()
            {
                BaseCategory = "ShowVehicle",
                BaseCategoryName = FirstCategoryNameMap["ShowVehicle"],
                SecnodCategory = "DefaultTwoLevelPrice",
                SecondCategoryName = SecondCategoryNameMap["DefaultTwoLevelPrice"],
                TableName = "dm_userdetail",
                TableColConfig = new List<TableColConfig>()
                {
                    new TableColConfig()
                    {
                        ColName = "twoavgprice",
                        TableColName = "默认二级车型均价"
                    }
                }
            },
            new UserFilterColConfig()
            {
                BaseCategory = "ShowVehicle",
                BaseCategoryName = FirstCategoryNameMap["ShowVehicle"],
                SecnodCategory = "DefaultFiveLevelPrice",
                SecondCategoryName = SecondCategoryNameMap["DefaultFiveLevelPrice"],
                TableName = "dm_userdetail",
                TableColConfig = new List<TableColConfig>()
                {
                    new TableColConfig()
                    {
                        ColName = "fiveavgprice",
                        TableColName = "默认五级车型均价"
                    }
                }
            },
            new UserFilterColConfig()
            {
                BaseCategory = "ShowVehicle",
                BaseCategoryName = FirstCategoryNameMap["ShowVehicle"],
                SecnodCategory = "DefaultVehicleKM",
                SecondCategoryName = SecondCategoryNameMap["DefaultVehicleKM"],
                TableName = "dm_userdetail",
                TableColConfig = new List<TableColConfig>()
                {
                    new TableColConfig()
                    {
                        ColName = "kilometer",
                        TableColName = "默认车辆公里数"
                    }
                }
            },
            new UserFilterColConfig()
            {
                BaseCategory = "ShowVehicle",
                BaseCategoryName = FirstCategoryNameMap["ShowVehicle"],
                SecnodCategory = "DefaultVehicleRoadTime",
                SecondCategoryName = SecondCategoryNameMap["DefaultVehicleRoadTime"],
                TableName = "dm_userdetail",
                TableColConfig = new List<TableColConfig>()
                {
                    new TableColConfig()
                    {
                        ColName = "onroaddtae",
                        TableColName = "默认车辆上路时间"
                    }
                }

            },
            new UserFilterColConfig()
            {
                BaseCategory = "ShowViews",
                BaseCategoryName = FirstCategoryNameMap["ShowViews"],
                SecnodCategory = "ListPagePV",
                SecondCategoryName = SecondCategoryNameMap["ListPagePV"],
                TableName = "dm_userlog",
                TableColConfig = new List<TableColConfig>()
                {
                    new TableColConfig()
                    {
                        ColName = "loaddate",
                        TableColName = "浏览日期"
                    },
                    new TableColConfig()
                    {
                        ColName = "pv",
                        TableColName = "页面浏览次数"
                    },
                    new TableColConfig()
                    {
                        ColName = "channel",
                        TableColName = "页面统计来源"
                    },
                    new TableColConfig()
                    {
                        ColName = "businesslines",
                        TableColName = "商品详情页业务线"
                    },
                    new TableColConfig()
                    {
                        ColName = "typename",
                        TableColName = "页面类型"
                    }
                }
            },
            new UserFilterColConfig()
            {
                BaseCategory = "ShowViews",
                BaseCategoryName = FirstCategoryNameMap["ShowViews"],
                SecnodCategory = "ProductDetailByCategoryPV",
                SecondCategoryName = SecondCategoryNameMap["ProductDetailByCategoryPV"],
                TableName = "dm_userlog",
                TableColConfig = new List<TableColConfig>()
                {
                    new TableColConfig()
                    {
                        ColName = "loaddate",
                        TableColName = "浏览日期"
                    },
                    new TableColConfig()
                    {
                        ColName = "pv",
                        TableColName = "页面浏览次数"
                    },
                    new TableColConfig()
                    {
                        ColName = "channel",
                        TableColName = "页面统计来源"
                    },
                    new TableColConfig()
                    {
                        ColName = "businesslines",
                        TableColName = "商品详情页业务线"
                    }
                    ,
                    new TableColConfig()
                    {
                        ColName = "category1",
                        TableColName = "商品详情页一级类目"
                    }
                    ,
                    new TableColConfig()
                    {
                        ColName = "category2",
                        TableColName = "商品详情页二级类目"
                    }
                    ,
                    new TableColConfig()
                    {
                        ColName = "category3",
                        TableColName = "商品详情页三级类目"
                    }
                }
            },
            new UserFilterColConfig()
            {
                BaseCategory = "ShowViews",
                BaseCategoryName = FirstCategoryNameMap["ShowViews"],
                SecnodCategory = "ProductDetailByPIDPV",
                SecondCategoryName = SecondCategoryNameMap["ProductDetailByPIDPV"],
                TableName = "dm_userlog",
                TableColConfig = new List<TableColConfig>()
                {
                    new TableColConfig()
                    {
                        ColName = "loaddate",
                        TableColName = "浏览日期"
                    },
                    new TableColConfig()
                    {
                        ColName = "pv",
                        TableColName = "页面浏览次数"
                    },
                    new TableColConfig()
                    {
                        ColName = "channel",
                        TableColName = "页面统计来源"
                    },
                    new TableColConfig()
                    {
                        ColName = "pid",
                        TableColName = "商品PID"
                    }
                }
            },
            new UserFilterColConfig()
            {
                BaseCategory = "ShowViews",
                BaseCategoryName = FirstCategoryNameMap["ShowViews"],
                SecnodCategory = "PageViewByTireSizePV",
                SecondCategoryName = SecondCategoryNameMap["PageViewByTireSizePV"],
                TableName = "dm_userlog",
                TableColConfig = new List<TableColConfig>()
                {
                    new TableColConfig()
                    {
                        ColName = "loaddate",
                        TableColName = "浏览日期"
                    },
                    new TableColConfig()
                    {
                        ColName = "pv",
                        TableColName = "页面浏览次数"
                    },
                    new TableColConfig()
                    {
                        ColName = "channel",
                        TableColName = "页面统计来源"
                    },
                    new TableColConfig()
                    {
                        ColName = "specifications",
                        TableColName = "商品详情页轮胎规格"
                    }
                }
            },
            new UserFilterColConfig()
            {
                BaseCategory = "ShowOrders",
                BaseCategoryName = FirstCategoryNameMap["ShowOrders"],
                SecnodCategory = "OrderByOrderChannel",
                SecondCategoryName = SecondCategoryNameMap["OrderByOrderChannel"],
                TableName = "dm_userorderdetail",
                TableColConfig = new List<TableColConfig>()
                {
                    new TableColConfig()
                    {
                        ColName = "typename",
                        TableColName = "订单购买状态"
                    },
                    new TableColConfig()
                    {
                        ColName = "loaddate",
                        TableColName = "订单购买状态变更日期"
                    },
                    new TableColConfig()
                    {
                        ColName = "channeltype",
                        TableColName = "订单渠道大类"
                    },
                    new TableColConfig()
                    {
                        ColName = "channelgather",
                        TableColName = "订单渠道小类"
                    }
                    ,
                    new TableColConfig()
                    {
                        ColName = "num",
                        TableColName = "产品数量"
                    }
                    ,
                    new TableColConfig()
                    {
                        ColName = "salesamount1",
                        TableColName = "销售额"
                    }
                    ,
                    new TableColConfig()
                    {
                        ColName = "salesamount2",
                        TableColName = "销售额_考虑优惠券"
                    }
                    ,
                    new TableColConfig()
                    {
                        ColName = "cost",
                        TableColName = "成本"
                    }
                }
            },
            new UserFilterColConfig()
            {
                BaseCategory = "ShowOrders",
                BaseCategoryName = FirstCategoryNameMap["ShowOrders"],
                SecnodCategory = "OrderByCategoryYewu",
                SecondCategoryName = SecondCategoryNameMap["OrderByCategoryYewu"],
                TableName = "dm_userorderdetail",
                TableColConfig = new List<TableColConfig>()
                {
                    new TableColConfig()
                    {
                        ColName = "typename",
                        TableColName = "订单购买状态"
                    },
                    new TableColConfig()
                    {
                        ColName = "loaddate",
                        TableColName = "订单购买状态变更日期"
                    },
                    new TableColConfig()
                    {
                        ColName = "num",
                        TableColName = "产品数量"
                    }
                    ,
                    new TableColConfig()
                    {
                        ColName = "salesamount1",
                        TableColName = "销售额"
                    },
                    new TableColConfig()
                    {
                        ColName = "salesamount2",
                        TableColName = "销售额_考虑优惠券"
                    },
                    new TableColConfig()
                    {
                        ColName = "cost",
                        TableColName = "成本"
                    },
                    new TableColConfig()
                    {
                        ColName = "businesslines",
                        TableColName = "产品业务线"
                    },
                    new TableColConfig()
                    {
                        ColName = "category",
                        TableColName = "产品品类"
                    } ,
                    new TableColConfig()
                    {
                        ColName = "category1",
                        TableColName = "一级类目"
                    } ,
                    new TableColConfig()
                    {
                        ColName = "category2",
                        TableColName = "二级类目"
                    } ,
                    new TableColConfig()
                    {
                        ColName = "category3",
                        TableColName = "三级类目"
                    }
                }
            },
            new UserFilterColConfig()
            {
                BaseCategory = "ShowOrders",
                BaseCategoryName = FirstCategoryNameMap["ShowOrders"],
                SecnodCategory = "OrderByPID",
                SecondCategoryName = SecondCategoryNameMap["OrderByPID"],
                TableName = "dm_userorderdetail",
                TableColConfig = new List<TableColConfig>()
                {
                    new TableColConfig()
                    {
                        ColName = "typename",
                        TableColName = "订单购买状态"
                    },
                    new TableColConfig()
                    {
                        ColName = "loaddate",
                        TableColName = "订单购买状态变更日期"
                    },
                    new TableColConfig()
                    {
                        ColName = "num",
                        TableColName = "产品数量"
                    }
                    ,
                    new TableColConfig()
                    {
                        ColName = "salesamount1",
                        TableColName = "销售额"
                    },
                    new TableColConfig()
                    {
                        ColName = "salesamount2",
                        TableColName = "销售额_考虑优惠券"
                    },
                    new TableColConfig()
                    {
                        ColName = "cost",
                        TableColName = "成本"
                    },
                    new TableColConfig()
                    {
                        ColName = "pid",
                        TableColName = "商品PID"
                    }
                }
            },
            new UserFilterColConfig()
            {
                BaseCategory = "ShowOrders",
                BaseCategoryName = FirstCategoryNameMap["ShowOrders"],
                SecnodCategory = "OrderByPID",
                SecondCategoryName = SecondCategoryNameMap["OrderByPID"],
                TableName = "dm_userorderdetail",
                TableColConfig = new List<TableColConfig>()
                {
                    new TableColConfig()
                    {
                        ColName = "typename",
                        TableColName = "订单购买状态"
                    },
                    new TableColConfig()
                    {
                        ColName = "loaddate",
                        TableColName = "订单购买状态变更日期"
                    },
                    new TableColConfig()
                    {
                        ColName = "num",
                        TableColName = "产品数量"
                    }
                    ,
                    new TableColConfig()
                    {
                        ColName = "salesamount1",
                        TableColName = "销售额"
                    },
                    new TableColConfig()
                    {
                        ColName = "salesamount2",
                        TableColName = "销售额_考虑优惠券"
                    },
                    new TableColConfig()
                    {
                        ColName = "cost",
                        TableColName = "成本"
                    },
                    new TableColConfig()
                    {
                        ColName = "orderdescription",
                        TableColName = "活动类型"
                    }
                }
            },
        };
    }
}
