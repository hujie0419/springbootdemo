using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.DownloadApp;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.Push.Models.WeiXinPush;

namespace Tuhu.Provisioning.Business.ShareConfig
{
    public class ShareConfigManager
    {
        #region Private Fields
        static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static readonly IConnectionManager connectionManager = new ConnectionManager(SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn);
        private static readonly IDBScopeManager DbScopeManager = new DBScopeManager(connectionManager);

        #region ReadOnly
        static string strConnReadOnly = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
        private static readonly IConnectionManager connectionManagerReadOnly = new ConnectionManager(SecurityHelp.IsBase64Formatted(strConnReadOnly) ? SecurityHelp.DecryptAES(strConnReadOnly) : strConnReadOnly);
        private static readonly IDBScopeManager DbScopeManagerReadOnly = new DBScopeManager(connectionManagerReadOnly);
        #endregion

        private static readonly ILog Logger = LoggerFactory.GetLogger("ShareConfig");
        private ShareConfigHandler handler = null;
        private ShareConfigHandler handlerReadOnly = null;

        public ShareConfigManager()
        {
            handler = new ShareConfigHandler(DbScopeManager);
            handlerReadOnly = new ShareConfigHandler(DbScopeManagerReadOnly);
        }
        #endregion
        public List<ShareConfigSource> SelectShareConfig(ShareConfigQuery model)
        {
            try
            {
                return handler.SelectShareConfig(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "SelectShareConfig", ex);
                Logger.Log(Level.Error, exception, "SelectShareConfig");
                throw ex;
            }
        }

        public int SelectPKIdByLocation(string location, string specialParam = null)
        {
            try
            {
                return handler.SelectPKIdByLocation(location, specialParam);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "SelectPKIdByLocation", ex);
                Logger.Log(Level.Error, exception, "SelectPKIdByLocation");
                throw ex;
            }
        }

        public string SelectLocationByPKId(int pkid)
        {
            try
            {
                return handler.SelectLocationByPKId(pkid);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "SelectLocationByPKId", ex);
                Logger.Log(Level.Error, exception, "SelectLocationByPKId");
                throw ex;
            }
        }

        public List<ShareSupervisionConfig> SelectShareSConfigByJumpId(int jumpId)
        {
            try
            {
                return handler.SelectShareSConfigByJumpId(jumpId);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "SelectShareSConfigByLocation", ex);
                Logger.Log(Level.Error, exception, "SelectShareSConfigByLocation");
                throw ex;
            }
        }

        public List<ShareConfigLog> SelectShareConfigLogById(int id)
        {
            try
            {
                return handler.SelectShareConfigLogById(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "SelectShareConfigLogById", ex);
                Logger.Log(Level.Error, exception, "SelectShareConfigLogById");
                throw ex;
            }
        }

        public bool UpdateShareConfig(ShareConfigSource scs)
        {
            try
            {
                return handler.UpdateShareConfig(scs);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "UpdateShareConfig", ex);
                Logger.Log(Level.Error, exception, "UpdateShareConfig");
                throw ex;
            }
        }

        public int InsertShareConfig(ShareConfigSource scs)
        {
            try
            {
                return handler.InsertShareConfig(scs);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "InsertShareConfig", ex);
                Logger.Log(Level.Error, exception, "InsertShareConfig");
                throw ex;
            }
        }

        public ShareSupervisionConfig SelectShareSConfigById(int id)
        {
            try
            {
                return handler.SelectShareSConfigById(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "SelectShareSConfigById", ex);
                Logger.Log(Level.Error, exception, "SelectShareSConfigById");
                throw ex;
            }
        }
        public bool InsertShareSConfig(ShareSupervisionConfig ssc)
        {
            try
            {
                return handler.InsertShareSConfig(ssc);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "InsertShareSConfig", ex);
                Logger.Log(Level.Error, exception, "InsertShareSConfig");
                throw ex;
            }
        }
        public bool UpdateShareSConfig(ShareSupervisionConfig ssc)
        {
            try
            {
                return handler.UpdateShareSConfig(ssc);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "UpdateShareSConfig", ex);
                Logger.Log(Level.Error, exception, "UpdateShareSConfig");
                throw ex;
            }
        }
        public bool DeleteShareSConfigById(int id)
        {
            try
            {
                return handler.DeleteShareSConfigById(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "DeleteShareSConfigById", ex);
                Logger.Log(Level.Error, exception, "DeleteShareSConfigById");
                throw ex;
            }
        }
        public bool DeleteShareConfigByLocation(string location, string specialParam, int jumpId)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                try
                {
                    dbHelper.BeginTransaction();
                    if (!handler.DeleteShareConfigByLocation(location, specialParam))
                    {
                        dbHelper.Rollback();
                        return false;
                    }

                    if (!handler.DeleteShareSConfigByJumpId(jumpId))
                    {
                        dbHelper.Rollback();
                        return false;
                    }
                    dbHelper.Commit();
                    return true;
                }
                catch (TuhuBizException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    var exception = new DownloadAppException(1, "DeleteShareConfigByLocation", ex);
                    Logger.Log(Level.Error, exception, "DeleteShareConfigByLocation");
                    throw ex;
                }
            }
        }

        public bool InsertShareSConfigs(List<ShareSupervisionConfig> sscList, string location, string author, int newId)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir")))
            {
                try
                {
                    if (sscList != null && sscList.Any())
                    {
                        dbHelper.BeginTransaction();
                        foreach (var ssc in sscList)
                        {
                            ssc.Location = location;
                            ssc.CreateDateTime = DateTime.Now;
                            ssc.UpdateDateTime = DateTime.Now;
                            ssc.Creator = author;
                            ssc.JumpId = newId;
                            if (!handler.InsertShareSConfig(ssc))
                            {
                                dbHelper.Rollback();
                                return false;
                            }
                        }
                        dbHelper.Commit();
                    }
                    return true;
                }
                catch (TuhuBizException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    var exception = new DownloadAppException(1, "InsertShareSConfigs", ex);
                    Logger.Log(Level.Error, exception, "InsertShareSConfigs");
                    throw ex;
                }
            }
        }

        public bool InsertShareConfigLog(ShareConfigLog scl)
        {
            try
            {
                return handler.InsertShareConfigLog(scl);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "InsertShareConfigLog", ex);
                Logger.Log(Level.Error, exception, "InsertShareConfigLog");
                throw ex;
            }
        }
        public bool DeleteShareConfigLog(int id)
        {
            try
            {
                return handler.DeleteShareConfigLog(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "DeleteShareConfigLog", ex);
                Logger.Log(Level.Error, exception, "DeleteShareConfigLog");
                throw ex;
            }
        }

        public async Task<IEnumerable<WxConfig>> SelectWxConfigsAsync()
        {
            try
            {
                using (var client = new Tuhu.Service.Push.WeiXinPushClient())
                {
                    var result = await client.SelectWxConfigsAsync();
                    if (result.Success)
                        return result.Result?.Where(_ => _.platform > 1).ToList() ?? new List<WxConfig>();
                    else
                        return new List<WxConfig>();
                }
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "SelectWxConfigsAsync", ex);
                Logger.Log(Level.Error, exception, "SelectWxConfigsAsync");
                return new List<WxConfig>();
            }
        }
    }
}
