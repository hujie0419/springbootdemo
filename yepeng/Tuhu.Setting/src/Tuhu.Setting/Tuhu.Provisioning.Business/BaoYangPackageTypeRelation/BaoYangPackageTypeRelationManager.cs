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
using Tuhu.Service.BaoYang;

namespace Tuhu.Provisioning.Business.BaoYangPackageTypeRelation
{
    public class BaoYangPackageTypeRelationManager
    {
        private static readonly Common.Logging.ILog Logger;
        private static readonly IConnectionManager baoyangConn =
     new ConnectionManager(ConfigurationManager.ConnectionStrings["BaoYang"].ConnectionString);
        private static readonly IConnectionManager baoyangReadConn =
      new ConnectionManager(ConfigurationManager.ConnectionStrings["BaoYang_AlwaysOnRead"].ConnectionString);
        private static readonly IDBScopeManager dbScopeManagerBaoYangRead;
        private static readonly IDBScopeManager dbScopeManagerBaoYang;
        static BaoYangPackageTypeRelationManager()
        {
            Logger = LogManager.GetLogger(typeof(BaoYangPackageTypeRelationManager));
            dbScopeManagerBaoYang = new DBScopeManager(baoyangConn);
            dbScopeManagerBaoYangRead = new DBScopeManager(baoyangReadConn);
        }

        /// <summary>
        /// 添加保养关联项目配置
        /// </summary>
        /// <param name="model"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool AddBaoYangPackageTypeRelation(BaoYangPackageTypeRelationViewModel model, string user)
        {
            var result = false;
            try
            {
                var oldValue = GetBaoYangPackageTypeRelation(model.MainPackageType);
                if (oldValue == null)
                {
                    var pkid = dbScopeManagerBaoYang.Execute
                        (conn => DalBaoYangPackageTypeRelation.AddBaoYangPackageTypeRelation(conn, model));
                    result = pkid > 0;
                    model.PKID = pkid;
                }
                else if (oldValue.IsDeleted)
                {
                    model.PKID = oldValue.PKID;
                    result = dbScopeManagerBaoYang.Execute
                        (conn => DalBaoYangPackageTypeRelation.UpdateBaoYangPackageTypeRelation(conn, model));
                }
                model.CreateDateTime = DateTime.Now;
                model.LastUpdateDateTime = DateTime.Now;
                var log = new BaoYangOprLog
                {
                    LogType = "BaoYangPackageTypeRelation",
                    IdentityID = $"{model.MainPackageType}",
                    OldValue = null,
                    NewValue = JsonConvert.SerializeObject(model),
                    Remarks = $"添加主项目为{model.MainPackageType}的保养关联项目配置",
                    OperateUser = user,
                };
                LoggerManager.InsertLog("BYOprLog", log);
            }
            catch (Exception ex)
            {
                result = false;
                Logger.Error("AddBaoYangPackageTypeRelation", ex);
            }
            return result;
        }

        /// <summary>
        /// 删除保养关联项目配置
        /// </summary>
        /// <param name="carNoPrefix"></param>
        /// <param name="surfaceCount"></param>
        /// <param name="servicePid"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool DeleteBaoYangPackageTypeRelation(string mainPackageType, string user)
        {
            var result = false;
            try
            {
                var oldValue = GetBaoYangPackageTypeRelation(mainPackageType);
                if (oldValue != null && !oldValue.IsDeleted)
                {
                    result = dbScopeManagerBaoYang.Execute(conn => DalBaoYangPackageTypeRelation.DeleteBaoYangPackageTypeRelation(conn, oldValue.PKID));
                    if (!result)
                    {
                        throw new Exception($"DeleteBaoYangPackageTypeRelation失败,待删除数据{JsonConvert.SerializeObject(oldValue)}");
                    }
                    var log = new BaoYangOprLog
                    {
                        LogType = "BaoYangPackageTypeRelation",
                        IdentityID = mainPackageType,
                        OldValue = JsonConvert.SerializeObject(oldValue),
                        NewValue = null,
                        Remarks = $"删除主项目为{mainPackageType}的保养关联项目配置",
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
                Logger.Error("DeleteBaoYangPackageTypeRelation", ex);
            }
            return result;
        }

        /// <summary>
        /// 更新保养关联项目配置
        /// </summary>
        /// <param name="model"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool UpdateBaoYangPackageTypeRelation(BaoYangPackageTypeRelationViewModel model, string user)
        {
            var result = false;
            try
            {
                var oldValue = GetBaoYangPackageTypeRelation(model.MainPackageType);
                if (oldValue != null)
                {
                    model.PKID = oldValue.PKID;
                    result = dbScopeManagerBaoYang.Execute(conn =>
                    DalBaoYangPackageTypeRelation.UpdateBaoYangPackageTypeRelation(conn, model));
                    if (!result)
                    {
                        throw new Exception($"UpdateBaoYangPackageTypeRelation失败,待更新数据{JsonConvert.SerializeObject(model)}");
                    }
                    model.CreateDateTime = oldValue.CreateDateTime;
                    model.LastUpdateDateTime = DateTime.Now;
                    var log = new BaoYangOprLog
                    {
                        LogType = "BaoYangPackageTypeRelation",
                        IdentityID = model.MainPackageType,
                        OldValue = JsonConvert.SerializeObject(oldValue),
                        NewValue = JsonConvert.SerializeObject(model),
                        Remarks = $"更新主项目为{model.MainPackageType}的保养关联项目配置",
                        OperateUser = user,
                    };
                    LoggerManager.InsertLog("BYOprLog", log);
                }
            }
            catch (Exception ex)
            {
                result = false;
                Logger.Error("UpdateBaoYangPackageTypeRelation", ex);
            }
            return result;
        }

        /// <summary>
        /// 查询保养关联项目配置
        /// </summary>
        /// <param name="servicePid"></param>
        /// <param name="carNoPrefix"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Tuple<List<BaoYangPackageTypeRelationViewModel>, int> SelectBaoYangPackageTypeRelation
            (string servicePid, int pageIndex, int pageSize)
        {
            var result = null as List<BaoYangPackageTypeRelationViewModel>;
            var totalCount = 0;
            try
            {
                var configs = dbScopeManagerBaoYangRead.Execute(conn =>
                      DalBaoYangPackageTypeRelation.SelectBaoYangPackageTypeRelation(conn, servicePid, pageIndex, pageSize, out totalCount))
                      ?? new List<BaoYangPackageTypeRelationViewModel>();
                var packageTypes = GetAllBaoYangPackageTypes();
                if (packageTypes != null && packageTypes.Any())
                {
                    configs.ForEach(
                        f =>
                        {
                            f.MainPackageName = packageTypes.FirstOrDefault(w => string.Equals(w.Type, f.MainPackageType))?.Name;
                            if (f.RelatedPackageTypeList != null && f.RelatedPackageTypeList.Any())
                            {
                                var packageNames = f.RelatedPackageTypeList.Select(s =>
                                 {
                                     return packageTypes.FirstOrDefault(w => string.Equals(w.Type, s))?.Name;
                                 }).ToList();
                                f.RelatedPackageNames = string.Join(",", packageNames);
                            }
                        });
                }
                result = configs;
            }
            catch (Exception ex)
            {
                result = null;
                Logger.Error("SelectBaoYangPackageTypeRelation", ex);
            }
            return Tuple.Create(result, totalCount);
        }

        /// <summary>
        /// 获取单个保养关联项目配置
        /// </summary>
        /// <param name="mainPackageType"></param>
        /// <returns></returns>
        public BaoYangPackageTypeRelationViewModel GetBaoYangPackageTypeRelation(string mainPackageType)
        {
            var result = null as BaoYangPackageTypeRelationViewModel;
            try
            {
                result = dbScopeManagerBaoYangRead.Execute(conn =>
                 DalBaoYangPackageTypeRelation.GetBaoYangPackageTypeRelation(conn, mainPackageType));
            }
            catch (Exception ex)
            {
                Logger.Error("GetBaoYangPackageTypeRelation", ex);
            }
            return result;
        }

        /// <summary>
        /// 主项目对应的配置是否存在
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool IsExistBaoYangPackageTypeRelation(BaoYangPackageTypeRelationViewModel model)
        {
            var result = true;
            try
            {
                result = dbScopeManagerBaoYangRead.Execute
                     (conn => DalBaoYangPackageTypeRelation.IsExistBaoYangPackageTypeRelation(conn, model));
            }
            catch (Exception ex)
            {
                Logger.Error("IsBaoYangPackageTypeRelation", ex);
            }
            return result;
        }


        /// <summary>
        /// 获取所有保养项目
        /// </summary>
        /// <returns></returns>
        public List<BaoYangPackage> GetAllBaoYangPackageTypes()
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
                        Name = s.ZhName
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("GetAllBaoYangPackageTypes", ex);
            }
            return result;
        }

        /// <summary>
        /// 刷新服务缓存
        /// </summary>
        /// <returns></returns>
        public bool RefreshCache()
        {
            var result = false;
            try
            {
                var key = "BaoYangPackageTypeRelation";
                using (var client = new CacheClient())
                {
                    var clientResult = client.Remove(new List<string>() { key });
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
