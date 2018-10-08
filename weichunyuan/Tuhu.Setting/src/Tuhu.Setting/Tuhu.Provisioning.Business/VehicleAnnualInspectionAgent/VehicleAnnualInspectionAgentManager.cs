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
                    $"服务Pid:{model.ServicePid},联系人:{model.Contact},联系电话:{model.TelNum}," +
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
                        $"联系人:{oldValue.Contact},联系电话:{oldValue.TelNum}的年检代办服务",
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
                        $"联系人:{model.Contact},联系电话{model.TelNum}的{model.ServiceName}服务",
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
    }
}
