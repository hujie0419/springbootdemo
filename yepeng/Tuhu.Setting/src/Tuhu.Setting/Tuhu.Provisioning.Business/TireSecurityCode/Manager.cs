using System;
using System.Collections.Generic;
using System.Configuration;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.TireSecurityCode;
using Tuhu.Provisioning.Business.DownloadApp;

namespace Tuhu.Provisioning.Business.TireSecurityCode
{
    public class Manager
    {
        #region
        static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static readonly IConnectionManager connectionManager = new ConnectionManager(SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn);
        private static readonly IDBScopeManager DbScopeManager = new DBScopeManager(connectionManager);

        static string strConnReadOnly = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
        private static readonly IConnectionManager connectionManagerReadOnly = new ConnectionManager(SecurityHelp.IsBase64Formatted(strConnReadOnly) ? SecurityHelp.DecryptAES(strConnReadOnly) : strConnReadOnly);
        private static readonly IDBScopeManager DbScopeManagerReadOnly = new DBScopeManager(connectionManagerReadOnly);

        private static readonly ILog Logger = LoggerFactory.GetLogger("ShareConfig");
        private Handle handler = null;
        private Handle handlerReadOnly = null;
        public Manager()
        {
            handler = new Handle(DbScopeManager);
            handlerReadOnly = new Handle(DbScopeManagerReadOnly);
        }
        #endregion

        public List<TireSecurityCodeConfig> QuerySecurityCodeConfigModel(TireSecurityCodeConfigQuery query)
        {
            try
            {
                return handler.QuerySecurityCodeConfigModel(query);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "QuerySecurityCodeConfigModel", ex);
                Logger.Log(Level.Error, exception, "QuerySecurityCodeConfigModel");
                throw ex;
            }
        }

        public List<UploadSecurityCodeLog> QueryUploadSecurityCodeLogModel(LogSearchQuery query)
        {
            try
            {
                return handler.QueryUploadSecurityCodeLogModel(query);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "QueryUploadSecurityCodeLogModel", ex);
                Logger.Log(Level.Error, exception, "QueryUploadSecurityCodeLogModel");
                throw ex;
            }
        }

        public List<UploadBarCodeLog> QueryUploadBarCodeLogModel(LogSearchQuery query)
        {
            try
            {
                return handler.QueryUploadBarCodeLogModel(query);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "QueryUploadBarCodeLogModel", ex);
                Logger.Log(Level.Error, exception, "QueryUploadBarCodeLogModel");
                throw ex;
            }
        }

        public bool InsertTireSecurityCodeConfig(List<TireSecurityCodeConfig> list)
        {
            try
            {
                return handler.InsertTireSecurityCodeConfig(list);
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

        public int InsertBarCodeConfig(List<InputBarCode> list)
        {
            try
            {
                return handler.InsertBarCodeConfig(list);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "InsertBarCodeConfig", ex);
                Logger.Log(Level.Error, exception, "InsertBarCodeConfig");
                throw ex;
            }
        }

        public List<InputBarCode> QueryInputBarCodeByError(string error, List<InputBarCode> list)
        {
            try
            {
                return handler.QueryInputBarCodeByError(error, list);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "QueryInputBarCodeByError", ex);
                Logger.Log(Level.Error, exception, "QueryInputBarCodeByError");
                throw ex;
            }
        }

        public bool DeleleSecurityCodeConfigModelByBatchNum(string batchNum)
        {
            try
            {
                return handler.DeleleSecurityCodeConfigModelByBatchNum(batchNum);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "DeleleSecurityCodeConfigModelByBatchNum", ex);
                Logger.Log(Level.Error, exception, "DeleleSecurityCodeConfigModelByBatchNum");
                throw ex;
            }
        }

        public bool DeleleBarCodeByBatchNum(string batchNum)
        {
            try
            {
                return handler.DeleleBarCodeByBatchNum(batchNum);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "DeleleBarCodeByBatchNum", ex);
                Logger.Log(Level.Error, exception, "DeleleBarCodeByBatchNum");
                throw ex;
            }
        }

        public List<TireSecurityCodeConfig> QuerySecurityCodeConfigModelByBatchNum(string batchNum)
        {
            try
            {
                return handler.QuerySecurityCodeConfigModelByBatchNum(batchNum);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "QuerySecurityCodeConfigModelByBatchNum", ex);
                Logger.Log(Level.Error, exception, "QuerySecurityCodeConfigModelByBatchNum");
                throw ex;
            }
        }

        public bool InsertUploadSecurityCodeLog(UploadSecurityCodeLog log)
        {
            try
            {
                return handler.InsertUploadSecurityCodeLog(log);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "InsertUploadSecurityCodeLog", ex);
                Logger.Log(Level.Error, exception, "InsertUploadSecurityCodeLog");
                throw ex;
            }
        }

        public bool InsertUploadBarCodeLog(UploadBarCodeLog log)
        {
            try
            {
                return handler.InsertUploadBarCodeLog(log);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "InsertUploadBarCodeLog", ex);
                Logger.Log(Level.Error, exception, "InsertUploadBarCodeLog");
                throw ex;
            }
        }
    }
}
