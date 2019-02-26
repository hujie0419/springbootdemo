using System;
using System.Collections.Generic;
using System.Configuration;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.Tire;
using Tuhu.Provisioning.Business.DownloadApp;
using System.Linq;

namespace Tuhu.Provisioning.Business.Tire
{
    public class TirePatternConfigManager
    {
        #region
        static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static readonly IConnectionManager connectionManager = new ConnectionManager(SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn);
        private static readonly IDBScopeManager DbScopeManager = new DBScopeManager(connectionManager);

        static string strConnReadOnly = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
        private static readonly IConnectionManager connectionManagerReadOnly = new ConnectionManager(SecurityHelp.IsBase64Formatted(strConnReadOnly) ? SecurityHelp.DecryptAES(strConnReadOnly) : strConnReadOnly);
        private static readonly IDBScopeManager DbScopeManagerReadOnly = new DBScopeManager(connectionManagerReadOnly);

        private static readonly ILog Logger = LoggerFactory.GetLogger("ShareConfig");
        private TirePatternConfigHandle handler = null;
        private TirePatternConfigHandle handlerReadOnly = null;
        public TirePatternConfigManager()
        {
            handler = new TirePatternConfigHandle(DbScopeManager);
            handlerReadOnly = new TirePatternConfigHandle(DbScopeManagerReadOnly);
        }
        #endregion

        public List<TirePatternChangeLog> SelectTireSpecParamsEditLog(string pid)
        {
            try
            {
                return handlerReadOnly.SelectTirePatternChangeLog(pid);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "SelectTirePatternChangeLog", ex);
                Logger.Log(Level.Error, exception, "SelectTirePatternChangeLog");
                throw ex;
            }
        }

        public List<TirePatternConfig> QueryTirePatternConfig(TirePatternConfigQuery query)
        {
            try
            {
                return handlerReadOnly.QueryTirePatternConfig(query);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "QueryTirePatternConfig", ex);
                Logger.Log(Level.Error, exception, "QueryTirePatternConfig");
                throw ex;
            }
        }

        public TirePatternConfig GetConfigByPattern(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern)) return null;
            try
            {
                var result = handler.GetConfigByPattern(pattern);
                if (result == null || !result.Any() || result.Count == 0)
                    return null;
                else return result.FirstOrDefault();
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "GetConfigByPattern", ex);
                Logger.Log(Level.Error, exception, "GetConfigByPattern");
                throw ex;
            }
        }

        public TirePatternConfig GetConfigByPKID(int pkid)
        {
            if (pkid == 0) return null;
            try
            {
                var result = handler.GetConfigByPKID(pkid);
                if (result == null || !result.Any() || result.Count == 0)
                    return null;
                else return result.FirstOrDefault();
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "GetConfigByPKID", ex);
                Logger.Log(Level.Error, exception, "GetConfigByPKID");
                throw ex;
            }
        }

        public List<string> GetALlPattern()
        {
            try
            {
                return handler.GetALlPattern();
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "GetALlPattern", ex);
                Logger.Log(Level.Error, exception, "GetALlPattern");
                throw ex;
            }
        }

        public bool InsertTirePatternConfig(TirePatternConfig config)
        {
            try
            {
                return handler.InsertTirePatternConfig(config);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "InsertTirePatternConfig", ex);
                Logger.Log(Level.Error, exception, "InsertTirePatternConfig");
                throw ex;
            }
        }

        public bool InsertTirePatternChangeLog(TirePatternChangeLog log)
        {
            try
            {
                return handler.InsertTirePatternChangeLog(log);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "InsertTirePatternChangeLog", ex);
                Logger.Log(Level.Error, exception, "InsertTirePatternChangeLog");
                throw ex;
            }
        }

        public bool UpdateTirePatternConfigExpectPattern(TirePatternConfig config)
        {
            try
            {
                return handler.UpdateTirePatternConfigExpectPattern(config);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "UpdateTirePatternConfigExpectPattern", ex);
                Logger.Log(Level.Error, exception, "UpdateTirePatternConfigExpectPattern");
                throw ex;
            }
        }

        public bool UpdateTirePatternConfig(TirePatternConfig oldConfig, TirePatternConfig newConfig)
        {
            try
            {
                return handler.UpdateTirePatternConfig(oldConfig, newConfig);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "UpdateTirePatternConfig", ex);
                Logger.Log(Level.Error, exception, "UpdateTirePatternConfig");
                throw ex;
            }
        }

        public List<string> GetAffectedPids(string pattern)
        {
            try
            {
                return handler.GetAffectedPids(pattern);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "GetAffectedPids", ex);
                Logger.Log(Level.Error, exception, "GetAffectedPids");
                throw ex;
            }
        }
    }
}
