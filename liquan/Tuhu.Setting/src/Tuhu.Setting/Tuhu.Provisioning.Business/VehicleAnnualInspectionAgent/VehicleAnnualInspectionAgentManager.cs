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
using Tuhu.Service.Shop;
using Tuhu.Service.Vender.Models;

namespace Tuhu.Provisioning.Business.VehicleAnnualInspectionAgent
{
    public class VehicleAnnualInspectionAgentManager
    {
        private static readonly Common.Logging.ILog Logger;
        private static readonly IConnectionManager gungnirReadConn =
      new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString);
        private static readonly IConnectionManager configurationConn =
     new ConnectionManager(ConfigurationManager.ConnectionStrings["Configuration"].ConnectionString);
        private static readonly IConnectionManager configurationReadConn =
      new ConnectionManager(ConfigurationManager.ConnectionStrings["Configuration_AlwaysOnRead"].ConnectionString);
        private static readonly IConnectionManager productcatalogReadConn =
      new ConnectionManager(ConfigurationManager.ConnectionStrings["Tuhu_productcatalog_AlwaysOnRead"].ConnectionString);
        private static readonly IConnectionManager tuhuLogConn =
     new ConnectionManager(ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString);
        private static readonly IDBScopeManager dbScopeManagerGungnirRead;
        private static readonly IDBScopeManager dbScopeManagerConfigurationRead;
        private static readonly IDBScopeManager dbScopeManagerConfiguration;
        private static readonly IDBScopeManager dbScopeManagerProductcatalogRead;
        private static readonly IDBScopeManager dbScopeManagerTuhulog;

        private static List<InspectionRegion> Cities = new List<InspectionRegion>() {
            new InspectionRegion(){
                RegionId=1,
                RegionName="上海市"
            },
            new InspectionRegion(){
                RegionId=2,
                RegionName="北京市"
            },
            new InspectionRegion(){
                RegionId=19,
                RegionName="天津市"
            },
            new InspectionRegion(){
                RegionId=20,
                RegionName="重庆市"
            },
        };

        static VehicleAnnualInspectionAgentManager()
        {
            Logger = LogManager.GetLogger(typeof(VehicleAnnualInspectionAgentManager));
            dbScopeManagerGungnirRead = new DBScopeManager(gungnirReadConn);
            dbScopeManagerConfiguration = new DBScopeManager(configurationConn);
            dbScopeManagerConfigurationRead = new DBScopeManager(configurationReadConn);
            dbScopeManagerProductcatalogRead = new DBScopeManager(productcatalogReadConn);
            dbScopeManagerTuhulog = new DBScopeManager(tuhuLogConn);
        }

        /// <summary>
        /// 添加年检代办配置
        /// </summary>
        /// <param name="model"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool AddVehicleAnnualInspectionAgent(VehicleAnnualInspectionAgentModel model, string user)
        {
            var result = false;
            try
            {
                var oldValue = GetVehicleAnnualInspectionAgent(model.ServicePid, model.TelNum, model.CarNoPrefix);
                if (oldValue == null)
                {
                    var pkid = dbScopeManagerConfiguration.Execute
                        (conn => DalVehicleAnnualInspectionAgent.AddVehicleAnnualInspectionAgent(conn, model));
                    result = pkid > 0;
                    model.PKID = pkid;
                }
                else if (oldValue.IsDeleted)
                {
                    model.PKID = oldValue.PKID;
                    result = dbScopeManagerConfiguration.Execute
                        (conn => DalVehicleAnnualInspectionAgent.UpdateVehicleAnnualInspectionAgent(conn, model));
                }
                model.CreateDateTime = DateTime.Now;
                model.LastUpdateDateTime = DateTime.Now;
                var log = new AnnualInspectionOprLogModel
                {
                    LogType = "VehicleAnnualInspectionAgent",
                    IdentityId = $"{model.CarNoPrefix}_{model.ServicePid}_{model.TelNum}",
                    OperationType = "Add",
                    OldValue = null,
                    NewValue = JsonConvert.SerializeObject(model),
                    Remarks = $"添加{model.CarNoPrefix},供应商:{model.VenderShortName}," +
                    $"服务Pid:{model.ServicePid},联系人:{model.Contact},联系电话:{model.TelNum} " +
                    $"的{model.ServiceName}服务",
                    Operator = user,
                };
                LoggerManager.InsertLog("AnnualInspectOprLog", log);
            }
            catch (Exception ex)
            {
                result = false;
                Logger.Error("AddVehicleAnnualInspectionAgent", ex);
            }
            return result;
        }

        public bool AddVehicleOnlineAnnualInspectionAgent(VehicleAnnualInspectionAgentModel model, string user)
        {
            var result = false;
            try
            {

                var oldValue = GetVehicleOnlineAnnualInspectionAgent(model.ServicePid, model.Province, model.City,model.Village);
                if (oldValue == null)
                {
                    var pkid = dbScopeManagerConfiguration.Execute
                        (conn => DalVehicleAnnualInspectionAgent.AddVehicleOnlineAnnualInspectionAgent(conn, model));
                    result = pkid > 0;
                    model.PKID = pkid;
                }
                else if (oldValue.IsDeleted)
                {
                    model.PKID = oldValue.PKID;
                    result = dbScopeManagerConfiguration.Execute
                        (conn => DalVehicleAnnualInspectionAgent.UpdateVehicleOnlineAnnualInspectionAgent(conn, model));
                }

                model.CreateDateTime = DateTime.Now;
                model.LastUpdateDateTime = DateTime.Now;
                var log = new AnnualInspectionOprLogModel
                {
                    LogType = "VehicleOnlineAnnualInspectionAgent",
                    IdentityId = $"{model.Province}_{model.City}_{model.Village}",
                    OperationType = "Add",
                    OldValue = null,
                    NewValue = JsonConvert.SerializeObject(model),
                    Remarks = $"添加{model.Province}_{model.City}_{model.Village},供应商:{model.VenderShortName}," +
                    $"服务Pid:{model.ServicePid},联系电话:{model.TelNum},联系邮箱:{model.Email} " +
                    $"的{model.ServiceName}服务",
                    Operator = user,
                };
                LoggerManager.InsertLog("AnnualInspectOprLog", log);
            }
            catch (Exception ex)
            {
                result = false;
                Logger.Error("AddVehicleOnlineAnnualInspectionAgent", ex);
            }
            return result;
        }


        /// <summary>
        /// 删除年检代办配置
        /// </summary>
        /// <param name="carNoPrefix"></param>
        /// <param name="telNum"></param>
        /// <param name="servicePid"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool DeleteVehicleAnnualInspectionAgent
            (string servicePid, string telNum, string carNoPrefix, string user)
        {
            var result = false;
            try
            {
                var oldValue = GetVehicleAnnualInspectionAgent(servicePid, telNum, carNoPrefix);
                if (oldValue != null && !oldValue.IsDeleted)
                {
                    result = dbScopeManagerConfiguration.Execute(conn => DalVehicleAnnualInspectionAgent.DeleteVehicleAnnualInspectionAgent(conn, oldValue.PKID));
                    if (!result)
                    {
                        throw new Exception($"DeleteVehicleAnnualInspectionAgent失败,待删除数据{JsonConvert.SerializeObject(oldValue)}");
                    }
                    var log = new AnnualInspectionOprLogModel
                    {
                        LogType = "VehicleAnnualInspectionAgent",
                        IdentityId = $"{carNoPrefix}_{servicePid}_{telNum}",
                        OperationType = "Delete",
                        OldValue = JsonConvert.SerializeObject(oldValue),
                        NewValue = null,
                        Remarks = $"删除{oldValue.CarNoPrefix},供应商:{oldValue.VenderShortName},服务Pid:{oldValue.ServicePid}," +
                        $"联系人:{oldValue.Contact},联系电话:{oldValue.TelNum} 的年检代办服务",
                        Operator = user,
                    };
                    LoggerManager.InsertLog("AnnualInspectOprLog", log);
                }
                else
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Logger.Error("DeleteVehicleAnnualInspectionAgent", ex);
            }
            return result;
        }


        /// <summary>
        /// 删除年检代办配置
        /// </summary>
        /// <param name="carNoPrefix"></param>
        /// <param name="telNum"></param>
        /// <param name="servicePid"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool DeleteVehicleOnlineAnnualInspectionAgent
            (string servicePid, string province, string city, string village,string user)
        {
            var result = false;
            try
            {
                var oldValue = GetVehicleOnlineAnnualInspectionAgent(servicePid, province,city, village);
                if (oldValue != null && !oldValue.IsDeleted)
                {
                    result = dbScopeManagerConfiguration.Execute(conn => DalVehicleAnnualInspectionAgent.DeleteVehicleAnnualInspectionAgent(conn, oldValue.PKID));
                    if (!result)
                    {
                        throw new Exception($"DeleteVehicleOnlineAnnualInspectionAgent,待删除数据{JsonConvert.SerializeObject(oldValue)}");
                    }
                    var log = new AnnualInspectionOprLogModel
                    {
                        LogType = "VehicleAnnualInspectionAgent",
                        IdentityId = $"{province}_{city}_{village}",
                        OperationType = "Delete",
                        OldValue = JsonConvert.SerializeObject(oldValue),
                        NewValue = null,
                        Remarks = $"删除{oldValue.Province}_{oldValue.City}_{oldValue.Village},供应商:{oldValue.VenderShortName}," +
                    "服务Pid:{model.ServicePid},联系电话:{model.TelNum},联系邮箱:{model.Email}" +
                    "的{model.ServiceName}服务的年检代办服务",
                        Operator = user,
                    };
                    LoggerManager.InsertLog("AnnualInspectOprLog", log);
                }
                else
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Logger.Error("DeleteVehicleOnlineAnnualInspectionAgent", ex);
            }
            return result;
        }

        /// <summary>
        /// 更新年检代办配置
        /// </summary>
        /// <param name="model"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool UpdateVehicleAnnualInspectionAgent
            (VehicleAnnualInspectionAgentModel model, string user)
        {
            var result = false;
            try
            {
                var oldValue = GetVehicleAnnualInspectionAgent(model.ServicePid, model.TelNum, model.CarNoPrefix);
                if (oldValue != null)
                {
                    result = dbScopeManagerConfiguration.Execute(conn =>
                    DalVehicleAnnualInspectionAgent.UpdateVehicleAnnualInspectionAgent(conn, model));
                    if (!result)
                    {
                        throw new Exception($"UpdateVehicleAnnualInspectionAgent失败,待更新数据{JsonConvert.SerializeObject(model)}");
                    }
                    model.CreateDateTime = oldValue.CreateDateTime;
                    model.LastUpdateDateTime = DateTime.Now;
                    var log = new AnnualInspectionOprLogModel
                    {
                        LogType = "VehicleAnnualInspectionAgent",
                        IdentityId = $"{model.CarNoPrefix}_{model.ServicePid}_{model.TelNum}",
                        OperationType = "Update",
                        OldValue = JsonConvert.SerializeObject(oldValue),
                        NewValue = JsonConvert.SerializeObject(model),
                        Remarks = $"更新{model.CarNoPrefix},供应商:{model.VenderShortName},服务Pid:{model.ServicePid}," +
                        $"联系人:{model.Contact},联系电话:{model.TelNum}的{model.ServiceName}服务",
                        Operator = user,
                    };
                    LoggerManager.InsertLog("AnnualInspectOprLog", log);
                }
            }
            catch (Exception ex)
            {
                result = false;
                Logger.Error("UpdateVehicleAnnualInspectionAgent", ex);
            }
            return result;
        }


        /// <summary>
        /// 更新线上年检代办配置
        /// </summary>
        /// <param name="model"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool UpdateVehicleOnlineAnnualInspectionAgent
            (VehicleAnnualInspectionAgentModel model, string user)
        {
            var result = false;
            try
            {
                var oldValue = GetVehicleOnlineAnnualInspectionAgent(model.ServicePid,model.Province,model.City,model.Village);
                if (oldValue != null)
                {
                    result = dbScopeManagerConfiguration.Execute(conn =>
                    DalVehicleAnnualInspectionAgent.UpdateVehicleOnlineAnnualInspectionAgent(conn, model));
                    if (!result)
                    {
                        throw new Exception($"UpdateVehicleOnlineAnnualInspectionAgent失败,待更新数据{JsonConvert.SerializeObject(model)}");
                    }
                    model.CreateDateTime = oldValue.CreateDateTime;
                    model.LastUpdateDateTime = DateTime.Now;
                    var log = new AnnualInspectionOprLogModel
                    {
                        LogType = "VehicleAnnualOnlineInspectionAgent",
                        IdentityId = $"{model.Province}_{model.City}_{model.Village}",
                        OperationType = "Update",
                        OldValue = JsonConvert.SerializeObject(oldValue),
                        NewValue = JsonConvert.SerializeObject(model),
                        Remarks = $"更新{model.Province}_{model.City}_{model.Village},供应商:{model.VenderShortName},服务Pid:{model.ServicePid}," +
                        $"联系电话:{model.TelNum},联系邮箱:{model.Email} 的{model.ServiceName}服务",
                        Operator = user,
                    };
                    LoggerManager.InsertLog("AnnualInspectOprLog", log);
                }
            }
            catch (Exception ex)
            {
                result = false;
                Logger.Error("UpdateVehicleOnlineAnnualInspectionAgent", ex);
            }
            return result;
        }

        /// <summary>
        /// 查询年检代办配置
        /// </summary>
        /// <param name="servicePid"></param>
        /// <param name="carNoPrefix"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Tuple<List<VehicleAnnualInspectionAgentModel>, int> SelectVehicleAnnualInspectionAgent
            (string servicePid, string carNoPrefix, int pageIndex, int pageSize)
        {
            var result = null as List<VehicleAnnualInspectionAgentModel>;
            var totalCount = 0;
            try
            {
                var configs = dbScopeManagerConfigurationRead.Execute(conn =>
                      DalVehicleAnnualInspectionAgent.SelectVehicleAnnualInspectionAgent(conn, servicePid, carNoPrefix, pageIndex, pageSize, out totalCount))
                      ?? new List<VehicleAnnualInspectionAgentModel>();
                var allService = GetAllAnnualInspectionService();
                configs.ForEach(
                    f =>
                    {
                        var service = allService.Where(w => string.Equals(w.ServicePid, f.ServicePid))?.FirstOrDefault();
                        f.ServiceName = service?.ServiceName;
                        f.Area = !string.IsNullOrWhiteSpace(f.Province) ? f.Province + "_" + f.City + "_" + f.Village : "";
                    });
                result = configs;
            }
            catch (Exception ex)
            {
                result = null;
                Logger.Error("SelectVehicleAnnualInspectionAgent", ex);
            }
            return Tuple.Create(result, totalCount);
        }

        /// <summary>
        /// 获取所有年检代办服务
        /// </summary>
        /// <returns></returns>
        public List<AnnualInspectionServiceModel> GetAllAnnualInspectionService()
        {
            var result = null as List<AnnualInspectionServiceModel>;
            try
            {
                result = dbScopeManagerProductcatalogRead.Execute
                    (conn => DalVehicleAnnualInspectionAgent.GetAllAnnualInspectionService(conn));
            }
            catch (Exception ex)
            {
                Logger.Error("GetAllAnnualInspectionService", ex);
            }
            return result ?? new List<AnnualInspectionServiceModel>();
        }


        /// <summary>
        /// 获取所有年检省份
        /// </summary>
        /// <returns></returns>
        public List<InspectionRegion> GetAllInspectionRegion()
        {
            var result = null as List<InspectionRegion>;
            try
            {
                using (var client = new RegionClient())
                {
                    var regionresponse=client.GetAllProvince();
                    if (regionresponse != null && regionresponse.Result != null)
                    {
                        result = regionresponse.Result.Select(p => new InspectionRegion()
                        {
                            RegionId = p.RegionId,
                            RegionName = p.RegionName
                        }).ToList();
                    }
                }
                    
            }
            catch (Exception ex)
            {
                Logger.Error("GetAllInspectionRegion", ex);
            }
            return result ?? new List<InspectionRegion>();
        }


        /// <summary>
        /// 获取所有年检所在的市 县
        /// </summary>
        /// <returns></returns>
        public List<InspectionRegion> GetAllInspectionCityByProvinceId(int provinceId)
        {
            var result = null as List<InspectionRegion>;
            //if (Cities.Select(p => p.RegionId).Contains(provinceId))
            //{
            //    result = Cities.Where(q => q.RegionId == provinceId).ToList();
            //    return result;                 
            //}
            try
            {
                using (var client = new RegionClient())
                {
                    var regionresponse = client.GetRegionByRegionId(provinceId);
                    if (regionresponse != null && regionresponse.Result != null && regionresponse.Result.ChildRegions.Any())
                    {
                        result = regionresponse.Result.ChildRegions.Select(p => new InspectionRegion()
                        {
                            RegionId = p.RegionId,
                            RegionName = p.RegionName
                        }).ToList();
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Error("GetAllInspectionCityByProvinceId", ex);
            }
            return result ?? new List<InspectionRegion>();
        }



        /// <summary>
        /// 获取所有年检所在的乡
        /// </summary>
        /// <returns></returns>
        public List<InspectionRegion> GetAllInspectionVillageByCityId(int cityId,int provinceId)
        {

            var result = null as List<InspectionRegion>;            

            try
            {
                using (var client = new RegionClient())
                {
                    var regionresponse = client.GetRegionByRegionId(cityId);
                    if (regionresponse != null && regionresponse.Result != null)
                    {
                        if (Cities.Select(p => p.RegionId).Contains(provinceId))
                        {
                            result = new List<InspectionRegion>() {new InspectionRegion()
                            {
                                RegionId=regionresponse.Result.RegionId,
                                RegionName=regionresponse.Result.RegionName
                            } };

                        }
                        else if (regionresponse.Result.ChildRegions.Any())
                        {
                            result = regionresponse.Result.ChildRegions.Select(p => new InspectionRegion()
                            {
                                RegionId = p.RegionId,
                                RegionName = p.RegionName
                            }).ToList();
                        }

                    }                                       
                }

            }
            catch (Exception ex)
            {
                Logger.Error("GetAllInspectionVillageByCityId", ex);
            }
            return result ?? new List<InspectionRegion>();
        }

        /// <summary>
        /// 获取所有年检代办供应商
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllAnnualInspectionVender()
        {
            var result = null as List<string>;
            try
            {
                using (var client = new Service.Config.ConfigClient())
                {
                    var clientResult = client.GetVehicleAnnualInspectionVenderList();
                    clientResult.ThrowIfException(true);
                    result = clientResult.Result?.Select(s => s.VenderShortName)?.ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("GetAllAnnualInspectionVender", ex);
            }
            return result ?? new List<string>();
        }

        /// <summary>
        /// 获取所有车牌前缀
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, List<string>> GetAllCarNoPrefix()
        {
            var result = null as Dictionary<string, List<string>>;
            try
            {
                var prefix = dbScopeManagerGungnirRead.Execute(conn => DalVehicleAnnualInspectionAgent.GetAllCarNoPrefix(conn));
                result = prefix.GroupBy(s => s.ProvinceName).ToDictionary(c => c.Key, c => c.Select(s => s.PlatePrefix).Distinct().ToList());
            }
            catch (Exception ex)
            {
                Logger.Error("GetAllCarNoPrefix", ex);
            }
            return result;
        }

        /// <summary>
        /// 判断年检配置唯一性
        /// 同PID同城市(车牌前缀)，只能有一条启用的数据
        /// PID、车牌前缀、供应商简称 作唯一性约束
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool IsExistAnnualInspectionAgent(VehicleAnnualInspectionAgentModel model)
        {
            var result = true;
            try
            {
                result = dbScopeManagerConfigurationRead.Execute
                     (conn => DalVehicleAnnualInspectionAgent.IsExistAnnualInspectionAgent(conn, model));
            }
            catch (Exception ex)
            {
                Logger.Error("IsExistAnnualInspectionAgent", ex);
            }
            return result;
        }

        /// <summary>
        /// 判断年检配置唯一性
        /// 同城市，只能有一条启用的数据       
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool IsExistOnlineAnnualInspectionAgent(VehicleAnnualInspectionAgentModel model)
        {
            var result = true;
            try
            {
                result = dbScopeManagerConfigurationRead.Execute
                     (conn => DalVehicleAnnualInspectionAgent.IsExistOnlineAnnualInspectionAgent(conn, model));
            }
            catch (Exception ex)
            {
                Logger.Error("IsExistOnlineAnnualInspectionAgent", ex);
            }
            return result;
        }

        /// <summary>
        /// 查询年检代办操作日志
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="identityId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<AnnualInspectionOprLogModel> SelectAnnualInspectionOprLog
            (string logType, string identityId, int pageIndex = 1, int pageSize = 20)
        {
            var result = null as List<AnnualInspectionOprLogModel>;
            try
            {
                result = dbScopeManagerTuhulog.Execute
                     (conn => DalVehicleAnnualInspectionAgent.SelectAnnualInspectionOprLog(conn, logType, identityId, pageIndex, pageSize));
            }
            catch (Exception ex)
            {
                Logger.Error("SelectAnnualInspectionOprLog", ex);
            }
            return result;
        }

        /// <summary>
        /// 刷新年检代办服务缓存
        /// </summary>
        /// <returns></returns>
        public bool RefreshAnnualInspectionAgentCache()
        {
            var result = false;
            try
            {
                var models = dbScopeManagerGungnirRead.Execute(conn => DalVehicleAnnualInspectionAgent.GetAllCarNoPrefix(conn));
                var carNoPrefix = models?.Select(s => s.PlatePrefix).ToList();
                if (carNoPrefix != null && carNoPrefix.Any())
                {
                    using (var client = new Service.Config.CacheClient())
                    {
                        var cacheResult = client.RefreshVehicleAnnualInspectionAgentCache(carNoPrefix);
                        cacheResult.ThrowIfException(true);
                        result = cacheResult.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("RefreshAnnualInspectionAgentCache", ex);
            }
            return result;
        }

        /// <summary>
        /// 获取年检代办配置
        /// </summary>
        /// <param name="servicePid"></param>
        /// <param name="telNum"></param>
        /// <param name="carNoPrefix"></param>
        /// <returns></returns>
        private VehicleAnnualInspectionAgentModel GetVehicleAnnualInspectionAgent
        (string servicePid, string telNum, string carNoPrefix)
        {
            return dbScopeManagerConfigurationRead.Execute
                     (conn => DalVehicleAnnualInspectionAgent.GetAnnualInspectionAgent(conn, servicePid, telNum, carNoPrefix));
        }


        private VehicleAnnualInspectionAgentModel GetVehicleOnlineAnnualInspectionAgent
      (string servicePid, string province, string city,string village)
        {
            return dbScopeManagerConfigurationRead.Execute
                     (conn => DalVehicleAnnualInspectionAgent.GetAnnualOnlineInspectionAgent(conn, servicePid, province, city, village));
        }
    }
}
