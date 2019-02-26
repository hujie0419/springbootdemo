using Microsoft.ApplicationBlocks.Data;
using System;
using System.Data;
using System.Data.SqlClient;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public static class DalMonitor
    {

        public static void Add(SqlConnection connection, object param)
        {
            var sqlparams = (object[])param;
            var parameters = new[]
            {
                new SqlParameter("@SubjectType", sqlparams[0].ToString()),
                new SqlParameter("@SubjectId", sqlparams[1].ToString()),
                new SqlParameter("@ErrorMessage", sqlparams[2].ToString()),
                new SqlParameter("@OperationUser", sqlparams[3].ToString()),
                new SqlParameter("@OperationName",sqlparams[4].ToString()),
                new SqlParameter("@MonitorLevel",(int)sqlparams[5]) ,
                new SqlParameter("@MonitorModule",EnumStringHelper.GetEnumDescription(sqlparams[6])) ,
                new SqlParameter("@OperationTime", DateTime.Now)
            };

            SqlHelper.ExecuteNonQuery(connection, CommandType.StoredProcedure, "procMonitoringOperationsAdd_V2", parameters);
        }

    }
}
