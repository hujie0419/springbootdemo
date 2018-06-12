using System;
using System.Configuration;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.Monitor
{
    public class MonitorManager : IMonitorManager
    {
        #region Private Fields

        private static readonly IConnectionManager ConnectionManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);
        private static readonly IConnectionManager ConnectionReadManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString);

        private static readonly IDBScopeManager DbScopeManager = new DBScopeManager(ConnectionManager);
        private static readonly IDBScopeManager DbScopeReadManager = new DBScopeManager(ConnectionReadManager);
        private static readonly ILog Logger = LoggerFactory.GetLogger("MonitorManager");


        private readonly MonitorHandler handler = null;
        private readonly MonitorHandler readHandler = null;

        #endregion

        public MonitorManager()
        {
            handler = new MonitorHandler(DbScopeManager);
            readHandler = new MonitorHandler(DbScopeReadManager);
        }
        public void AddMonitorInfo(string subjectType, string subjectId, string errorMessage, string operationUser, string operationName, MonitorLevel level = MonitorLevel.Info, MonitorModule module = MonitorModule.Other)
        {
            try
            {
                handler.AddMonitorInfo(subjectType, subjectId, errorMessage, operationUser, operationName, level, module);
            }
            catch (Exception innerEx)
            {
                var exception = new MonitorException(BizErrorCode.SystemError, "查询异常订单错误", innerEx);
                Logger.Log(Level.Error, "Error occurred in GetExceptionOrders.", exception);

                throw exception;
            }
        }
    }

}
