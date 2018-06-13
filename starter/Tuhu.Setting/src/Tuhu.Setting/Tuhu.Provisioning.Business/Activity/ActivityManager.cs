using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business.Activity
{
    public class ActivityManager
    {
        #region Private Fields  
        private static readonly IConnectionManager ConnectionManager = new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);
        private static readonly IDBScopeManager DbScopeManager = new DBScopeManager(ConnectionManager);

        private readonly IDBScopeManager DbGungnirManager = null;
        private readonly IDBScopeManager DbGungnirReadOnlyManager = null;

        private readonly ActivityHandler handler;
        private readonly ActivityHandler handlerReadonly;

        #endregion

        public ActivityManager()
        {
            string gungnirConnStr = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            string gungnirReadOnlyConnStr = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(gungnirConnStr))
            {
                gungnirConnStr = SecurityHelp.DecryptAES(gungnirConnStr);
            }
            if (SecurityHelp.IsBase64Formatted(gungnirReadOnlyConnStr))
            {
                gungnirReadOnlyConnStr = SecurityHelp.DecryptAES(gungnirReadOnlyConnStr);
            }

            DbGungnirManager = new DBScopeManager(new ConnectionManager(gungnirConnStr));
            DbGungnirReadOnlyManager = new DBScopeManager(new ConnectionManager(gungnirReadOnlyConnStr));
            handler = new ActivityHandler();
            handlerReadonly = new ActivityHandler();
        }
    }
}
