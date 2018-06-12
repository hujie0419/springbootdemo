using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.BaoYang;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.DataAccess;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.TipBannerConfig
{
    public class TipBannerConfigManager
    {
        private static readonly Common.Logging.ILog Logger;
        private static readonly IConnectionManager configConnRo =
      new ConnectionManager(ConfigurationManager.ConnectionStrings["Configuration"].ConnectionString);
        private static readonly IConnectionManager configReadConnRo =
      new ConnectionManager(ConfigurationManager.ConnectionStrings["Configuration_AlwaysOnRead"].ConnectionString);
        private static readonly IDBScopeManager dbScopeManagerConfigRead;
        private static readonly IDBScopeManager dbScopeManagerConfig;

        static TipBannerConfigManager()
        {
            Logger = Common.Logging.LogManager.GetLogger(typeof(TipBannerConfigManager));
            dbScopeManagerConfig = new DBScopeManager(configConnRo);
            dbScopeManagerConfigRead = new DBScopeManager(configReadConnRo);
        }

        public bool AddTipBannerTypeConfig(TipBannerTypeConfigModel model, string user)
        {
            var result = false;
            try
            {
                var pkid = dbScopeManagerConfig.Execute(conn => DalTipBannerConfig.AddTipBannerTypeConfig(conn, model));
                if (pkid > 0)
                {
                    result = true;
                    model.TypeId = pkid;
                    var log = new BaoYangOprLog
                    {
                        LogType = "TipBannerTypeConfig",
                        IdentityID = model.TypeName,
                        OldValue = null,
                        NewValue = JsonConvert.SerializeObject(model),
                        Remarks = "AddType",
                        OperateUser = user,
                    };
                    LoggerManager.InsertLog("BYOprLog", log);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("AddTipBannerTypeConfig", ex);
            }

            return result;
        }

        public bool IsRepeatTipBannerTypeConfig(string typeName)
        {
            var isRepeat = false;
            try
            {
                isRepeat = dbScopeManagerConfigRead.Execute(conn => DalTipBannerConfig.IsRepeatTipBannerTypeConfig(conn, typeName));
            }
            catch (Exception ex)
            {
                Logger.Error("IsRepeatTipBannerTypeConfig", ex);
            }
            return isRepeat;
        }


        public bool AddTipBannerDetailConfig(TipBannerConfigDetailModel model, string user)
        {
            var result = false;
            try
            {
                var pkid = dbScopeManagerConfig.Execute(conn => DalTipBannerConfig.AddTipBannerDetailConfig(conn, model));
                if (pkid > 0)
                {
                    result = true;
                    model.PKID = pkid;
                    model.CreateDateTime = DateTime.Now;
                    model.LastUpdateDateTime = DateTime.Now;
                    var log = new BaoYangOprLog
                    {
                        LogType = "TipBannerDetailConfig",
                        IdentityID = model.TypeName,
                        OldValue = null,
                        NewValue = JsonConvert.SerializeObject(model),
                        Remarks = "Add",
                        OperateUser = user,
                    };
                    LoggerManager.InsertLog("BYOprLog", log);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("AddTipBannerDetailConfig", ex);
            }
            return result;
        }

        public bool IsRepeatTipBannerDetailConfig(TipBannerConfigDetailModel model)
        {
            var isRepeat = false;
            try
            {
                isRepeat = dbScopeManagerConfigRead.Execute(conn => DalTipBannerConfig.IsRepeatTipBannerDetailConfig(conn, model));
            }
            catch (Exception ex)
            {
                Logger.Error("IsRepeatTipBannerDetailConfig", ex);
            }
            return isRepeat;
        }


        public bool DeleteTipBannerDetailConfigByPKID(int pkid, string user)
        {
            var result = false;
            try
            {
                var oldValue = dbScopeManagerConfigRead.Execute(conn => DalTipBannerConfig.GetTipBannerDetailConfigByPKID(conn, pkid));
                if (oldValue != null)
                {
                    result = dbScopeManagerConfig.Execute(conn => DalTipBannerConfig.DeleteTipBannerDetailConfigByPKID(conn, pkid));
                    if (result)
                    {
                        var log = new BaoYangOprLog
                        {
                            LogType = "TipBannerDetailConfigByPKID",
                            IdentityID = oldValue.TypeName,
                            OldValue = JsonConvert.SerializeObject(oldValue),
                            NewValue = null,
                            Remarks = "Delete",
                            OperateUser = user,
                        };
                        LoggerManager.InsertLog("BYOprLog", log);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("DeleteTipBannerDetailConfigByPKID", ex);
            }
            return result;
        }

        public bool UpdateTipBannerDetailConfig(TipBannerConfigDetailModel model, string user)
        {
            var result = false;
            try
            {
                var oldValue = dbScopeManagerConfigRead.Execute(conn => DalTipBannerConfig.GetTipBannerDetailConfigByPKID(conn, model.PKID));
                if (oldValue != null)
                {
                    result = dbScopeManagerConfig.Execute(conn => DalTipBannerConfig.UpdateTipBannerDetailConfig(conn, model));
                    if (result)
                    {
                        model.LastUpdateDateTime = DateTime.Now;
                        var log = new BaoYangOprLog
                        {
                            LogType = "TipBannerDetailConfig",
                            IdentityID = oldValue.TypeName,
                            OldValue = JsonConvert.SerializeObject(oldValue),
                            NewValue = JsonConvert.SerializeObject(model),
                            Remarks = "Update",
                            OperateUser = user,
                        };
                        LoggerManager.InsertLog("BYOprLog", log);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("UpdateTipBannerDetailConfig", ex);
            }
            return result;
        }

        public Tuple<List<TipBannerConfigDetailModel>, int> SelectTipBannerDetailConfig(int typeId, int pageIndex, int pageSize)
        {
            int totalCount = 0;
            List<TipBannerConfigDetailModel> tipBanners = null;
            try
            {
                tipBanners = dbScopeManagerConfigRead.Execute(conn =>
                     DalTipBannerConfig.SelectTipBannerDetailConfig(conn, typeId, pageIndex, pageSize, out totalCount));
            }
            catch (Exception ex)
            {
                Logger.Error("SelectTipBannerDetailConfig", ex);
            }
            return new Tuple<List<TipBannerConfigDetailModel>, int>(tipBanners, totalCount);
        }

        public List<TipBannerTypeConfigModel> GetAllTipBannerTypeConfig()
        {
            List<TipBannerTypeConfigModel> result = null;
            try
            {
                result = dbScopeManagerConfigRead.Execute(conn => DalTipBannerConfig.GetAllTipBannerTypeConfig(conn));
            }
            catch (Exception ex)
            {
                Logger.Error("GetAllTipBannerTypeConfig", ex);
            }
            return result ?? new List<TipBannerTypeConfigModel>();
        }

        #region 清除服务缓存
        public bool RefreshTipBannerConfigCache()
        {
            var result = false;
            try
            {
                var typeNames = dbScopeManagerConfigRead.Execute(
                    conn => DalTipBannerConfig.GetAllTipBannerTypeConfig(conn))
                    .Select(s => s.TypeName).ToList();
                using (var client = new Tuhu.Service.Config.CacheClient())
                {
                    var cacheResult = client.RefreshTipBannerConfigCache(typeNames);
                    cacheResult.ThrowIfException(true);
                    result = cacheResult.Result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("RefreshTipBannerConfigCache", ex);
            }
            return result;
        }
        #endregion
    }
}
