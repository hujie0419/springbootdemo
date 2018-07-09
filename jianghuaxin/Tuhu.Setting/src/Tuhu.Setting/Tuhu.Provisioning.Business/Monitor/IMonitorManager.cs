using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.Monitor
{
    public interface IMonitorManager
    {
        void AddMonitorInfo(string subjectType, string subjectId, string errorMessage, string operationUser, string operationName, MonitorLevel level = MonitorLevel.Info, MonitorModule module = MonitorModule.Other);


    }
}
