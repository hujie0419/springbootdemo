using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaoYangRefreshCacheService.BLL;
using BaoYangRefreshCacheService.DAL;
using BaoYangRefreshCacheService.Model;
using Common.Logging;
using System.Threading;
using System.Net.Http;
using Newtonsoft.Json;
using System.Configuration;

namespace BaoYangRefreshCacheService.BLL
{
    public class KpiBll
    {
        private const string AuthSecret = "143461e5-9909-4ffb-bf7d-f847d30ef14b";
        private static readonly string host = ConfigurationManager.AppSettings["Parts_Host"];

        private KpiDal dal;
        private IEnumerable<BrandCount> personList;
        private ILog logger;
        public KpiBll(ILog logger)
        {
            dal = new KpiDal();
            personList = dal.GetAllPersonWithBrand();
            this.logger = logger;
        }

        /// <summary>
        /// 插入保存数据
        /// </summary>
        /// <param name="list"></param>
        public void Insert(List<KpiReportDetail> list, string reportName, string version = "1.0", string createUser = "SystemAuto")
        {
            var reportTypeID = dal.GetKpiReportTypeIDByReportName(reportName);
            dal.ExcuteTranfacantion(list, reportName, reportTypeID, version, createUser);
        }

        #region 适配覆盖

        /// <summary>
        /// 获取适配覆盖率
        /// </summary>
        /// <returns></returns>
        public List<KpiReportDetail> GetAllAdaptationCoverage()
        {
            Func<IEnumerable<string>, string, string, string, int, List<BrandCount>> getDataFunc = (names, group, action, refType, rank) =>
            {
                var dict = new Dictionary<string, string>
                {
                    ["authSecret"] = AuthSecret,
                    ["names"] = string.Join(",", names),
                    ["group"] = group,
                    ["action"] = action,
                    ["refType"] = refType,
                    ["rank"] = rank.ToString(),
                };
                return Post<List<BrandCount>>($"{host}/OpenReport/GetNumOfCoverage", dict) ?? new List<BrandCount>();
            };

            Func<IEnumerable<string>, string, string, string, List<BrandCount>> getDataForTireFunc = (names, group, action, refType) =>
            {
                var dict = new Dictionary<string, string>
                {
                    ["authSecret"] = AuthSecret,
                    ["names"] = string.Join(",", names),
                    ["group"] = group,
                    ["action"] = action,
                    ["refType"] = refType,
                };
                return Post<List<BrandCount>>($"{host}/OpenReport/GetNumOfCoverageForTire", dict) ?? new List<BrandCount>();
            };

            Func<IEnumerable<BrandCount>, IEnumerable<BrandCount>> associateFunc = totalBrandCountList
                => from x in personList
                   join y in totalBrandCountList on x.Brand equals y.Brand into temp
                   from z in temp.DefaultIfEmpty(new BrandCount())
                   select new BrandCount { Brand = x.Brand, Count = z.Count, TotalCount = z.TotalCount, Name = x.Name };

            Func<IEnumerable<BrandCount>, IEnumerable<BrandCount>, string, string, string, string, IEnumerable<KpiReportDetail>> convertToBaseDataFunc =
                (leftList, joinList, displayName, typeName, categoryName, hotVehicleType)
                => from x in leftList
                   join y in joinList on x.Brand equals y.Brand into temp
                   from z in temp.DefaultIfEmpty(new BrandCount { Brand = null, Count = 0, TotalCount = 0 })
                   select new KpiReportDetail
                   {
                       CategoryName = categoryName,
                       TypeName = typeName,
                       HotVehicleType = hotVehicleType,
                       ParameterName = displayName,
                       VehicleAdaptCount = z.Count,
                       VehicleBrand = x.Brand,
                       VehicleTotalCount = x.TotalCount,
                       Name = x.Name,
                   };

            var config = BaoYangConfigBll.GetBaoYangKpiReportConfig();
            var coverageRatios = config.CoverageRatios;
            var tireCoverageRatios = config.TireCoverageRatios;

            var associateAllList = associateFunc(dal.GetAllTidCountWithBrand()).ToList();
            var associate100List = associateFunc(dal.GetHotVehicleTidCountWithBrand(100)).ToList();
            var associate400List = associateFunc(dal.GetHotVehicleTidCountWithBrand(400)).ToList();
            var associateVehicleSeriesList = associateFunc(dal.GetVehicleSeriesCountWithBrand());

            var list = coverageRatios.SelectMany(item => item.Actions.SelectMany
                (action =>
                {
                    var numForAll = getDataFunc(item.Names, item.Group, action, item.RefType, 0);
                    var numFor100 = getDataFunc(item.Names, item.Group, action, item.RefType, 100);
                    var numFor400 = getDataFunc(item.Names, item.Group, action, item.RefType, 400);

                    var baseDataForAll = convertToBaseDataFunc(associateAllList, numForAll, item.DisplayName, action, string.Empty, "All");
                    var baseDataFor100 = convertToBaseDataFunc(associate100List, numFor100, item.DisplayName, action, string.Empty, "100");
                    var baseDataFor400 = convertToBaseDataFunc(associate400List, numFor400, item.DisplayName, action, string.Empty, "400");
                    return baseDataForAll.Concat(baseDataFor100).Concat(baseDataFor400).ToList();
                })
            ).ToList();

            list.AddRange(tireCoverageRatios.Where(x => string.Equals(x.Group, "two")).SelectMany(item => item.Actions.SelectMany(action =>
            {
                var baseData = getDataForTireFunc(item.Names, item.Group, action, item.RefType);
                return convertToBaseDataFunc(associateVehicleSeriesList, baseData, item.DisplayName, action, item.Group, string.Empty);
            })));

            list.AddRange(tireCoverageRatios.Where(x => string.Equals(x.Group, "five")).SelectMany(item => item.Actions.SelectMany(action =>
            {
                var baseData = getDataForTireFunc(item.Names, item.Group, action, item.RefType);
                return convertToBaseDataFunc(associateAllList, baseData, item.DisplayName, action, item.Group, string.Empty);
            })));

            return list;
        }

        private string GetCategoryName(string partName)
        {
            switch (partName)
            {
                case "机油滤清器":
                case "空气滤清器":
                case "空调滤清器":
                case "燃油滤清器(内置+外置)":
                case "燃油滤清器":
                case "内置燃油滤":
                case "外置空调滤清器":
                    return "四滤";
                case "刹车片前":
                case "刹车片后":
                case "刹车盘前":
                case "刹车盘后":
                    return "刹车盘片";
                case "前雨刷(专用雨刷+左右前雨刷)":
                case "左前雨刷":
                case "右前雨刷":
                case "后雨刷":
                case "前雨刷":
                    return "雨刷";
                case "近光灯":
                case "远光灯":
                case "远近一体":
                case "大灯(远近一体+远近光灯)":
                case "前雾灯":
                    return "车灯";
                case "火花塞":
                case "火花塞数量":
                    return "火花塞";
                case "助力转向油":
                case "制动液":
                case "发动机油":
                case "发动机防冻液":
                case "空调制冷剂":
                case "变速箱油":
                    return "油品";
                default:
                    return "其它";
            }
        }

        #endregion

        #region 车型完备度

        public List<KpiReportDetail> GetAllVehicleParameter()
        {
            Func<string, int, IEnumerable<BrandCount>> getDataFunc = (fieldName, rank) =>
            {
                var dict = new Dictionary<string, string>
                {
                    ["authSecret"] = AuthSecret,
                    ["fieldName"] = fieldName,
                    ["rank"] = rank.ToString(),
                };
                return Post<List<BrandCount>>($"{host}/OpenReport/GetVehicleParameterComplete", dict) ?? new List<BrandCount>();
            };

            Func<IEnumerable<BrandCount>, IEnumerable<BrandCount>, string, string, IEnumerable<KpiReportDetail>> convertToBaseDataFunc =
                (leftList, joinList, hotVehicleType, zhName) =>
                (from x in leftList
                 join y in joinList on x.Brand equals y.Brand into temp
                 from z in temp.DefaultIfEmpty(new BrandCount { Brand = null, Count = 0, TotalCount = 0 })
                 select new KpiReportDetail
                 {
                     VehicleTotalCount = x.TotalCount,
                     VehicleAdaptCount = z.Count,
                     ParameterName = zhName,
                     HotVehicleType = hotVehicleType,
                     VehicleBrand = x.Brand,
                     Name = x.Name,
                 });
            var columnKeyValues = Post<Dictionary<string, string>>($"{host}/OpenReport/GetVehicleParameterCompleteConfig",
                new Dictionary<string, string>
                {
                    ["authSecret"] = AuthSecret,
                }) ?? new Dictionary<string, string>();

            var list = new List<KpiReportDetail>();

            #region 全部车型

            var totalList = (from x in personList
                             join y in dal.GetAllTidCountWithBrand() on x.Brand equals y.Brand into temp
                             from z in temp.DefaultIfEmpty(new BrandCount())
                             select new BrandCount
                             {
                                 Brand = x.Brand,
                                 Count = z.Count,
                                 TotalCount = z.TotalCount,
                                 Name = x.Name
                             }).ToList();
            list.AddRange(columnKeyValues.SelectMany(kv => convertToBaseDataFunc(totalList, getDataFunc(kv.Key, 0), "All", kv.Value)));

            #endregion

            #region 前100热门车型

            totalList = (from x in personList
                         join y in dal.GetHotVehicleTidCountWithBrand(100) on x.Brand equals y.Brand into temp
                         from z in temp.DefaultIfEmpty(new BrandCount())
                         select new BrandCount
                         {
                             Brand = x.Brand,
                             Count = z.Count,
                             TotalCount = z.TotalCount,
                             Name = x.Name
                         }).ToList();
            list.AddRange(columnKeyValues.SelectMany(kv => convertToBaseDataFunc(totalList, getDataFunc(kv.Key, 100), "100", kv.Value)));

            #endregion

            #region 前400热门车型

            totalList = (from x in personList
                         join y in dal.GetHotVehicleTidCountWithBrand(400) on x.Brand equals y.Brand into temp
                         from z in temp.DefaultIfEmpty(new BrandCount())
                         select new BrandCount
                         {
                             Brand = x.Brand,
                             Count = z.Count,
                             TotalCount = z.TotalCount,
                             Name = x.Name
                         }).ToList();
            list.AddRange(columnKeyValues.SelectMany(kv => convertToBaseDataFunc(totalList, getDataFunc(kv.Key, 400), "400", kv.Value)));

            #endregion

            return list;
        }

        #endregion

        #region 适配审核

        /// <summary>
        /// 获取所有配件是否审核
        /// </summary>
        /// <param name="currentTime"></param>
        /// <returns></returns>
        public List<KpiReportDetail> GetAllPartValidated()
        {
            var config = BaoYangConfigBll.GetBaoYangAdaptationConfig();
            var list = new List<KpiReportDetail>();
            config.ProductAdaptations.ForEach(x =>
            {
                var detail = GetIsValidatedCount(x);
                list.AddRange(detail);
            });
            config.SpecialProductAdaptations.ForEach(x =>
            {
                var detailList = new List<KpiReportDetail>();
                x.PartNames.Split(", ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList()
                    .ForEach(y => detailList.AddRange(GetIsValidatedCount(y)));
                list.AddRange(detailList);
            });
            return list;
        }

        /// <summary>
        /// 获取ProductAdaptation适配审核
        /// </summary>
        /// <param name="item"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        private IEnumerable<KpiReportDetail> GetIsValidatedCount(ProductAdaptation item)
        {
            return from x in personList
                   join y in dal.GetValidatedBrandCount(item.PartNames) on x.Brand equals y.Brand into temp
                   from z in temp.DefaultIfEmpty(new BrandCount())
                   select new KpiReportDetail
                   {
                       CategoryName = GetCategoryName(item.DisplayName),
                       VehicleAdaptCount = z.Count,
                       VehicleTotalCount = z.TotalCount,
                       ParameterName = item.DisplayName,
                       Name = x.Name,
                       VehicleBrand = x.Brand,
                   };
        }

        /// <summary>
        /// 获取SpecialProductAdaptation适配审核
        /// </summary>
        /// <param name="partName"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        private IEnumerable<KpiReportDetail> GetIsValidatedCount(string partName)
        {
            return from x in personList
                   join y in dal.GetValidatedBrandCount(partName) on x.Brand equals y.Brand into temp
                   from z in temp.DefaultIfEmpty(new BrandCount())
                   select new KpiReportDetail
                   {
                       CategoryName = GetCategoryName(partName),
                       VehicleAdaptCount = z.Count,
                       VehicleTotalCount = z.TotalCount,
                       ParameterName = partName,
                       VehicleBrand = x.Brand,
                       Name = x.Name,
                   };
        }

        #endregion

        #region 新增适配

        public List<KpiReportDetail> GetAllNewAdaptationCount()
        {
            Func<IEnumerable<string>, string, string, string, List<BrandCount>> getDataFunc = (names, group, action, refType) =>
            {
                var dict = new Dictionary<string, string>
                {
                    ["authSecret"] = AuthSecret,
                    ["names"] = string.Join(",", names),
                    ["group"] = group,
                    ["action"] = action,
                    ["refType"] = refType,
                };
                return Post<List<BrandCount>>($"{host}/OpenReport/GetChangedNum", dict) ?? new List<BrandCount>();
            };

            var adaptationChangeds = BaoYangConfigBll.GetBaoYangKpiReportConfig().AdaptationChangeds;
            var list = adaptationChangeds.SelectMany(item => item.Actions.SelectMany(action =>
                from x in personList
                join y in getDataFunc(item.Names, item.Group, action, item.RefType) on x.Brand equals y.Brand into temp
                from z in temp.DefaultIfEmpty(new BrandCount())
                select new KpiReportDetail
                {
                    TypeName = action,
                    ParameterName = item.DisplayName,
                    VehicleAdaptCount = z.Count,
                    VehicleBrand = x.Brand,
                    Name = x.Name
                })).ToList();

            return list;
        }

        #endregion

        #region 适配准确度

        public List<KpiReportDetail> GetNoAdapted()
        {
            var list = new List<KpiReportDetail>();

            var totalList = from x in personList
                            join y in dal.GetTotalOrderCount() on x.Brand equals y.Brand into temp
                            from z in temp.DefaultIfEmpty(new BrandCount())
                            select new BrandCount { Brand = x.Brand, Count = z.Count, TotalCount = z.TotalCount, Name = x.Name };

            list.AddRange(from x in totalList
                          join y in dal.GetOrderResponseDepart() on x.Brand equals y.Brand into temp
                          from z in temp.DefaultIfEmpty(new BrandCount { Brand = null, Count = 0, TotalCount = 0 })
                          select new KpiReportDetail
                          {
                              ParameterName = "Development",
                              VehicleAdaptCount = z.Count,
                              VehicleTotalCount = x.TotalCount,
                              VehicleBrand = x.Brand,
                              Name = x.Name,
                          });

            list.AddRange(from x in totalList
                          join y in dal.GetOrederFeedBackAuditDataNoAdaptedCount() on x.Brand equals y.Brand into temp
                          from z in temp.DefaultIfEmpty(new BrandCount { Brand = null, Count = 0, TotalCount = 0 })
                          select new KpiReportDetail
                          {
                              ParameterName = "AdaptData",
                              VehicleAdaptCount = z.Count,
                              VehicleTotalCount = x.TotalCount,
                              VehicleBrand = x.Brand,
                              Name = x.Name,
                          });

            return list;
        }

        #endregion

        public void CreateKpiReport()
        {
            logger.Info("生成KPI报表请求开始....");
            try
            {
                logger.Info("生成KPI报表 车型参数完备度开始....");
                Insert(GetAllVehicleParameter(), "车型参数完备度", "1.3");
                logger.Info("生成KPI报表 车型参数完备度完成....");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }

            try
            {
                logger.Info("生成KPI报表 适配覆盖率开始....");
                Insert(GetAllAdaptationCoverage(), "适配覆盖率", "1.3");
                logger.Info("生成KPI报表 适配覆盖率结束....");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }

            try
            {
                logger.Info("生成KPI报表 数据审核进度开始....");
                Insert(GetAllPartValidated(), "数据审核进度", "1.2");
                logger.Info("生成KPI报表 数据审核进度结束....");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }

            Thread.Sleep(30000);
            try
            {
                logger.Info("生成KPI报表 适配准确度开始....");
                Insert(GetNoAdapted(), "适配准确度", "1.2");
                logger.Info("生成KPI报表 适配准确度结束....");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }

            logger.Info("生成KPI报表请求结束....");

            try
            {
                logger.Info("生成KPI报表 适配数据工作量开始....");
                Insert(GetAllNewAdaptationCount(), "适配数据工作量", "1.3");
                logger.Info("生成KPI报表 适配数据工作量结束....");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
        }

        private static T Post<T>(string url, Dictionary<string, string> dict)
        {
            using (var httpClient = new HttpClient())
            {
                var response = httpClient.PostAsync(url, new FormUrlEncodedContent(dict)).Result;
                if (response.IsSuccessStatusCode)
                {
                    var str = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<T>(str);
                }
                return default(T);
            }
        }
    }
}
