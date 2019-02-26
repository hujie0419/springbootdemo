using Common.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.Shop.Models.Request;

namespace Tuhu.Provisioning.Business.PaintDiscountConfig
{
    public class PaintDiscountConfigManager
    {
        private static readonly Common.Logging.ILog Logger;
        private static readonly IConnectionManager configurationConn =
     new ConnectionManager(ConfigurationManager.ConnectionStrings["Configuration"].ConnectionString);
        private static readonly IConnectionManager configurationReadConn =
      new ConnectionManager(ConfigurationManager.ConnectionStrings["Configuration_AlwaysOnRead"].ConnectionString);
        private static readonly IConnectionManager productcatalogReadConn =
      new ConnectionManager(ConfigurationManager.ConnectionStrings["Tuhu_productcatalog_AlwaysOnRead"].ConnectionString);
        private static readonly IConnectionManager tuhuLogConn =
     new ConnectionManager(ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString);
        private static readonly IDBScopeManager dbScopeManagerConfigurationRead;
        private static readonly IDBScopeManager dbScopeManagerConfiguration;
        private static readonly IDBScopeManager dbScopeManagerProductcatalogRead;
        private static readonly IDBScopeManager dbScopeManagerTuhulog;

        static PaintDiscountConfigManager()
        {
            Logger = LogManager.GetLogger(typeof(PaintDiscountConfigManager));
            dbScopeManagerConfiguration = new DBScopeManager(configurationConn);
            dbScopeManagerConfigurationRead = new DBScopeManager(configurationReadConn);
            dbScopeManagerProductcatalogRead = new DBScopeManager(productcatalogReadConn);
            dbScopeManagerTuhulog = new DBScopeManager(tuhuLogConn);
        }

        #region 喷漆打折价格体系配置

        /// <summary>
        /// 添加喷漆价格体系配置
        /// </summary>
        /// <param name="model"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public int AddPaintDiscountPackage(PaintDiscountPackageModel model, string user)
        {
            var pkid = 0;
            try
            {
                var oldValue = dbScopeManagerConfigurationRead.Execute(conn =>
                    DalPaintDiscountConfig.GetPaintDiscountPackage(conn, model.PackageName));
                if (oldValue == null)
                {
                    model.CreateDateTime = DateTime.Now;
                    pkid = dbScopeManagerConfiguration.Execute
                        (conn => DalPaintDiscountConfig.AddPaintDiscountPackage(conn, model));
                    model.PKID = pkid;
                }
                else if (oldValue.IsDeleted)
                {
                    model.PKID = oldValue.PKID;
                    model.CreateDateTime = oldValue.CreateDateTime;
                    var updateResult = dbScopeManagerConfiguration.Execute
                        (conn => DalPaintDiscountConfig.UpdatePaintDiscountPackage(conn, model));
                    if (updateResult)
                    {
                        pkid = oldValue.PKID;
                    }
                }
                model.LastUpdateDateTime = DateTime.Now;
                var log = new PaintDiscountOprLogModel
                {
                    LogType = "PaintDiscountPackage",
                    IdentityId = model.PackageName,
                    OperationType = "Add",
                    OldValue = null,
                    NewValue = JsonConvert.SerializeObject(model),
                    Remarks = $"添加价格体系:{model.PackageName}的配置",
                    Operator = user,
                };
                LoggerManager.InsertLog("PaintDiscountOprLog", log);
            }
            catch (Exception ex)
            {
                Logger.Error("AddPaintDiscountPackage", ex);
            }
            return pkid;
        }

        /// <summary>
        /// 删除喷漆打折价格体系配置
        /// </summary>
        /// <param name="model"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool DeletePaintDiscountPackage(PaintDiscountPackageModel model, string user)
        {
            var result = false;
            try
            {
                var oldValue = dbScopeManagerConfigurationRead.Execute(conn =>
                 DalPaintDiscountConfig.GetPaintDiscountPackage(conn, model.PKID));
                var logs = new List<PaintDiscountOprLogModel>();
                if (oldValue != null && !oldValue.IsDeleted)
                {
                    var packageRegions = GetPackageRegionByPackageId(oldValue.PKID)
                        ?? new List<PaintDiscountPackageRegionModel>();//城市配置
                    var discountDetails = dbScopeManagerConfigurationRead.Execute(conn =>
                       DalPaintDiscountConfig.GetPaintDiscountDetailByPackageId(conn, oldValue.PKID))
                       ?? new List<PaintDiscountConfigModel>();//打折详情配置
                    dbScopeManagerConfiguration.CreateTransaction(conn =>
                    {
                        var delResult = DalPaintDiscountConfig.DeletePaintDiscountPackage(conn, oldValue.PKID);
                        if (!delResult)
                        {
                            throw new Exception($"DeletePaintDiscountPackage失败," +
                                $"待删除数据：{JsonConvert.SerializeObject(oldValue)}");
                        }
                        logs.Add(new PaintDiscountOprLogModel
                        {
                            LogType = "PaintDiscountPackage",
                            IdentityId = model.PackageName,
                            OperationType = "Delete",
                            OldValue = JsonConvert.SerializeObject(oldValue),
                            NewValue = null,
                            Remarks = $"删除价格体系：{model.PackageName}的配置",
                            Operator = user,
                        });
                        foreach (var packageRegion in packageRegions)
                        {
                            var delRegion = DalPaintDiscountConfig.DeletePaintDiscountPackageRegion(conn, packageRegion.PKID);
                            if (!delRegion)
                            {
                                throw new Exception($"DeletePaintDiscountPackageRegion失败," +
                                    $"待删除数据：{JsonConvert.SerializeObject(packageRegion)}");
                            }
                            logs.Add(new PaintDiscountOprLogModel()
                            {
                                LogType = "PaintDiscountPackageRegion",
                                IdentityId = $"{packageRegion.PackageId}_{packageRegion.RegionId}_{packageRegion.ShopId}",
                                OperationType = "Delete",
                                OldValue = JsonConvert.SerializeObject(packageRegion),
                                NewValue = null,
                                Remarks = $"删除城市：{packageRegion.RegionId},门店：{packageRegion.ShopId}的配置",
                                Operator=user
                            });
                        }
                        foreach (var discountDetail in discountDetails)
                        {
                            var delDiscount = DalPaintDiscountConfig.DeletePaintDiscountConfig(conn, discountDetail.PKID);
                            if (!delDiscount)
                            {
                                throw new Exception($"DeletePaintDiscountConfig失败," +
                                    $"待删除数据：{JsonConvert.SerializeObject(discountDetail)}");
                            }
                            logs.Add(new PaintDiscountOprLogModel()
                            {
                                LogType = "PaintDiscountConfig",
                                IdentityId = $"{discountDetail.PackageId}_{discountDetail.ServicePid}_{discountDetail.SurfaceCount}",
                                OperationType = "Delete",
                                OldValue = JsonConvert.SerializeObject(discountDetail),
                                NewValue = null,
                                Remarks = $"删除服务Pid:{discountDetail.ServicePid},面数:{discountDetail.SurfaceCount}的配置",
                                Operator = user,
                            });
                        }
                        result = true;
                    });
                }
                else
                {
                    result = true;
                }
                if (result && logs.Any())
                {
                    logs.ForEach(log => LoggerManager.InsertLog("PaintDiscountOprLog", log));
                }
            }
            catch (Exception ex)
            {
                result = false;
                Logger.Error("DeletePaintDiscountPackage", ex);
            }
            return result;
        }

        /// <summary>
        /// 更新喷漆价格体系配置
        /// </summary>
        /// <param name="model"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool UpdatePaintDiscountPackage(PaintDiscountPackageModel model, string user)
        {
            var result = false;
            try
            {
                var oldValue = dbScopeManagerConfigurationRead.Execute(conn =>
                DalPaintDiscountConfig.GetPaintDiscountPackage(conn,model.PKID));
                if (oldValue != null && !oldValue.IsDeleted)
                {
                    model.CreateDateTime = oldValue.CreateDateTime;
                    model.LastUpdateDateTime = DateTime.Now;
                    result = dbScopeManagerConfiguration.Execute(conn =>
                    DalPaintDiscountConfig.UpdatePaintDiscountPackage(conn, model));
                    if (!result)
                    {
                        throw new Exception($"UpdatePaintDiscountPackage失败," +
                            $"待更新数据：{JsonConvert.SerializeObject(model)}");
                    }
                    var log = new PaintDiscountOprLogModel
                    {
                        LogType = "PaintDiscountPackage",
                        IdentityId = model.PackageName,
                        OperationType = "Update",
                        OldValue = JsonConvert.SerializeObject(oldValue),
                        NewValue = JsonConvert.SerializeObject(model),
                        Remarks = $"更新价格体系:{model.PackageName}的配置",
                        Operator = user,
                    };
                    LoggerManager.InsertLog("PaintDiscountOprLog", log);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("UpdatePaintDiscountPackage", ex);
            }
            return result;
        }

        /// <summary>
        /// 查询喷漆价格体系配置
        /// </summary>
        /// <param name="packageName"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Tuple<List<PaintDiscountPackageViewModel>, int> SelectPaintDiscountPackageForView
        (int packageId, int userType, int pageIndex, int pageSize)
        {
            var result = new List<PaintDiscountPackageViewModel>();
            var totalCount = 0;
            try
            {
                var configs = dbScopeManagerConfigurationRead.Execute(conn =>
                DalPaintDiscountConfig.SelectPaintDiscountPackage
                   (conn, packageId, userType, pageIndex, pageSize, out totalCount))
                   ?? new List<PaintDiscountPackageModel>();
                if (configs != null && configs.Any())
                {
                    foreach (var package in configs)
                    {
                        var regionShops = GetPackageRegionForView(package.PKID);
                        result.Add(new PaintDiscountPackageViewModel()
                        {
                            PKID = package.PKID,
                            PackageName = package.PackageName,
                            UserType = package.UserType,
                            IsEnabled = package.IsEnabled,
                            IsDeleted = package.IsDeleted,
                            CreateDateTime = package.CreateDateTime,
                            LastUpdateDateTime = package.LastUpdateDateTime,
                            RegionShops = regionShops
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Logger.Error("SelectPaintDiscountPackage", ex);
            }
            return Tuple.Create(result, totalCount);
        }

        /// <summary>
        /// 获取喷漆价格体系配置
        /// </summary>
        /// <param name="packageId"></param>
        /// <returns></returns>
        public PaintDiscountPackageModel GetPaintDiscountPackage(int packageId)
        {
            var result = null as PaintDiscountPackageModel;
            try
            {
                var config = dbScopeManagerConfigurationRead.Execute(conn =>
                  DalPaintDiscountConfig.GetPaintDiscountPackage(conn, packageId));
                result = (config != null && !config.IsDeleted) ? config : null;
            }
            catch (Exception ex)
            {
                Logger.Error("GetPaintDiscountPackage", ex);
            }
            return result;
        }

        /// <summary>
        /// 是否存在重复的喷漆打折价格体系配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool IsExistPaintDiscountPackage(PaintDiscountPackageModel model)
        {
            var result = true;
            try
            {
                result = dbScopeManagerConfigurationRead.Execute(conn =>
                  DalPaintDiscountConfig.IsExistPaintDiscountPackage(conn, model));
            }
            catch (Exception ex)
            {
                Logger.Error("IsExistPaintDiscountPackage", ex);
            }
            return result;
        }

        /// <summary>
        /// 获取所有喷漆价格体系配置
        /// </summary>
        /// <returns></returns>
        public List<PaintDiscountServiceModel> GetAllPaintDiscountPackage()
        {
            var result = null as List<PaintDiscountServiceModel>;
            try
            {
                result = dbScopeManagerConfigurationRead.Execute(conn =>
                  DalPaintDiscountConfig.GetAllPaintDiscountPackage(conn));
            }
            catch (Exception ex)
            {
                Logger.Error("GetAllPaintDiscountPackage", ex);
            }
            return result;
        }

        #endregion

        #region 喷漆打折城市配置

        /// <summary>
        /// 更新喷漆价格体系城市门店配置
        /// </summary>
        /// <param name="packageId"></param>
        /// <param name="userType"></param>
        /// <param name="regionShops"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool UpsertPackageRegion
            (int packageId, int userType, List<PaintDiscountPackageRegionModel> packageRegions, string user)
        {
            var result = false;
            try
            {
                var existConfigs = dbScopeManagerConfigurationRead.Execute(conn =>
                         DalPaintDiscountConfig.GetPackageRegionByPackageId(conn, packageId));
                var insertList = new List<PaintDiscountPackageRegionModel>();
                var updateList = new List<PaintDiscountPackageRegionModel>();
                var logs = new List<PaintDiscountOprLogModel>();
                packageRegions?.ForEach(s =>
                {
                    var existConfig = existConfigs?.FirstOrDefault(e =>
                     e.PackageId == s.PackageId
                     && e.RegionId == s.RegionId
                     && e.ShopId == s.ShopId);
                    if (existConfig == null)
                    {
                        insertList.Add(s);
                    }
                    else if (existConfig.IsDeleted)
                    {
                        s.PKID = existConfig.PKID;
                        s.CreateDateTime = existConfig.CreateDateTime;
                        s.LastUpdateDateTime = DateTime.Now;
                        updateList.Add(s);
                    }
                });

                var deleteList = existConfigs?.Where(s =>
                 !s.IsDeleted &&
                 !packageRegions.Any(p => p.PackageId == s.PackageId &&
                 p.RegionId == s.RegionId &&
                 p.ShopId == s.ShopId))?.ToList() ?? new List<PaintDiscountPackageRegionModel>();

                dbScopeManagerConfiguration.CreateTransaction(conn =>
                {
                    foreach (var insertModel in insertList)
                    {
                        var pkid = DalPaintDiscountConfig.AddPaintDiscountPackageRegion(conn, insertModel);
                        if (pkid <1)
                        {
                            throw new Exception($"AddPaintDiscountPackageRegion失败," +
                                 $"数据：{JsonConvert.SerializeObject(insertModel)}");
                        }
                        insertModel.PKID = pkid;
                        insertModel.CreateDateTime = DateTime.Now;
                        insertModel.LastUpdateDateTime = DateTime.Now;
                        logs.Add(new PaintDiscountOprLogModel()
                        {
                            LogType = "PaintDiscountPackageRegion",
                            IdentityId = $"{insertModel.PackageId}_{insertModel.RegionId}_{insertModel.ShopId}",
                            OperationType = "Add",
                            OldValue = null,
                            NewValue = JsonConvert.SerializeObject(insertModel),
                            Operator = $"新增喷漆价格体系：{insertModel.PackageId}下城市：" +
                            $"{insertModel.RegionId},门店：{insertModel.ShopId}"
                        });
                    }
                    foreach (var updateModel in updateList)
                    {
                        var isUpdated = DalPaintDiscountConfig.UpdatePaintDiscountPackageRegion(conn, updateModel);
                        if (!isUpdated)
                        {
                            throw new Exception($"AddPaintDiscountPackageRegion失败," +
                                $"数据：{JsonConvert.SerializeObject(updateModel)}");
                        }
                        logs.Add(new PaintDiscountOprLogModel()
                        {
                            LogType = "PaintDiscountPackageRegion",
                            IdentityId = $"{updateModel.PackageId}_{updateModel.RegionId}_{updateModel.ShopId}",
                            OperationType = "Add",
                            OldValue = null,
                            NewValue = JsonConvert.SerializeObject(updateModel),
                            Operator = $"新增喷漆价格体系：{updateModel.PackageId}下城市：" +
                            $"{updateModel.RegionId},门店：{updateModel.ShopId}"
                        });
                    }
                    foreach (var deleteModel in deleteList)
                    {
                        var isDeleted = DalPaintDiscountConfig.DeletePaintDiscountPackageRegion(conn, deleteModel.PKID);
                        if (!isDeleted)
                        {
                            throw new Exception($"DeletePaintDiscountPackageRegion失败," +
                                $"数据：{JsonConvert.SerializeObject(deleteModel)}");
                        }
                        logs.Add(new PaintDiscountOprLogModel()
                        {
                            LogType = "PaintDiscountPackageRegion",
                            IdentityId = $"{deleteModel.PackageId}_{deleteModel.RegionId}_{deleteModel.ShopId}",
                            OperationType = "Delete",
                            OldValue = JsonConvert.SerializeObject(deleteModel),
                            NewValue = null,
                            Operator = $"删除喷漆价格体系：{deleteModel.PackageId}下城市：" +
                            $"{deleteModel.RegionId},门店：{deleteModel.ShopId}"
                        });
                    }
                    result = true;
                    foreach (var log in logs)
                    {
                        LoggerManager.InsertLog("PaintDiscountOprLog", log);
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.Error("EditPackageRegion", ex);
            }
            return result;
        }

        /// <summary>
        /// 获取所有二级城市
        /// </summary>
        /// <returns></returns>
        public List<Service.Shop.Models.Region.MiniRegion> GetAllRegion()
        {
            using (var client = new Service.Shop.RegionClient())
            {
                var serviceResult = client.GetAllMiniRegion();
                var regions = serviceResult.Result;
                if (regions != null && regions.Any())
                {
                    var directRegion = new int[4] { 1, 2, 19, 20 };
                    regions?.ToList().ForEach(s =>
                    {
                        if (directRegion.Contains(s.RegionId))
                        {
                            s.ChildRegions = null;
                        }
                        else
                        {
                            s.ChildRegions?.ToList().ForEach(v => v.ChildRegions = null);
                        }
                    });
                }
                return regions?.ToList() ?? new List<Service.Shop.Models.Region.MiniRegion>();
            }
        }

        /// <summary>
        /// 根据门店Id获取门店信息
        /// </summary>
        /// <param name="shopIds"></param>
        /// <returns></returns>
        private List<PaintDiscoutPackageShop> SelectSimpleShops(List<int> shopIds)
        {
            var reuslt = new List<PaintDiscoutPackageShop>();
            if (shopIds != null && shopIds.Any())
            {
                using (var client = new Service.Shop.ShopClient())
                {
                    for (var i = 1; i <= Math.Ceiling(shopIds.Count() * 1.0 / 200); i++)
                    {
                        var clientResult = client.SelectShops(shopIds.Skip((i - 1) * 200).Take(200));
                        clientResult.ThrowIfException(true);
                        var batchShops = clientResult.Result?.Select(s => new PaintDiscoutPackageShop()
                        {
                            ShopId = s.ShopId,
                            CityId = s.CityId,
                            ShopName = s.CarparName
                        })?.ToList() ?? new List<PaintDiscoutPackageShop>();
                        reuslt.AddRange(batchShops);
                    }
                }
            }
            return reuslt;
        }

        /// <summary>
        /// 获取该价格体系信息以及城市门店信息
        /// </summary>
        /// <param name="packageId"></param>
        /// <returns></returns>
        public List<PaintDiscountPackageRegionViewModel> GetPackageRegionForView(int packageId)
        {
            var result = null as List<PaintDiscountPackageRegionViewModel>;
            try
            {
                var configs = GetPackageRegionByPackageId(packageId);
                result = ConvertToView(configs);
            }
            catch (Exception ex)
            {
                Logger.Error("GetPackageRegionForView", ex);
            }
            return result;
        }

        /// <summary>
        /// 获取该价格体系下有效城市与门店配置
        /// </summary>
        /// <param name="packageId"></param>
        /// <returns></returns>
        private List<PaintDiscountPackageRegionModel> GetPackageRegionByPackageId(int packageId)
        {
            var allConfigs = dbScopeManagerConfigurationRead.Execute(conn =>
             DalPaintDiscountConfig.GetPackageRegionByPackageId(conn, packageId));
            return allConfigs?.Where(s => !s.IsDeleted)?.ToList();
        }

        /// <summary>
        /// 喷漆打折城市配置DbModel转换为ViewModel
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        private List<PaintDiscountPackageRegionViewModel> ConvertToView(List<PaintDiscountPackageRegionModel> models)
        {
            var result = new List<PaintDiscountPackageRegionViewModel>();
            var regions = GetAllRegion();
            var groups = models?.GroupBy(s => new { s.RegionId }).ToDictionary(s => s.Key, s => s.ToList());
            if (groups != null && groups.Any() && regions.Any())
            {
                var directRegion = new int[4] { 1, 2, 19, 20 };
                groups?.ToList().ForEach(s =>
                {
                    if (s.Key.RegionId == 0)//全国
                    {
                        result.Add(new PaintDiscountPackageRegionViewModel()
                        {
                            ProvinceId = s.Key.RegionId,
                            ProvinceName = "全国",
                            CityName = "全国",
                            CityId = 0,
                            Shops = null
                        });
                    }
                    else if (directRegion.Contains(s.Key.RegionId))//直辖市
                    {
                        var region = regions.Where(r => r.RegionId == s.Key.RegionId)?.FirstOrDefault();
                        if (region != null)
                        {
                            var model = new PaintDiscountPackageRegionViewModel()
                            {
                                ProvinceId = region.RegionId,
                                ProvinceName = region.RegionName,
                                CityId = s.Key.RegionId,
                                CityName = region.RegionName
                            };
                            var shopIds = s.Value.Where(w => w.ShopId.HasValue).Select(ss => ss.ShopId.Value).ToList();
                            if (shopIds != null && shopIds.Any())
                            {
                                model.Shops = SelectSimpleShops(shopIds);
                            }
                            result.Add(model);
                        }
                    }
                    else//二级城市
                    {
                        var region = regions.FirstOrDefault(w => w.ChildRegions != null
                        && w.ChildRegions.Any(c => c.RegionId == s.Key.RegionId));
                        if (region != null && region.ChildRegions != null)
                        {
                            var child = region?.ChildRegions?.FirstOrDefault(w => w.RegionId == s.Key.RegionId);
                            if (child != null)
                            {
                                var model = new PaintDiscountPackageRegionViewModel()
                                {
                                    ProvinceId = region.RegionId,
                                    ProvinceName = region.RegionName,
                                    CityId = child.RegionId,
                                    CityName = child.RegionName
                                };
                                var shopIds = s.Value.Where(w => w.ShopId.HasValue).Select(ss => ss.ShopId.Value).ToList();
                                if (shopIds != null && shopIds.Any())
                                {
                                    model.Shops = SelectSimpleShops(shopIds);
                                }
                                result.Add(model);
                            }
                        }
                    }
                });
            }
            else
            {
                result = null;
            }
            return result;
        }

        /// <summary>
        /// 获取该地区下所有的喷漆门店
        /// </summary>
        /// <param name="regionId"></param>
        /// <returns></returns>
        public List<PaintDiscoutPackageShop> GetAllPaintShopsByRegionId(int regionId)
        {
            var result = null as List<PaintDiscoutPackageShop>;
            try
            {
                var shopIds = GetAllPaintShopIdsByRegionId(regionId);
                result = SelectSimpleShops(shopIds);
            }
            catch (Exception ex)
            {
                result = null;
                Logger.Error("GetAllPaintShopsByRegionId", ex);
            }
            return result ?? new List<PaintDiscoutPackageShop>();
        }

        /// <summary>
        /// 获取该地区下的喷漆门店Id
        /// </summary>
        /// <param name="regionId"></param>
        /// <returns></returns>
        private List<int> GetAllPaintShopIdsByRegionId(int regionId)
        {
            var paintServices = GetAllPaintServices();
            var pageIndex = 1;
            var pageSize = 200;
            var totalPage = 0;
            var result = new List<int>();
            Func<int> searchShop = () =>
            {
                using (var client = new Service.Shop.ShopClient())
                {
                    var request = new ShopSearchRequest()
                    {
                        BelongRegionId = regionId,
                        PageIndex = pageIndex,
                        PageSize = pageSize,
                        Filters = new List<ShopQueryFilterModel>()
                    {
                        new ShopQueryFilterModel()
                        {
                        FilterValueType="ServiceId",
                        JoinType="OR",
                        Values= paintServices?.Select(s=>s.ServiceId)?.ToList()
                        }
                    }
                    };
                    var clientResult = client.SearchShopIds(request);
                    clientResult.ThrowIfException(true);
                    if (clientResult.Result?.Source != null && clientResult.Result.Source.Any())
                    {
                        result.AddRange(clientResult.Result.Source);
                    }
                    return clientResult.Result?.Pager?.TotalPage ?? 0;
                }
            };
            var serviceIds = paintServices?.Select(s => s.ServiceId)?.ToList();
            if (regionId > 0 && serviceIds != null && serviceIds.Any())
            {
                totalPage = searchShop();
                while (totalPage > 0 && pageIndex > 0 && pageIndex < totalPage)
                {
                    pageIndex++;
                    searchShop();
                }
            }
            return result.Distinct().ToList();
        }

        /// <summary>
        /// 获取所有喷漆服务
        /// </summary>
        /// <returns></returns>
        private List<Service.Paint.Model.PaintServiceProductModel> GetAllPaintServices()
        {
            using (var client = new Service.Shop.ShopClient())
            {
                var serviceResult = client.SelectAllServices();
                serviceResult.ThrowIfException();
                return serviceResult.Result.Where(o => string.Equals(o.ServersType, "Paint"))
                            .Select(o => new Service.Paint.Model.PaintServiceProductModel()
                            {
                                ServiceId = o.ServiceId,
                                ServiceName = o.ServersName,
                                Price = o.DefaultPrice
                            }).ToList();
            }
        }

        /// <summary>
        /// 转换成PackageRegionModel
        /// </summary>
        /// <param name="packageId"></param>
        /// <param name="models"></param>
        /// <returns></returns>
        public List<PaintDiscountPackageRegionModel> ConvertToPackageRegionModel
            (int packageId, List<RegionShopPairModel> regionShopPair)
        {
            var packageRegions = new List<PaintDiscountPackageRegionModel>();
            try
            {
                if (regionShopPair != null && regionShopPair.Any())
                {
                    regionShopPair.ForEach(s =>
                    {
                        if (s.ShopIds != null && s.ShopIds.Any())
                        {
                            packageRegions.AddRange(s.ShopIds.Select(v => new PaintDiscountPackageRegionModel
                            {
                                PackageId = packageId,
                                RegionId = s.RegionId,
                                ShopId = v
                            }));
                        }
                        else
                        {
                            packageRegions.Add(new PaintDiscountPackageRegionModel()
                            {
                                PackageId = packageId,
                                RegionId = s.RegionId,
                                ShopId = null
                            });
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                packageRegions = null;
                Logger.Error("ConvertToPackageRegionModel", ex);
            }
            return packageRegions??new List<PaintDiscountPackageRegionModel>();
        }

        /// <summary>
        /// 城市、是否限制新用户、门店 是否重复
        /// </summary>
        /// <param name="packageId"></param>
        /// <param name="userType"></param>
        /// <param name="models"></param>
        /// <returns></returns>
        public List<string> GetRepeatPackageRegion
            (int packageId, int userType, List<PaintDiscountPackageRegionModel> models)
        {
            var result = null as IEnumerable<string>;
            var existModels = new List<PaintDiscountPackageRegionModel>();
            try
            {
                foreach (var model in models)
                {
                    var userTypes = new List<int>();
                    switch (userType)
                    {
                        case (int)UserType.NewUser:
                            userTypes = new List<int>() { (int)UserType.NewUser, (int)UserType.AllUser };
                            break;
                        case (int)UserType.OldUser:
                            userTypes = new List<int>() { (int)UserType.OldUser, (int)UserType.AllUser };
                            break;
                        case (int)UserType.AllUser:
                            userTypes = new List<int>() { (int)UserType.NewUser, (int)UserType.OldUser, (int)UserType.AllUser };
                            break;
                        default: break;
                    }
                    var isExist = dbScopeManagerConfigurationRead.Execute(conn =>
                        DalPaintDiscountConfig.IsExistPackageRegion(conn,
                         model, userTypes));
                    if (isExist)
                    {
                        existModels.Add(model);
                    }
                }
                var viewModels = ConvertToView(existModels);
                var userTypeStr = userType == (int)UserType.NewUser ? "新用户"
                        : userType == (int)UserType.OldUser ? "老用户"
                        : userType == (int)UserType.AllUser ? "全部用户" : string.Empty;
                result = viewModels?.Select(s => $"{s.ProvinceName}-{s.CityName}-{userTypeStr}--" +
                $"{string.Join(",", s.Shops?.Select(ss => ss.ShopName)?.ToList() ?? new List<string>() { "全部门店" })}" +
                $" 已存在,无法重复添加");
            }
            catch (Exception ex)
            {
                Logger.Error("GetRepeatPackageRegion", ex);
            }
            return result?.ToList();
        }
        #endregion

        #region 喷漆打折详情配置

        /// <summary>
        /// 添加喷漆打折配置
        /// </summary>
        /// <param name="model"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool AddPaintDiscountConfig(PaintDiscountConfigModel model, string user)
        {
            var result = false;
            try
            {
                var oldValue = GetPaintDiscountConfig(model.PackageId, model.ServicePid, model.SurfaceCount);
                if (oldValue == null)
                {
                    var pkid = dbScopeManagerConfiguration.Execute
                        (conn => DalPaintDiscountConfig.AddPaintDiscountConfig(conn, model));
                    result = pkid > 0;
                    model.PKID = pkid;
                }
                else if (oldValue.IsDeleted)
                {
                    model.PKID = oldValue.PKID;
                    result = dbScopeManagerConfiguration.Execute
                        (conn => DalPaintDiscountConfig.UpdatePaintDiscountConfig(conn, model));
                }
                model.CreateDateTime = DateTime.Now;
                model.LastUpdateDateTime = DateTime.Now;
                var log = new PaintDiscountOprLogModel
                {
                    LogType = "PaintDiscountConfig",
                    IdentityId = $"{model.PackageId}_{model.ServicePid}_{model.SurfaceCount}",
                    OperationType = "Add",
                    OldValue = null,
                    NewValue = JsonConvert.SerializeObject(model),
                    Remarks = $"添加服务Pid:{model.ServicePid},面数:{model.SurfaceCount}的配置",
                    Operator = user,
                };
                LoggerManager.InsertLog("PaintDiscountOprLog", log);
            }
            catch (Exception ex)
            {
                result = false;
                Logger.Error("AddPaintDiscountConfig", ex);
            }
            return result;
        }

        /// <summary>
        /// 删除喷漆打折配置
        /// </summary>
        /// <param name="carNoPrefix"></param>
        /// <param name="surfaceCount"></param>
        /// <param name="servicePid"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool DeletePaintDiscountConfig
            (int packageId, string servicePid, int surfaceCount, string user)
        {
            var result = false;
            try
            {
                var oldValue = GetPaintDiscountConfig(packageId, servicePid, surfaceCount);
                if (oldValue != null && !oldValue.IsDeleted)
                {
                    result = dbScopeManagerConfiguration.Execute(conn => 
                    DalPaintDiscountConfig.DeletePaintDiscountConfig(conn, oldValue.PKID));
                    if (result)
                    {
                        var log = new PaintDiscountOprLogModel
                        {
                            LogType = "PaintDiscountConfig",
                            IdentityId = $"{packageId}_{servicePid}_{surfaceCount}",
                            OperationType = "Delete",
                            OldValue = JsonConvert.SerializeObject(oldValue),
                            NewValue = null,
                            Remarks = $"删除服务Pid:{oldValue.ServicePid},面数:{oldValue.SurfaceCount}的配置",
                            Operator = user,
                        };
                        LoggerManager.InsertLog("PaintDiscountOprLog", log);
                    }
                }
                else
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Logger.Error("DeletePaintDiscountConfig", ex);
            }
            return result;
        }

        /// <summary>
        /// 更新喷漆打折配置
        /// </summary>
        /// <param name="model"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool UpdatePaintDiscountConfig
            (PaintDiscountConfigModel model, string user)
        {
            var result = false;
            try
            {
                var oldValue = GetPaintDiscountConfig(model.PackageId, model.ServicePid, model.SurfaceCount);
                if (oldValue != null)
                {
                    model.PKID = oldValue.PKID;
                    result = dbScopeManagerConfiguration.Execute(conn =>
                    DalPaintDiscountConfig.UpdatePaintDiscountConfig(conn, model));
                    if (result)
                    {
                        model.CreateDateTime = oldValue.CreateDateTime;
                        model.LastUpdateDateTime = DateTime.Now;
                        var log = new PaintDiscountOprLogModel
                        {
                            LogType = "PaintDiscountConfig",
                            IdentityId = $"{model.PackageId}_{model.ServicePid}_{model.SurfaceCount}",
                            OperationType = "Update",
                            OldValue = JsonConvert.SerializeObject(oldValue),
                            NewValue = JsonConvert.SerializeObject(model),
                            Remarks = $"更新服务Pid:{model.ServicePid},面数:{model.SurfaceCount}的配置",
                            Operator = user,
                        };
                        LoggerManager.InsertLog("PaintDiscountOprLog", log);
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Logger.Error("UpdatePaintDiscountConfig", ex);
            }
            return result;
        }

        /// <summary>
        /// 批量更新活动图片
        /// </summary>
        /// <param name="models"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool MultUpdatePaintConfig(List<PaintDiscountConfigModel> models, string user)
        {
            var result = false;
            try
            {
                if (models != null && models.Any())
                {
                    var logs = new List<PaintDiscountOprLogModel>();
                    dbScopeManagerConfiguration.CreateTransaction(conn =>
                    {
                        foreach (var model in models)
                        {
                            var oldValue = GetPaintDiscountConfig(model.PackageId, model.ServicePid, model.SurfaceCount);
                            if (oldValue != null)
                            {
                                model.PKID = oldValue.PKID;
                                model.CreateDateTime = oldValue.CreateDateTime;
                                var updateResult = DalPaintDiscountConfig.UpdatePaintDiscountConfig(conn, model);
                                if (!updateResult)
                                {
                                    result = false;
                                    throw new Exception($"UpdatePaintDiscountConfig失败," +
                                        $"待更新数据：{JsonConvert.SerializeObject(model)}");
                                }
                                model.LastUpdateDateTime = DateTime.Now;
                                var log = new PaintDiscountOprLogModel
                                {
                                    LogType = "PaintDiscountConfig",
                                    IdentityId = $"{model.PackageId}_{model.ServicePid}_{model.SurfaceCount}",
                                    OperationType = "Update",
                                    OldValue = JsonConvert.SerializeObject(oldValue),
                                    NewValue = JsonConvert.SerializeObject(model),
                                    Remarks = $"更新服务Pid:{model.ServicePid},面数:{model.SurfaceCount}的配置",
                                    Operator = user,
                                };
                                logs.Add(log);
                            }
                        }
                        result = true;
                    });
                    if (result && logs.Any())
                    {
                        logs.ForEach(s => LoggerManager.InsertLog("PaintDiscountOprLog", s));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("MultUpdatePaintConfig", ex);
            }
            return result;
        }

        /// <summary>
        /// 上传喷漆打折配置
        /// 存在则更新，不存在则添加
        /// </summary>
        /// <param name="models"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool UploadPaintDiscountConfig
            (List<PaintDiscountConfigModel> models, string user)
        {
            var result = false;
            var insertList = new List<PaintDiscountConfigModel>();
            var updateList = new List<PaintDiscountConfigModel>();
            var logs = new List<PaintDiscountOprLogModel>();
            if (models != null && models.Any())
            {
                foreach (var model in models)
                {
                    var oldValue = GetPaintDiscountConfig(model.PackageId, model.ServicePid, model.SurfaceCount);
                    if (oldValue != null)
                    {
                        model.PKID = oldValue.PKID;
                        model.CreateDateTime = oldValue.CreateDateTime;
                        model.LastUpdateDateTime = DateTime.Now;
                        updateList.Add(model);
                        var log = new PaintDiscountOprLogModel
                        {
                            LogType = "PaintDiscountConfig",
                            IdentityId = $"{model.PackageId}_{model.ServicePid}_{model.SurfaceCount}",
                            OperationType = "Update",
                            OldValue = JsonConvert.SerializeObject(oldValue),
                            NewValue = JsonConvert.SerializeObject(model),
                            Remarks = $"更新服务Pid:{model.ServicePid},面数:{model.SurfaceCount}的配置",
                            Operator = user,
                        };
                        logs.Add(log);
                    }
                    else
                    {
                        insertList.Add(model);
                    }
                }
                dbScopeManagerConfiguration.CreateTransaction(conn =>
                {
                    foreach (var updateModel in updateList)
                    {
                        var updateResult = DalPaintDiscountConfig.UpdatePaintDiscountConfig(conn, updateModel);
                        if (!updateResult)
                        {
                            throw new Exception($"UpdatePaintDiscountConfig失败," +
                                $"待更新数据：{JsonConvert.SerializeObject(updateModel)}");
                        }
                    }
                    foreach (var insertModel in insertList)
                    {
                        var pkid = DalPaintDiscountConfig.AddPaintDiscountConfig(conn, insertModel);
                        var insertResult = pkid > 0;
                        if (!insertResult)
                        {
                            throw new Exception($"AddPaintDiscountConfig失败," +
                                $"待插入数据：{JsonConvert.SerializeObject(insertResult)}");
                        }
                        else
                        {
                            insertModel.PKID = pkid;
                            insertModel.CreateDateTime = DateTime.Now;
                            insertModel.LastUpdateDateTime = DateTime.Now;
                            var log = new PaintDiscountOprLogModel
                            {
                                LogType = "PaintDiscountConfig",
                                IdentityId = $"{insertModel.PackageId}_{insertModel.ServicePid}_{insertModel.SurfaceCount}",
                                OperationType = "Add",
                                OldValue = null,
                                NewValue = JsonConvert.SerializeObject(insertModel),
                                Remarks = $"添加服务Pid:{insertModel.ServicePid},面数:{insertModel.SurfaceCount}的配置",
                                Operator = user,
                            };
                            logs.Add(log);
                        }
                    }
                    result = true;
                });
                if (result)
                {
                    foreach (var log in logs)
                    {
                        LoggerManager.InsertLog("PaintDiscountOprLog", log);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 查询喷漆打折配置
        /// </summary>
        /// <param name="servicePid"></param>
        /// <param name="carNoPrefix"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Tuple<List<PaintDiscountConfigModel>, int> SelectPaintDiscountConfig
            (int packageId, string servicePid, int pageIndex, int pageSize)
        {
            var result = null as List<PaintDiscountConfigModel>;
            var totalCount = 0;
            try
            {
                var configs = dbScopeManagerConfigurationRead.Execute(conn =>
                      DalPaintDiscountConfig.SelectPaintDiscountConfig
                      (conn, packageId, servicePid, pageIndex, pageSize, out totalCount))
                      ?? new List<PaintDiscountConfigModel>();
                var allService = GetAllPaintDiscountService();
                configs.ForEach(
                    f =>
                    {
                        var service = allService.Where(w => string.Equals(w.ServicePid, f.ServicePid))?.FirstOrDefault();
                        f.ServiceName = service?.ServiceName;
                    });
                result = configs;
            }
            catch (Exception ex)
            {
                result = null;
                Logger.Error("SelectPaintDiscountConfig", ex);
            }
            return Tuple.Create(result, totalCount);
        }

        /// <summary>
        /// 获取所有喷漆打折服务
        /// </summary>
        /// <returns></returns>
        public List<PaintDiscountServiceModel> GetAllPaintDiscountService()
        {
            var result = null as List<PaintDiscountServiceModel>;
            try
            {
                result = dbScopeManagerProductcatalogRead.Execute
                    (conn => DalPaintDiscountConfig.GetAllPaintDiscountService(conn));
            }
            catch (Exception ex)
            {
                Logger.Error("GetAllPaintDiscountService", ex);
            }
            return result ?? new List<PaintDiscountServiceModel>();
        }

        /// <summary>
        /// 判断喷漆打折配置唯一性
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool IsExistPaintDiscountConfig(PaintDiscountConfigModel model)
        {
            var result = true;
            try
            {
                result = dbScopeManagerConfigurationRead.Execute
                     (conn => DalPaintDiscountConfig.IsExistPaintDiscountConfig(conn, model));
            }
            catch (Exception ex)
            {
                Logger.Error("IsExistPaintDiscountConfig", ex);
            }
            return result;
        }

        /// <summary>
        /// 查询喷漆打折操作日志
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="identityId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<PaintDiscountOprLogModel> SelectPaintDiscountOprLog
            (string logType, string identityId, int pageIndex = 1, int pageSize = 20)
        {
            var result = null as List<PaintDiscountOprLogModel>;
            try
            {
                result = dbScopeManagerTuhulog.Execute
                     (conn => DalPaintDiscountConfig.SelectPaintDiscountOprLog
                     (conn, logType, identityId, pageIndex, pageSize));
            }
            catch (Exception ex)
            {
                Logger.Error("SelectPaintDiscountOprLog", ex);
            }
            return result;
        }

        /// <summary>
        /// 刷新喷漆打折服务缓存
        /// </summary>
        /// <returns></returns>
        public bool RefreshPaintDiscountConfigCache(int packageId)
        {
            var result = false;
            try
            {
                var models = dbScopeManagerProductcatalogRead.Execute(conn =>
                DalPaintDiscountConfig.GetAllPaintDiscountService(conn));
                var cacheKeys = models?.Select(s => $"PaintDiscountPackageDetail/{packageId}/{s.ServicePid}")
                    .Distinct().ToList();
                if (cacheKeys != null && cacheKeys.Any())
                {
                    result = cacheKeys.All(key => RemovePaintRedisCache(key));
                }
            }
            catch (Exception ex)
            {
                Logger.Error("RefreshPaintDiscountConfigCache", ex);
            }
            return result;
        }

        /// <summary>
        /// 获取喷漆打折配置
        /// </summary>
        /// <param name="servicePid"></param>
        /// <param name="surfaceCount"></param>
        /// <returns></returns>
        private PaintDiscountConfigModel GetPaintDiscountConfig
        (int packageId, string servicePid, int surfaceCount)
        {
            return dbScopeManagerConfigurationRead.Execute
                     (conn => DalPaintDiscountConfig.GetPaintDiscountConfig(conn, packageId, servicePid, surfaceCount));
        }

        public List<PaintDiscountConfigModel> GetPaintDiscountDetailByPackageId(int packageId)
        {
            var result = null as List<PaintDiscountConfigModel>;
            try
            {
                result= dbScopeManagerConfigurationRead.Execute(conn =>
                DalPaintDiscountConfig.GetPaintDiscountDetailByPackageId(conn, packageId));
            }
            catch(Exception ex)
            {
                Logger.Error("GetPaintDiscountDetailByPackageId", ex);
            }
            return result;
        }
        #endregion

        #region 刷新缓存
        /// <summary>
        /// 移除喷漆服务Redis缓存
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        public bool RemovePaintRedisCache(string cacheKey)
        {
            var result = false;
            try
            {
                using (var client = new Service.Paint.PaintClient())
                {
                    var clientResult = client.RemoveRedisCacheKey(cacheKey);
                    clientResult.ThrowIfException(true);
                    result = clientResult.Success && clientResult.Result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("RemovePaintRedisCache", ex);
            }
            return result;
        }

        /// <summary>
        /// 刷新喷漆打折旧配置的服务缓存
        /// </summary>
        /// <returns></returns>
        public bool RefreshPaintDiscountConfigCache()
        {
            var result = false;
            try
            {
                var models = dbScopeManagerProductcatalogRead.Execute(conn =>
                DalPaintDiscountConfig.GetAllPaintDiscountService(conn));
                var servicePids = models?.Select(s => s.ServicePid).Distinct().ToList();
                if (servicePids != null && servicePids.Any())
                {
                    using (var client = new Service.Config.CacheClient())
                    {
                        var cacheResult = client.RefreshPaintDiscountConfigCache(servicePids);
                        cacheResult.ThrowIfException(true);
                        result = cacheResult.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("RefreshPaintDiscountConfigCache", ex);
            }
            return result;
        }
        #endregion
    }
}
