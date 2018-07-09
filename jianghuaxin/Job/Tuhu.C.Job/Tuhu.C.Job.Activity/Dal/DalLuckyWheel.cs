using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Tuhu.C.Job.Activity.Models;
using static System.String;

namespace Tuhu.C.Job.Activity.Dal
{
    public class DalLuckyWheel
    {
        private static ILog Logger = LogManager.GetLogger<DalLuckyWheel>();
        public static IEnumerable<LuckyWheelUserlotteryCountModel> GetBiLuckyWheelUserlotteryCount()
        {
            using (var dbHelper = DbHelper.CreateLogDbHelper(true))
            {
                using (var cmd = new SqlCommand(@"select * from Tuhu_bi..LuckyWheelUserlotteryCount with(nolock) order by CreateDateTime"))
                {
                    cmd.CommandType = CommandType.Text;
                    return dbHelper.ExecuteSelect<LuckyWheelUserlotteryCountModel>(cmd);
                }
            }
        }
        /// <summary>
        /// 大翻盘 查询 同步 记录是否存在
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="userGroup">分组id</param>
        /// <returns></returns>
        public static int GetLogLuckyWheelUserlotteryCount(Guid userId, Guid userGroup)
        {
            using (var dbHelper = DbHelper.CreateLogDbHelper(true))
            {
                using (var cmd = new SqlCommand(@"select top 1 PKID from Tuhu_Log..LuckyWheelUserlotteryCount with(nolock) where UserId= @UserId and userGroup=@UserGroup"))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@UserGroup", userGroup);
                    return Convert.ToInt32(dbHelper.ExecuteScalar(cmd));
                }
            }
        }

        public static int GetLogLuckyWheelUserlotteryCount(Guid userId, string HashKey)
        {
            using (var dbHelper = DbHelper.CreateLogDbHelper(true))
            {
                using (var cmd = new SqlCommand(@"select top 1 PKID from Tuhu_Log..LuckyWheelUserlotteryCount with(nolock) where UserId= @UserId and HashKey=@HashKey"))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@HashKey", HashKey);
                    return Convert.ToInt32(dbHelper.ExecuteScalar(cmd));
                }
            }
        }

        public static DateTime GetLogLuckyWheelUserlotteryCreateDateTime()
        {
            using (var dbHelper = DbHelper.CreateLogDbHelper(true))
            {
                using (var cmd = new SqlCommand(@"select top 1 CreateDateTime from Tuhu_Log..LuckyWheelUserlotteryCount with(nolock) order by CreateDateTime desc"))
                {
                    cmd.CommandType = CommandType.Text;
                    return Convert.ToDateTime(dbHelper.ExecuteScalar(cmd));
                }
            }
        }
        public static int UpdateLogLuckyWheelUserlotteryCount(LuckyWheelUserlotteryCountModel model)
        {
            string sql = "";
            if (IsNullOrWhiteSpace(model.HashKey))//大翻盘 【老逻辑】
            {
                sql = @"update Tuhu_Log..LuckyWheelUserlotteryCount with(rowlock) set Count=Count+@Count,Record=Record+@Record,UpdateDateTime=@UpdateDateTime  where UserId= @UserId and userGroup=@UserGroup";
            }
            else
            {
                sql = @"update Tuhu_Log..LuckyWheelUserlotteryCount with(rowlock) set Count=Count+@Count,Record=Record+@Record,UpdateDateTime=@UpdateDateTime  where UserId= @UserId and HashKey=@HashKey";
            }
            
            using (var dbHelper = DbHelper.CreateLogDbHelper())
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@UserId", model.UserId);
                    cmd.Parameters.AddWithValue("@UserGroup", model.UserGroup);
                    cmd.Parameters.AddWithValue("@Count", model.Count);
                    cmd.Parameters.AddWithValue("@CreateDateTime", model.CreateDateTime);
                    cmd.Parameters.AddWithValue("@UpdateDateTime", model.UpdateDateTime);

                    cmd.Parameters.AddWithValue("@HashKey", model.HashKey);
                    cmd.Parameters.AddWithValue("@Record", model.Record);

                    return Convert.ToInt32(dbHelper.ExecuteNonQuery(cmd));
                }
            }
        }
        public static int InsertLogLuckyWheelUserlotteryCountAsync(LuckyWheelUserlotteryCountModel model)
        {
            using (var dbHelper = DbHelper.CreateLogDbHelper())
            {
                using (var cmd = new SqlCommand(@"insert into Tuhu_Log..LuckyWheelUserlotteryCount (UserId,UserGroup,Count,Record) 
                                                values(@UserId,@UserGroup,@Count,0)"))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@UserId", model.UserId);
                    cmd.Parameters.AddWithValue("@UserGroup", model.UserGroup);
                    cmd.Parameters.AddWithValue("@Count", model.Count);
                    return Convert.ToInt32(dbHelper.ExecuteNonQuery(cmd));
                }
            }
        }

        public static int BatchDeleteLogLuckyWheelUserlotteryCount(List<int> datas)
        {
            var sql = @"delete  from tuhu_bi..LuckyWheelUserlotteryCount where PKID in ({0})";
            var parmSql = Format(sql, Join(",", datas));
            using (var dbHelper = DbHelper.CreateLogDbHelper())
            {
                using (var cmd = new SqlCommand(parmSql))
                {
                    cmd.CommandType = CommandType.Text;
                    return Convert.ToInt32(dbHelper.ExecuteNonQuery(cmd));
                }
            }
        }
        public static bool BatchInsertLogLuckyWheelUserlotteryCount(List<LuckyWheelUserlotteryCountModel> datas)
        {
            DataTable dt = new DataTable("LuckyWheelUserlotteryCount");
            DataColumn dc0 = new DataColumn("PKID", Type.GetType("System.Int32"));
            DataColumn dc1 = new DataColumn("UserGroup", Type.GetType("System.Guid"));
            DataColumn dc2 = new DataColumn("UserId", Type.GetType("System.Guid"));
            DataColumn dc3 = new DataColumn("Count", Type.GetType("System.Int32"));
            DataColumn dc4 = new DataColumn("Record", Type.GetType("System.Int32"));
            DataColumn dc5 = new DataColumn("CreateDateTime", Type.GetType("System.DateTime"));
            DataColumn dc6 = new DataColumn("UpdateDateTime", Type.GetType("System.DateTime"));
            DataColumn dc7 = new DataColumn("HashKey", Type.GetType("System.String"));

            dt.Columns.Add(dc0);
            dt.Columns.Add(dc1);
            dt.Columns.Add(dc2);
            dt.Columns.Add(dc3);
            dt.Columns.Add(dc4);
            dt.Columns.Add(dc5);
            dt.Columns.Add(dc6);
            dt.Columns.Add(dc7);
            foreach (var item in datas)
            {
                var dr = dt.NewRow();
                dr["PKID"] = DBNull.Value;
                dr["UserGroup"] = item.UserGroup;
                dr["UserId"] = item.UserId;
                dr["Count"] = item.Count;
                dr["Record"] = 0;
                dr["CreateDateTime"] = DateTime.Now;
                dr["UpdateDateTime"] = DateTime.Now;
                dr["HashKey"] = item.HashKey;
                dt.Rows.Add(dr);
            }
            using (var db = DbHelper.CreateLogDbHelper())
            {
                try
                {
                    using (var cmd = new SqlBulkCopy(db.Connection.ConnectionString))
                    {
                        cmd.BatchSize = datas.Count();
                        cmd.BulkCopyTimeout = 30;
                        cmd.DestinationTableName = "Tuhu_log..LuckyWheelUserlotteryCount";
                        cmd.WriteToServer(dt);
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"批量导入大翻盘抽奖记录失败：error:{ex}");
                    return false;
                }
            }
        }
    }
}
