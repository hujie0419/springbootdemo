using System;
using System.Collections.Generic;
using System.Configuration;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.DownloadApp;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.WheelAdapterConfig
{
    public class WheelAdapterConfigManager
    {
        #region
        static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static readonly IConnectionManager connectionManager = new ConnectionManager(SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn);
        private static readonly IDBScopeManager DbScopeManager = new DBScopeManager(connectionManager);

        static string strConnReadOnly = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
        private static readonly IConnectionManager connectionManagerReadOnly = new ConnectionManager(SecurityHelp.IsBase64Formatted(strConnReadOnly) ? SecurityHelp.DecryptAES(strConnReadOnly) : strConnReadOnly);
        private static readonly IDBScopeManager DbScopeManagerReadOnly = new DBScopeManager(connectionManagerReadOnly);

        private static readonly ILog Logger = LoggerFactory.GetLogger("ShareConfig");
        private WheelAdapterConfigHandler handler = null;
        private WheelAdapterConfigHandler handlerReadOnly = null;
        public WheelAdapterConfigManager()
        {
            handler = new WheelAdapterConfigHandler(DbScopeManager);
            handlerReadOnly = new WheelAdapterConfigHandler(DbScopeManagerReadOnly);
        }
        #endregion

        public List<Str> SelectBrands()
        {
            try
            {
                return handlerReadOnly.SelectBrands();
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "SelectBrands", ex);
                Logger.Log(Level.Error, exception, "SelectBrands");
                throw ex;
            }
        }
        public List<Str> SelectVehiclesAndId(string brand)
        {
            try
            {
                return handlerReadOnly.SelectVehiclesAndId(brand);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "SelectVehiclesAndId", ex);
                Logger.Log(Level.Error, exception, "SelectVehiclesAndId");
                throw ex;
            }
        }
        public List<Str> SelectPaiLiang(string vehicleid)
        {
            try
            {
                return handlerReadOnly.SelectPaiLiang(vehicleid);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "SelectPaiLiang", ex);
                Logger.Log(Level.Error, exception, "SelectPaiLiang");
                throw ex;
            }
        }
        public List<Str> SelectYear(string vehicleid, string pailiang)
        {
            try
            {
                return handlerReadOnly.SelectYear(vehicleid, pailiang);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "SelectYear", ex);
                Logger.Log(Level.Error, exception, "SelectYear");
                throw ex;
            }
        }
        public List<Str> SelectNianAndSalesName(string vehicleid, string pailiang, string year)
        {
            try
            {
                return handlerReadOnly.SelectNianAndSalesName(vehicleid, pailiang, year);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "SelectNianAndSalesName", ex);
                Logger.Log(Level.Error, exception, "SelectNianAndSalesName");
                throw ex;
            }
        }
        public List<VehicleTypeInfo> QueryVehicleTypeInfo(WheelAdapterConfigQuery query)
        {
            try
            {
                return handlerReadOnly.QueryVehicleTypeInfo(query);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "QueryVehicleTypeInfo", ex);
                Logger.Log(Level.Error, exception, "QueryVehicleTypeInfo");
                throw ex;
            }
        }
        public List<VehicleTypeInfo> QueryVehicleTypeInfoByTID(WheelAdapterConfigQuery query)
        {
            try
            {
                return handlerReadOnly.QueryVehicleTypeInfoByTID(query);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "QueryVehicleTypeInfoByTID", ex);
                Logger.Log(Level.Error, exception, "QueryVehicleTypeInfoByTID");
                throw ex;
            }
        }
        public List<WheelAdapterConfigWithTid> SelectWheelAdapterConfigWithTid(string tid)
        {
            try
            {
                return handlerReadOnly.SelectWheelAdapterConfigWithTid(tid);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "SelectWheelAdapterConfigWithTid", ex);
                Logger.Log(Level.Error, exception, "SelectWheelAdapterConfigWithTid");
                throw ex;
            }
        }
        public bool InsertWheelAdapterConfigWithTid(WheelAdapterConfigWithTid wac)
        {
            try
            {
                return handler.InsertWheelAdapterConfigWithTid(wac);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "InsertWheelAdapterConfigWithTid", ex);
                Logger.Log(Level.Error, exception, "InsertWheelAdapterConfigWithTid");
                throw ex;
            }
        }

        public bool InsertWheelAdapterConfigWithTid(WheelAdapterConfigWithTid wac,IEnumerable<string> tids)
        {
            try
            {
                return handler.InsertWheelAdapterConfigWithTid(wac,tids);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "InsertWheelAdapterConfigWithTid", ex);
                Logger.Log(Level.Error, exception, "InsertWheelAdapterConfigWithTid");
                throw ex;
            }
        }

        public bool UpdateWheelAdapterConfigWithTid(WheelAdapterConfigWithTid wac)
        {
            try
            {
                return handler.UpdateWheelAdapterConfigWithTid(wac);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "UpdateWheelAdapterConfigWithTid", ex);
                Logger.Log(Level.Error, exception, "UpdateWheelAdapterConfigWithTid");
                throw ex;
            }
        }
        public bool InsertWheelAdapterConfigLog(WheelAdapterConfigLog wacl)
        {
            try
            {
                return handler.InsertWheelAdapterConfigLog(wacl);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "InsertWheelAdapterConfigLog", ex);
                Logger.Log(Level.Error, exception, "InsertWheelAdapterConfigLog");
                throw ex;
            }
        }
        public List<WheelAdapterConfigLog> SelectWheelAdapterConfigLog(string tid)
        {
            try
            {
                return handlerReadOnly.SelectWheelAdapterConfigLog(tid);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DownloadAppException(1, "SelectWheelAdapterConfigLog", ex);
                Logger.Log(Level.Error, exception, "SelectWheelAdapterConfigLog");
                throw ex;
            }
        }
    }
}
