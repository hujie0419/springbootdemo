using BaoYangRefreshCacheService.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu;

namespace BaoYangRefreshCacheService.DAL
{
    public class VehiclePartDal
    {
        private static readonly string dbconn_Read = ConfigurationManager.ConnectionStrings["VehiclePartsRead"].ConnectionString;
        private static readonly string dbconn_Write = ConfigurationManager.ConnectionStrings["VehiclePartsWrite"].ConnectionString;
        public static Tuple<int, int> GetOeCodeLiYangMapMin()
        {
            const string sql = @"SELECT Min(PKID) as M,Max(PKID) as N
From  [Tuhu_epc].[dbo].[VehicleParts_LiYang] with(nolock)";
            Func<DataTable, Tuple<int, int>> action = delegate (DataTable dt)
            {
                if (dt != null && dt.Rows.Count > 0)
                {
                    return Tuple.Create(dt.Rows[0].GetValue<int>("M"), dt.Rows[0].GetValue<int>("N"));
                }
                return null;
            };
            using (var db = DbHelper.CreateDbHelper(dbconn_Read))
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandTimeout = 2 * 60;
                return db.ExecuteQuery(cmd, action);
            }
        }
        public static IEnumerable<VehiclePartsLiYangEsModel> GetOeCodeLiYangMap()
        {
            const string sql = @"SELECT Distinct
Category,
MainCategory,
SubGroup
From  [Tuhu_epc].[dbo].[VehicleParts_LiYang] with(nolock)";
            using (var db = DbHelper.CreateDbHelper(dbconn_Read))
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandTimeout = 5 * 60;
                var result = db.ExecuteSelect<VehiclePartsLiYangEsModel>(cmd);
                return result;
            }
        }
        public static async Task<IEnumerable<VehicleTypeIdMap>> GetVehicleTypeIdMap()
        {
            const string sql = @"SELECT DISTINCT
 [Tid]
,[ExternalId]
  FROM [Gungnir].[dbo].[tbl_Vehicle_Type_IdMap] WITH(NOLOCK)
  WHERE [Source]='LiYang'";
            using (var cmd = new SqlCommand(sql))
            {
                var result = await DbHelper.ExecuteSelectAsync<VehicleTypeIdMap>(true, cmd);
                return result;
            }
        }
        public static async Task<IEnumerable<LiYangVehicleModel>> GetLiYangVehicleModelS()
        {
            const string sql = @"SELECT DISTINCT
[LiYangLevelId]
,[LiYangId]
FROM [Tuhu_epc].[dbo].[LiYangVehicle_IdMap] WITH(NOLOCK)";
            using (var db = DbHelper.CreateDbHelper(dbconn_Read))
            using (var cmd = new SqlCommand(sql))
            {
                var result = await db.ExecuteSelectAsync<LiYangVehicleModel>(cmd);
                return result;
            }
        }

        public static bool BatchInsertDatas(List<VehiclePartsLiYangModel> datas)
        {
            DataTable dt = new DataTable("VehicleParts_LiYang_SplitIds");
            DataColumn dc0 = new DataColumn("PKID", Type.GetType("System.Int64"));
            DataColumn dc1 = new DataColumn("LiYangLevelId", Type.GetType("System.String"));
            DataColumn dc2 = new DataColumn("VehiclePartsLiYangId", Type.GetType("System.Int64"));
            DataColumn dc3 = new DataColumn("CreateDateTime", Type.GetType("System.DateTime"));
            DataColumn dc4 = new DataColumn("LastUpdateDateTime", Type.GetType("System.DateTime"));

            dt.Columns.Add(dc0);
            dt.Columns.Add(dc1);
            dt.Columns.Add(dc2);
            dt.Columns.Add(dc3);
            dt.Columns.Add(dc4);
            var datas_temp = datas.Distinct().ToArray();
            foreach (var item in datas_temp)
            {
                var dr = dt.NewRow();
                dr["PKID"] = DBNull.Value;
                dr["LiYangLevelId"] = item.LiYangId;
                dr["VehiclePartsLiYangId"] = item.PKID;
                dr["CreateDateTime"] = DateTime.Now;
                dr["LastUpdateDateTime"] = DateTime.Now;
                dt.Rows.Add(dr);
            }
            using (var db = DbHelper.CreateDbHelper(dbconn_Write))
            {
                try
                {
                    using (var cmd = new SqlBulkCopy(db.Connection.ConnectionString))
                    {
                        cmd.BatchSize = datas_temp.Count();
                        cmd.BulkCopyTimeout = 60 * 5;
                        cmd.DestinationTableName = $"[Tuhu_epc].[dbo].[VehicleParts_LiYang_SplitIds]";
                        cmd.WriteToServer(dt);
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 获取所有Tid与LiYangLevelId对应关系
        /// </summary>
        /// <param name="liYangLevelId"></param>
        /// <param name="tid"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<VehicleTypeIdMap>> GetVehicleTypeLevelIdMap()
        {
            const string sql = @"SELECT DISTINCT
                                        s.Tid ,
                                        s.ExternalId
                                FROM    Gungnir..tbl_Vehicle_Type_IdMap AS s WITH ( NOLOCK )
                                WHERE   s.Source = 'LiYangLevelId';";
            using (var cmd = new SqlCommand(sql))
            {
                var result = await DbHelper.ExecuteSelectAsync<VehicleTypeIdMap>(true, cmd);
                return result;
            }
        }

        /// <summary>
        /// 插入Tid与LiYangLevelId对应关系
        /// </summary>
        /// <param name="liYangLevelId"></param>
        /// <param name="tid"></param>
        /// <returns></returns>
        public static bool InsertTidLiYangLevelIdMaps(string liYangLevelId, string tid)
        {
            const string sql = @"Insert into  [Gungnir].[dbo].tbl_Vehicle_Type_IdMap 
(Tid, ExternalId, Source, CreateTime, UpdateTime)
Values(@Tid,@LiYangLevelId,'LiYangLevelId',Getdate(),GetDate())";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@LiYangLevelId", liYangLevelId);
                cmd.Parameters.AddWithValue("@Tid", tid);
                return (DbHelper.ExecuteNonQuery(cmd)) > 0;
            }
        }

        public static bool InsertVehicleParts_LiYangCategory(string Category, string MainCategory, string SubGroup, long[] pkids)
        {
            const string sql = @"Insert into  [Tuhu_epc].[dbo].VehicleParts_LiYangCategory 
(Category, MainCategory, SubGroup, CreateTime, LastUpdateTime, VehicleParts_LiYang_PKID)
Values(@Category,@MainCategory,@SubGroup,Getdate(),GetDate(),@VehicleParts_LiYang_PKID)";
            using (var db = DbHelper.CreateDbHelper(dbconn_Read))
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@Category", Category);
                cmd.Parameters.AddWithValue("@MainCategory", MainCategory);
                cmd.Parameters.AddWithValue("@SubGroup", SubGroup);
                cmd.Parameters.AddWithValue("@VehicleParts_LiYang_PKID", string.Join(",", pkids));
                return (db.ExecuteNonQuery(cmd)) > 0;
            }
        }

        public static IEnumerable<long> GetPkIdsByCategory(string Category, string MainCategory, string SubGroup)
        {
            const string sql = @"SELECT PKID
From  [Tuhu_epc].[dbo].[VehicleParts_LiYang] with(nolock)
WHERE Category=@Category AND MainCategory=@MainCategory AND SubGroup=@SubGroup";
            List<long> list = new List<long>();

            using (var db = DbHelper.CreateDbHelper(dbconn_Read))
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandTimeout = 5 * 60;
                cmd.Parameters.AddWithValue("@Category", Category);
                cmd.Parameters.AddWithValue("@MainCategory", MainCategory);
                cmd.Parameters.AddWithValue("@SubGroup", SubGroup);
                var result = db.ExecuteQuery(cmd, (DataTable dt) =>
                {
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow item in dt.Rows)
                        {
                            list.Add(item.GetValue<long>("PKID"));
                        }
                    }
                    return list;
                });
                return result;
            }
        }

        public static bool DeleteVehicleParts_LiYangCategory()
        {
            const string sql = @"DELETE FROM   [Tuhu_epc].[dbo].VehicleParts_LiYangCategory";
            using (var db = DbHelper.CreateDbHelper(dbconn_Read))
            using (var cmd = new SqlCommand(sql))
            {
                return (db.ExecuteNonQuery(cmd)) > 0;
            }
        }
    }
}
