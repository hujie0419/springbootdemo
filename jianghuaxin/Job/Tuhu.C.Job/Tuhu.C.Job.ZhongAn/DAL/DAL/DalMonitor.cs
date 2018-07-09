using K.DLL.Common;
using K.Domain;
using System;
using System.Data.SqlClient;

namespace K.DLL.DAL
{
    public static class DalMonitor
    {
        public static string AddMonitorOperation(MonitorOperation operation)
        {
            var parameters = new[]
            {
                new SqlParameter("@SubjectType", operation.SubjectType ?? string.Empty),
                new SqlParameter("@SubjectId", operation.SubjectId ?? string.Empty),
                new SqlParameter("@ErrorMessage", operation.ErrorMessage ?? string.Empty),
                new SqlParameter("@OperationUser", operation.OperationUser ?? string.Empty),
                new SqlParameter("@OperationName", operation.OperationName ?? string.Empty),
                new SqlParameter("@MonitorLevel", operation.MonitorLevel) ,
                new SqlParameter("@MonitorModule", operation.MonitorModule ?? string.Empty) ,
                new SqlParameter("@OperationTime", DateTime.Now)
            };

            return TuhuLogDataOp.GetPara(@"INSERT  INTO dbo.MonitoringOperations  
            (SubjectType, SubjectId, ErrorMessage, OperationUser, OperationName, OperationTime, IsActive, MonitorLevel, MonitorModule)  
            VALUES  (@SubjectType, @SubjectId, @ErrorMessage, @OperationUser, @OperationName, @OperationTime, 1, @MonitorLevel, @MonitorModule);
            SELECT @@IDENTITY", parameters);
        }
    }
}
