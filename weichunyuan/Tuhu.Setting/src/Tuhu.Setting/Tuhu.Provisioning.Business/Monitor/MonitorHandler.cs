using System.Configuration;
using Tuhu.Component.Framework;
using Tuhu.Component.Framework.ThreadPools;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.DAO;

namespace Tuhu.Provisioning.Business.Monitor
{
    public class MonitorHandler
    {
        private static readonly IConnectionManager Gungnir =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);

        private readonly IDBScopeManager dbManager;

        public MonitorHandler(IDBScopeManager dbManager)
        {
            this.dbManager = dbManager;
        }

        public void AddMonitorInfo(string subjectType, string subjectId, string errorMessage, string operationUser, string operationName, MonitorLevel level = MonitorLevel.Info, MonitorModule module = MonitorModule.Other)
        {

            var parms = new object[] { subjectType, subjectId, errorMessage, operationUser, operationName, level, module };
            TuHuThreadPool.QueueUserWorkItem(HandlerAddMonitorInfo, parms);
        }
        private void HandlerAddMonitorInfo(object obj)
        {
            try
            {
                dbManager.Execute(conn => DalMonitor.Add(conn, obj));

            }
            catch
            {

                //To do
            }
        }
    }
}
