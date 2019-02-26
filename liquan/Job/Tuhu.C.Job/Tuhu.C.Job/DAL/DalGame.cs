using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Tuhu.Service.Activity.Models.Response;
using Common.Logging;

namespace Tuhu.C.Job.DAL
{
    public class DalGame
    {
        private static readonly ILog Logger = LogManager.GetLogger<DalGame>();

        /// <summary>
        /// 插入游戏每日排名信息
        /// </summary>
        /// <param name="rankList"></param>
        /// <returns></returns>
        public static bool InsertGameRankList(List<GameRankInfoModel> rankList)
        {
            var result = false;
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    using (var cmd = new SqlCommand("", conn, tran))
                    {
                        try
                        {
                            var rankTmp = rankList.Select(x => new
                            {
                                UserId = x.UserID,
                                Rank = x.Rank,
                                Point = x.Point,
                                ActivityId = 6
                            });
                            DataTable productDT = ToDataTable(rankTmp);
                            cmd.CommandText = @"CREATE TABLE #rankTmp([UserId] uniqueidentifier NULL,
	                                                                   Rank int NULL,
	                                                                   Point int NULL,
                                                                      ActivityId int NULL);";
                            cmd.ExecuteNonQuery();
                            using (SqlBulkCopy bulkcopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.KeepIdentity, tran))
                            {
                                bulkcopy.BulkCopyTimeout = 660;
                                bulkcopy.DestinationTableName = "#rankTmp";
                                bulkcopy.WriteToServer(productDT);
                                bulkcopy.Close();
                            }
                            string sqlInsert = @"INSERT [Activity].[dbo].[tbl_GameDailyRank]
                                                                    (
                                                                        [ActivityId],
                                                                        [UserID],
                                                                        [Rank],
                                                                        [Point]
                                                                    )
                                                                    SELECT [ActivityId],
                                                                        [UserID],
                                                                        [Rank],
                                                                        [Point]
                                                                    FROM #rankTmp;";
                            cmd.CommandText = sqlInsert;
                            var dbResult = cmd.ExecuteNonQuery();
                            if (dbResult == rankList.Count)
                            {
                                tran.Commit();
                                result = true;
                            }
                            else
                            {
                                tran.Rollback();
                            }
                        }
                        catch (Exception ex)
                        {
                            result = false;
                            tran.Rollback();
                            Logger.Error($"InsertGameRankList", ex);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 获取上一活动日的奖品信息
        /// </summary>
        /// <returns></returns>
        public static int GetBumBelbeeLastDayPrizeCount()
        {
            string sql = @"Select COUNT(1)
                             From [Activity].[dbo].[tbl_GameUserPrize] With (NOLOCK)
                             WHERE DATEDIFF(DAY,GetPrizeDate,GETDATE())=1
                             AND activityid=6
                             AND PrizeType<>'FREECOUPON'";
            using (var cmd = new SqlCommand(sql))
            {
                var dbResult = DbHelper.ExecuteScalar(cmd);
                int.TryParse(dbResult.ToString(), out int result);
                return result;
            }
        }

        public static DataTable ToDataTable<T>(IEnumerable<T> collection)
        {
            var props = typeof(T).GetProperties();
            var dt = new DataTable();
            dt.Columns.AddRange(props.Select(p => new DataColumn(p.Name, p.PropertyType)).ToArray());
            if (collection.Count() > 0)
            {
                for (int i = 0; i < collection.Count(); i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in props)
                    {
                        object obj = pi.GetValue(collection.ElementAt(i), null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    dt.LoadDataRow(array, true);
                }
            }
            return dt;
        }
    }
}
