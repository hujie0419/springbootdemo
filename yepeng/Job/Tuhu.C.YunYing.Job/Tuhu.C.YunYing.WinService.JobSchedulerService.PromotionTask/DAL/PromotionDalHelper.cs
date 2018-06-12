using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Tuhu.C.YunYing.WinService.JobSchedulerService.PromotionTask.Model;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.PromotionTask.DAL
{
    public static class PromotionDalHelper
    {
        /// <summary>
        /// 关闭超期的触发任务
        /// </summary>
        public static int ClosePromotionTaskJob()
        {
            using (var cmd = new SqlCommand(@"
                   UPDATE Gungnir..tbl_promotiontask WITH(ROWLOCK)
					  SET TaskStatus = 2, 
						  Executetime = Getdate()  
					WHERE Taskendtime<=getdate() and TaskType=2 and TaskStatus = 1") {CommandTimeout = 10 * 60})
                return DbHelper.ExecuteNonQuery(cmd);
        }

        public static int ClosePromotionTaskJob(int promotionTaskId)
        {
            using (var cmd = new SqlCommand(@"
                   UPDATE  dbo.tbl_PromotionTask WITH ( ROWLOCK )  
                            SET     TaskStatus = 2 ,
                                    ExecuteTime = GETDATE() ,
                                    CloseTime = GETDATE() 
                            WHERE   PromotionTaskId = @PromotionTaskId; "))
            {
                cmd.Parameters.AddWithValue("@PromotionTaskId",promotionTaskId);
                return DbHelper.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// 获取有效的塞券任务
        /// 已审核+单次任务+超过执行时间，只获取SelectUserType为 1和 2的
        /// </summary>
        /// <returns></returns>
        public static List<PromotionTaskCls> GetValidPromotionTask()
        {
            using (var cmd = new SqlCommand(@"
                    SELECT PromotionTaskId,TaskName,TaskStartTime,CreateTime,UpdateTime,SelectUserType,IsLimitOnce,Tasktype,SmsId,SmsParam,PromotionTaskActivityId
                     FROM Gungnir..tbl_PromotionTask (NOLOCK)
                    WHERE taskStartTime<=getdate() and TaskStatus = 1 and TaskType=1 AND SelectUserType IN(1,2)") {CommandTimeout = 5 * 60})
                return DbHelper.ExecuteQuery<List<PromotionTaskCls>>(true,cmd, dt =>
                {
                    return dt.Rows.OfType<DataRow>().ConvertTo<PromotionTaskCls>().ToList();
                });
        }
        /// <summary>
        /// 获取有效的塞券任务
        /// 已审核+单次任务+超过执行时间，只获取SelectUserType为 3
        /// </summary>
        /// <returns></returns>
        public static List<PromotionTaskCls> GetValidActivityPromotionTask()
        {
            using (var cmd = new SqlCommand(@"
                    SELECT PromotionTaskId,TaskName,TaskStartTime,CreateTime,UpdateTime,SelectUserType,IsLimitOnce,Tasktype,SmsId,SmsParam,PromotionTaskActivityId
                     FROM Gungnir..tbl_PromotionTask (NOLOCK)
                    WHERE taskStartTime<=getdate() and TaskStatus = 1 and TaskType=1 AND SelectUserType=3") { CommandTimeout = 5 * 60 })
                return DbHelper.ExecuteQuery<List<PromotionTaskCls>>(true, cmd, dt =>
                {
                    return dt.Rows.OfType<DataRow>().ConvertTo<PromotionTaskCls>().ToList();
                });
        }

        /// <summary>
        /// 获取单个任务
        /// </summary>
        /// <returns></returns>
        public static PromotionTaskCls GetPromotionTask(int promotionTaskId)
        {
            using (var cmd = new SqlCommand(@"
                    SELECT PromotionTaskId,TaskStatus,ExecuteTime
                     FROM Gungnir..tbl_PromotionTask (NOLOCK)
                    WHERE PromotionTaskId=@PromotionTaskId"))
            {
                cmd.Parameters.AddWithValue("@PromotionTaskId", promotionTaskId);
                return DbHelper.ExecuteFetch<PromotionTaskCls>(cmd);
            }

        }

        /// <summary>
        /// 执行塞券任务
        /// </summary>
        /// <param name="proTask">塞券任务</param>
        /// <returns></returns>
        public static void RunPromotionTask(PromotionTaskCls proTask)
        {
            using (var cmd = new SqlCommand(@"Promotion_SendPromotionToUserRepeatTaskJob"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 20 * 60; //20分钟超时
                cmd.Parameters.Add(
                    new SqlParameter("@PromotionTaskId", proTask?.PromotionTaskId ?? 0)
                );
                DbHelper.ExecuteNonQuery(cmd);
            }
        }

        public static IEnumerable<PromotionTaskHistoryUsers> GetPromotionTaskHistoryUsers(int minPkid, int pageSize,
            int promotionTaskId)
        {

            using (var cmd =
                new SqlCommand(
                    $@"SELECT TOP {
                            pageSize
                        } * FROM Gungnir..tbl_PromotionSingleTaskUsersHistory WITH(NOLOCK) WHERE PromotionSingleTaskUsersHistoryId>@MinPkid AND PromotionTaskId=@PromotionTaskId ORDER BY PromotionSingleTaskUsersHistoryId ASC"
                ))
            {
                cmd.Parameters.AddWithValue("@MinPkid", minPkid);
                cmd.Parameters.AddWithValue("@PromotionTaskId", promotionTaskId);
                return DbHelper.ExecuteSelect<PromotionTaskHistoryUsers>(cmd);
            }
        }

        public static IEnumerable<PromotionTaskUser> GetPromotionTaskUsers(int minPkid, int pageSize,
            int promotionTaskId)
        {
            using (var cmd =
                new SqlCommand(
                    $@"SELECT TOP {
                            pageSize
                        } * FROM Gungnir..tbl_PromotionSingleTaskUsers WITH(NOLOCK) WHERE PromotionSingleTaskUsersId>@MinPkid AND PromotionTaskId=@PromotionTaskId ORDER BY PromotionSingleTaskUsersId ASC"
                ))
            {
                cmd.Parameters.AddWithValue("@MinPkid", minPkid);
                cmd.Parameters.AddWithValue("@PromotionTaskId", promotionTaskId);
                return DbHelper.ExecuteSelect<PromotionTaskUser>(cmd);
            }
        }

        public static IEnumerable<PromotionTaskUserObject> GetPromotionTaskUserObjects(IEnumerable<string> mobiles)
        {
            using (var cmd =
                new SqlCommand(
                    $@"SELECT UserID,u_mobile_number AS Mobile FROM Tuhu_profiles..UserObject AS U WITH(NOLOCK) JOIN Tuhu_profiles..SplitString(@Mobiles,',',1) AS M ON U.u_mobile_number=M.Item WHERE U.IsActive=1"
                ))
            {
                cmd.Parameters.AddWithValue("@Mobiles", string.Join(",",mobiles));
                return DbHelper.ExecuteSelect<PromotionTaskUserObject>(true, cmd);
            }
        }

        public static IEnumerable<PromotionListInfo> GetTaskPromotionList(int promotionTaskId)
        {
            using (var cmd =
                new SqlCommand(
                    $@"SELECT * FROM Gungnir..tbl_PromotionTaskPromotionList WITH(NOLOCK) WHERE PromotionTaskId=@PromotionTaskId"
                ))
            {
                cmd.Parameters.AddWithValue("@PromotionTaskId", promotionTaskId);
                return DbHelper.ExecuteSelect<PromotionListInfo>(true, cmd);
            }
        }

        public static IEnumerable<CouponRules> GetCouponRules(IEnumerable<int> ruleIds)
        {
            using (var cmd =
                new SqlCommand(
                    $@"SELECT PKID,Name,Type FROM Activity..tbl_CouponRules WITH(NOLOCK) WHERE PKID IN({string.Join(",",ruleIds)})"
                ))
            {
                return DbHelper.ExecuteSelect<CouponRules>(true, cmd);
            }
        }

        public static int UpdatePromotionTaskActivityStatus(int promotionTaskActivityId, int promotionTaskId,
            int status)
        {
            using (var dbHelper = DbHelper.CreateLogDbHelper())
            {
                using (var cmd =dbHelper.CreateCommand(
                        @"UPDATE Tuhu_bi..tbl_PromotionTaskActivity WITH(ROWLOCK) SET Status=@Status,UpdateTime=GETDATE() WHERE PKID=@PKID AND PromotionTaskId=@PromotionTaskId AND Status=0"))
                {
                    cmd.Parameters.Add(new SqlParameter("@Status", status));
                    cmd.Parameters.Add(new SqlParameter("@PKID", promotionTaskActivityId));
                    cmd.Parameters.Add(new SqlParameter("@PromotionTaskId", promotionTaskId));
                    return dbHelper.ExecuteNonQuery(cmd);
                }
            }

        }

        public static IEnumerable<PromotionTaskActivityUsers> SelectPromotionTaskActivityUsers(long minPkid, int pageSize,int promotionTaskId,int promotionTaskActivityId)
        {
            using (var dbHelper = DbHelper.CreateLogDbHelper(true))
            {
                using (var cmd = dbHelper.CreateCommand(
                        $@"SELECT TOP {pageSize} U.* FROM Tuhu_bi..tbl_PromotionTaskActivityUsers as U WITH(NOLOCK)
                        join Tuhu_bi..tbl_PromotionTaskActivity AS A WITH(NOLOCK) ON U.PromotionTaskActivityId=A.PKID 
                        WHERE A.PKID=@PKID AND PromotionTaskId=@PromotionTaskId AND Status=0 AND U.PKID>{minPkid} ORDER BY U.PKID ASC "))
                {
                    cmd.Parameters.Add(new SqlParameter("@PKID", promotionTaskActivityId));
                    cmd.Parameters.Add(new SqlParameter("@PromotionTaskId", promotionTaskId));
                    return dbHelper.ExecuteSelect<PromotionTaskActivityUsers>(cmd);
                }
            }
        }

        public static int SelectPromotionTaskActivityUsersCount(long minPkid,int promotionTaskId, int promotionTaskActivityId)
        {
            using (var dbHelper = DbHelper.CreateLogDbHelper(true))
            {
                using (var cmd = dbHelper.CreateCommand(
                    $@"SELECT COUNT(1) FROM Tuhu_bi..tbl_PromotionTaskActivityUsers as U WITH(NOLOCK)
                        join Tuhu_bi..tbl_PromotionTaskActivity AS A WITH(NOLOCK) ON U.PromotionTaskActivityId=A.PKID 
                        WHERE A.PKID=@PKID AND PromotionTaskId=@PromotionTaskId AND Status=0 AND U.PKID>{minPkid} "))
                {
                    cmd.Parameters.Add(new SqlParameter("@PKID", promotionTaskActivityId));
                    cmd.Parameters.Add(new SqlParameter("@PromotionTaskId", promotionTaskId));
                    return (int)dbHelper.ExecuteScalar(cmd);
                }
            }
        }

        public static void BulkCopy(string toTableName, DataTable dt,string connStr)
        {
            var sqlConn = new SqlConnection(connStr);

            SqlBulkCopy bulkCopy = new SqlBulkCopy(sqlConn) {BulkCopyTimeout = 20 * 60};
            bulkCopy.DestinationTableName = toTableName;
            foreach (DataColumn c in dt.Columns)
            {
                bulkCopy.ColumnMappings.Add(c.ColumnName, c.ColumnName);
            }
            bulkCopy.BatchSize = dt.Rows.Count;
            try
            {
                sqlConn.Open();
                if (dt != null && dt.Rows.Count != 0)
                {
                    bulkCopy.WriteToServer(dt);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                sqlConn.Close();
                if (bulkCopy != null)
                    bulkCopy.Close();
            }
        }

        public static void MovePromotionTaskActivityUsers(IEnumerable<PromotionTaskActivityUsers> users ,int promotionTaskId)
        {
            var dt = GetTableSchema(users, promotionTaskId);
            BulkCopy("tbl_PromotionSingleTaskUsers", dt,
                ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);
        }

        public static DataTable GetTableSchema(IEnumerable<PromotionTaskActivityUsers> users, int promotionTaskId)
        {
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("PromotionTaskId", typeof(int)),
                new DataColumn("UserCellPhone", typeof(string)),
                new DataColumn("CeateTime", typeof(DateTime))
            });
            foreach (var u in users)
            {
                DataRow r = dt.NewRow();
                r["PromotionTaskId"] = promotionTaskId;
                r["UserCellPhone"] = u.UserTel;
                r["CeateTime"] = DateTime.Now;
                dt.Rows.Add(r);
            }
            return dt;
        }

        public static void MovePromotionTaskSingleUsers(IEnumerable<PromotionTaskUser> users)
        {
            var dt = GetSigngleUsersTableSchema(users);
            BulkCopy("tbl_PromotionSingleTaskUsersHistory", dt,
                ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);
        }
        public static DataTable GetSigngleUsersTableSchema(IEnumerable<PromotionTaskUser> users)
        {
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("PromotionTaskId", typeof(int)),
                new DataColumn("UserCellPhone", typeof(string)),
                new DataColumn("CreateTime", typeof(DateTime)),
                new DataColumn("SendState", typeof(int)),
                new DataColumn("OrderNo", typeof(string))
            });
            foreach (var u in users)
            {
                DataRow r = dt.NewRow();
                r["PromotionTaskId"] = u.PromotionTaskId;
                r["UserCellPhone"] = u.UserCellPhone;
                r["SendState"] = 0;
                r["OrderNo"] = u.OrderNo;
                r["CreateTime"] = DateTime.Now;
                dt.Rows.Add(r);
            }
            return dt;
        }

        public static void RemovePromotionTaskSingleUsers(IEnumerable<int> ids)
        {
            using (var cmd =
                new SqlCommand(
                    $@"DELETE FROM Gungnir..tbl_PromotionSingleTaskUsers WHERE PromotionSingleTaskUsersId IN(SELECT Item FROM Gungnir..SplitString(@Ids,',',1))"
                ))
            {
                cmd.Parameters.AddWithValue("@Ids", string.Join(",", ids));
                DbHelper.ExecuteNonQuery(cmd);
            }
        }

        public static void RemovePromotionTaskSingleUsersByMobiles(IEnumerable<string> mobiles,int promotionTaskId)
        {
            using (var cmd =
                new SqlCommand(
                    $@"DELETE FROM Gungnir..tbl_PromotionSingleTaskUsers WHERE UserCellPhone IN(SELECT Item FROM Gungnir..SplitString(@Mobiles,',',1)) AND PromotionTaskId=@PromotionTaskId"
                ))
            {
                cmd.Parameters.AddWithValue("@Mobiles", string.Join(",", mobiles));
                cmd.Parameters.AddWithValue("@PromotionTaskId", promotionTaskId);
                DbHelper.ExecuteNonQuery(cmd);
            }
        }

        public static DataTable SelectIsPushGetCouponRules()
        {
            using (var cmd =
                new SqlCommand(
                    $@"SELECT PKID,RuleId FROM Activity..tbl_GetCouponRules AS GCR WITH(NOLOCK) WHERE GCR.IsPush=1 AND (DeadLineDate IS NULL OR DeadLineDate>GETDATE())"
                ))
            {
                return DbHelper.ExecuteQuery(true, cmd, dt => dt);
            }
        }

        public static IEnumerable<PromotionCodeModel> SelectPushPromotion(int minPkid, int pageSize,IEnumerable<int> ruleIds,IEnumerable<int> getRuleIds,int days)
        {
            if (getRuleIds != null && getRuleIds.Any() && ruleIds != null && ruleIds.Any())
            {
                string startTime = DateTime.Now.AddDays(days).ToString("yyyy-MM-dd");
                string endTime = DateTime.Now.AddDays(days).ToString("yyyy-MM-dd") + " 23:59:59";
                using (var cmd =
                    new SqlCommand(
                        $@"SELECT TOP {pageSize} P.PKID,P.UserId FROM  Gungnir..tbl_PromotionCode AS P WITH(NOLOCK)
                        WHERE P.PKID>@MinPkid"
//#if !DEBUG
                        +$@" AND P.EndTime BETWEEN @StartTime AND @EndTime
                             AND RuleId IN({string.Join(",", ruleIds)})
                             AND GetRuleId IN({string.Join(",", getRuleIds)})"
//#endif
                        + " ORDER BY P.PKID ASC;"
                    ))
                {
                    cmd.Parameters.AddWithValue("@MinPkid", minPkid);
                    cmd.Parameters.AddWithValue("@StartTime", startTime);
                    cmd.Parameters.AddWithValue("@EndTime", endTime);
                    return DbHelper.ExecuteSelect<PromotionCodeModel>(true, cmd);
                }
            }
            return null;
        }


        public static IEnumerable<PromotionCodeModel> SelectPushPromotionByUserId(IEnumerable<string> page,IEnumerable<int> ruleIds,IEnumerable<int> getRuleIds,int days)
        {
            if (getRuleIds != null && getRuleIds.Any()&& page != null && page.Any() && ruleIds != null && ruleIds.Any())
            {
                string startTime = DateTime.Now.AddDays(days).ToString("yyyy-MM-dd");
                string endTime = DateTime.Now.AddDays(days).ToString("yyyy-MM-dd") + " 23:59:59";
                using (var cmd =
                    new SqlCommand(
                        $@"SELECT P.UserId,Discount FROM  Gungnir..tbl_PromotionCode AS P WITH(NOLOCK)
                        WHERE UserId IN (SELECT Item FROM Gungnir..SplitString(@Page,',', 1)) AND Status=0 "
//#if !DEBUG
                        +$@" AND P.EndTime BETWEEN @StartTime AND @EndTime
                             AND RuleId IN({string.Join(",", ruleIds)})
                             AND GetRuleId IN({string.Join(",", getRuleIds)})"
//#endif
                        + " ORDER BY P.PKID ASC;"
                    ))
                {
                    cmd.Parameters.Add(new SqlParameter("@Page", string.Join(",", page)));
                    cmd.Parameters.Add(new SqlParameter("@StartTime", startTime));
                    cmd.Parameters.Add(new SqlParameter("@EndTime", endTime));
                    return DbHelper.ExecuteSelect<PromotionCodeModel>(true, cmd);
                }
            }
            return null;
        }

        public static void MoveFilterOrderData(int promotionTaskId)
        {
            DataTable data;
            using (var cmd = new SqlCommand("Gungnir.dbo.Promotion_FilterUserByCondition") {CommandTimeout = 20 * 60})
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FilterStartTime", null);
                cmd.Parameters.AddWithValue("@FilterEndTime", null);
                cmd.Parameters.AddWithValue("@Brand", null);
                cmd.Parameters.AddWithValue("@Category", null);
                cmd.Parameters.AddWithValue("@Pid", null);
                cmd.Parameters.AddWithValue("@SpendMoney", 0);
                cmd.Parameters.AddWithValue("@PurchaseNum", 0);
                cmd.Parameters.AddWithValue("@Area", null);
                cmd.Parameters.AddWithValue("@Channel", null);
                cmd.Parameters.AddWithValue("@InstallType", null);
                cmd.Parameters.AddWithValue("@OrderStatus", 0);
                cmd.Parameters.AddWithValue("@Vehicle", null);
                cmd.Parameters.AddWithValue("@PromotionTaskId", promotionTaskId);
                cmd.Parameters.AddWithValue("@FilterOrderNo", null);
                data = DbHelper.ExecuteQuery(true, cmd, dt => dt);
            }
            if (data != null && data.Rows.Count > 0)
            {
                data.Columns.Add(new DataColumn("PromotionTaskId", typeof(int)) {DefaultValue = promotionTaskId});
                data.Columns["MobileNum"].ColumnName = "UserCellPhone";
                data.Columns.Remove("UserID");
                data.Columns.Add(new DataColumn("CeateTime", typeof(DateTime)) {DefaultValue = DateTime.Now});
                BulkCopy("tbl_PromotionSingleTaskUsers", data,
                    ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);
            }
        }
    }
}
