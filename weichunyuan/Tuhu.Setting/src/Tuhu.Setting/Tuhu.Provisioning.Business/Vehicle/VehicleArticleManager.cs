using Common.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.Vehicle
{
    public class VehicleArticleManager
    {
        private readonly Common.Logging.ILog Logger;
        private readonly IConnectionManager configurationConn =
     new ConnectionManager(ConfigurationManager.ConnectionStrings["Configuration"].ConnectionString);
        private readonly IConnectionManager configurationReadConn =
      new ConnectionManager(ConfigurationManager.ConnectionStrings["Configuration_AlwaysOnRead"].ConnectionString);
        private readonly IDBScopeManager dbScopeManagerConfigurationRead;
        private readonly IDBScopeManager dbScopeManagerConfiguration;
        private readonly string _user;

        public VehicleArticleManager(string user)
        {
            Logger = LogManager.GetLogger(typeof(VehicleArticleManager));
            dbScopeManagerConfiguration = new DBScopeManager(configurationConn);
            dbScopeManagerConfigurationRead = new DBScopeManager(configurationReadConn);
            _user = user;
        }

        /// <summary>
        /// 添加或更新选车攻略配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpSertVehicleArticle(VehicleArticleModel model)
        {
            var result = false;
            try
            {
                var oldValue = dbScopeManagerConfigurationRead.Execute(conn =>
                    DalVehicleArticle.GetVehicleArticle(conn, model));
                if (oldValue == null)
                {
                    model.CreateDateTime = DateTime.Now;
                    var pkid = dbScopeManagerConfiguration.Execute
                        (conn => DalVehicleArticle.AddVehicleArticle(conn, model));
                    result = pkid > 0;
                    if (result)
                    {
                        model.PKID = pkid;
                        model.LastUpdateDateTime = DateTime.Now;
                        var log = new VehicleArticleOprLogModel
                        {
                            LogType = "VehicleArticle",
                            IdentityId = $"{model.VehicleId}_{model.PaiLiang}_{model.Nian}",
                            OperationType = "Add",
                            OldValue = null,
                            NewValue = JsonConvert.SerializeObject(model),
                            Remarks = $"添加选车攻略:{model.VehicleId}_{model.PaiLiang}_{model.Nian}的配置",
                            Operator = _user,
                        };
                        LoggerManager.InsertLog("VehicleArticleOprLog", log);
                    }
                }
                else
                {
                    model.CreateDateTime = oldValue.CreateDateTime;
                    model.LastUpdateDateTime = DateTime.Now;
                    result = dbScopeManagerConfiguration.Execute(conn =>
                    DalVehicleArticle.UpdateVehicleArticle(conn, model));
                    if (result)
                    {
                        var log = new VehicleArticleOprLogModel
                        {
                            LogType = "VehicleArticle",
                            IdentityId = $"{model.VehicleId}_{model.PaiLiang}_{model.Nian}",
                            OperationType = "Update",
                            OldValue = JsonConvert.SerializeObject(oldValue),
                            NewValue = JsonConvert.SerializeObject(model),
                            Remarks = $"更新选车攻略:{ model.VehicleId }_{ model.PaiLiang }_{ model.Nian }的配置",
                            Operator = _user,
                        };
                        LoggerManager.InsertLog("VehicleArticleOprLog", log);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("UpSertVehicleArticle", ex);
            }
            return result;
        }

        /// <summary>
        /// 删除选车攻略配置
        /// </summary>
        /// <param name="model"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool DeleteVehicleArticle(VehicleArticleModel model)
        {
            var result = false;
            try
            {
                var oldValue = dbScopeManagerConfigurationRead.Execute(conn =>
                 DalVehicleArticle.GetVehicleArticleByPkid(conn, model.PKID));
                if (oldValue != null)
                {
                    result = dbScopeManagerConfiguration.Execute(conn =>
                    DalVehicleArticle.DeleteVehicleArticle(conn, oldValue.PKID));
                    var log = new VehicleArticleOprLogModel
                    {
                        LogType = "VehicleArticle",
                        IdentityId = $"{model.VehicleId}_{model.PaiLiang}_{model.Nian}",
                        OperationType = "Delete",
                        OldValue = JsonConvert.SerializeObject(oldValue),
                        NewValue = null,
                        Remarks = $"删除选车攻略：{ model.VehicleId }_{ model.PaiLiang }_{ model.Nian }的配置",
                        Operator = _user
                    };
                    LoggerManager.InsertLog("VehicleArticleOprLog", log);
                }
                else
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Logger.Error("DeleteVehicleArticle", ex);
            }
            return result;
        }

        /// <summary>
        /// 批量删除选车攻略配置
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public bool MultDeleteVehicleArticle(List<VehicleArticleModel> models)
        {
            var result = false;
            try
            {
                var delList = new List<VehicleArticleModel>();
                var logs = new List<VehicleArticleOprLogModel>();
                foreach (var model in models)
                {
                    var oldValue = dbScopeManagerConfigurationRead.Execute(conn =>
                     DalVehicleArticle.GetVehicleArticle(conn, model));
                    if (oldValue != null)
                    {
                        delList.Add(oldValue);
                        var log = new VehicleArticleOprLogModel
                        {
                            LogType = "VehicleArticle",
                            IdentityId = $"{model.VehicleId}_{model.PaiLiang}_{model.Nian}",
                            OperationType = "Delete",
                            OldValue = JsonConvert.SerializeObject(oldValue),
                            NewValue = null,
                            Remarks = $"删除选车攻略：{ model.VehicleId }_{ model.PaiLiang }_{ model.Nian }的配置",
                            Operator = _user
                        };
                        logs.Add(log);
                    }
                }
                if (delList.Any())
                {
                    dbScopeManagerConfiguration.CreateTransaction(conn =>
                    {
                        foreach (var model in delList)
                        {
                            var delResult = DalVehicleArticle.DeleteVehicleArticle(conn, model.PKID);
                            if (!delResult)
                            {
                                throw new Exception($"DeleteVehicleArticle失败," +
                                            $"待删除数据：{JsonConvert.SerializeObject(model)}");
                            }
                        }
                        result = true;
                    });
                }
                if (result && logs.Any())
                {
                    logs.ForEach(s => LoggerManager.InsertLog("VehicleArticleOprLog", s));
                }
            }
            catch (Exception ex)
            {
                Logger.Error("MultDeleteVehicleArticle", ex);
            }
            return result;
        }

        /// <summary>
        /// 批量添加或更新选车攻略配置
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public bool MultUpsertVehicleArticle(List<VehicleArticleModel> models)
        {
            var result = false;
            try
            {
                var addList = new List<VehicleArticleModel>();
                var updateList = new List<VehicleArticleModel>();
                var logs = new List<VehicleArticleOprLogModel>();
                foreach (var model in models)
                {
                    var oldValue = dbScopeManagerConfigurationRead.Execute(conn =>
                     DalVehicleArticle.GetVehicleArticle(conn, model));
                    if (oldValue != null)
                    {
                        model.PKID = oldValue.PKID;
                        model.CreateDateTime = oldValue.CreateDateTime;
                        model.LastUpdateDateTime = DateTime.Now;
                        updateList.Add(model);
                        var log = new VehicleArticleOprLogModel
                        {
                            LogType = "VehicleArticle",
                            IdentityId = $"{model.VehicleId}_{model.PaiLiang}_{model.Nian}",
                            OperationType = "Update",
                            OldValue = JsonConvert.SerializeObject(oldValue),
                            NewValue = JsonConvert.SerializeObject(model),
                            Remarks = $"更新选车攻略:{ model.VehicleId }_{ model.PaiLiang }_{ model.Nian }的配置",
                            Operator = _user,
                        };
                        logs.Add(log);
                    }
                    else
                    {
                        addList.Add(model);
                    }
                }
                dbScopeManagerConfiguration.CreateTransaction(conn =>
                {
                    foreach (var model in addList)
                    {
                        var pkid = DalVehicleArticle.AddVehicleArticle(conn, model);
                        if (pkid < 1)
                        {
                            throw new Exception($"AddVehicleArticle失败," +
                                            $"待添加数据：{JsonConvert.SerializeObject(model)}");
                        }
                        model.PKID = pkid;
                        model.CreateDateTime = DateTime.Now;
                        model.LastUpdateDateTime = DateTime.Now;
                        var log = new VehicleArticleOprLogModel
                        {
                            LogType = "VehicleArticle",
                            IdentityId = $"{model.VehicleId}_{model.PaiLiang}_{model.Nian}",
                            OperationType = "Add",
                            OldValue = null,
                            NewValue = JsonConvert.SerializeObject(model),
                            Remarks = $"添加选车攻略:{model.VehicleId}_{model.PaiLiang}_{model.Nian}的配置",
                            Operator = _user,
                        };
                        logs.Add(log);
                    }
                    foreach (var model in updateList)
                    {
                        var updateResult = DalVehicleArticle.UpdateVehicleArticle(conn, model);
                        if (!updateResult)
                        {
                            throw new Exception($"UpdateVehicleArticle失败," +
                                           $"待更新数据：{JsonConvert.SerializeObject(model)}");
                        }
                    }
                    result = true;
                });
                if (result)
                {
                    logs.ForEach(s => LoggerManager.InsertLog("VehicleArticleOprLog", s));
                }
            }
            catch (Exception ex)
            {
                Logger.Error("MultUpsertVehicleArticle", ex);
            }
            return result;
        }

        /// <summary>
        /// 根据等级获取选车攻略配置
        /// </summary>
        /// <param name="model"></param>
        /// <param name="vehicleLevel"></param>
        /// <returns></returns>
        public Tuple<List<VehicleArticleViewModel>, int> SelectVehicleArticleModelByLevel
            (VehicleSearchModel model, int vehicleLevel)
        {
            var result = null as List<VehicleArticleViewModel>;
            var totalCount = 0;
            try
            {
                switch (vehicleLevel)
                {
                    case 2:
                        result = dbScopeManagerConfigurationRead.Execute(conn =>
                   DalVehicleArticle.SelectVehicleArticleModelForVehicleId(conn, model, out totalCount)); break;
                    case 3:
                        result = dbScopeManagerConfigurationRead.Execute(conn =>
                    DalVehicleArticle.SelectVehicleArticleModelForPaiLiang(conn, model, out totalCount)); break;
                    case 4:
                        var vehicleConfigs = dbScopeManagerConfigurationRead.Execute(conn =>
                      DalVehicleArticle.SelectVehiclesForNian(conn, model));
                        var nianConfigs = dbScopeManagerConfigurationRead.Execute(conn =>
                       DalVehicleArticle.SelectVehicleArticleModelForNiann(conn, model));
                        result = new List<VehicleArticleViewModel>();
                        if (!string.IsNullOrEmpty(model.Nian))
                        {
                            vehicleConfigs = vehicleConfigs.Where(s => string.Equals(s.Nian, model.Nian)).ToList();
                        }
                        foreach (var vehicleConfig in vehicleConfigs)
                        {
                            var nianConfig = nianConfigs.FirstOrDefault(s => string.Equals(s.Nian, vehicleConfig.Nian));
                            if (nianConfig != null || !model.IsOnlyConfiged)
                            {
                                result.Add(new VehicleArticleViewModel()
                                {
                                    PKID = nianConfig?.PKID ?? 0,
                                    Brand = vehicleConfig.Brand,
                                    VehicleId = vehicleConfig.VehicleId,
                                    VehicleSeries = vehicleConfig.VehicleSeries,
                                    PaiLiang = vehicleConfig.PaiLiang,
                                    Nian = vehicleConfig.Nian,
                                    ArticleUrl = nianConfig?.ArticleUrl,
                                    CreateDateTime = nianConfig?.CreateDateTime,
                                    LastUpdateDateTime = nianConfig?.LastUpdateDateTime
                                });
                            }
                        }
                        break;
                    default: break;
                }
            }
            catch (Exception ex)
            {
                result = null;
                Logger.Error("SelectVehicleArticleModelForVehicleId", ex);
            }
            return Tuple.Create(result, totalCount);
        }

        /// <summary>
        /// 选车攻略配置是否重复
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool IsExistVehicleArticle(VehicleArticleModel model)
        {
            var result = true;
            try
            {
                result = dbScopeManagerConfigurationRead.Execute(conn =>
                  DalVehicleArticle.IsExistVehicleArticle(conn, model));
            }
            catch (Exception ex)
            {
                Logger.Error("IsExistVehicleArticle", ex);
            }
            return result;
        }

        /// <summary>
        /// 清除缓存
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public bool RemoveCache(List<VehicleArticleModel> models)
        {
            var result = false;
            try
            {
                var type = "VehicleArticle";
                var keys = models.Select(vehicle => $"{vehicle.VehicleId}/{vehicle.PaiLiang}/{vehicle.Nian}").ToList();
                using (var client = new Service.Vehicle.CacheClient())
                {
                    var clientResult = client.RemoveByType(type, keys);
                    result = clientResult.Result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("RefreshCache", ex);
            }
            return result;
        }
    }
}
