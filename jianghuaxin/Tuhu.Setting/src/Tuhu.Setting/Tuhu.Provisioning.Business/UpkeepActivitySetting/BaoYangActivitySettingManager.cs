using Common.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.BaoYang;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.BaoYang;
using Tuhu.Service.BaoYang.Config;

namespace Tuhu.Provisioning.Business
{
    public class BaoYangActivitySettingManager
    {
        private static readonly Common.Logging.ILog Logger = LogManager.GetLogger(typeof(BaoYangActivitySettingManager));

        private static readonly IConnectionManager grConnectionManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);
        private static readonly IConnectionManager grAlwaysOnReadConnectionManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString);
        private static readonly IConnectionManager tuhuLogConnectionManager =
           new ConnectionManager(ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString);

        private readonly IDBScopeManager dbScopeManagerGungnir = null;
        private readonly IDBScopeManager dbScopeManagerGungnirRead = null;
        private readonly IDBScopeManager dbScopeManagerTuhuLog = null;


        public BaoYangActivitySettingManager()
        {
            dbScopeManagerGungnir = new DBScopeManager(grConnectionManager);
            dbScopeManagerGungnirRead = new DBScopeManager(grAlwaysOnReadConnectionManager);
            dbScopeManagerTuhuLog = new DBScopeManager(tuhuLogConnectionManager);
        }

        public List<IGrouping<int, BaoYangActivitySetting>> GetUpkeepActivitySetting(string sqlStr, int pageSize, int pageIndex, out int recordCount)
        {
            try
            {
                return DalBaoYangActivitySetting.GetUpkeepActivitySetting(sqlStr, pageSize, pageIndex, out recordCount);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error("GetUpkeepActivitySetting", ex);
                throw ex;
            }
        }

        public List<ServiceTypeSetting> GetServiceTypeSetting()
        {
            try
            {
                return DalBaoYangActivitySetting.GetServiceTypeSetting();
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error("GetServiceTypeSetting", ex);
                throw ex;
            }
        }

        public BaoYangActivitySetting GetUpkeepActivitySettingById(int id)
        {
            try
            {
                return DalBaoYangActivitySetting.GetUpkeepActivitySettingById(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error("GetUpkeepActivitySettingById", ex);
                throw ex;
            }
        }

        public ServiceTypeSetting GetServiceTypeSettingById(int id)
        {
            try
            {
                return DalBaoYangActivitySetting.GetServiceTypeSettingById(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error("GetServiceTypeSettingById", ex);
                throw ex;
            }
        }

        public bool InsertUpkeepActivitySetting(BaoYangActivitySetting model)
        {
            try
            {
                int id = 0;
                string activityNum = "";
                if (DalBaoYangActivitySetting.InsertUpkeepActivitySetting(model, out id, out activityNum))
                {
                    using (var client = new CacheClient())
                    {
                        client.UpdateBaoYangActivityAsync(activityNum);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error("InsertUpkeepActivitySetting", ex);
                throw ex;
            }
        }

        public bool InsertServiceTypeSetting(ServiceTypeSetting model)
        {
            try
            {
                return DalBaoYangActivitySetting.InsertServiceTypeSetting(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error("InsertServiceTypeSetting", ex);
                throw ex;
            }
        }

        public bool UpdateUpkeepActivitySetting(BaoYangActivitySetting model)
        {
            try
            {

                if (DalBaoYangActivitySetting.UpdateUpkeepActivitySetting(model))
                {
                    using (var client = new CacheClient())
                    {
                        client.UpdateBaoYangActivityAsync(model.ActivityNum);
                    }
                    CleanBaoYangActivitySettingCache(model.ActivityNum);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error("UpdateUpkeepActivitySetting", ex);
                throw ex;
            }
        }

        public bool UpdateServiceTypeSetting(ServiceTypeSetting model)
        {
            try
            {
                return DalBaoYangActivitySetting.UpdateServiceTypeSetting(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error("UpdateServiceTypeSetting", ex);
                throw ex;
            }
        }

        public bool DeleteUpkeepActivitySetting(int id, string activityNum)
        {
            try
            {
                if (DalBaoYangActivitySetting.DeleteUpkeepActivitySetting(id))
                {
                    using (var client = new CacheClient())
                    {
                        client.UpdateBaoYangActivityAsync(activityNum);
                    }
                    CleanBaoYangActivitySettingCache(activityNum);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error("DeleteUpkeepActivitySetting", ex);
                throw ex;
            }
        }

        public bool DeleteServiceTypeSetting(int id)
        {
            try
            {
                return DalBaoYangActivitySetting.DeleteServiceTypeSetting(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error("DeleteServiceTypeSetting", ex);
                throw ex;
            }
        }


        public List<BaoYangActivityRelevance> GetRelevanceBrand(string catalogNames)
        {
            try
            {
                return DalBaoYangActivitySetting.GetRelevanceBrand(catalogNames);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error("GetRelevanceBrand", ex);
                throw ex;
            }
        }

        public List<BaoYangActivityRelevance> GetRelevanceSeries(string catalogName)
        {
            try
            {
                return DalBaoYangActivitySetting.GetRelevanceSeries(catalogName);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error("GetRelevanceSeries", ex);
                throw ex;
            }
        }

        public List<BaoYangActivityRelevance> GetStoreAuthentication()
        {
            try
            {
                return DalBaoYangActivitySetting.GetStoreAuthentication();
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error("GetStoreAuthentication", ex);
                throw ex;
            }
        }

        /// <summary>
        /// 根据活动Id获取保养活动关联配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<BaoYangActivitySettingItem> GetBaoYangActivitySettingItemByBaoYangActivityId(int id)
        {
            var result = null as List<BaoYangActivitySettingItem>;
            try
            {
                result = DalBaoYangActivitySetting.GetBaoYangActivitySettingItemByBaoYangActivityId(id);
            }
            catch (Exception ex)
            {
                Logger.Error("GetBaoYangActivitySettingItemByBaoYangActivityId", ex);
            }
            return result;
        }

        /// <summary>
        /// 获取保养活动关联配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BaoYangActivitySettingItem GetBaoYangActivitySettingItemById(int id)
        {
            var result = null as BaoYangActivitySettingItem;
            try
            {
                result = DalBaoYangActivitySetting.GetBaoYangActivitySettingItemById(id);
            }
            catch (Exception ex)
            {
                Logger.Error("GetBaoYangActivitySettingItemById", ex);
            }
            return result;
        }
        public bool InsertBaoYangActivitySettingItem(BaoYangActivitySettingItem model, string actId)
        {
            try
            {
                if (DalBaoYangActivitySetting.InsertBaoYangActivitySettingItem(model))
                {
                    using (var client = new CacheClient())
                    {
                        client.UpdateBaoYangActivityAsync(actId);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error("InsertBaoYangActivitySettingItem", ex);
                throw ex;
            }
        }

        public bool UpdateBaoYangActivitySettingItem(BaoYangActivitySettingItem model)
        {
            try
            {
                if (DalBaoYangActivitySetting.UpdateBaoYangActivitySettingItem(model))
                {
                    using (var client = new CacheClient())
                    {
                        client.UpdateBaoYangActivityAsync(DalBaoYangActivitySetting.GetUpkeepActivitySettingById(model.BaoYangActivityId).ActivityNum);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error("UpdateBaoYangActivitySettingItem", ex);
                throw ex;
            }
        }

        public bool DeleteBaoYangActivitySettingItem(int id, int baoYangActivityId)
        {
            try
            {
                if (DalBaoYangActivitySetting.DeleteBaoYangActivitySettingItem(id))
                {
                    using (var client = new CacheClient())
                    {
                        client.UpdateBaoYangActivityAsync(DalBaoYangActivitySetting.GetUpkeepActivitySettingById(baoYangActivityId).ActivityNum);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error("DeleteBaoYangActivitySettingItem", ex);
                throw ex;
            }
        }

        /// <summary>
        /// 根据保养活动Id获取保养活动名称
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public string GetBaoYangActivityNameByActivityId(string activityId)
        {
            var result = string.Empty;
            try
            {
                result = dbScopeManagerGungnirRead.Execute(conn => DalBaoYangActivitySetting.GetBaoYangActivityNameByActivityId(conn, activityId));
            }
            catch (Exception ex)
            {
                Logger.Error("GetBaoYangActivityNameByActivityId", ex);
            }
            return result;
        }

        #region 保养活动分车型配置

        /// <summary>
        /// 获取所有车型品牌
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, List<string>> GetAllVehicleBrand()
        {
            var result = null as Dictionary<string, List<string>>;
            try
            {
                var brands = dbScopeManagerGungnirRead.Execute(conn => DalBaoYangActivitySetting.GetAllVehicleBrand(conn));
                if (brands != null && brands.Any())
                {
                    result = brands.Where(v => !string.IsNullOrWhiteSpace(v)).GroupBy(v => v.Substring(0, 1)).OrderBy(g => g.Key).ToDictionary(c => c.Key, c => c.ToList());
                }
            }
            catch (Exception ex)
            {
                Logger.Error("GetAllVehicleBrand", ex);
            }
            return result;
        }

        /// <summary>
        /// 获取所有机油等级
        /// </summary>
        /// <returns></returns>
        public List<OilLevelModel> GetAllOilLevel()
        {
            var result = null as List<OilLevelModel>;
            try
            {
                result = dbScopeManagerGungnirRead.Execute(conn => DalBaoYangActivitySetting.GetAllOilLevel(conn));
            }
            catch (Exception ex)
            {
                Logger.Error("GetAllOilLevel", ex);
            }
            return result;
        }

        /// <summary>
        /// 获取所有机油粘度
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllOilViscosity()
        {
            var result = null as List<string>;
            try
            {
                result = dbScopeManagerGungnirRead.Execute(conn => DalBaoYangActivitySetting.GetAllOilViscosity(conn));
            }
            catch (Exception ex)
            {
                Logger.Error("GetAllOilViscosity", ex);
            }
            return result;
        }

        /// <summary>
        /// 查询保养活动车型配置
        /// </summary>
        /// <param name="model"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Tuple<List<BaoYangActivityVehicleViewModel>, int> SelectBaoYangActivityVehicle(BaoYangActivityVehicleSearchModel model, int pageIndex, int pageSize)
        {
            var result = null as Tuple<List<BaoYangActivityVehicleViewModel>, int>;
            try
            {
                result = dbScopeManagerGungnirRead.Execute(conn => DalBaoYangActivitySetting.SelectBaoYangActivityVehicle(conn, model, pageIndex, pageSize));
                var oilLevels = GetAllOilLevel();
                if (oilLevels != null && oilLevels.Any() && result != null && result.Item1 != null && result.Item1.Any())
                {
                    result.Item1.ForEach(s => s.OilLevel = (string.Join(",", oilLevels.Where(o => s.OilLevel.Split(',').ToList().Contains(o.OilLevel)).Select(r => r.OilType).Distinct().ToList())));
                }
            }
            catch (Exception ex)
            {
                Logger.Error("SelectBaoYangActivityVehicle", ex);
            }
            return result ?? Tuple.Create(null as List<BaoYangActivityVehicleViewModel>, 0);
        }

        /// <summary>
        /// 批量新增或更新保养活动车型配置
        /// </summary>
        /// <param name="vehicleIds"></param>
        /// <param name="activityId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public Tuple<bool, bool> MultiAddOrEditBaoYangActivityVehicle(List<string> vehicleIds, string activityId, string user)
        {
            bool result = false;
            var cacheResult = false;
            try
            {
                if (vehicleIds != null && vehicleIds.Any() && !string.IsNullOrWhiteSpace(activityId))
                {
                    var insertModels = new List<BaoYangActivityVehicleAndRegionModel>();
                    var updateValues = new List<BaoYangActivityVehicleAndRegionModel>();
                    var logData = new List<BaoYangOprLog>();
                    foreach (var vehicleId in vehicleIds)
                    {
                        var oldValue = GetBaoYangActivityVehicleByVehicleId(vehicleId);
                        if (oldValue == null)
                        {
                            var insertModel = new BaoYangActivityVehicleAndRegionModel
                            {
                                VehicleId = vehicleId,
                                ActivityId = activityId,
                                RegionId = 0,
                                Type = "Vehicle",
                                CreateDateTime = DateTime.Now,
                                LastUpdateDateTime = DateTime.Now
                            };
                            insertModels.Add(insertModel);
                        }
                        else
                        {
                            updateValues.Add(oldValue);
                        }
                    }
                    dbScopeManagerGungnir.CreateTransaction(conn =>
                    {
                        foreach (var insertModel in insertModels)
                        {
                            var pkid = DalBaoYangActivitySetting.AddBaoYangActivityVehicle(conn, insertModel);
                            if (pkid < 1)
                            {
                                throw new Exception($"AddBaoYangActivityVehicle失败,{JsonConvert.SerializeObject(insertModel)}");
                            }
                            insertModel.PKID = pkid;
                            var log = new BaoYangOprLog
                            {
                                LogType = "BaoYangActivityVehicle",
                                IdentityID = insertModel.VehicleId,
                                OldValue = null,
                                NewValue = JsonConvert.SerializeObject(insertModel),
                                Remarks = "Add",
                                OperateUser = user,
                            };
                            logData.Add(log);
                        }
                        foreach (var oldValue in updateValues)
                        {
                            var updateModel = new BaoYangActivityVehicleAndRegionModel
                            {
                                VehicleId = oldValue.VehicleId,
                                ActivityId = activityId,
                                RegionId = oldValue.RegionId,
                                Type = oldValue.Type,
                                CreateDateTime = oldValue.CreateDateTime,
                                LastUpdateDateTime = DateTime.Now
                            };
                            var updateResult = DalBaoYangActivitySetting.UpdateBaoYangActivityVehicle(conn, updateModel);
                            if (!updateResult)
                            {
                                throw new Exception($"UpdateBaoYangActivityVehicle失败,{JsonConvert.SerializeObject(updateModel)}");
                            }
                            var log = new BaoYangOprLog
                            {
                                LogType = "BaoYangActivityVehicle",
                                IdentityID = updateModel.VehicleId,
                                OldValue = JsonConvert.SerializeObject(oldValue),
                                NewValue = JsonConvert.SerializeObject(updateModel),
                                Remarks = "Update",
                                OperateUser = user,
                            };
                            logData.Add(log);
                        }
                        result = true;
                    });
                    foreach (var log in logData)
                    {
                        LoggerManager.InsertLog("BYOprLog", log);
                    }
                    cacheResult = CleanBaoYangActivityVehicleCache(vehicleIds);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("MultiAddOrEditBaoYangActivityVehicle", ex);
            }
            return Tuple.Create(result, cacheResult);
        }

        /// <summary>
        /// 批量删除保养活动车型配置
        /// </summary>
        /// <param name="vehicleIds"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public Tuple<bool, bool> MultDeleteBaoYangActivityVehicle(List<string> vehicleIds, string user)
        {
            var result = false;
            var cacheResult = false;
            try
            {
                var logData = new List<BaoYangOprLog>();
                if (vehicleIds != null && vehicleIds.Any())
                {
                    dbScopeManagerGungnir.CreateTransaction(conn =>
                    {
                        foreach (var vehicleId in vehicleIds)
                        {
                            if (!string.IsNullOrWhiteSpace(vehicleId))
                            {
                                var oldValue = GetBaoYangActivityVehicleByVehicleId(vehicleId);
                                if (oldValue != null)
                                {
                                    var delResult = DalBaoYangActivitySetting.DeleteBaoYangActivityVehicle(conn, vehicleId);
                                    if (!delResult)
                                    {
                                        throw new Exception($"DeleteBaoYangActivityVehicle失败,{JsonConvert.SerializeObject(oldValue)}");
                                    }
                                    var log = new BaoYangOprLog
                                    {
                                        LogType = "BaoYangActivityVehicle",
                                        IdentityID = vehicleId,
                                        OldValue = JsonConvert.SerializeObject(oldValue),
                                        NewValue = null,
                                        Remarks = "Delete",
                                        OperateUser = user,
                                    };
                                    logData.Add(log);
                                }
                            }
                        }
                        result = true;
                    });
                    foreach (var log in logData)
                    {
                        LoggerManager.InsertLog("BYOprLog", log);
                    }
                    cacheResult = CleanBaoYangActivityVehicleCache(vehicleIds);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("MultDeleteBaoYangActivityVehicle", ex);
            }
            return Tuple.Create(result, cacheResult);
        }

        /// <summary>
        /// 根据二级车型获取保养活动车型配置
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <returns></returns>
        public BaoYangActivityVehicleAndRegionModel GetBaoYangActivityVehicleByVehicleId(string vehicleId)
        {
            var result = null as BaoYangActivityVehicleAndRegionModel;
            try
            {
                if (!string.IsNullOrWhiteSpace(vehicleId))
                {
                    result = dbScopeManagerGungnirRead.Execute(conn => DalBaoYangActivitySetting.GetBaoYangActivityVehicleByVehicleId(conn, vehicleId));
                }
            }
            catch (Exception ex)
            {
                Logger.Error("GetBaoYangActivityVehicleByVehicleId", ex);
            }
            return result;
        }

        #endregion

        #region 保养活动分地区配置

        /// <summary>
        /// 获取所有地区信息 到市一级
        /// </summary>
        /// <returns></returns>
        public List<Tuhu.Service.Shop.Models.Region.MiniRegion> GetAllRegion()
        {
            using (var client = new Tuhu.Service.Shop.RegionClient())
            {
                var serviceResult = client.GetAllMiniRegion();
                var regions = serviceResult.Result;
                if (regions != null && regions.Any())
                {
                    var directRegion = new int[4] { 1, 2, 19, 20 };
                    regions.ForEach(s =>
                    {
                        if (directRegion.Contains(s.RegionId))
                        {
                            s.ChildRegions = null;
                        }
                        else
                        {
                            s.ChildRegions.ForEach(v => v.ChildRegions = null);
                        }
                    });
                }
                return regions?.ToList() ?? new List<Tuhu.Service.Shop.Models.Region.MiniRegion>();
            }
        }

        /// <summary>
        /// 查询保养活动地区配置
        /// </summary>
        /// <param name="regionIds"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<BaoYangActivityRegionViewModel> SelectBaoYangActivityRegion(List<int> regionIds, int pageIndex, int pageSize)
        {
            var result = null as List<BaoYangActivityRegionViewModel>;
            try
            {
                result = dbScopeManagerGungnirRead.Execute(conn => DalBaoYangActivitySetting.SelectBaoYangActivityRegion(conn, regionIds, pageIndex, pageSize));
                var regions = GetAllRegion();
                if (result != null && result.Any() && regions.Any())
                {
                    var directRegion = new int[4] { 1, 2, 19, 20 };
                    result.ForEach(s =>
                    {
                        if (directRegion.Contains(s.RegionId))
                        {
                            var region = regions.Where(r => r.RegionId == s.RegionId)?.FirstOrDefault();
                            if (region != null)
                            {
                                s.ProvinceName = region.RegionName;
                                s.CityName = region.RegionName;
                            }
                        }
                        else
                        {
                            regions.ForEach(r =>
                            {
                                if (r.ChildRegions != null)
                                {
                                    var region = r.ChildRegions.Where(c => c.RegionId == s.RegionId)?.FirstOrDefault();
                                    if (region != null)
                                    {
                                        s.ProvinceName = r.RegionName;
                                        s.CityName = region.RegionName;
                                        return;
                                    }
                                }
                            });
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error("SelectBaoYangActivityRegion", ex);
            }
            return result;
        }

        /// <summary>
        /// 批量新增或更新保养活动地区配置
        /// </summary>
        /// <param name="regionIds"></param>
        /// <param name="activityId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public Tuple<bool, bool> MultiAddOrEditBaoYangActivityRegion(List<int> regionIds, string activityId, string user)
        {
            var result = false;
            var cacheResult = false;
            try
            {
                if (regionIds != null && regionIds.Any() && !string.IsNullOrWhiteSpace(activityId))
                {
                    var insertModels = new List<BaoYangActivityVehicleAndRegionModel>();
                    var updateModels = new List<BaoYangActivityVehicleAndRegionModel>();
                    var logData = new List<BaoYangOprLog>();
                    foreach (var regionId in regionIds)
                    {
                        var oldValue = GetBaoYangActivityRegionByRegionId(regionId);
                        if (oldValue == null)
                        {
                            var insertModel = new BaoYangActivityVehicleAndRegionModel
                            {
                                VehicleId = null,
                                ActivityId = activityId,
                                RegionId = regionId,
                                Type = "Region",
                                CreateDateTime = DateTime.Now,
                                LastUpdateDateTime = DateTime.Now
                            };
                            insertModels.Add(insertModel);
                        }
                        else
                        {
                            updateModels.Add(oldValue);
                        }
                    }
                    dbScopeManagerGungnir.CreateTransaction(conn =>
                    {
                        foreach (var insertModel in insertModels)
                        {
                            var pkid = DalBaoYangActivitySetting.AddBaoYangActivityRegion(conn, insertModel);
                            if (pkid < 1)
                            {
                                throw new Exception($"AddBaoYangActivityRegion失败,{JsonConvert.SerializeObject(insertModel)}");
                            }
                            insertModel.PKID = pkid;
                            var log = new BaoYangOprLog
                            {
                                LogType = "BaoYangActivityRegion",
                                IdentityID = insertModel.RegionId.ToString(),
                                OldValue = null,
                                NewValue = JsonConvert.SerializeObject(insertModel),
                                Remarks = "Add",
                                OperateUser = user,
                            };
                            logData.Add(log);
                        }
                        foreach (var oldValue in updateModels)
                        {
                            var updateModel = new BaoYangActivityVehicleAndRegionModel
                            {
                                PKID=oldValue.PKID,
                                VehicleId = oldValue.VehicleId,
                                ActivityId = activityId,
                                RegionId = oldValue.RegionId,
                                Type = oldValue.Type,
                                CreateDateTime = oldValue.CreateDateTime,
                                LastUpdateDateTime = DateTime.Now
                            };
                            var updateResult = DalBaoYangActivitySetting.UpdateBaoYangActivityRegion(conn, updateModel);
                            if (!updateResult)
                            {
                                throw new Exception($"UpdateBaoYangActivityRegion失败,{JsonConvert.SerializeObject(updateModel)}");
                            }

                            var log = new BaoYangOprLog
                            {
                                LogType = "BaoYangActivityRegion",
                                IdentityID = updateModel.RegionId.ToString(),
                                OldValue = JsonConvert.SerializeObject(oldValue),
                                NewValue = JsonConvert.SerializeObject(updateModel),
                                Remarks = "Update",
                                OperateUser = user,
                            };
                            logData.Add(log);
                        }
                        result = true;
                    });
                    foreach (var log in logData)
                    {
                        LoggerManager.InsertLog("BYOprLog", log);
                    }
                    cacheResult = CleanBaoYangActivityRegionCache(regionIds);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("MultiAddOrEditBaoYangActivityRegion", ex);
            }
            return Tuple.Create(result, cacheResult);
        }

        /// <summary>
        /// 批量删除保养活动地区配置
        /// </summary>
        /// <param name="regionIds"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public Tuple<bool, bool> MultDeleteBaoYangActivityRegion(List<int> regionIds, string user)
        {
            var result = false;
            var cacheResult = false;
            try
            {
                var logData = new List<BaoYangOprLog>();
                if (regionIds != null && regionIds.Any())
                {
                    dbScopeManagerGungnir.CreateTransaction(conn =>
                    {
                        foreach (var regionId in regionIds)
                        {
                            if (regionId > 0)
                            {
                                var oldValue = GetBaoYangActivityRegionByRegionId(regionId);
                                if (oldValue != null)
                                {
                                    var delResult = DalBaoYangActivitySetting.DeleteBaoYangActivityRegion(conn, regionId);
                                    if (!delResult)
                                    {
                                        throw new Exception($"DeleteBaoYangActivityRegion失败,{JsonConvert.SerializeObject(oldValue)}");
                                    }
                                    var log = new BaoYangOprLog
                                    {
                                        LogType = "BaoYangActivityRegion",
                                        IdentityID = regionId.ToString(),
                                        OldValue = JsonConvert.SerializeObject(oldValue),
                                        NewValue = null,
                                        Remarks = "Delete",
                                        OperateUser = user,
                                    };
                                    logData.Add(log);
                                }
                            }
                        }
                        result = true;
                    });
                    foreach (var log in logData)
                    {
                        LoggerManager.InsertLog("BYOprLog", log);
                    }
                    cacheResult = CleanBaoYangActivityRegionCache(regionIds);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("MultDeleteBaoYangActivityRegion", ex);
            }
            return Tuple.Create(result, cacheResult);
        }

        /// <summary>
        /// 根据地区Id获取保养活动地区配置
        /// </summary>
        /// <param name="regionId"></param>
        /// <returns></returns>
        public BaoYangActivityVehicleAndRegionModel GetBaoYangActivityRegionByRegionId(int regionId)
        {
            var result = null as BaoYangActivityVehicleAndRegionModel;
            try
            {
                if (regionId > 0)
                {
                    result = dbScopeManagerGungnirRead.Execute(conn => DalBaoYangActivitySetting.GetBaoYangActivityRegionByRegionId(conn, regionId));
                }
            }
            catch (Exception ex)
            {
                Logger.Error("GetBaoYangActivityRegionByRegionId", ex);
            }
            return result;
        }

        #endregion

        #region 查询日志

        public List<BaoYangOprLog> GetBaoYangOprLogByIdentityIdAndType(string identityId, string type)
        {
            var list = null as List<BaoYangOprLog>;
            try
            {
                list = dbScopeManagerTuhuLog.Execute(conn =>
                DalBaoYangActivitySetting.GetBaoYangOprLogByIdentityIdAndType(conn, identityId, type));
            }
            catch (Exception ex)
            {
                Logger.Error("GetBaoYangOprLogByIdentityIdAndType", ex);
            }
            return list;
        }

        #endregion

        #region 清除缓存

        /// <summary>
        /// 清除保养活动车型配置服务缓存
        /// </summary>
        /// <param name="vehicleIds"></param>
        /// <returns></returns>
        public bool CleanBaoYangActivityVehicleCache(List<string> vehicleIds)
        {
            var result = true;
            try
            {
                if (vehicleIds != null && vehicleIds.Any())
                {
                    var keyPrefix = "BaoYangActivityIdForVehicle/";
                    var manager = new BaoYangManager();
                    var keys = vehicleIds.Select(s => $"{keyPrefix}{s}").ToList();
                    result = manager.CleanBaoYangCaches(keys);
                }
            }
            catch (Exception ex)
            {
                result = false;
                Logger.Error("CleanBaoYangActivityVehicleCache", ex);
            }
            return result;
        }

        /// <summary>
        /// 清除保养活动地区配置服务缓存
        /// </summary>
        /// <param name="regionIds"></param>
        /// <returns></returns>
        public bool CleanBaoYangActivityRegionCache(List<int> regionIds)
        {
            var result = true;
            try
            {
                if (regionIds != null && regionIds.Any())
                {
                    var keyPrefix = "BaoYangActivityIdForRegion/";
                    var manager = new BaoYangManager();
                    var keys = regionIds.Select(s => $"{keyPrefix}{s}").ToList();
                    result = manager.CleanBaoYangCaches(keys);
                }
            }
            catch (Exception ex)
            {
                result = false;
                Logger.Error("CleanBaoYangActivityRegionCache", ex);
            }
            return result;
        }

        /// <summary>
        /// 清除保养活动配置缓存
        /// </summary>
        /// <param name="activityNum"></param>
        /// <returns></returns>
        public bool CleanBaoYangActivitySettingCache(string activityNum)
        {
            var result = true;
            try
            {
                if (!string.IsNullOrWhiteSpace(activityNum))
                {
                    var keyPrefix = "BaoYangActivitySetting/";
                    var manager = new BaoYangManager();
                    var keys = new List<string>() { $"{keyPrefix}{activityNum}" };
                    result = manager.CleanBaoYangCaches(keys);
                }
            }
            catch (Exception ex)
            {
                result = false;
                Logger.Error("CleanBaoYangActivitySettingCache", ex);
            }
            return result;
        }

        #endregion

        public List<BaoYangPackage> GetBaoYangPackageDescription()
        {
            var result = null as List<BaoYangPackage>;
            try
            {
                using (var client = new BaoYangClient())
                {
                    var clientResult = client.GetBaoYangPackageDescription();
                    clientResult.ThrowIfException(true);
                    var packageDescription = clientResult.Result;
                    result = packageDescription?.Select(s => new BaoYangPackage()
                    {
                        Type = s.PackageType,
                        Name = s.ZhName,
                        Items = string.Join(",", s.Items.Select(v => v.BaoYangType))
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("GetBaoYangPackageDescription", ex);
            }
            return result;
        }

        /// <summary>
        /// 获取保养类型描述
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public BaoYangPackageDescription GetBaoYangPackageDescriptionByType(string type)
        {
            var result = null as BaoYangPackageDescription;
            try
            {
                using (var client = new BaoYangClient())
                {
                    var clientResult = client.GetBaoYangPackageDescription();
                    clientResult.ThrowIfException(true);
                    var packageDescription = clientResult.Result;
                    result = packageDescription?.FirstOrDefault(s => string.Equals(type, s.PackageType));
                }
            }
            catch (Exception ex)
            {
                Logger.Error("GetBaoYangPackageDescriptionByType", ex);
            }
            return result;
        }
    }
}
