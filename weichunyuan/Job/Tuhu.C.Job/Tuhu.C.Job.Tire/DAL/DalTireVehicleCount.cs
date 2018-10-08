using Common.Logging;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.C.Job.Tire.Model;

namespace Tuhu.C.Job.Tire.DAL
{
    public class DalTireVehicleCount
    {
        public static IEnumerable<VehicleMatchTireModel> GetVehicleData()
        {
            const string sql = @"SELECT  ProductID ,
        TiresMatch
FROM    Gungnir..tbl_Vehicle_Type WITH ( NOLOCK )
WHERE   LEN(TiresMatch) > 0
        AND LEN(ProductID) > 0;";
            using (var cmd = new SqlCommand(sql))
            {
                return DbHelper.ExecuteSelect<VehicleMatchTireModel>(true, cmd);
            }
        }
        public static void SetTireMatchVehicle(List<TireMatchVehicleCount> data, ILog logger)
        {
            const string sql = @"SELECT PID FROM Configuration..tbl_TireMatchVehicleCount WITH(NOLOCK)";
            var dat = DbHelper.ExecuteSelect<TireMatchVehicleCount>(true, new SqlCommand(sql)).Select(g => g.PID);
            var dat4delete = dat.Where(g => !data.Select(t => t.PID).Contains(g)).Select(g => $"N'{g}'");
            if (dat4delete.Any())
            {
                string sql4delete = @"DELETE  FROM Configuration..tbl_TireMatchVehicleCount
        WHERE   PID IN ( " + string.Join(",", dat4delete) + @" );";
                DbHelper.ExecuteNonQuery(new SqlCommand(sql4delete));
            }
            logger.Info($"轮胎适配车型数量统计,删除{dat4delete.Count()}条数据");
            var dat4update = data.Where(g => dat.Contains(g.PID));
            var sql4update = @"UPDATE  Configuration..tbl_TireMatchVehicleCount
        SET     VehicleCount = @Count, UpdateDateTime=GETDATE()
        WHERE   PID = @pid;";
            dat4update.ForEach(g =>
            {
                using (var cmd = new SqlCommand(sql4update))
                {
                    cmd.Parameters.AddWithValue("@Count", g.VehicleCount);
                    cmd.Parameters.AddWithValue("@pid", g.PID);
                    DbHelper.ExecuteNonQuery(cmd);
                }
            });
            logger.Info($"轮胎适配车型数量统计,更新{dat4update.Count()}条数据");
            var dat4insert = data.Where(g => !dat.Contains(g.PID)).Select(g => $"(N'{g.PID}',{g.VehicleCount})");
            if (dat4insert.Any())
            {
                string sql4insert = @"INSERT  INTO Configuration..tbl_TireMatchVehicleCount
        ( PID, VehicleCount ) VALUES " + string.Join(",", dat4insert) + @" ;";
                DbHelper.ExecuteNonQuery(new SqlCommand(sql4insert));
            }
            logger.Info($"轮胎适配车型数量统计,新增{dat4insert.Count()}条数据");
        }

    }
}
