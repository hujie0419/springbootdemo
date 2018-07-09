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

namespace Tuhu.Provisioning.Business.BaoYangOilViscosityPriorityConfig
{
    public class BaoYangOilViscosityPriorityConfigManager
    {
        private static readonly Common.Logging.ILog Logger;
        private static readonly IConnectionManager gungnirConn =
      new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);
        private static readonly IConnectionManager gungnirReadConn =
      new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString);
        private static readonly IDBScopeManager dbScopeManagerGungnirRead;
        private static readonly IDBScopeManager dbScopeManagerGungnir;

        static BaoYangOilViscosityPriorityConfigManager()
        {
            Logger = Common.Logging.LogManager.GetLogger(typeof(BaoYangOilViscosityPriorityConfigManager));
            dbScopeManagerGungnir = new DBScopeManager(gungnirConn);
            dbScopeManagerGungnirRead = new DBScopeManager(gungnirReadConn);
        }

        #region 机油粘度优先级配置
        /// <summary>
        /// 添加或编辑机油粘度优先级配置
        /// </summary>
        /// <param name="model"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool AddOrEditBaoYangOilViscosityPriorityConfig(BaoYangOilViscosityPriorityConfigModel model, string user)
        {
            var result = false;
            var log = null as BaoYangOprLog;
            try
            {
                var oldValue = dbScopeManagerGungnirRead.Execute(conn => DalBaoYangOilViscosityPriorityConfig.GetBaoYangOilViscosityPriorityConfig(conn, model.OriginViscosity, model.ConfigType));
                #region 添加 判断是否存在该原厂粘度被逻辑删除的记录 存在则更新 不存在则插入
                if (oldValue == null)
                {
                    #region 插入记录
                    var pkid = dbScopeManagerGungnir.Execute(conn => DalBaoYangOilViscosityPriorityConfig.AddBaoYangOilViscosityPriorityConfig(conn, model));
                    if (pkid > 0)
                    {
                        result = true;
                        model.PKID = pkid;
                        log = new BaoYangOprLog
                        {
                            LogType = "BaoYangOilViscosityPriorityConfig",
                            IdentityID = model.OriginViscosity,
                            OldValue = null,
                            NewValue = JsonConvert.SerializeObject(model),
                            Remarks = $"添加{model.ConfigType}类原厂机油粘度{model.OriginViscosity}推荐优先级:{model.ViscosityPriority}",
                            OperateUser = user,
                        };
                    }
                    #endregion
                }
                else if (oldValue.IsDeleted)
                {
                    #region 更新逻辑删除的记录
                    result = dbScopeManagerGungnir.Execute(conn => DalBaoYangOilViscosityPriorityConfig.UpdateBaoYangOilViscosityPriorityConfig(conn, model));
                    log = new BaoYangOprLog
                    {
                        LogType = "BaoYangOilViscosityPriorityConfig",
                        IdentityID = model.OriginViscosity,
                        OldValue = null,
                        NewValue = JsonConvert.SerializeObject(model),
                        Remarks = $"添加{model.ConfigType}类原厂机油粘度{model.OriginViscosity}推荐优先级:{model.ViscosityPriority}",
                        OperateUser = user,
                    };
                    #endregion
                }
                #endregion
                #region 更新
                else
                {
                    
                    result = dbScopeManagerGungnir.Execute(conn => DalBaoYangOilViscosityPriorityConfig.UpdateBaoYangOilViscosityPriorityConfig(conn, model));
                    log = new BaoYangOprLog
                    {
                        LogType = "BaoYangOilViscosityPriorityConfig",
                        IdentityID = model.OriginViscosity,
                        OldValue = JsonConvert.SerializeObject(oldValue),
                        NewValue = JsonConvert.SerializeObject(model),
                        Remarks = $"更新{model.ConfigType}类原厂机油粘度{model.OriginViscosity}推荐优先级:{model.ViscosityPriority}",
                        OperateUser = user,
                    };
                }
                #endregion
                if (result && log != null)
                {
                    LoggerManager.InsertLog("BYOprLog", log);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("AddOrEditBaoYangOilViscosityPriorityConfig", ex);
            }
            return result;
        }

        /// <summary>
        /// 删除保养项目加价配置 --只做逻辑删除
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool DeleteBaoYangOilViscosityPriorityConfig(string originViscosity, string configType, string user)
        {
            var result = false;
            try
            {
                var oldValue = dbScopeManagerGungnirRead.Execute(conn => DalBaoYangOilViscosityPriorityConfig.GetBaoYangOilViscosityPriorityConfig(conn, originViscosity, configType));
                if (oldValue != null && !oldValue.IsDeleted)
                {
                    result = dbScopeManagerGungnir.Execute(conn => DalBaoYangOilViscosityPriorityConfig.DeleteBaoYangOilViscosityPriorityConfig(conn, oldValue.PKID));
                    if (!result)
                    {
                        throw new Exception($"DeleteBaoYangOilViscosityPriorityConfig失败,待删除数据{JsonConvert.SerializeObject(oldValue)}");
                    }
                    var log = new BaoYangOprLog
                    {
                        LogType = "BaoYangOilViscosityPriorityConfig",
                        IdentityID = oldValue.OriginViscosity,
                        OldValue = JsonConvert.SerializeObject(oldValue),
                        NewValue = null,
                        Remarks = $"删除{oldValue.ConfigType}类原厂机油粘度{oldValue.OriginViscosity}推荐优先级:{oldValue.ViscosityPriority}",
                        OperateUser = user,
                    };
                    LoggerManager.InsertLog("BYOprLog", log);
                }
                else
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Logger.Error("DeleteBaoYangOilViscosityPriorityConfig", ex);
            }
            return result;
        }

        /// <summary>
        /// 查询机油粘度优先级配置表
        /// </summary>
        /// <param name="originViscosity"></param>
        /// <param name="configType"></param>
        /// <returns></returns>
        public List<BaoYangOilViscosityPriorityConfigModel> SelectBaoYangOilViscosityPriorityConfig(string originViscosity, string configType)
        {
            var result = new List<BaoYangOilViscosityPriorityConfigModel>();
            try
            {
                var configs = dbScopeManagerGungnirRead.Execute(conn =>
                      DalBaoYangOilViscosityPriorityConfig.SelectBaoYangOilViscosityPriorityConfig(conn, originViscosity, configType))
                      ?? new List<BaoYangOilViscosityPriorityConfigModel>();
                var allViscosity = string.IsNullOrWhiteSpace(originViscosity) ? new BaoYangConfigManager().ViscosityList() : new List<string>() { originViscosity };
                allViscosity?.ForEach(s =>
                {
                    var config = configs.Where(c => string.Equals(c.OriginViscosity, s)).FirstOrDefault();
                    if (config == null)
                    {
                        result.Add(new BaoYangOilViscosityPriorityConfigModel()
                        {
                            OriginViscosity = s,
                            ConfigType = configType
                        });
                    }
                    else
                    {
                        result.Add(config);
                    }
                });
            }
            catch (Exception ex)
            {
                result = null;
                Logger.Error("SelectBaoYangOilViscosityPriorityConfig", ex);
            }
            return result;
        }

        /// <summary>
        /// 清除机油粘度优先级配置服务缓存
        /// </summary>
        /// <param name="regionIds"></param>
        /// <returns></returns>
        public bool RemoveOilViscosityPriorityConfigCache(string configType,string originViscosity)
        {
            var result = false;
            try
            {
                if (!string.IsNullOrWhiteSpace(configType) && !string.IsNullOrWhiteSpace(originViscosity))
                {
                    var cacheKey = "BaoYangOilViscosityPriorityConfig/{0}/{1}";
                    var manager = new BaoYangManager();
                    var key = string.Format(cacheKey, configType, originViscosity);
                    result = manager.CleanBaoYangCaches(new List<string> { key });
                }
            }
            catch (Exception ex)
            {
                result = false;
                Logger.Error("RemoveOilViscosityPriorityConfigCache", ex);
            }
            return result;
        }
        #endregion

        #region 机油粘度特殊地区配置

        public Dictionary<string, List<string>> GetAllGroupedOilViscosity()
        {
            var viscosities = new BaoYangConfigManager().ViscosityList();
            viscosities = viscosities.Where(s => !string.IsNullOrEmpty(s) && s.Contains("-")).ToList();
            return viscosities.GroupBy(s => s.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries)[0]).ToDictionary(v => v.Key, v => v.ToList());
        }

        /// <summary>
        /// 查询保养活动地区配置
        /// </summary>
        /// <param name="regionIds"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<OilViscosityRegionModel> SelectOilViscosityRegionConfig(List<int> regionIds, int pageIndex, int pageSize)
        {
            var result = null as List<OilViscosityRegionModel>;
            try
            {
                result = dbScopeManagerGungnirRead.Execute(conn => DalBaoYangOilViscosityPriorityConfig.SelectOilViscosityRegionConfig(conn, regionIds, pageIndex, pageSize));
                var regions = new BaoYangActivitySettingManager().GetAllRegion();
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
                Logger.Error("SelectOilViscosityRegionConfig", ex);
            }
            return result;
        }

        /// <summary>
        /// 批量新增或更新机油粘度特殊地区配置
        /// </summary>
        /// <param name="regionIds"></param>
        /// <param name="oilViscosity"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool MultAddOrEditOilViscosityRegionConfig(List<int> regionIds, string oilViscosity, string user)
        {
            var result = false;
            try
            {
                if (regionIds != null && regionIds.Any() && !string.IsNullOrWhiteSpace(oilViscosity))
                {
                    var insertModels = new List<OilViscosityRegionModel>();
                    var backupModels = new List<OilViscosityRegionModel>();
                    var updateModels = new List<OilViscosityRegionModel>();
                    var logData = new List<BaoYangOprLog>();
                    foreach (var regionId in regionIds)
                    {
                        var oldValue = GetOilViscosityRegionConfigByRegionId(regionId);
                        if (oldValue == null)
                        {
                            var insertModel = new OilViscosityRegionModel
                            {
                                RegionId = regionId,
                                OilViscosity = oilViscosity,
                                CreateDateTime = DateTime.Now,
                                LastUpdateDateTime = DateTime.Now
                            };
                            insertModels.Add(insertModel);
                        }
                        else
                        {
                            if (oldValue.IsDeleted)
                            {
                                backupModels.Add(oldValue);
                            }
                            else
                            {
                                updateModels.Add(oldValue);
                            }
                        }
                    }
                    dbScopeManagerGungnir.CreateTransaction(conn =>
                    {
                        foreach (var insertModel in insertModels)
                        {
                            var pkid = DalBaoYangOilViscosityPriorityConfig.AddOilViscosityRegionConfig(conn, insertModel);
                            if (pkid < 1)
                            {
                                throw new Exception($"AddOilViscosityRegionConfig失败,{JsonConvert.SerializeObject(insertModel)}");
                            }
                            insertModel.PKID = pkid;
                            var log = new BaoYangOprLog
                            {
                                LogType = "OilViscosityRegionConfig",
                                IdentityID = insertModel.RegionId.ToString(),
                                OldValue = null,
                                NewValue = JsonConvert.SerializeObject(insertModel),
                                Remarks = $"添加机油粘度配置:{oilViscosity}",
                                OperateUser = user,
                            };
                            logData.Add(log);
                        }
                        foreach (var oldValue in updateModels)
                        {
                            var updateModel = new OilViscosityRegionModel
                            {
                                PKID = oldValue.PKID,
                                RegionId = oldValue.RegionId,
                                OilViscosity=oilViscosity,
                                CreateDateTime = oldValue.CreateDateTime,
                                LastUpdateDateTime = DateTime.Now,
                                IsDeleted=false
                            };
                            var updateResult = DalBaoYangOilViscosityPriorityConfig.UpdateOilViscosityRegionConfig(conn, updateModel);
                            if (!updateResult)
                            {
                                throw new Exception($"UpdateOilViscosityRegionConfig失败,{JsonConvert.SerializeObject(updateModel)}");
                            }

                            var log = new BaoYangOprLog
                            {
                                LogType = "OilViscosityRegionConfig",
                                IdentityID = updateModel.RegionId.ToString(),
                                OldValue = JsonConvert.SerializeObject(oldValue),
                                NewValue = JsonConvert.SerializeObject(updateModel),
                                Remarks = $"更新机油粘度配置:{oilViscosity}",
                                OperateUser = user,
                            };
                            logData.Add(log);
                        }
                        foreach (var backupModel in backupModels)
                        {
                            var updateModel = new OilViscosityRegionModel
                            {
                                PKID = backupModel.PKID,
                                RegionId = backupModel.RegionId,
                                OilViscosity = oilViscosity,
                                CreateDateTime = backupModel.CreateDateTime,
                                LastUpdateDateTime = DateTime.Now,
                                IsDeleted=false
                            };
                            var updateResult = DalBaoYangOilViscosityPriorityConfig.UpdateOilViscosityRegionConfig(conn, updateModel);
                            if (!updateResult)
                            {
                                throw new Exception($"UpdateOilViscosityRegionConfig失败,{JsonConvert.SerializeObject(updateModel)}");
                            }

                            var log = new BaoYangOprLog
                            {
                                LogType = "OilViscosityRegionConfig",
                                IdentityID = updateModel.RegionId.ToString(),
                                OldValue = JsonConvert.SerializeObject(backupModel),
                                NewValue = JsonConvert.SerializeObject(updateModel),
                                Remarks = $"添加机油粘度配置:{oilViscosity}",
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
                }
            }
            catch (Exception ex)
            {
                Logger.Error("MultiAddOrEditOilViscosityRegionConfig", ex);
            }
            return result;
        }

        /// <summary>
        /// 批量删除机油粘度特殊地区配置
        /// </summary>
        /// <param name="regionIds"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool MultDeleteOilViscosityRegionConfig(List<int> regionIds, string user)
        {
            var result = false;
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
                                var oldValue = GetOilViscosityRegionConfigByRegionId(regionId);
                                if (oldValue != null && !oldValue.IsDeleted)
                                {
                                    var delResult = DalBaoYangOilViscosityPriorityConfig.DeleteOilViscosityRegionConfig(conn, regionId);
                                    if (!delResult)
                                    {
                                        throw new Exception($"DeleteOilViscosityRegionConfig失败,{JsonConvert.SerializeObject(oldValue)}");
                                    }
                                    var log = new BaoYangOprLog
                                    {
                                        LogType = "OilViscosityRegionConfig",
                                        IdentityID = regionId.ToString(),
                                        OldValue = JsonConvert.SerializeObject(oldValue),
                                        NewValue = null,
                                        Remarks = $"删除机油粘度配置:{oldValue.OilViscosity}",
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
                }
            }
            catch (Exception ex)
            {
                Logger.Error("MultDeleteOilViscosityRegionConfig", ex);
            }
            return result;
        }

        /// <summary>
        /// 根据地区Id获取机油粘度特殊地区配置
        /// </summary>
        /// <param name="regionId"></param>
        /// <returns></returns>
        public OilViscosityRegionModel GetOilViscosityRegionConfigByRegionId(int regionId)
        {
            var result = null as OilViscosityRegionModel;
            try
            {
                if (regionId > 0)
                {
                    result = dbScopeManagerGungnirRead.Execute(conn => DalBaoYangOilViscosityPriorityConfig.GetOilViscosityRegionConfigByRegionId(conn, regionId));
                }
            }
            catch (Exception ex)
            {
                Logger.Error("GetOilViscosityRegionConfigByRegionId", ex);
            }
            return result;
        }

        /// <summary>
        /// 清除机油粘度特殊地区配置服务缓存
        /// </summary>
        /// <param name="regionIds"></param>
        /// <returns></returns>
        public bool RemoveOilViscosityRegionConfigCache(List<int> regionIds)
        {
            var result = true;
            try
            {
                if (regionIds != null && regionIds.Any())
                {
                    var keyPrefix = "OilViscosityRegionConfig/";
                    var manager = new BaoYangManager();
                    var keys = regionIds.Select(s => $"{keyPrefix}{s}").ToList();
                    result = manager.CleanBaoYangCaches(keys);
                }
            }
            catch (Exception ex)
            {
                result = false;
                Logger.Error("RemoveOilViscosityRegionConfigCache", ex);
            }
            return result;
        }
        #endregion
    }
}
