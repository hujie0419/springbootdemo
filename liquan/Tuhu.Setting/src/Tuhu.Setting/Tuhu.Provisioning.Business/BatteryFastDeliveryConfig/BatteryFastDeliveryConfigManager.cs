using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.Shop;

namespace Tuhu.Provisioning.Business.BatteryFastDeliveryConfig
{
    public class BatteryFastDeliveryConfigManager
    {
        private static readonly Common.Logging.ILog Logger;
        private static readonly IConnectionManager configConnString =
      new ConnectionManager(ConfigurationManager.ConnectionStrings["Configuration"].ConnectionString);
        private static readonly IConnectionManager configReadConnString =
      new ConnectionManager(ConfigurationManager.ConnectionStrings["Configuration_AlwaysOnRead"].ConnectionString);
        private static readonly IConnectionManager productReadConnString =
      new ConnectionManager(ConfigurationManager.ConnectionStrings["Tuhu_productcatalog_AlwaysOnRead"].ConnectionString);
        private static readonly IDBScopeManager dbScopeManagerConfigRead;
        private static readonly IDBScopeManager dbScopeManagerConfig;
        private static readonly IDBScopeManager dbScopeManagerProductRead;
        private static readonly IDBScopeManager dbScopeManagerGungnirRead;
        private static readonly IDBScopeManager dbScopeManagerGungnir;


        static BatteryFastDeliveryConfigManager()
        {
            Logger = Common.Logging.LogManager.GetLogger(typeof(BatteryFastDeliveryConfigManager));
            dbScopeManagerConfig = new DBScopeManager(configConnString);
            dbScopeManagerConfigRead = new DBScopeManager(configReadConnString);
            dbScopeManagerProductRead = new DBScopeManager(productReadConnString);

            dbScopeManagerGungnir = new DBScopeManager(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);
            dbScopeManagerGungnirRead = new DBScopeManager(ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString);
        }

        #region BatteryFastDelivery

        public Tuple<bool, bool> AddBatteryFastDelivery(BatteryFastDeliveryModel model, List<BatteryFastDeliveryProductsModel> productModels)
        {
            var result = false;
            var cacheResult = true;
            try
            {
                dbScopeManagerConfig.CreateTransaction(conn =>
                {
                    var fastDeliveryId = DalBatteryFastDelivery.AddBatteryFastDelivery(conn, model);
                    if (fastDeliveryId > 0)
                    {
                        if (productModels != null && productModels.Any())
                        {
                            foreach (var productModel in productModels)
                            {
                                productModel.FastDeliveryId = fastDeliveryId;
                                var insertResult = DalBatteryFastDelivery.AddBatteryFastDeliveryProducts(conn, productModel);
                            }
                            cacheResult = RefreshFastDeliveryServiceCache(model.RegionId, productModels.Select(s => s.ProductPid).ToList());
                        }
                        result = true;
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.Error("AddBatteryFastDelivery", ex);
            }
            return Tuple.Create(result, cacheResult);
        }

        public Tuple<bool, bool> DeleteBatteryFastDelivery(int fastDeliveryId, int regionId)
        {
            var result = false;
            var cacheResult = true;
            try
            {
                var products = dbScopeManagerConfigRead.Execute(conn => DalBatteryFastDelivery.GetBatteryFastDeliveryProductsByFastDeliveryId(conn, fastDeliveryId.ToString()))
                    .Select(s => s.ProductPid).ToList();
                if (products.Any())
                {
                    dbScopeManagerConfig.CreateTransaction(conn =>
                    {
                        var delMain = DalBatteryFastDelivery.DeleteBatteryFastDelivery(conn, fastDeliveryId);
                        var delProduct = DalBatteryFastDelivery.DeleteBatteryProductsByFastDeliveryId(conn, fastDeliveryId);
                        if (delMain && delProduct)
                        {
                            cacheResult = RefreshFastDeliveryServiceCache(regionId, products);
                            result = true;
                        }
                    });
                }
                else
                {
                    result = dbScopeManagerConfig.Execute(conn => DalBatteryFastDelivery.DeleteBatteryFastDelivery(conn, fastDeliveryId));
                }
            }
            catch (Exception ex)
            {
                Logger.Error("DeleteBatteryFastDelivery", ex);
            }
            return Tuple.Create(result, cacheResult);
        }

        /// <summary>
        /// 更新蓄电池极速达配置
        /// </summary>
        /// <param name="model"></param>
        /// <param name="productModels"></param>
        /// <returns></returns>
        public Tuple<bool, bool> UpdateBatteryFastDelivery(BatteryFastDeliveryModel model, List<BatteryFastDeliveryProductsModel> productModels)
        {
            var result = false;
            var cacheResult = true;
            try
            {
                var existProducts = dbScopeManagerConfigRead.Execute(conn => DalBatteryFastDelivery.GetBatteryFastDeliveryProductsByFastDeliveryId(conn, model.PKID.ToString()));
                if ((productModels == null || !productModels.Any()) && (existProducts == null || !existProducts.Any()))
                {
                    result = dbScopeManagerConfig.Execute(conn => DalBatteryFastDelivery.UpdateBatteryFastDelivery(conn, model));
                }
                else
                {
                    dbScopeManagerConfig.CreateTransaction(conn =>
                    {
                        var updateMain = DalBatteryFastDelivery.UpdateBatteryFastDelivery(conn, model);
                        productModels = productModels ?? new List<BatteryFastDeliveryProductsModel>();
                        existProducts = existProducts ?? new List<BatteryFastDeliveryProductsModel>();
                        var updateProducts = UpdateBatteryFastDeliveryProducts(conn, model.RegionId, productModels, existProducts);
                        if (updateMain && updateProducts)
                        {
                            var refreshPids = productModels.Select(s => s.ProductPid).Union(existProducts.Select(e => e.ProductPid)).ToList();
                            cacheResult = RefreshFastDeliveryServiceCache(model.RegionId, refreshPids);
                            result = true;
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error("UpdateBatteryFastDelivery", ex);
            }
            return Tuple.Create(result, cacheResult);
        }

        public Tuple<List<BatteryFastDeliveryModel>, int> SelectBatteryFastDelivery(string serviceTypePid, int pageIndex, int pageSize)
        {
            int totalCount = 0;
            List<BatteryFastDeliveryModel> fastDeliveryInfos = null;
            List<BatteryFastDeliveryProductsModel> batteryList = null;
            try
            {
                fastDeliveryInfos = dbScopeManagerConfigRead.Execute(conn =>
                   DalBatteryFastDelivery.SelectBatteryFastDelivery(conn, serviceTypePid, pageIndex, pageSize, out totalCount));
                var fastDeliveryIdsStr = string.Join(",", fastDeliveryInfos.Select(s => s.PKID).ToList());
                batteryList = dbScopeManagerConfigRead.Execute(conn => DalBatteryFastDelivery.GetBatteryFastDeliveryProductsByFastDeliveryId(conn, fastDeliveryIdsStr));
                var batteryDic = batteryList.GroupBy(item => item.FastDeliveryId)
                            .ToDictionary(i => i.Key,
                                i => i.ToList().GroupBy(t => t.Brand).ToDictionary(j => j.Key, j => j.Select(s => s.ProductPid).Take(5).ToList()));
                using (var client = new RegionClient())
                {
                    foreach (var fastDeliveryInfo in fastDeliveryInfos)
                    {
                        fastDeliveryInfo.Products = batteryDic.ContainsKey(fastDeliveryInfo.PKID) ? batteryDic[fastDeliveryInfo.PKID] : null;
                        var serviceResult = client.GetSimpleRegionByRegionId(fastDeliveryInfo.RegionId);
                        serviceResult.ThrowIfException(true);
                        if (serviceResult?.Result != null)
                        {
                            fastDeliveryInfo.ProvinceName = serviceResult.Result.ProvinceName;
                            fastDeliveryInfo.CityName = serviceResult.Result.CityName;
                            fastDeliveryInfo.DistrictName = serviceResult.Result.DistrictName;
                        }
                    }
                    return new Tuple<List<BatteryFastDeliveryModel>, int>(fastDeliveryInfos, totalCount);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("SelectBatteryFastDelivery", ex);
                return new Tuple<List<BatteryFastDeliveryModel>, int>(null, 0);
            }
        }

        public bool ChangeBatteryFastDeliveryStatus(int pkid, bool isEnabled, string user)
        {
            var success = false;
            try
            {
                List<string> products = null;
                BatteryFastDeliveryModel config = null;
                dbScopeManagerConfigRead.Execute(conn =>
                {
                    config = DalBatteryFastDelivery.SelectBatteryFastDeliveryById(conn, pkid);
                    if (config != null)
                    {
                        var list = DalBatteryFastDelivery.GetBatteryFastDeliveryProductsByFastDeliveryId(conn, pkid.ToString());
                        products = list.Select(x => x.ProductPid).ToList();
                    }
                    products = products ?? new List<string>();
                });
                if (config != null && config.IsEnabled != isEnabled)
                {
                    success = dbScopeManagerConfig.Execute(conn => DalBatteryFastDelivery.UpdateBatteryFastDeliveryStatus(conn, pkid, isEnabled));
                }
                if (success)
                {
                    RecordOperationLog(new BaoYangOprLog
                    {
                        LogType = "BatteryFastDelivery",
                        Remarks = $"{(isEnabled ? "启用配置" : "禁用配置")}",
                        OldValue = JsonConvert.SerializeObject(new { PKID = pkid, IsEnabled = !isEnabled }),
                        NewValue = JsonConvert.SerializeObject(new { PKID = pkid, IsEnabled = isEnabled }),
                        IdentityID = pkid.ToString(),
                        OperateUser = user,
                    });
                    RefreshFastDeliveryServiceCache(config.RegionId, products);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("ChangeBatteryFastDeliveryStatus", ex);
            }
            return success;
        }

        public List<BatteryFastDeliveryForViewModel> GetAllBatteryForView(int fastDeliveryId)
        {
            List<BatteryFastDeliveryForViewModel> allBattery = null;
            try
            {
                var sort = new List<string> { "风帆", "瓦尔塔" };
                var batteries = dbScopeManagerProductRead.Execute(conn =>
                    DalBatteryFastDelivery.GetAllBattery(conn)//获取所有的蓄电池信息
                );
                allBattery = batteries.OrderBy(x =>
                {
                    var index = sort.FindIndex(o => x.Brand.Contains(o));
                    if (index < 0) index = int.MaxValue;
                    return index;
                }).ToList();
                if (fastDeliveryId > 0)
                {
                    var existBattery = dbScopeManagerConfigRead.Execute(conn =>
                     DalBatteryFastDelivery.GetBatteryFastDeliveryProductsByFastDeliveryId(conn, fastDeliveryId.ToString())
                     ).Select(s => s.ProductPid).ToList();//勾选已选中的蓄电池
                    allBattery.ForEach(s => s.IsChecked = existBattery.Contains(s.PID));
                }
            }
            catch (Exception ex)
            {
                Logger.Error("GetAllBatteryForView", ex);
            }
            return allBattery;
        }

        public List<BatteryServiceTypeModel> GetAllBatteryServiceType()
        {
            List<BatteryServiceTypeModel> allService = null;
            try
            {
                allService = dbScopeManagerProductRead.Execute(conn =>
               DalBatteryFastDelivery.GetAllBatteryServiceType(conn));//获取所有的蓄电池极速达服务类目
            }
            catch (Exception ex)
            {
                Logger.Error("GetAllBatteryServiceType", ex);
            }
            return allService;
        }

        public List<BatteryFastDeliveryProductsModel> GetBatteryFastDeliveryProductsByFastDeliveryId(int fastDeliveryId)
        {
            List<BatteryFastDeliveryProductsModel> batteryList = null;
            try
            {
                batteryList = dbScopeManagerConfigRead.Execute(conn =>
               DalBatteryFastDelivery.GetBatteryFastDeliveryProductsByFastDeliveryId(conn, fastDeliveryId.ToString()));
            }
            catch (Exception ex)
            {
                Logger.Error("GetBatteryFastDeliveryProductsByFastDeliveryId", ex);
            }
            return batteryList ?? new List<BatteryFastDeliveryProductsModel>();
        }

        public bool IsRepeatBatteryFastDelivery(string serviceTypePid, int regionId)
        {
            bool isRepeat = true;
            try
            {
                isRepeat = dbScopeManagerConfigRead.Execute(conn =>
                 DalBatteryFastDelivery.IsRepeatBatteryFastDelivery(conn, serviceTypePid, regionId));
            }
            catch (Exception ex)
            {
                Logger.Error("IsRepeatBatteryFastDelivery", ex);
            }
            return isRepeat;
        }

        /// <summary>
        /// 更新蓄电池极速达附表 产品表
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="regionId"></param>
        /// <param name="selectedList"></param>
        /// <param name="existList"></param>
        /// <returns></returns>
        private bool UpdateBatteryFastDeliveryProducts(SqlConnection conn, int regionId, List<BatteryFastDeliveryProductsModel> selectedList, List<BatteryFastDeliveryProductsModel> existList)
        {
            var selectPids = selectedList.Select(s => s.ProductPid).ToList();
            var existPids = existList.Select(e => e.ProductPid).ToList();
            var deleteList = existList.Where(e => !selectPids.Contains(e.ProductPid)).ToList();
            var insertList = selectedList.Where(s => !existPids.Contains(s.ProductPid)).ToList();

            //执行删除配置操作
            if (deleteList.Any())
            {
                foreach (var model in deleteList)
                {
                    if (model.FastDeliveryId > 0 && !string.IsNullOrEmpty(model.Brand) && !string.IsNullOrEmpty(model.ProductPid))
                    {
                        var delResult = DalBatteryFastDelivery.DeleteBatteryFastDeliveryProducts(conn, model);
                    }
                }
            }
            //执行插入操作
            if (insertList.Any())
            {
                foreach (var model in insertList)
                {
                    if (model.FastDeliveryId > 0 && !string.IsNullOrEmpty(model.Brand) && !string.IsNullOrEmpty(model.ProductPid))
                    {
                        var addResult = DalBatteryFastDelivery.AddBatteryFastDeliveryProducts(conn, model);
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 刷新蓄电池极速达服务缓存
        /// </summary>
        /// <param name="regionId"></param>
        /// <param name="productPids"></param>
        /// <returns></returns>
        private bool RefreshFastDeliveryServiceCache(int regionId, List<string> productPids)
        {
            var result = true;
            try
            {
                if (productPids.Any())
                {
                    var cacheKeys = new List<string>();
                    var batteryFastDeliveryServiceCacheKey = "BatteryFastDeliveryService/{0}/{1}";
                    productPids.ForEach(s => cacheKeys.Add(string.Format(batteryFastDeliveryServiceCacheKey, regionId, s)));
                    using (var client = new Tuhu.Service.BaoYang.CacheClient())
                    {
                        var cacheResult = client.Remove(cacheKeys);
                        cacheResult.ThrowIfException(true);
                        result = cacheResult.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Logger.Error("RefreshFastDeliveryServiceCache", ex);
            }
            return result;
        }

        #endregion


        #region 服务类蓄电池产品排序

        /// <summary>
        /// 获取蓄电池品牌
        /// </summary>
        /// <returns></returns>
        public List<string> GetBatteryBrands()
        {
            List<string> result = null;
            try
            {
                result = dbScopeManagerProductRead.Execute(conn => DalBatteryFastDelivery.SelectBatteryBrands(conn));
            }
            catch (Exception ex)
            {
                Logger.Error(nameof(GetBatteryBrands), ex);
            }
            return result ?? new List<string>();
        }

        /// <summary>
        /// 获取服务类蓄电池产品配置
        /// </summary>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="index"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public Tuple<int, List<BatteryFastDeliveryPriority>> GetBatteryFastDeliveryPriorities(int? province, int? city, int index, int size)
        {
            int count = 0;
            List<BatteryFastDeliveryPriority> list = null;
            try
            {
                var result = dbScopeManagerGungnirRead.Execute(conn => DalBatteryFastDelivery.SelectBatteryFastDeliveryPriorities(conn,
                    province, city, index, size));
                count = result.Item1;
                list = result.Item2;
            }
            catch (Exception ex)
            {
                Logger.Error(nameof(GetBatteryFastDeliveryPriorities), ex);
            }
            return Tuple.Create(count, list ?? new List<BatteryFastDeliveryPriority>());
        }

        /// <summary>
        /// 根据id获取配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BatteryFastDeliveryPriority GetBatteryFastDeliveryPriority(int id)
        {
            BatteryFastDeliveryPriority result = null;
            try
            {
                result = dbScopeManagerGungnirRead.Execute(conn => DalBatteryFastDelivery.GetBatteryFastDeliveryPriorities(conn, id));
            }
            catch (Exception ex)
            {
                Logger.Error(nameof(GetBatteryFastDeliveryPriority), ex);
            }
            return result;
        }

        public bool IsExistsBatteryFastDeliveryPriority(BatteryFastDeliveryPriority priority)
        {
            var result = true;
            try
            {
                result = dbScopeManagerGungnirRead.Execute(conn => DalBatteryFastDelivery.IsExistsBatteryFastDeliveryPriority(conn, priority));
            }
            catch (Exception ex)
            {
                Logger.Error(nameof(IsExistsBatteryFastDeliveryPriority), ex);
            }
            return result;
        }

        /// <summary>
        /// 添加配置
        /// </summary>
        /// <param name="priority"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool AddBatteryFastDeliveryPriority(BatteryFastDeliveryPriority priority, string user)
        {
            var success = false;
            try
            {
                priority.PKID = dbScopeManagerGungnir.Execute(conn => DalBatteryFastDelivery.AddBatteryFastDeliveryPriority(conn, priority));
                success = priority.PKID > 0;
                if (success)
                {
                    RecordOperationLog(new BaoYangOprLog
                    {
                        IdentityID = priority.PKID.ToString(),
                        LogType = "BatteryFastDeliveryPriority",
                        NewValue = JsonConvert.SerializeObject(priority),
                        OldValue = string.Empty,
                        OperateUser = user,
                        Remarks = "add",
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error(nameof(AddBatteryFastDeliveryPriority), ex);
            }
            return success;
        }

        /// <summary>
        /// 修改配置
        /// </summary>
        /// <param name="priority"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool UpdateBatteryFastDeliveryPriority(BatteryFastDeliveryPriority priority, string user)
        {
            var success = false;
            try
            {
                var origin = GetBatteryFastDeliveryPriority(priority.PKID);
                if (origin != null)
                {
                    priority.CityId = origin.CityId;
                    priority.CityName = origin.CityName;
                    priority.ProvinceId = origin.ProvinceId;
                    priority.ProvinceName = origin.ProvinceName;
                    priority.CreateDateTime = origin.CreateDateTime;
                    success = dbScopeManagerGungnir.Execute(conn => DalBatteryFastDelivery.UpdateBatteryFastDeliveryPriority(conn, priority));
                    priority.LastUpdateDateTime = DateTime.Now;
                }
                if (success)
                {
                    RecordOperationLog(new BaoYangOprLog
                    {
                        IdentityID = priority.PKID.ToString(),
                        LogType = "BatteryFastDeliveryPriority",
                        NewValue = JsonConvert.SerializeObject(priority),
                        OldValue = JsonConvert.SerializeObject(origin),
                        OperateUser = user,
                        Remarks = "update",
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error(nameof(UpdateBatteryFastDeliveryPriority), ex);
            }
            return success;
        }

        /// <summary>
        /// 删除配置
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool DeleteBatteryFastDeliveryPriority(int id, string user)
        {
            var success = false;
            try
            {
                var origin = GetBatteryFastDeliveryPriority(id);
                if (origin != null)
                {
                    success = dbScopeManagerGungnir.Execute(conn => DalBatteryFastDelivery.DeleteBatteryFastDeliveryPriority(conn, id));
                }
                if (success)
                {
                    RecordOperationLog(new BaoYangOprLog
                    {
                        IdentityID = id.ToString(),
                        LogType = "BatteryFastDeliveryPriority",
                        NewValue = string.Empty,
                        OldValue = JsonConvert.SerializeObject(origin),
                        OperateUser = user,
                        Remarks = "delete",
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error(nameof(DeleteBatteryFastDeliveryPriority), ex);
            }
            return success;
        }

        /// <summary>
        /// 记录操作日志
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        private bool RecordOperationLog(BaoYangOprLog log) => Business.Logger.LoggerManager.InsertLog("BYOprLog", log);

        public bool RefreshBatteryFastDeliveryPrioritiesCache(int provinceId, int cityId)
        {
            var result = false;
            try
            {
                const string keyPrefix = "PrioritySetting/BatteryFastDelivery/{0}/{1}";
                var key = string.Format(keyPrefix, provinceId, cityId);
                using (var client = new Tuhu.Service.BaoYang.CacheClient())
                {
                    var cacheResult = client.Remove(new[] { key });
                    cacheResult.ThrowIfException(true);
                    result = cacheResult.Result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("RefreshBatteryFastDeliveryPrioritiesCache", ex);
            }
            return result;
        }

        #endregion
    }
}
