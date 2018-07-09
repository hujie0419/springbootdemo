using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Common.Logging;
using Tuhu.C.Job.UserGrouping.Models;

namespace Tuhu.C.Job.UserGrouping.Dal
{
  public  class DalUserGrouping
    {
        private static ILog Logger = LogManager.GetLogger<DalUserGrouping>();
        public static IEnumerable<UserGroupingModel> GetUserGroupingModel()
        {
            using (var dbHelper = DbHelper.CreateLogDbHelper(true))
            {
                using (var cmd = new SqlCommand(@"select DeviceId,UserId,Tag,StartDateTime,EndDateTime  from Tuhu_bi..tbl_UserStatisticsGrouping with(nolock) where StartDateTime <getdate()and EndDateTime >getdate()"))
                {
                    cmd.CommandType = CommandType.Text;
                    return dbHelper.ExecuteSelect<UserGroupingModel>(cmd);
                }
            }
        }
        public static UserGroupingModel GetLogUserGroupingModel(UserGroupingModel model)
        {
            using (var dbHelper = DbHelper.CreateLogDbHelper(true))
            {
                using (var cmd = new SqlCommand(@"select DeviceId,UserId,Tag,StartDateTime,EndDateTime  from Tuhu_Log..tbl_UserStatisticsGrouping with(nolock) where  DeviceId=@DeviceId and UserId=@UserId and Tag=@Tag "))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@DeviceId", model.DeviceId);
                    cmd.Parameters.AddWithValue("@UserId", model.UserId);
                    cmd.Parameters.AddWithValue("@Tag", model.Tag);
                    return dbHelper.ExecuteFetch<UserGroupingModel>(cmd);
                }
            }
        }
        public static int InserttblUserStatisticsGrouping(UserGroupingModel model)
        {
            using (var dbHelper = DbHelper.CreateLogDbHelper())
            {
                using (var cmd = new SqlCommand(@"Insert into Tuhu_Log..tbl_UserStatisticsGrouping(DeviceId,UserId,Tag,StartDateTime,EndDateTime) values(@DeviceId,@UserId,@Tag,@StartDateTime,@EndDateTime)"))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@DeviceId", model.DeviceId);
                    cmd.Parameters.AddWithValue("@UserId", model.UserId);
                    cmd.Parameters.AddWithValue("@Tag", model.Tag);
                    cmd.Parameters.AddWithValue("@StartDateTime", model.StartDateTime);
                    cmd.Parameters.AddWithValue("@EndDateTime", model.EndDateTime);
                    return dbHelper.ExecuteNonQuery(cmd);
                }
            }
        }

        public static int UpdatetblUserStatisticsGrouping(UserGroupingModel model)
        {
            using (var dbHelper = DbHelper.CreateLogDbHelper())
            {
                using (var cmd = new SqlCommand(@"Update Tuhu_Log..tbl_UserStatisticsGrouping set StartDateTime=@StartDateTime,EndDateTime=@EndDateTime where DeviceId=@DeviceId and UserId=@UserId and Tag=@Tag"))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@DeviceId", model.DeviceId);
                    cmd.Parameters.AddWithValue("@UserId", model.UserId);
                    cmd.Parameters.AddWithValue("@Tag", model.Tag);
                    cmd.Parameters.AddWithValue("@StartDateTime", model.StartDateTime);
                    cmd.Parameters.AddWithValue("@EndDateTime", model.EndDateTime);
                    return dbHelper.ExecuteNonQuery(cmd);
                }
            }
        }

        public static bool BatchInserttblUserStatisticsGrouping(List<UserGroupingModel> datas)
        {
            DataTable dt = new DataTable("tbl_UserStatisticsGrouping");
            DataColumn dc0 = new DataColumn("PKID", Type.GetType("System.Int32"));
            DataColumn dc1 = new DataColumn("DeviceId", Type.GetType("System.Guid"));
            DataColumn dc2 = new DataColumn("UserId", Type.GetType("System.Guid"));
            DataColumn dc3 = new DataColumn("Tag", Type.GetType("System.String"));
            DataColumn dc4 = new DataColumn("StartDateTime", Type.GetType("System.DateTime"));
            DataColumn dc5 = new DataColumn("EndDateTime", Type.GetType("System.DateTime"));

            dt.Columns.Add(dc0);
            dt.Columns.Add(dc1);
            dt.Columns.Add(dc2);
            dt.Columns.Add(dc3);
            dt.Columns.Add(dc4);
            dt.Columns.Add(dc5);
            foreach (var item in datas)
            {
                var dr = dt.NewRow();
                dr["PKID"] = DBNull.Value;
                dr["DeviceId"] = item.DeviceId;
                dr["UserId"] = item.UserId;
                dr["Tag"] = item.Tag;
                dr["StartDateTime"] = item.StartDateTime;
                dr["EndDateTime"] = item.EndDateTime;
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
                        cmd.DestinationTableName = "Tuhu_Log..tbl_UserStatisticsGrouping";
                        cmd.WriteToServer(dt);
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"批量插入用户分组数据失败：error:{ex}");
                    return false;
                }
            }
        }
    }
}
