using System;
using System.Collections.Generic;
using System.Configuration;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.Tire;
using Tuhu.Provisioning.Business.DownloadApp;

namespace Tuhu.Provisioning.Business.Tire
{
    public class TireSpecParamsConfigManager
    {
        #region
        static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static readonly IConnectionManager connectionManager = new ConnectionManager(SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn);
        private static readonly IDBScopeManager DbScopeManager = new DBScopeManager(connectionManager);

        static string strConnReadOnly = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
        private static readonly IConnectionManager connectionManagerReadOnly = new ConnectionManager(SecurityHelp.IsBase64Formatted(strConnReadOnly) ? SecurityHelp.DecryptAES(strConnReadOnly) : strConnReadOnly);
        private static readonly IDBScopeManager DbScopeManagerReadOnly = new DBScopeManager(connectionManagerReadOnly);

        private static readonly ILog Logger = LoggerFactory.GetLogger("ShareConfig");
        private TireSpecParamsConfigHandle handler = null;
        private TireSpecParamsConfigHandle handlerReadOnly = null;
        public TireSpecParamsConfigManager()
        {
            handler = new TireSpecParamsConfigHandle(DbScopeManager);
            handlerReadOnly = new TireSpecParamsConfigHandle(DbScopeManagerReadOnly);
        }
        #endregion

        public List<TireSpecParamsEditLog> SelectTireSpecParamsEditLog(string pid)
        {
            try
            {
                return handlerReadOnly.SelectTireSpecParamsEditLog(pid);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "SelectTireSpecParamsEditLog", ex);
                Logger.Log(Level.Error, exception, "SelectTireSpecParamsEditLog");
                throw ex;
            }
        }

        public List<TireSpecParamsModel> QueryTireSpecParamsModel(TireSpecParamsConfigQuery query)
        {
            try
            {
                return handlerReadOnly.QueryTireSpecParamsModel(query);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "QueryTireSpecParamsModel", ex);
                Logger.Log(Level.Error, exception, "QueryTireSpecParamsModel");
                throw ex;
            }
        }

        public TireSpecParamsModel SelectTireSpecParamsModelByPid(string pid)
        {
            try
            {
                return handlerReadOnly.SelectTireSpecParamsModelByPid(pid);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "SelectTireSpecParamsModelByPid", ex);
                Logger.Log(Level.Error, exception, "SelectTireSpecParamsModelByPid");
                throw ex;
            }
        }

        public bool InsertTireSpecParamsConfig(TireSpecParamsConfig config)
        {
            try
            {
                return handler.InsertTireSpecParamsConfig(config);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "InsertTireSpecParamsConfig", ex);
                Logger.Log(Level.Error, exception, "InsertTireSpecParamsConfig");
                throw ex;
            }
        }

        public bool UpdateTireSpecParamsConfig(TireSpecParamsConfig config)
        {
            try
            {
                return handler.UpdateTireSpecParamsConfig(config);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "UpdateTireSpecParamsConfig", ex);
                Logger.Log(Level.Error, exception, "UpdateTireSpecParamsConfig");
                throw ex;
            }
        }

        public bool InsertTireSpecParamsEditLog(TireSpecParamsEditLog log)
        {
            try
            {
                return handler.InsertTireSpecParamsEditLog(log);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "InsertTireSpecParamsEditLog", ex);
                Logger.Log(Level.Error, exception, "InsertTireSpecParamsEditLog");
                throw ex;
            }
        }

        public bool CheckPidExist(string pid)
        {
            try
            {
                return handlerReadOnly.CheckPidExist(pid);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "CheckPidExist", ex);
                Logger.Log(Level.Error, exception, "CheckPidExist");
                throw ex;
            }
        }
    }
}
