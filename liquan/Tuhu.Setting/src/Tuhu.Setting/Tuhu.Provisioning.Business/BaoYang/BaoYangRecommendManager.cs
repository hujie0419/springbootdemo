using Common.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.BaoYang;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.Business.Product;
using Tuhu.Provisioning.Business.ServiceProxy;
using Tuhu.Provisioning.DataAccess;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.DAO.BaoYang;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.BaoYang;
using Tuhu.Provisioning.DataAccess.Entity.BaoYangProductPriority;
using Tuhu.Service.Product;
using Tuhu.Service.Product.Models;
using Tuhu.Service.Shop.Models.Region;
using Tuhu.Provisioning.DataAccess.Request;
using System.Threading;
using Tuhu.Provisioning.Business.Redis;
using static Tuhu.Provisioning.Business.Redis.FlashCounter;
using System.Data;
using NPOI.SS.UserModel;

namespace Tuhu.Provisioning.Business.BaoYang
{
    public class BaoYangRecommendManager
    {
        private static readonly Common.Logging.ILog logger = LogManager.GetLogger(typeof(BaoYangRecommendManager));
        private static readonly IConnectionManager byAlwaysOnReadConnectionManager =
          new ConnectionManager(ConfigurationManager.ConnectionStrings["BaoYang_AlwaysOnRead"].ConnectionString);
        private static readonly IConnectionManager connectionManager =
         new ConnectionManager(ConfigurationManager.ConnectionStrings["BaoYang"].ConnectionString);
        private readonly IDBScopeManager TuhuBaoYangReadDbScopeManager = null;
        private readonly IDBScopeManager dbScopeManagerBY = null;
        private static string _counterClient = "baoYangRecommendManagerArea";
        public BaoYangRecommendManager()
        {
            dbScopeManagerBY = new DBScopeManager(connectionManager);
            TuhuBaoYangReadDbScopeManager = new DBScopeManager(byAlwaysOnReadConnectionManager);
        }
        #region 常规推荐
        /// <summary>
        /// 获取产品推荐排序
        /// </summary>
        /// <returns></returns>
        public List<BaoYangProductPriorityModel> GetBaoYangProductPriority(string partName, string category)
        {
            var result = new List<BaoYangProductPriorityModel>();
            try
            {
                var productManager = new ProductManager();
                //因为前雨刷 和后雨刷总成  后雨刷非总成 是用的一个category   需要分开写逻辑
                switch (partName)
                {
                    case "雨刷":
                        result = productManager.GetBaoYangWiperProductSeries(category, "前", string.Empty);
                        break;
                    case "非总成后雨刷":
                        result = productManager.GetBaoYangWiperProductSeries(category, "后", "非总成");
                        break;
                    case "总成后雨刷":
                        result = productManager.GetBaoYangWiperProductSeries(category, "后", "总成");
                        break;
                    default:
                        result = productManager.GetBaoYangProductSeriesByCategory(category);
                        break;
                }
                var priorities = new BaoYangManager().GetProductPriorityRegularByPartName(new List<string> { partName }) ?? new List<ProductPriorityRegular>();
                foreach (var item in result)
                {
                    item.Priority = priorities.FirstOrDefault(x => string.Equals(x.Brand, item.Brand, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(x.Series, item.Series, StringComparison.OrdinalIgnoreCase))?.Priority ?? Int32.MaxValue;
                }
                result = result.OrderBy(x => x.Priority).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("GetBaoYangProductPriority", ex);
            }
            return result;
        }


        public Dictionary<string, IEnumerable<string>> GetAllBrandAndSeries(string partName, string category)
        {
            var result = new Dictionary<string, IEnumerable<string>>();
            try
            {
                var list = new List<BaoYangProductPriorityModel>();
                var productManager = new ProductManager();
                //因为前雨刷 和后雨刷总成  后雨刷非总成 是用的一个category   需要分开写逻辑
                switch (partName)
                {
                    case "雨刷":
                        list = productManager.GetBaoYangWiperProductSeries(category, "前", string.Empty);
                        break;
                    case "非总成后雨刷":
                        list = productManager.GetBaoYangWiperProductSeries(category, "后", "非总成");
                        break;
                    case "总成后雨刷":
                        list = productManager.GetBaoYangWiperProductSeries(category, "后", "总成");
                        break;
                    default:
                        list = productManager.GetBaoYangProductSeriesByCategory(category);
                        break;
                }
                result = list.GroupBy(x => x.Brand).Select(x => new { k = x.Key, v = x.Select(s => s.Series) }).ToDictionary(k => k.k, v => v.v);
            }
            catch (Exception ex)
            {
                logger.Error("GetBaoYangProductPriority", ex);
            }
            return result;
        }

        /// <summary>
        /// 保存产品推荐排序
        /// </summary>
        /// <returns></returns>
        public bool SaveBaoYangProductPriority(List<BaoYangProductPriorityModel> settings, string partName, string userName)
        {
            var result = false;
            try
            {
                var originalList = new BaoYangManager().GetProductPriorityRegularByPartName(new List<string> { partName }).Select(x => new BaoYangProductPriorityModel()
                {
                    Brand = x.Brand,
                    Priority = x.Priority,
                    Series = x.Series
                }).ToList() ?? new List<BaoYangProductPriorityModel>();
                dbScopeManagerBY.CreateTransaction(conn =>
                {
                    DalBaoYangRecommend.DeleteProductPrioritySettingNewByPartName(conn, partName);
                    foreach (var item in settings)
                    {
                        DALBaoyang.InsertProductPriorityRegular(conn, item, partName);
                    }
                });
                var partNames = new List<string>() { partName };
                InsertProductPrioityLog(JsonConvert.SerializeObject(originalList), JsonConvert.SerializeObject(settings), userName, partName, "ProductPriority_Regular");
                result = true;
            }
            catch (Exception ex)
            {
                logger.Error("SaveBaoYangProductPriority", ex);
            }
            return result;
        }

        /// <summary>
        /// 添加产品推荐常规日志
        /// </summary>
        /// <param name="originalList"></param>
        /// <param name="newList"></param>
        /// <param name="userName"></param>
        /// <param name="partName"></param>
        /// <param name="IdentityID"></param>
        /// <returns></returns>
        private bool InsertProductPrioityLog(string originalList, string newList, string userName, string IdentityID, string logType)
        {
            var result = false;
            var log = new BaoYangOprLog()
            {
                IdentityID = IdentityID,
                LogType = logType,
                Remarks = "修改",
                NewValue = newList,
                OldValue = originalList,
                CreateTime = DateTime.Now,
                OperateUser = userName
            };
            LoggerManager.InsertLog("BYOprLog", log);
            return result;
        }

        /// <summary>
        /// 保存机油推荐排序
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool SaveOilBaoYangProductPriority(List<BaoYangProductPriorityModel> settings, string grade, string viscosity, string userName)
        {
            var result = false;
            try
            {
                var originalList = TuhuBaoYangReadDbScopeManager.Execute(conn => DALBaoyang.GetOilBaoYangProductPriority(conn))
                    .Where(x => string.Equals(x.Grade, grade, StringComparison.OrdinalIgnoreCase) && string.Equals(x.Viscosity, viscosity, StringComparison.OrdinalIgnoreCase))
                    .Select(x => new BaoYangProductPriorityModel
                    {
                        Brand = x.Brand,
                        Priority = x.Priority,
                        Series = x.Series
                    }).ToList();
                dbScopeManagerBY.CreateTransaction(conn =>
                {
                    DalBaoYangRecommend.DeleteOilProductPriority(conn, grade, viscosity);
                    foreach (var item in settings)
                    {
                        DalBaoYangRecommend.InsertOilProductPriority(conn, item, grade, viscosity);
                    }
                });
                InsertProductPrioityLog(JsonConvert.SerializeObject(originalList), JsonConvert.SerializeObject(settings), userName, $"{grade}_{viscosity}", "ProductPriority_Regular_Oil");
                result = true;
            }
            catch (Exception ex)
            {
                logger.Error("SaveOilProductPriority", ex);
            }
            return result;
        }

        /// <summary>
        /// 获取机油产品推荐排序
        /// </summary>
        /// <returns></returns>
        public List<OilBaoYangProductPriorityModel> GetOilBaoYangProductPriority()
        {
            var result = new List<OilBaoYangProductPriorityModel>();
            try
            {
                var products = new ProductManager().GetOilBaoYangProductSeries();
                var priorities = TuhuBaoYangReadDbScopeManager.Execute(conn => DALBaoyang.GetOilBaoYangProductPriority(conn));
                foreach (var item in products)
                {
                    switch (item.Grade)
                    {
                        case "全合成机油":
                            item.Grade = "全合成";
                            break;
                        case "半合成机油":
                            item.Grade = "半合成";
                            break;
                        case "矿物机油":
                            item.Grade = "矿物油";
                            break;
                    }
                    item.Priority = priorities.FirstOrDefault(x =>
                    string.Equals(x.Brand, item.Brand, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(x.Series, item.Series, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(x.Grade, item.Grade, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(x.Viscosity, item.Viscosity, StringComparison.OrdinalIgnoreCase)
                    )?.Priority ?? Int32.MaxValue;
                }
                result = products.GroupBy(x => x.Grade).Select(grade => new OilBaoYangProductPriorityModel
                {
                    Grade = grade.Key,
                    Viscosities = grade.GroupBy(v => v.Viscosity).Select(
                            viscosity => new OilViscosityModel()
                            {
                                Viscosity = viscosity.Key,
                                Detaileds = viscosity.ToList().Select(
                                y => new BaoYangProductPriorityModel
                                { Brand = y.Brand, Priority = y.Priority, Series = y.Series }).OrderBy(k => k.Priority).ToList()
                            }).OrderBy(x =>
                            {
                                int.TryParse(Regex.Replace(x.Viscosity, "\\D", string.Empty), out int sequence);
                                return sequence > 0 ? sequence : int.MaxValue;
                            }).ToList()
                }).ToList();

            }
            catch (Exception ex)
            {
                logger.Error("GetOilProductPriority", ex);
            }
            return result;
        }

        #endregion

        #region 特殊推荐

        #region 地区配置
        /// <summary>
        /// 获取产品推荐地区配置
        /// </summary>
        /// <returns></returns>
        public async Task<List<BaoYangProductPriorityAreaView>> GetProductPriorityArea(string partName)
        {
            List<BaoYangProductPriorityAreaView> result = null;
            try
            {
                var areas = TuhuBaoYangReadDbScopeManager.Execute(conn => DalBaoYangRecommend.GetProductPriorityArea(conn, partName));
                result = await ConvertToAreaView(areas);
            }
            catch (Exception ex)
            {
                logger.Error("GetOilBaoYangProductPriority", ex);
            }
            return result ?? new List<BaoYangProductPriorityAreaView>();
        }


        /// <summary>
        /// 获取车身类别
        /// </summary>
        /// <returns></returns>
        public List<string> GetVehicleBodyType()
        {
            var result = new List<string>();
            try
            {
                result = TuhuBaoYangReadDbScopeManager.Execute(conn => DalVehicleType.GetVehicleTypeBody(conn)) ?? result;
            }
            catch (Exception ex)
            {
                logger.Error("GetVehicleTypeBody", ex);
            }
            return result;
        }

        /// <summary>
        /// 保存地区配置模板
        /// </summary>
        /// <param name="area"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<Tuple<int, string>> SaveProductPriorityArea(BaoYangProductPriorityAreaView area, string userName)
        {
            var result = string.Empty;
            try
            {
                var areas = await GetProductPriorityArea(area.partName);
                var originalList = areas.FirstOrDefault(x => x.AreaId == area.AreaId) ?? new BaoYangProductPriorityAreaView();
                result = IsProductPriorityAreaRepeat(areas.Where(x => x.AreaId != area.AreaId).ToList(), new List<BaoYangProductPriorityAreaView>() { area });
                if (area.AreaId == 0)
                {
                    area.AreaId = await GetAreaMaxId(area.partName, area.AreaId);
                    if (area.AreaId == 0)
                    {
                        result = "当前操作过于频繁，请隔10秒之后在进行操作";
                    }
                }
                if (string.IsNullOrWhiteSpace(result))
                {
                    var productPriorityAreas = new List<ProductPriorityArea>();
                    foreach (var item in area.Details)
                    {
                        if (item.Citys != null && item.Citys.Any())
                        {
                            foreach (var city in item.Citys)
                            {
                                productPriorityAreas.Add(new ProductPriorityArea()
                                {
                                    AreaId = area.AreaId,
                                    PartName = area.partName,
                                    IsEnabled = area.IsEnabled,
                                    RegionId = city.CityId
                                });
                            }
                        }
                        else
                        {
                            productPriorityAreas.Add(new ProductPriorityArea()
                            {
                                AreaId = area.AreaId,
                                PartName = area.partName,
                                IsEnabled = area.IsEnabled,
                                RegionId = item.RegionId
                            });
                        }
                    }
                    dbScopeManagerBY.CreateTransaction(conn =>
                    {
                        DalBaoYangRecommend.DeleteProductPriorityArea(conn, area.partName, area.AreaId);
                        productPriorityAreas.ForEach(r =>
                        {
                            DalBaoYangRecommend.InsertProductPriorityArea(conn, r);
                        });
                    });
                    InsertProductPrioityLog(JsonConvert.SerializeObject(originalList), JsonConvert.SerializeObject(area), userName, $"{area.partName}", "ProductPriority_Area");
                }
            }
            catch (Exception ex)
            {
                result = "系统出现异常";
                logger.Error("GetBaoYangProductPriorityArea", ex);
            }
            return Tuple.Create(area.AreaId, result);
        }

        private async Task<int> GetAreaMaxId(string partName, int areaId)
        {
            var result = 0;
            areaId = TuhuBaoYangReadDbScopeManager.Execute(conn => DalBaoYangRecommend.GetMaxAreaIdByPartName(conn, partName));
            result = areaId == 0 ? 1 : areaId + 1;
            var flashCount = await GetFlashCount(_counterClient, areaId.ToString(), TimeSpan.FromSeconds(10), OperateType.Increment);
            if (flashCount >= 1)
            {
                result = 0;
            }
            return result;
        }
        /// <summary>
        /// 设置地区模板启用状态
        /// </summary>
        /// <param name="areaId"></param>
        /// <param name="isEnabled"></param>
        /// <param name="partName"></param>
        /// <returns></returns>
        public async Task<Tuple<bool, string>> UpdatePriorityAreaIsEnabled(int areaId, bool isEnabled, string partName)
        {
            var result = string.Empty;
            try
            {
                if (isEnabled)
                {
                    var areas = (await GetProductPriorityArea(partName));
                    //如果是开启状态 需要判断地区之间是否冲突  
                    result = IsProductPriorityAreaRepeat(areas.Where(x => x.IsEnabled && x.AreaId != areaId).ToList(), areas.Where(x => x.AreaId == areaId).ToList());
                }
                if (string.IsNullOrWhiteSpace(result))
                {
                    dbScopeManagerBY.Execute(conn => { DalBaoYangRecommend.UpdatePriorityAreaIsEnabled(conn, areaId, isEnabled, partName); });
                }
            }
            catch (Exception ex)
            {
                result = "系统出现异常";
                logger.Error("UpdatePriorityAreaIsEnabled", ex);
            }
            return Tuple.Create(result == string.Empty, result);
        }

        /// <summary>
        /// 比较地区配置是否有重复
        /// </summary>
        /// <param name="areas">目标数据</param>
        /// <param name="views">待比较数据</param>
        /// <returns></returns>
        private string IsProductPriorityAreaRepeat(List<BaoYangProductPriorityAreaView> areas, List<BaoYangProductPriorityAreaView> views)
        {
            var result = string.Empty;
            var areaConfigRegions = areas?.SelectMany(x => x.Details)?.ToList() ?? new List<BaoYangProductPriorityAreaDetail>();
            var viewRegions = views?.SelectMany(x => x.Details)?.ToList() ?? new List<BaoYangProductPriorityAreaDetail>();
            var regions = (from a in areaConfigRegions
                           from b in viewRegions
                           where a.RegionId == b.RegionId
                           select new { b.RegionId, b.Citys, b.RegionName }).Distinct().ToList();
            regions.ForEach(region =>
            {
                if (region.Citys != null && region.Citys.Any())
                {
                    region.Citys.ForEach(city =>
                    {
                        //和areas省市全部冲突
                        if (areaConfigRegions.Any(x => x.RegionId == region.RegionId && x.Citys.Any(k => k.CityId == city.CityId)))
                        {
                            result = $"{result}{region.RegionName}下{city.CityName}配置重复;";
                        }
                        //areas配置已经把整个省配置了
                        if (areaConfigRegions.Any(x => x.RegionId == region.RegionId && (x.Citys == null || !x.Citys.Any())))
                        {
                            result = $"{result}{region.RegionName}已经配置;";
                        }
                    });
                }
                else
                {
                    //views把整个省配置  与views配置冲突
                    result = $"{result}{region.RegionName}配置重复;";
                }
            });
            return result;
        }

        public bool HasArea(int areaid, string partName)
        {
            var result = false;
            try
            {
                var areas = TuhuBaoYangReadDbScopeManager.Execute(conn => DalBaoYangRecommend.GetProductPriorityArea(conn, partName));
                result = areas?.Any(x => x.AreaId == areaid) ?? result;
            }
            catch (Exception ex)
            {
                result = true;
                logger.Error("HasArea", ex);
            }
            return result;
        }

        /// <summary>
        /// 地区配置转换
        /// </summary>
        /// <param name="areas"></param>
        /// <returns></returns>
        private async Task<List<BaoYangProductPriorityAreaView>> ConvertToAreaView(List<ProductPriorityArea> areas)
        {
            var result = new List<BaoYangProductPriorityAreaView>();
            var regionProxy = new RegionService();
            var allMinRegion = await regionProxy.GetAllMiniRegionAsync();
            var provlnces = allMinRegion.ToDictionary(k => k.RegionId, v => v.RegionName) ?? new Dictionary<int, string>();
            var areaGroup = areas.GroupBy(x => x.AreaId).ToList();
            result = areaGroup.Select(area =>
            {
                var areaView = new BaoYangProductPriorityAreaView()
                {
                    partName = area.FirstOrDefault().PartName,
                    AreaId = area.Key,
                    IsEnabled = area.FirstOrDefault().IsEnabled,
                    Details = new List<BaoYangProductPriorityAreaDetail>()
                };
                foreach (var detail in area)
                {
                    var item = new BaoYangProductPriorityAreaDetail();
                    if (provlnces.ContainsKey(detail.RegionId) && !areaView.Details.Any(x => x.RegionId == detail.RegionId))
                    {
                        item.RegionId = detail.RegionId;
                        item.RegionName = provlnces[detail.RegionId];
                        item.Citys = new List<City>();
                        areaView.Details.Add(item);
                    }
                    else
                    {
                        var region = allMinRegion.FirstOrDefault(provlnce => provlnce.ChildRegions.Any(k => k.RegionId == detail.RegionId));
                        if (region != null)
                        {
                            var city = region.ChildRegions.FirstOrDefault(c => c.RegionId == detail.RegionId) ?? new MiniRegion();
                            if (!areaView.Details.Any(x => x.RegionId == region.RegionId))
                            {
                                item.RegionId = region.RegionId;
                                item.RegionName = provlnces[region.RegionId];
                                item.Citys = new List<City>() { new City() { CityId = city.RegionId, CityName = city.RegionName } };
                                areaView.Details.Add(item);
                            }
                            else
                            {
                                areaView.Details.FirstOrDefault(x => x.RegionId == region.RegionId).Citys.Add(new City() { CityId = city.RegionId, CityName = city.RegionName });
                            }
                        }
                    }
                }
                return areaView;
            }).OrderBy(x => x.AreaId).ToList();
            return result;
        }

        /// <summary>
        /// 过滤非地区配置ID的城市配置
        /// </summary>
        /// <param name="areaId"></param>
        /// <returns></returns>
        public async Task<List<MiniRegion>> GetRegion(int areaId, string partName)
        {
            var result = new List<MiniRegion>();
            try
            {
                var regionProxy = new RegionService();
                var allMinRegion = await regionProxy.GetAllMiniRegionAsync();
                var areas = (await GetProductPriorityArea(partName)).Where(x => x.AreaId != areaId).SelectMany(x => x.Details);
                foreach (var area in areas)
                {
                    if (area.RegionId != 0 && !string.IsNullOrWhiteSpace(area.RegionName) && (area.Citys == null || !area.Citys.Any()))
                    {
                        //只有省级的时候 
                        allMinRegion.RemoveAll(x => x.RegionId == area.RegionId && string.Equals(x.RegionName, area.RegionName, StringComparison.OrdinalIgnoreCase));
                    }
                    else if (area.Citys != null && area.Citys.Any())
                    {
                        //选到了市级时
                        var regionItem = allMinRegion.FirstOrDefault(x => x.RegionId == area.RegionId && string.Equals(x.RegionName, area.RegionName, StringComparison.OrdinalIgnoreCase));
                        area.Citys.ForEach(city =>
                        {
                            var childRegions = regionItem.ChildRegions.ToList();
                            childRegions.RemoveAll(x => x.RegionId == city.CityId);
                            regionItem.ChildRegions = childRegions;
                        });
                    }
                }
                allMinRegion.ForEach(x => { x.ChildRegions = x.ChildRegions.OrderBy(o => o.PinYin).ToList(); });
                result = allMinRegion.OrderBy(x => x.PinYin).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("GetOilBaoYangProductPriority", ex); throw;
            }
            return result;
        }
        #endregion

        #region 优先级配置
        /// <summary>
        /// 获取特殊车型推荐视图
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Tuple<int, List<VehicleProductPriorityView>> GetVehicleProductPriorityView(VehicleProductPriorityRequest request)
        {
            var result = new List<VehicleProductPriorityView>();
            int total = 0;
            try
            {
                result = TuhuBaoYangReadDbScopeManager.Execute(conn => DalVehicleType.GetVehicleProductPriorityView(conn, request)) ?? new List<VehicleProductPriorityView>();
                var vehicleIds = result.Select(x => x.VehicleId)?.ToList() ?? new List<string>();
                var details = new List<ProductPriorityAreaDetail>();
                if (vehicleIds.Any())
                {
                    details = TuhuBaoYangReadDbScopeManager.Execute(conn => DalBaoYangRecommend.GetProductPriorityAreaDetail(conn, request))?.Where(x => vehicleIds.Contains(x.VehicleId)).ToList() ?? details;
                }
                switch (request.Status)
                {
                    case VehicleProductPriorityRequestStatus.Enable:
                        details.RemoveAll(x => !x.IsEnabled);
                        break;
                    case VehicleProductPriorityRequestStatus.Disable:
                        details.RemoveAll(x => x.IsEnabled);
                        break;
                    case VehicleProductPriorityRequestStatus.No:
                        result.RemoveAll(x => details.Select(y => y.VehicleId).ToArray().Contains(x.VehicleId));
                        break;
                }
                if (request.Status != VehicleProductPriorityRequestStatus.No)
                {
                    details.ForEach(detail => result.FirstOrDefault(x => string.Equals(x.VehicleId, detail.VehicleId))?.Details.Add(detail)); 
                }
                if (request.Seq > 0 || !string.IsNullOrWhiteSpace(request.ProductBrand) || !string.IsNullOrWhiteSpace(request.ProductSeries)
                   || request.Status == VehicleProductPriorityRequestStatus.Disable || request.Status == VehicleProductPriorityRequestStatus.Enable)
                {
                    result.RemoveAll(x => !x.Details.Any());
                }
                result.ForEach(x => x.IsEnabled = x.Details?.FirstOrDefault()?.IsEnabled ?? false);
                total = result.Count;
                result = result.OrderBy(x => x.Brand).ThenBy(x => x.Vehicle).Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize).ToList();
                var vehicleList = GetVehicleInfo(result?.Select(x => x.VehicleId).Distinct());
                result.ForEach(x =>
                {
                    var items = vehicleList.Where(v => string.Equals(v.ProductID, x.VehicleId)).ToList() ?? new List<DataAccess.Entity.VehicleTypeModel>();
                    x.VehicleBodyType = string.Join(",", items.Select(v => v.VehicleBodyType).ToList());
                    x.AvgPrice = items.FirstOrDefault()?.AvgPrice ?? 0;
                });
            }
            catch (Exception ex)
            {
                logger.Error("GetVehicleProductPriority", ex);
            }
            return Tuple.Create(total, result);
        }

        /// <summary>
        /// 获取车身类别和价格信息
        /// </summary>
        /// <param name="list"></param>
        private List<DataAccess.Entity.VehicleTypeModel> GetVehicleInfo(IEnumerable<string> vehicleIds)
        {
            var result = new List<DataAccess.Entity.VehicleTypeModel>();
            if (vehicleIds != null && vehicleIds.Any())
            {
                result = TuhuBaoYangReadDbScopeManager.Execute(conn => DalVehicleType.GetVehicleTypeByVehicleIds(conn, vehicleIds)) ?? new List<DataAccess.Entity.VehicleTypeModel>();
            }
            return result;
        }

        /// <summary>
        /// 保存车型地区排序视图
        /// </summary>
        /// <param name="views"></param>
        /// <param name="partName"></param>
        /// <param name="areaId"></param>
        /// <returns></returns>
        public Tuple<List<int>, string> SaveVehicleProductPriorityView(List<VehicleProductPriorityView> views, string partName, int areaId, string userName)
        {
            var result = string.Empty;
            var pkids = new List<int>();
            try
            {
                var original = GetProductPriorityAreaDetail(views.Select(x => x.VehicleId).Distinct().ToArray());
                var details = new List<ProductPriorityAreaDetail>();
                views.ForEach(x =>
                {

                    x.Details.ForEach(y =>
                    {
                        var detail = new ProductPriorityAreaDetail();
                        detail.AreaId = areaId;
                        detail.VehicleId = x.VehicleId;
                        detail.PartName = partName;
                        detail.IsEnabled = x.IsEnabled;
                        detail.Brand = y.Brand;
                        detail.Seq = y.Seq;
                        detail.Series = y.Series;
                        details.Add(detail);
                    });
                });

                if (details != null && details.Any())
                {
                    dbScopeManagerBY.CreateTransaction(conn =>
                    {
                        bool deleteResult = DalBaoYangRecommend.DeleteVehicleProductPriority(conn, areaId, details.Select(x => x.VehicleId).Distinct().ToArray(), partName);
                        details.ForEach(x =>
                        {
                            int pkid = DalBaoYangRecommend.InsertVehicleProductPriority(conn, x);
                            pkids.Add(pkid);
                        });

                    });
                    var log = new BaoYangOprLog()
                    {
                        IdentityID = $"{partName}-{areaId}",
                        LogType = "ProductPriorityAreaDetail",
                        Remarks = "修改",
                        NewValue = JsonConvert.SerializeObject(details),
                        OldValue = JsonConvert.SerializeObject(original),
                        CreateTime = DateTime.Now,
                        OperateUser = userName
                    };
                    LoggerManager.InsertLog("BYOprLog", log);
                }
            }
            catch (Exception ex)
            {
                result = "保存失败：系统出现未知错误";
                logger.Error("SaveVehicleProductPriority", ex);
            }
            return Tuple.Create(pkids, result);
        }

        /// <summary>
        /// 保存机油特殊车型配置排序
        /// </summary>
        /// <param name="list"></param>
        /// <param name="partName"></param>
        /// <returns></returns>
        public Tuple<List<int>, string> SaveVehicleOilProductPriorityView(List<VehicleOilProductPriorityView> list, int areaId, string userName)
        {
            var result = string.Empty;
            var pkids = new List<int>();
            try
            {
                var productPriorityAreaOilList = GetProductPriorityAreaOil()?.Where(x => x.AreaId == areaId) ?? new List<ProductPriorityAreaOil>();
                list.ForEach(x =>
                {
                    if (productPriorityAreaOilList.Any(oil => string.Equals(oil.Viscosity, x.Viscosity) && string.Equals(oil.Grade, x.Grade) && string.Equals(oil.VehicleId, x.VehicleId) && !oil.IsDeleted && oil.PKID != x.AreaOilId))
                    {
                        result = $"{result}{x.Vehicle}{x.Grade}{x.Viscosity}下优先级选项相同，请检查后再提交<br/>";
                    }
                    else
                    {
                        var original = new ProductPriorityAreaOil();
                        if (x.AreaOilId == 0)
                        {
                            original = productPriorityAreaOilList.FirstOrDefault(oil => string.Equals(oil.Viscosity, x.Viscosity) && string.Equals(oil.Grade, x.Grade) && string.Equals(oil.VehicleId, x.VehicleId) && !oil.IsDeleted) ?? original;
                            x.AreaOilId = original?.PKID ?? 0;
                        }
                        else
                        {
                            original = productPriorityAreaOilList.FirstOrDefault(y => y.PKID == x.AreaOilId);
                        }
                        dbScopeManagerBY.CreateTransaction(conn =>
                        {
                            int pkid = x.AreaOilId;
                            var updateResut = false;
                            if (pkid > 0)
                            {
                                updateResut = DalBaoYangRecommend.UpdateProductPriorityAreaOilByPkid(conn, new ProductPriorityAreaOil() { PKID = x.AreaOilId, AreaId = areaId, Grade = x.Grade, IsEnabled = x.IsEnabled, Viscosity = x.Viscosity, VehicleId = x.VehicleId, NewViscosity = x.NewViscosity });
                            }
                            if (updateResut)
                            {
                                DalBaoYangRecommend.DeleteProductPriorityAreaOilDetailByAreaOilId(conn, x.AreaOilId);
                            }
                            else
                            {
                                pkid = DalBaoYangRecommend.InsertProductPriorityAreaOil(conn, new ProductPriorityAreaOil() { AreaId = areaId, Grade = x.Grade, IsEnabled = x.IsEnabled, Viscosity = x.Viscosity, VehicleId = x.VehicleId, NewViscosity = x.NewViscosity });
                            }
                            if (pkid > 0)
                            {
                                pkids.Add(pkid);
                                x.Details.ForEach(detail =>
                                {
                                    DalBaoYangRecommend.InsertProductPriorityAreaOilDetail(conn, new ProductPriorityAreaOilDetail() { AreaOilId = pkid, Brand = detail.Brand, Seq = detail.Seq, Series = detail.Series, Grade = detail.Grade });
                                });
                            }
                        });
                        var newList = x.Details.Select(t => new ProductPriorityOilDetail
                        {
                            AreaId = areaId,
                            AreaOilId = x.AreaOilId,
                            Brand = t.Brand,
                            Grade = x.Grade,
                            NewViscosity = x.NewViscosity,
                            IsEnabled = x.IsEnabled,
                            ProductPriorityGrade = t.Grade,
                            Seq = t.Seq,
                            Series = t.Series,
                            VehicleId = x.VehicleId,
                            Viscosity = x.Viscosity
                        }).ToList();
                        InsertProductPrioityLog(JsonConvert.SerializeObject(original), JsonConvert.SerializeObject(newList), userName, $"{x.VehicleId}-{x.Viscosity}-{x.Grade}-{areaId}", "ProductPriority_Area_OilDetail");

                    }
                });
            }
            catch (Exception ex)
            {
                result = "保存失败：系统出现未知错误";
                logger.Error("SaveVehicleOilProductPriority", ex);
            }
            return Tuple.Create(pkids, result);
        }

        /// <summary>
        /// 获取机油特殊车型配置列表    
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Tuple<int, List<VehicleOilProductPriorityView>> GetVehicleOilProductPriorityView(OilVehicleProductPriorityRequst request)
        {
            var result = new List<VehicleOilProductPriorityView>();
            var total = 0;
            try
            {
                //获取车型信息
                var vehicles = TuhuBaoYangReadDbScopeManager.Execute(conn => DalVehicleType.GetVehicleByProductPriorityRequest(conn, request)) ?? new List<VehicleOilProductPriorityView>();
                if (vehicles != null && vehicles.Any())
                {
                    var vehicleIds = vehicles.Select(x => x.VehicleId).Distinct();
                    var details = new List<ProductPriorityOilDetail>();
                    if (vehicleIds.Any())
                    {
                        details = TuhuBaoYangReadDbScopeManager.Execute(conn => DalBaoYangRecommend.GetOilProductPriorityAreaDetail(conn, request))?.Where(x => vehicleIds.Contains(x.VehicleId)).ToList() ?? details;
                    }
                    //获取推荐配置 
                    if (details != null && details.Any())
                    {
                        var group = details.GroupBy(x => new { x.AreaOilId, x.VehicleId, x.Viscosity, x.Grade });
                        foreach (var keys in group)
                        {
                            {
                                var item = vehicles.FirstOrDefault(x => string.Equals(x.VehicleId, keys.Key.VehicleId, StringComparison.OrdinalIgnoreCase) &&
                                string.Equals(x.Viscosity, keys.Key.Viscosity, StringComparison.OrdinalIgnoreCase) &&
                                string.Equals(x.Grade, keys.Key.Grade, StringComparison.OrdinalIgnoreCase));
                                switch (request.Status)
                                {
                                    case VehicleProductPriorityRequestStatus.Enable:
                                        if (!keys.FirstOrDefault().IsEnabled)
                                        {
                                            vehicles.Remove(item);
                                        }
                                        break;
                                    case VehicleProductPriorityRequestStatus.Disable:
                                        if (keys.FirstOrDefault().IsEnabled)
                                        {
                                            vehicles.Remove(item);
                                        }
                                        break;
                                    case VehicleProductPriorityRequestStatus.No:
                                        vehicles.Remove(item);
                                        break;
                                }
                                if (item != null)
                                {
                                    item.AreaOilId = keys.Key.AreaOilId;
                                    item.NewViscosity = keys.FirstOrDefault().NewViscosity;
                                    item.IsEnabled = keys.FirstOrDefault().IsEnabled;
                                    //排除没有优先级推荐的数据
                                    item.Details.AddRange(keys.Where(x => x.Seq > 0).Select(x => new ProductPriorityAreaOilDetailView { Brand = x.Brand, Grade = x.ProductPriorityGrade, Series = x.Series, Seq = x.Seq }).ToList());
                                }
                            }
                        }
                    }

                    //过滤所有没有配置新粘度
                    if (request.IsNewViscosity)
                    {
                        vehicles.RemoveAll(x => !string.IsNullOrWhiteSpace(x.NewViscosity));
                    }
                    //分页基础数据
                    if (request.Seq > 0 || !string.IsNullOrWhiteSpace(request.ProductPriorityGrade) || !string.IsNullOrWhiteSpace(request.ProductBrand)
                        || !string.IsNullOrWhiteSpace(request.ProductSeries) || request.Status == VehicleProductPriorityRequestStatus.Disable
                        || request.Status == VehicleProductPriorityRequestStatus.Enable)
                    {
                        //如果子查询有一个非空 则以子查询结果集为分页结果集 
                        vehicles = vehicles.Where(x => x.Details.Any()).ToList();
                    }
                    if (!string.IsNullOrWhiteSpace(request.NewViscosity))
                    {
                        vehicles.RemoveAll(x => string.IsNullOrWhiteSpace(x.NewViscosity)); 
                    }
                    total = vehicles.Count();
                    result = vehicles.OrderBy(x => x.Brand).ThenBy(x => x.Vehicle).ThenBy(x => x.VehicleId).Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize).ToList();
                    //添加车型和车价信息
                    var vehicleList = GetVehicleInfo(result.Select(x => x.VehicleId).Distinct());
                    result.ForEach(x =>
                    {
                        var items = vehicleList.Where(v => string.Equals(v.ProductID, x.VehicleId)).ToList() ?? new List<DataAccess.Entity.VehicleTypeModel>();
                        x.VehicleBodyType = string.Join(",", items.Select(v => v.VehicleBodyType).ToArray());
                        x.AvgPrice = items.FirstOrDefault()?.AvgPrice ?? 0;
                    });
                }
            }
            catch (Exception ex)
            {
                logger.Error("GetVehicleOilProductPriority", ex);
            }
            return Tuple.Create(total, result);
        }

        /// <summary>
        /// 获取特殊车型机油推荐 包含配置详情
        /// </summary>
        /// <param name="pkids"></param>
        /// <returns></returns>
        private List<ProductPriorityAreaOil> GetProductPriorityAreaOil(IEnumerable<int> pkids)
        {
            var result = new List<ProductPriorityAreaOil>();
            var details = TuhuBaoYangReadDbScopeManager.Execute(conn => DalBaoYangRecommend.GetOilProductPriorityAreaDetailByPkids(conn, pkids)) ?? new List<ProductPriorityOilDetail>();
            details.GroupBy(x => new { x.AreaOilId, x.VehicleId, x.Viscosity, x.Grade }).ToList().ForEach(keys =>
            {
                var item = new ProductPriorityAreaOil();
                item.PKID = keys.Key.AreaOilId;
                item.NewViscosity = keys.FirstOrDefault().NewViscosity;
                item.IsEnabled = keys.FirstOrDefault().IsEnabled;
                item.VehicleId = keys.Key.VehicleId;
                item.Viscosity = keys.Key.Viscosity;
                item.AreaId = keys.FirstOrDefault().AreaId;
                item.Grade = keys.Key.Grade;
                item.IsDeleted = true;
                item.Details.AddRange(keys.Select(x => new ProductPriorityAreaOilDetail { Brand = x.Brand, Grade = x.ProductPriorityGrade, Series = x.Series, Seq = x.Seq }).ToList());
                result.Add(item);
            });
            return result;
        }

        private List<ProductPriorityAreaOil> GetProductPriorityAreaOil()
        {
            var result = new List<ProductPriorityAreaOil>();
            var details = TuhuBaoYangReadDbScopeManager.Execute(conn => DalBaoYangRecommend.GetOilProductPriorityAreaDetail(conn)) ?? new List<ProductPriorityOilDetail>();
            details.GroupBy(x => new { x.AreaOilId, x.VehicleId, x.Viscosity, x.Grade }).ToList().ForEach(keys =>
            {
                var item = new ProductPriorityAreaOil();
                item.PKID = keys.Key.AreaOilId;
                item.NewViscosity = keys.FirstOrDefault().NewViscosity;
                item.IsEnabled = keys.FirstOrDefault().IsEnabled;
                item.VehicleId = keys.Key.VehicleId;
                item.Viscosity = keys.Key.Viscosity;
                item.AreaId = keys.FirstOrDefault().AreaId;
                item.Grade = keys.Key.Grade;
                item.IsDeleted = true;
                item.Details.AddRange(keys.Select(x => new ProductPriorityAreaOilDetail { Brand = x.Brand, Grade = x.ProductPriorityGrade, Series = x.Series, Seq = x.Seq }).ToList());
                result.Add(item);
            });
            return result;
        }

        /// <summary>
        /// 获取特殊车型推荐
        /// </summary>
        /// <param name="pkids"></param>
        /// <returns></returns>
        private List<ProductPriorityAreaDetail> GetProductPriorityAreaDetail(IEnumerable<string> vehicleIds)
        {
            var result = new List<ProductPriorityAreaDetail>();
            var details = TuhuBaoYangReadDbScopeManager.Execute(conn => DalBaoYangRecommend.GetProductPriorityAreaByVehicleIds(conn, vehicleIds)) ?? result;
            return result;
        }

        /// <summary>
        /// 删除机油特殊车型推荐
        /// </summary>
        /// <param name="areaOilIds"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public string DeleProductPriorityAreaOilByAreaOilIds(IEnumerable<int> areaOilIds, string userName)
        {
            var result = string.Empty;
            try
            {
                var olds = GetProductPriorityAreaOil(areaOilIds);
                var deleteResult = dbScopeManagerBY.Execute(conn => DalBaoYangRecommend.DeleteProductPriorityAreaOil(conn, areaOilIds));
                if (deleteResult)
                {
                    olds.ForEach(x =>
                    {
                        var log = new BaoYangOprLog()
                        {
                            IdentityID = $"{x.VehicleId}-{x.Viscosity}-{x.Grade}-{x.AreaId}",
                            LogType = "ProductPriorityAreaOil",
                            Remarks = "删除",
                            NewValue = JsonConvert.SerializeObject(x),
                            OldValue = "[]",
                            CreateTime = DateTime.Now,
                            OperateUser = userName
                        };
                        LoggerManager.InsertLog("BYOprLog", log);
                    });
                }
            }
            catch (Exception ex)
            {
                result = "删除失败：系统出现未知错误";
                logger.Error("DeleProductPriorityAreaOil", ex);
            }
            return result;
        }

        /// <summary>
        /// 删除特殊车型推荐
        /// </summary>
        /// <param name="pkids"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public string DeleteProductPriorityAreaDetailByvehicleIds(IEnumerable<string> vehicleIds, string userName)
        {
            var result = string.Empty;
            try
            {
                var olds = GetProductPriorityAreaDetail(vehicleIds);
                var deleteResult = dbScopeManagerBY.Execute(conn => DalBaoYangRecommend.DeleteProductPriorityAreaDetail(conn, vehicleIds));
                if (deleteResult)
                {
                    olds.ForEach(x =>
                    {
                        var log = new BaoYangOprLog()
                        {
                            IdentityID = $"{x.VehicleId}-{x.PartName}-{x.AreaId}",
                            LogType = "ProductPriorityAreaDetail",
                            Remarks = "删除",
                            NewValue = JsonConvert.SerializeObject(x),
                            OldValue = "[]",
                            CreateTime = DateTime.Now,
                            OperateUser = userName
                        };
                        LoggerManager.InsertLog("BYOprLog", log);
                    });
                }
            }
            catch (Exception ex)
            {
                result = "删除失败：系统出现未知错误";
                logger.Error("DeleteProductPriorityAreaByPkids", ex);
            }
            return result;
        }

        /// <summary>
        ///  修改保养车型推荐启用状态
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="isEnabled"></param>
        /// <returns></returns>
        public bool UpdateVehicleAreaDetailEnabledByPkid(string vehicleId, bool isEnabled)
        {
            var result = false;
            try
            {
                result = dbScopeManagerBY.Execute(conn => DalBaoYangRecommend.UpdateAreaDetailEnabledBypkid(conn, vehicleId, isEnabled));
            }
            catch (Exception ex)
            {
                logger.Error("UpdateProductPriorityAreaDetailByPkid", ex);
            }
            return result;
        }

        /// <summary>
        /// 修改保养车型机油推荐状态
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="isEnabled"></param>
        /// <returns></returns>
        public bool UpdateVehicleOilAreaEnabledByPkid(int pkid, bool isEnabled)
        {
            var result = false;
            try
            {
                result = dbScopeManagerBY.Execute(conn => DalBaoYangRecommend.UpdateOilAreaEnabledBypkid(conn, pkid, isEnabled));
            }
            catch (Exception ex)
            {
                logger.Error("UpdateProductPriorityAreaDetailByPkid", ex);
            }
            return result;
        }
        #endregion
        #endregion

        /// <summary>
        /// 批量导入
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public async Task<string> BatchImportExcel(ISheet sheet, string userName)
        {
            var result = string.Empty;
            try
            {
                var list = ConvertBatchimportExcelModel(sheet);
                if (list.Any())
                {
                    var oilList = list.Where(x => string.Equals(x.PartName, "机油", StringComparison.OrdinalIgnoreCase)).ToList();
                    list.RemoveAll(x => string.Equals(x.PartName, "机油", StringComparison.OrdinalIgnoreCase));
                    var oilConfig = GetProductPriorityAreaOil();
                    var oilRequsetList = oilList.GroupBy(x => new { x.VehicleId, x.Viscosity, x.Grade, x.AreaId })
                                            .Select(x => new VehicleOilProductPriorityRequest()
                                            {
                                                AreaId = x.Key.AreaId,
                                                Views = x.Select(y => new { y.VehicleId, y.NewViscosity, y.Grade, y.Viscosity }).Distinct()
                                                .Select(y => new VehicleOilProductPriorityView()
                                                {
                                                    AreaOilId = oilConfig?.FirstOrDefault(o => string.Equals(o.VehicleId, y.VehicleId) && string.Equals(o.Viscosity, y.Viscosity)
                                                                                        && string.Equals(o.Grade, y.Grade) && o.AreaId == x.Key.AreaId)?.PKID ?? 0,
                                                    VehicleId = y.VehicleId,
                                                    NewViscosity = y.NewViscosity,
                                                    Grade = y.Grade,
                                                    IsEnabled = true,
                                                    Viscosity = y.Viscosity,
                                                    Details = new List<ProductPriorityAreaOilDetailView>()
                                                }).ToList(),
                                                Details = x.Select(y => new ProductPriorityAreaOilDetailView()
                                                {
                                                    Seq = y.Seq,
                                                    Brand = y.Brand,
                                                    Series = y.Series,
                                                    Grade = y.RecommendedGrade
                                                }).ToList()
                                            }).ToList();
                    var requestList = list.GroupBy(x => new { x.AreaId, x.PartName, x.Viscosity, x.VehicleId })
                      .Select(x => new VehicleProductPriorityRequst
                      {
                          AreaId = x.Key.AreaId,
                          partName = x.Key.PartName,
                          views = x.Select(y => y.VehicleId).Distinct().Select(y => new VehicleProductPriorityView()
                          {

                              VehicleId = y,
                              IsEnabled = true,
                              Details = new List<ProductPriorityAreaDetail>()
                          }).ToList(),
                          Details = x.Select(y => new ProductPriorityAreaDetail()
                          {

                              AreaId = y.AreaId,
                              PartName = y.PartName,
                              VehicleId = y.VehicleId,
                              Seq = y.Seq,
                              Series = y.Series,
                              Brand = y.Brand,
                              IsEnabled = true
                          }).ToList()
                      }).ToList();
                    string oilBatchResult = await BatchAddOilCheckAsync(oilRequsetList);
                    string batchResult = await BatchAddCheckAsync(requestList);
                    if (string.IsNullOrWhiteSpace(oilBatchResult) && string.IsNullOrWhiteSpace(batchResult))
                    {
                        foreach (var oil in oilRequsetList)
                        {
                            var saveResult = SaveVehicleOilProductPriorityView(oil.Views, oil.AreaId, userName);
                            if (!string.IsNullOrWhiteSpace(saveResult.Item2))
                            {
                                result = $"{result}{saveResult.Item2};";
                            }
                        }

                        foreach (var x in requestList)
                        {

                            var saveResult = SaveVehicleProductPriorityView(x.views, x.partName, x.AreaId, userName);
                            if (!string.IsNullOrWhiteSpace(saveResult.Item2))
                            {
                                result = $"{result}{saveResult};";
                            }
                        }
                    }
                    else
                    {
                        result = $"{batchResult}{oilBatchResult}";
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("BatchimportExcel", ex);

            }
            return result;
        }

        /// <summary>
        /// 批量添加机油数据验证
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Task<string> BatchAddOilCheckAsync(List<VehicleOilProductPriorityRequest> data)
        {

            Func<IEnumerable<VehicleOilProductPriorityView>, bool> func = list =>
            {
                var funcResult = false;
                list.ToList().ForEach(x =>
                {
                    var grade = 0;
                    switch (x.Grade?.Trim() ?? string.Empty)
                    {
                        case "全合成":
                            grade = 3;
                            break;
                        case "半合成":
                            grade = 2;
                            break;
                        case "矿物油":
                            grade = 1;
                            break;
                        default:
                            grade = 0;
                            break;
                    }
                    if (x.Details != null && x.Details.Any())
                    {
                        var grades = x.Details.Select(y =>
                        {
                            int leve = 0;
                            switch (y.Grade?.Trim() ?? string.Empty)
                            {
                                case "全合成":
                                    leve = 3;
                                    break;
                                case "半合成":
                                    leve = 2;
                                    break;
                                case "矿物油":
                                    leve = 1;
                                    break;
                                default:
                                    leve = 0;
                                    break;
                            }
                            return leve;
                        }).OrderBy(y => y).ToArray();
                        if (grade > grades[0])
                        {
                            funcResult = true;
                        }
                    }
                });
                return funcResult;
            };
            return Task.Run(() =>
          {
              var errorInfo = string.Empty;
              try
              {
                  foreach (var request in data)
                  {
                      if (request == null || request.Views == null || !request.Views.Any())
                      {
                          errorInfo = "参数验证失败";
                          break;
                      }

                      if (request.Views.Any(x => string.IsNullOrWhiteSpace(x.NewViscosity)) && (request.Details == null || !request.Details.Any()))
                      {
                          errorInfo = "粘度和优先级必须配置一个";
                          break;
                      }

                      if (request.Details != null && request.Details.Any())
                      {
                          if (request.Details.Any(x => string.IsNullOrWhiteSpace(x.Brand) || string.IsNullOrWhiteSpace(x.Grade) || string.IsNullOrWhiteSpace(x.Series) || x.Seq <= 0))
                          {
                              errorInfo = "品牌等级系列和优先级为空";
                              break;
                          }

                          if (request.Details.Count != request.Details.Select(y => y.Seq).Distinct().Count())
                          {
                              errorInfo = "排序存在重复";
                              break;
                          }
                          if (request.Details.Count != request.Details.Select(y => new { y.Brand, y.Series, y.Grade }).Distinct().ToArray().Length)
                          {
                              errorInfo = "品牌系列存在重复";
                              break;
                          }
                          request.Views.ForEach(x => x.Details.AddRange(request.Details));
                          if (func(request.Views))
                          {
                              errorInfo = "推荐等级必须大于原厂等级";
                              break;
                          }
                      }

                      if (request.AreaId > 0 && !HasArea(request.AreaId, "机油"))
                      {
                          errorInfo = "不存在的地区配置";
                          break;
                      }
                  }

              }
              catch (Exception ex)
              {
                  errorInfo = "程序出现异常";
                  logger.Error("BatchOilBaoYangProductPriorityAsync", ex);
              }
              return errorInfo;
          }); 
        }

        /// <summary>
        /// 批量添加数据验证
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Task<string> BatchAddCheckAsync(List<VehicleProductPriorityRequst> data)
        {
            return Task.Run(() =>
            {
                var errorInfo = string.Empty;
                try
                {
                    foreach (var request in data)
                    {
                        if (request == null || request.views == null || !request.views.Any() ||
                          request.Details == null || !request.Details.Any() || request.Details.Any(x => string.IsNullOrWhiteSpace(x.Brand) || string.IsNullOrWhiteSpace(x.Series)) ||
                          string.IsNullOrWhiteSpace(request.partName))
                        {
                            errorInfo = "参数验证失败";
                            break;
                        }
                        if (request.Details.Count != request.Details.Select(y => y.Seq).Distinct().Count())
                        {
                            errorInfo = $"排序存在重复";
                            break;

                        }
                        if (request.Details.Count != request.Details.Select(y => new { y.Brand, y.Series }).Distinct().ToArray().Length)
                        {
                            errorInfo = $"品牌系列存在重复";
                            break;
                        }
                        if (request.AreaId > 0 && !HasArea(request.AreaId, request.partName.Trim()))
                        {
                            errorInfo = $"{request.partName}-地区模板编号：{request.AreaId}不存在的地区配置";
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    errorInfo = "程序出现异常";
                    logger.Error("BatchAddCheckAsync", ex);
                }
                return errorInfo;
            });
        }

        /// <summary>
        /// 实体转换
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        private List<BatchimportExcelModel> ConvertBatchimportExcelModel(ISheet sheet)
        {
            var result = new List<BatchimportExcelModel>();
            Func<ICell, string> getStringValue = cell =>
            {
                if (cell != null)
                {
                    if (cell.CellType == CellType.Numeric)
                    {
                        return cell.NumericCellValue.ToString();
                    }
                    return cell.StringCellValue.Trim();
                }
                return string.Empty;
            };
            var titleRow = sheet.GetRow(sheet.FirstRowNum);
            var index = titleRow.FirstCellNum;
            if (titleRow.GetCell(index++)?.StringCellValue.Trim() == "车型ID" && titleRow.GetCell(index++)?.StringCellValue.Trim() == "地区ID" &&
               titleRow.GetCell(index++)?.StringCellValue.Trim() == "零件名称" && titleRow.GetCell(index++)?.StringCellValue.Trim() == "优先级" &&
               titleRow.GetCell(index++)?.StringCellValue.Trim() == "品牌" && titleRow.GetCell(index++)?.StringCellValue.Trim() == "系列" &&
               titleRow.GetCell(index++)?.StringCellValue.Trim() == "新粘度" && titleRow.GetCell(index++)?.StringCellValue.Trim() == "原厂粘度" &&
               titleRow.GetCell(index++)?.StringCellValue.Trim() == "原厂等级" && titleRow.GetCell(index)?.StringCellValue.Trim() == "推荐等级")
            {
                for (var rowIndex = sheet.FirstRowNum + 1; rowIndex <= sheet.LastRowNum; rowIndex++)
                {
                    var row = sheet.GetRow(rowIndex);
                    if (row == null) continue;
                    var cellIndex = row.FirstCellNum;
                    var item = new BatchimportExcelModel();
                    item.VehicleId = getStringValue(row.GetCell(cellIndex++));
                    if (string.IsNullOrWhiteSpace(item.VehicleId)) continue;
                    int regionId = 0;
                    int.TryParse(getStringValue(row.GetCell(cellIndex++)), out regionId);
                    item.AreaId = regionId;
                    item.PartName = getStringValue(row.GetCell(cellIndex++));
                    int seq = 0;
                    int.TryParse(getStringValue(row.GetCell(cellIndex++)), out seq);
                    item.Seq = seq;
                    item.Brand = getStringValue(row.GetCell(cellIndex++));
                    item.Series = getStringValue(row.GetCell(cellIndex++));
                    if (string.Equals(item.PartName, "机油", StringComparison.OrdinalIgnoreCase))
                    {
                        item.NewViscosity = getStringValue(row.GetCell(cellIndex++));
                        item.Viscosity = getStringValue(row.GetCell(cellIndex++));
                        item.Grade = getStringValue(row.GetCell(cellIndex++));
                        item.RecommendedGrade = getStringValue(row.GetCell(cellIndex++));
                    }
                    result.Add(item);
                }
                return result;

            }
            return result;
        }
    }
}
