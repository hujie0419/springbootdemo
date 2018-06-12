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
    public class DalVehiclePartsLiYang
    {
        /// <summary>
        /// 批量插入全车件数据
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public static bool BatchAddDalVehiclePartsLiYang(SqlConnection conn, SqlTransaction trans, DataTable table)
        {
            using (var sqlBulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.KeepIdentity, trans))
            {
                sqlBulkCopy.BatchSize = 8000;
                sqlBulkCopy.DestinationTableName = "Tuhu_epc.dbo.VehicleParts_LiYang";
                foreach (DataColumn cloumn in table.Columns)
                {
                    sqlBulkCopy.ColumnMappings.Add(cloumn.ColumnName, cloumn.ColumnName);
                }
                sqlBulkCopy.WriteToServer(table);
                return true;
            }
        }

        public static bool DeleteVehiclePartsLiYangByBrandandSeries(SqlConnection conn, SqlTransaction trans, int areaKey, string series)
        {

            var sql = @"Delete from Tuhu_epc.dbo.VehicleParts_LiYang Where AreaKey=@acreKey and Series=@series";
            using (var cmd = new SqlCommand(sql, conn, trans))
            {
                cmd.Parameters.AddWithValue("@acreKey", areaKey);
                cmd.Parameters.AddWithValue("@series", series);
                return cmd.ExecuteNonQuery() >= 0;
            }

        }

        public static string GetBrandById(SqlConnection conn, SqlTransaction trans, int pkid)
        {
            var sql = @"SELECT top 1 [Brand]+'-'+[SubBrand]  FROM [Tuhu_epc].[dbo].[VehicleParts_LiYangBrand] with(nolock) where pkid=@pkid";
            using (var cmd = new SqlCommand(sql, conn, trans))
            {
                cmd.Parameters.AddWithValue("@pkid", pkid);
                return cmd.ExecuteScalar().ToString();
            }
        }
    }
}
