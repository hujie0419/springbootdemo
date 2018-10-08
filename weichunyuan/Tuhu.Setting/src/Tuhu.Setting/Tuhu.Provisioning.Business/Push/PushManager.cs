using System;
using System.Collections.Generic;
using System.Configuration;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.Monitor;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.Push;

namespace Tuhu.Provisioning.Business.Push
{
    public class PushManager : IPushManager
    {
        private PushHandler handler = null;
        private readonly IConnectionManager connectionManager = null;
        private readonly IConnectionManager bIConnectionMgr = null;
        private readonly IDBScopeManager dbScopeManagerBI = null;
        private readonly IDBScopeManager dbScopeManagerGR = null;

        private static readonly ILog logger = LoggerFactory.GetLogger("PushManager");

        public PushManager()
        {
            this.connectionManager =
                new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString);
            this.bIConnectionMgr =
                new ConnectionManager(ConfigurationManager.ConnectionStrings["Tuhu_BI"].ConnectionString);

            this.dbScopeManagerGR = new DBScopeManager(this.connectionManager);
            this.dbScopeManagerBI = new DBScopeManager(this.bIConnectionMgr);
            this.handler = new PushHandler();
        }

        public IList<string> GetPushMsgPersonConfig()
        {
            IList<string> result = null;

            try
            {
                result = dbScopeManagerGR.Execute(conn => handler.GetPushMsgPersonConfig(conn));
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex.Message);
            }

            return result;
        }

        public List<TagConfigModel> GetAllPushTag()
        {
            List<TagConfigModel> result = null;

            try
            {
                result = dbScopeManagerBI.Execute(conn => handler.GetAllPushTag(conn));
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex.Message);
                ExceptionMonitor.AddNewMonitor("Push", ex, "GetAllPushTag", MonitorLevel.Error);
            }

            return result;
        }
    }
}
