using Common.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.ActivityJob.Models.Rebate;

namespace Tuhu.C.ActivityJob.Dal.Rebate
{
    public class RebateDal
    {
        protected static readonly ILog Logger = LogManager.GetLogger(typeof(RebateDal));

        /// <summary>
        /// 获取16元返现中未关联安装门店的数据
        /// </summary>
        /// <returns></returns>
        public static List<RebateApplyConfigModel> GetRebateApplyConfigList()
        {
            string sql = @"
                            SELECT PKID,
                                   OrderId,
                                   OpenId,
                                   UserPhone,
                                   InstallShopId
                              FROM Activity.dbo.RebateApplyConfig WITH (NOLOCK)
                             WHERE Source            = 'Rebate16'
                               AND IsDelete          = 0
                               AND (   InstallShopId IS NULL
                                  OR   InstallShopId = ''
                                  OR   InstallShopId = 0);";
            using (var cmd = new SqlCommand(sql))
            {
                var result = DbHelper.ExecuteSelect<RebateApplyConfigModel>(true, cmd);
                return result == null ? new List<RebateApplyConfigModel>() : result.ToList();
            }
        }

        /// <summary>
        /// 批量更新安装门店
        /// </summary>
        /// <param name="vehicleProductList"></param>
        /// <returns></returns>
        public static int BatchUpdateInstallShopId(IEnumerable<IEnumerable<RebateApplyConfigModel>> vehicleProductList)
        {
            int updateCount = 0;
         
                try
                {
                    var vehicleProductNewList = vehicleProductList.ToList();
                    Parallel.For(0, vehicleProductNewList.Count, new ParallelOptions() { MaxDegreeOfParallelism = 4 }, (i) =>
                    {
                        using(var dbHelper = DbHelper.CreateDbHelper(false))
                        using (var cmd = new SqlCommand(""))
                        {
                            var RebateInfoTmp = vehicleProductNewList[i].Select(m => new
                            {
                                PKID = m.PKID,
                                InstallShopId = m.InstallShopId
                            });
                            DataTable RebateInfoDT = ToDataTable(RebateInfoTmp);

                            cmd.CommandText = @"
                                            CREATE TABLE #RebateInfoTmp" + i + @" ([PKID] [INT] NOT NULL,
                                                                            [InstallShopId] [INT] NULL);";
                            int a = dbHelper.ExecuteNonQuery(cmd);
                                
                            using (SqlBulkCopy bulkcopy = new SqlBulkCopy(dbHelper.Connection as SqlConnection))
                            {
                                bulkcopy.DestinationTableName = $"#RebateInfoTmp{i}";
                                bulkcopy.WriteToServer(RebateInfoDT);
                                bulkcopy.Close();
                            }

                            string sql = @"
                                        MERGE INTO [Activity].[dbo].[RebateApplyConfig] WITH (ROWLOCK) AS R
                                        USING #RebateInfoTmp" + i + @" AS temp
                                            ON R.PKID = temp.PKID
                                        WHEN MATCHED THEN UPDATE SET R.InstallShopId = temp.InstallShopId,
						                                                R.UpdateTime = GETDATE();";
                            cmd.CommandText = sql;
                            int result = dbHelper.ExecuteNonQuery(cmd);
                            updateCount = updateCount + result;

                        }
                    });
                }
                catch (Exception ex)
                {
                    Logger.Error($"BatchUpdateInstallShopId接口异常：{ex.Message},堆栈信息：{ex.StackTrace}");
                }
            return updateCount;
        }

        /// <summary>
        /// IEnumerable转DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 根据来源获取返现申请数据
        /// </summary>
        /// <param name="sources"></param>
        /// <returns></returns>
        public static List<RebateApplyConfigModel> GetRebateApplyConfigListBySource(List<string> sources)
        {
            string sql = @"
                        SELECT PKID,
                               OrderId,
                               UserPhone,
                               Source,
                               OpenId,
                               ActivityId
                          FROM Activity.dbo.RebateApplyConfig AS R WITH (NOLOCK)
                          JOIN Activity..SplitString(@Sources, ',', 1) AS B
                            ON R.Source = B.Item
                         WHERE R.IsDelete = 0
                         ORDER BY Source;";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@Sources", string.Join(",", sources));
                var result = DbHelper.ExecuteSelect<RebateApplyConfigModel>(true, cmd);
                return result == null ? new List<RebateApplyConfigModel>() : result.ToList();
            }
        }

        /// <summary>
        /// 批量更新活动id
        /// </summary>
        /// <param name="rebateApplyList"></param>
        /// <returns></returns>
        public static int BatchUpdateActivityId(IEnumerable<IEnumerable<RebateApplyConfigModel>> rebateApplyList)
        {
            int updateCount = 0;

            try
            {
                var rebateApplyNewList = rebateApplyList.ToList();
                Parallel.For(0, rebateApplyNewList.Count, new ParallelOptions() { MaxDegreeOfParallelism = 4 }, (i) =>
                {
                    using (var dbHelper = DbHelper.CreateDbHelper(false))
                    using (var cmd = new SqlCommand(""))
                    {
                        var RebateInfoTmp = rebateApplyNewList[i].Select(m => new
                        {
                            PKID = m.PKID,
                            ActivityId = m.ActivityId
                        });
                        DataTable RebateInfoDT = ToDataTable(RebateInfoTmp);

                        cmd.CommandText = @"
                                            CREATE TABLE #RebateInfoTmp" + i + @" ([PKID] [INT] NOT NULL,
                                                                            [ActivityId] [uniqueidentifier] NULL);";
                        int a = dbHelper.ExecuteNonQuery(cmd);

                        using (SqlBulkCopy bulkcopy = new SqlBulkCopy(dbHelper.Connection as SqlConnection))
                        {
                            bulkcopy.DestinationTableName = $"#RebateInfoTmp{i}";
                            bulkcopy.WriteToServer(RebateInfoDT);
                            bulkcopy.Close();
                        }

                        string sql = @"
                                        MERGE INTO [Activity].[dbo].[RebateApplyConfig] WITH (ROWLOCK) AS R
                                        USING #RebateInfoTmp" + i + @" AS temp
                                            ON R.PKID = temp.PKID
                                        WHEN MATCHED THEN UPDATE SET R.ActivityId = temp.ActivityId,
						                                                R.UpdateTime = GETDATE();";
                        cmd.CommandText = sql;
                        int result = dbHelper.ExecuteNonQuery(cmd);
                        updateCount = updateCount + result;

                    }
                });
            }
            catch (Exception ex)
            {
                Logger.Error($"BatchUpdateActivityId接口异常：{ex.Message},堆栈信息：{ex.StackTrace}");
            }
            return updateCount;
        }

        /// <summary>
        /// 获取所有待审核的返现申请数据
        /// </summary>
        /// <returns></returns>
        public static List<RebateApplyConfigModel> GetApplyingRebateList()
        {
            string sql = @"SELECT  PKID,
                                    OrderId,
                                    UserPhone,
                                    Status,
                                    ActivityId
                                FROM Activity.dbo.RebateApplyConfig WITH (NOLOCK)
                                WHERE Status   = 'Applying'
                                AND IsDelete = 0;";
            using (var cmd = new SqlCommand(sql))
            {
                var result = DbHelper.ExecuteSelect<RebateApplyConfigModel>(true, cmd);
                return result == null ? new List<RebateApplyConfigModel>() : result.ToList();
            }
        }

        /// <summary>
        /// 审核状态更新为已支付
        /// </summary>
        /// <param name="pkidList"></param>
        /// <returns></returns>
        public static int UpdateRebateStatus(IEnumerable<IEnumerable<int>> pkidList)
        {
            int updateStatusCount = 0;
            try
            {
                var pkidNewList = pkidList.ToList();
                Parallel.For(0, pkidNewList.Count, new ParallelOptions() { MaxDegreeOfParallelism = 4 }, (i) =>
                {
                    string sql = @"UPDATE r
                                       SET r.Status = 'Complete',
                                           r.UpdateTime = GETDATE()
                                      FROM [Activity].[dbo].[RebateApplyConfig] r WITH (ROWLOCK)
                                      JOIN Activity..SplitString(@pkidList, ',', 1) AS B
                                        ON r.PKID = B.Item;";
                    using (var cmd = new SqlCommand(sql))
                    {
                        cmd.Parameters.AddWithValue("@pkidList", string.Join(",", pkidNewList[i]));
                        var result = DbHelper.ExecuteNonQuery(false, cmd);
                        updateStatusCount = updateStatusCount + result;
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.Error($"UpdateRebateStatus接口异常：{ex.Message},堆栈信息：{ex.StackTrace}");
            }
            return updateStatusCount;
        }

        /// <summary>
        /// 根据活动id集合获取返现活动信息
        /// </summary>
        /// <param name="sources"></param>
        /// <returns></returns>
        public static List<RebateApplyPageConfig> GetRebateApplyPageConfigList(List<Guid> activityIds)
        {
            string sql = @"SELECT RP.ActivityId,
                                   RP.ActivityType,
                                   RP.ActivityName
                              FROM Activity.dbo.RebateApplyPageConfig AS RP WITH (NOLOCK)
                              JOIN Activity..SplitString(@ActivityIds, ',', 1) AS B
                                ON RP.ActivityId = B.Item
                             WHERE RP.IsDelete = 0 AND RP.ActivityType = 'Online';";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@ActivityIds", string.Join(",", activityIds));
                var result = DbHelper.ExecuteSelect<RebateApplyPageConfig>(true, cmd);
                return result == null ? new List<RebateApplyPageConfig>() : result.ToList();
            }
        }

        /// <summary>
        /// 获取返现申请数量
        /// </summary>
        /// <param name="syncAll"></param>
        /// <returns></returns>
        public static int GetRebateApplyCount(bool syncAll = false)
        {
            var condition = syncAll ? string.Empty : " CreateTime > DATEADD(HOUR, -24, GETDATE()) AND";

            using (var cmd = new SqlCommand($@"
                SELECT COUNT(1)
                FROM Activity.dbo.RebateApplyConfig WITH (NOLOCK)
                WHERE{condition} TechId IS NULL AND IsDelete = 0;"))
            {
                cmd.CommandType = CommandType.Text;
                return Convert.ToInt32(DbHelper.ExecuteScalar(true, cmd));
            }
        }

        /// <summary>
        /// 批量获取返现申请
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="maxPkid"></param>
        /// <param name="syncAll"></param>
        /// <returns></returns>
        public static List<RebateApplyConfigModel> GetRebateApplys(int pageSize, ref int maxPkid, bool syncAll = false)
        {
            var condition = syncAll ? string.Empty : " AND CreateTime > DATEADD(HOUR, -24, GETDATE())";

            using (var cmd = new SqlCommand($@"
                SELECT TOP {pageSize}
                       PKID,
                       OrderId
                FROM Activity.dbo.RebateApplyConfig WITH (NOLOCK)
                WHERE PKID > {maxPkid}{condition}
                      AND TechId IS NULL
                      AND IsDelete = 0
                ORDER BY PKID;"))
            {
                cmd.CommandType = CommandType.Text;
                var result = DbHelper.ExecuteSelect<RebateApplyConfigModel>(true, cmd)
                    ?? new List<RebateApplyConfigModel>();

                maxPkid = result.Any() ? result.Max(x => x.PKID) : maxPkid;
                return result.ToList();
            }
        }

        /// <summary>
        /// 批量更新技师ID
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public static bool BatchUpdateTechIds(List<PkidWithTechId> models)
        {
            using (var dbHelper = DbHelper.CreateDbHelper())
            {
                var dtModels = ToDataTable(models);
                dbHelper.BeginTransaction();

                try
                {
                    var sqlTemp = @"CREATE TABLE #RebateApplyConfigTemp([PKID] [INT] NOT NULL,
	                            [TechId] [INT] NULL);";

                    dbHelper.ExecuteNonQuery(sqlTemp);

                    using (SqlBulkCopy bulkcopy = new SqlBulkCopy((SqlConnection)dbHelper.Connection, SqlBulkCopyOptions.KeepIdentity, (SqlTransaction)dbHelper.Transaction))
                    {
                        bulkcopy.BulkCopyTimeout = 660;
                        bulkcopy.DestinationTableName = "#RebateApplyConfigTemp";
                        bulkcopy.WriteToServer(dtModels);
                        bulkcopy.Close();
                    }

                    var sqlUpdate = @"MERGE INTO Activity.dbo.RebateApplyConfig WITH (ROWLOCK) AS R
                                USING #RebateApplyConfigTemp AS T
                                ON R.PKID = T.PKID
                                WHEN MATCHED THEN
                                    UPDATE SET R.TechId = T.TechId,
                                                R.UpdateTime = GETDATE();";

                    dbHelper.ExecuteNonQuery(sqlUpdate);
                    dbHelper.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    dbHelper.Rollback();
                    Logger.Error($"批量更新技师ID异常", ex);
                    return false;
                }
            }
        }
    }
}
