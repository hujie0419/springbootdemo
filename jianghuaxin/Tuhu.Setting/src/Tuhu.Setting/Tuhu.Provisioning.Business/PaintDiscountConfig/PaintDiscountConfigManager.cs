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
                var oldValue = GetPaintDiscountConfig(model.ServicePid, model.SurfaceCount);
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
                    IdentityId = $"{model.ServicePid}_{model.SurfaceCount}",
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
            (string servicePid, int surfaceCount, string user)
        {
            var result = false;
            try
            {
                var oldValue = GetPaintDiscountConfig(servicePid, surfaceCount);
                if (oldValue != null && !oldValue.IsDeleted)
                {
                    result = dbScopeManagerConfiguration.Execute(conn => DalPaintDiscountConfig.DeletePaintDiscountConfig(conn, oldValue.PKID));
                    if (!result)
                    {
                        throw new Exception($"DeletePaintDiscountConfig失败,待删除数据{JsonConvert.SerializeObject(oldValue)}");
                    }
                    var log = new PaintDiscountOprLogModel
                    {
                        LogType = "PaintDiscountConfig",
                        IdentityId = $"{servicePid}_{surfaceCount}",
                        OperationType = "Delete",
                        OldValue = JsonConvert.SerializeObject(oldValue),
                        NewValue = null,
                        Remarks = $"删除服务Pid:{oldValue.ServicePid},面数:{oldValue.SurfaceCount}的配置",
                        Operator = user,
                    };
                    LoggerManager.InsertLog("PaintDiscountOprLog", log);
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
                var oldValue = GetPaintDiscountConfig(model.ServicePid, model.SurfaceCount);
                if (oldValue != null)
                {
                    model.PKID = oldValue.PKID;
                    result = dbScopeManagerConfiguration.Execute(conn =>
                    DalPaintDiscountConfig.UpdatePaintDiscountConfig(conn, model));
                    if (!result)
                    {
                        throw new Exception($"UpdatePaintDiscountConfig失败,待更新数据{JsonConvert.SerializeObject(model)}");
                    }
                    model.CreateDateTime = oldValue.CreateDateTime;
                    model.LastUpdateDateTime = DateTime.Now;
                    var log = new PaintDiscountOprLogModel
                    {
                        LogType = "PaintDiscountConfig",
                        IdentityId = $"{model.ServicePid}_{model.SurfaceCount}",
                        OperationType = "Update",
                        OldValue = JsonConvert.SerializeObject(oldValue),
                        NewValue = JsonConvert.SerializeObject(model),
                        Remarks = $"更新服务Pid:{model.ServicePid},面数:{model.SurfaceCount}的配置",
                        Operator = user,
                    };
                    LoggerManager.InsertLog("PaintDiscountOprLog", log);
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
                    result = models.All(s => UpdatePaintDiscountConfig(s, user));
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
            if (models != null && models.Any())
            {
                Logger.Info($"导入喷漆打折配置 开始,导入人{user}");
                foreach (var model in models)
                {
                    var isExist = IsExistPaintDiscountConfig(model);
                    result = isExist ? UpdatePaintDiscountConfig(model, user) : AddPaintDiscountConfig(model, user);
                }
                Logger.Info($"导入喷漆打折配置 结束,导入人{user}");
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
            (string servicePid, int pageIndex, int pageSize)
        {
            var result = null as List<PaintDiscountConfigModel>;
            var totalCount = 0;
            try
            {
                var configs = dbScopeManagerConfigurationRead.Execute(conn =>
                      DalPaintDiscountConfig.SelectPaintDiscountConfig(conn, servicePid, pageIndex, pageSize, out totalCount))
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
                     (conn => DalPaintDiscountConfig.SelectPaintDiscountOprLog(conn, logType, identityId, pageIndex, pageSize));
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

        /// <summary>
        /// 获取喷漆打折配置
        /// </summary>
        /// <param name="servicePid"></param>
        /// <param name="surfaceCount"></param>
        /// <returns></returns>
        private PaintDiscountConfigModel GetPaintDiscountConfig
        (string servicePid, int surfaceCount)
        {
            return dbScopeManagerConfigurationRead.Execute
                     (conn => DalPaintDiscountConfig.GetPaintDiscountConfig(conn, servicePid, surfaceCount));
        }
    }
}
