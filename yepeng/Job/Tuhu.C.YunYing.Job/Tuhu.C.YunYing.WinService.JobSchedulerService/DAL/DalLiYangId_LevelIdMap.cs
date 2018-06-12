using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.YunYing.WinService.JobSchedulerService.Model;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.DAL
{
   public class DalLiYangId_LevelIdMap
    {
        /// <summary>
        /// 批量导入LiYangId和LiYangLevelId对应关系
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public static bool BatchAddLiYangId_LevelIdMap(SqlConnection conn, SqlTransaction trans, DataTable table)
        {
            using (var sqlBulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.KeepIdentity, trans))
            {
                sqlBulkCopy.BatchSize = 8000;
                sqlBulkCopy.DestinationTableName = "Tuhu_epc.dbo.LiYangVehicle_IdMap";
                foreach (DataColumn cloumn in table.Columns)
                {
                    sqlBulkCopy.ColumnMappings.Add(cloumn.ColumnName, cloumn.ColumnName);
                }
                sqlBulkCopy.WriteToServer(table);
                return true;
            }
        }
    }
}
