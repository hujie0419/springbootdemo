using System;
using System.Collections.Generic;
using System.Linq;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Component.Framework;
using Tuhu.Component.Framework.Identity;

namespace Tuhu.Provisioning.Business.Monitor
{
    public static class ExceptionMonitor
    {
        private static readonly IMonitorManager Manager = new MonitorManager();

        private static readonly ILog Logger = LoggerFactory.GetLogger("ExceptionMonitor");

        public static void AddNewMonitor(string subjectType, string subjectId, string errorMessage, string operationUser,
            string operationName, MonitorLevel level = MonitorLevel.Info, MonitorModule module = MonitorModule.Other)
        {
            try
            {
                Manager.AddMonitorInfo(subjectType, subjectId, errorMessage, operationUser, operationName, level, module);
            }
            catch (Exception ex)
            {
                var exception = new MonitorException(BizErrorCode.SystemError, "添加异常监控错误", ex);
                Logger.Log(Level.Error, "Error occurred in AddNewMonitor.", exception);
            }
        }

        public static void AddNewMonitor(string subjectType, Exception ex, string operationName, MonitorLevel level = MonitorLevel.Info, MonitorModule module = MonitorModule.Other)
        {
            AddNewMonitor(subjectType, string.Empty, ex.ToString(), ThreadIdentity.Operator.Name, operationName, level, module);
        }
    }
}
