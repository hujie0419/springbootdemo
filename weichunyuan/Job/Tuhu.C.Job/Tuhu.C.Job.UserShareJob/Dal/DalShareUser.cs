using System.Collections.Generic;
using Tuhu.C.Job.UserShareJob.Model;
using System.Data.SqlClient;
using System;
using System.Linq;
using System.Data;
using Common.Logging;
using System.Configuration;

namespace Tuhu.C.Job.UserShareJob.Dal
{
    public class DalShareUser
    {
        private static ILog DalShareUserLogger = LogManager.GetLogger<DalShareUser>();

        public static IEnumerable<ShareUserModel> GetAllShareUsers(int pageIndex, int pageSize)
        {
            const string sql = @"select distinct(UserId) from (
 select RecommendUserId UserId from Gungnir.dbo.RecNewUserObject with(nolock)
 union all
 select Invitation_Userid UserId  from SystemLog.dbo.tbl_Recommend with(nolock)
 ) t  Order by UserId
 OFFSET @Begin ROWS FETCH NEXT @PageSize ROWS  ONLY";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@Begin", (pageIndex - 1) * pageSize);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);
                return DbHelper.ExecuteSelect<ShareUserModel>(true, cmd);
            }
        }

        public static IEnumerable<UserShareAwardmodel> GetShareUserDatas_Old(List<ShareUserModel> userIds)
        {
            const string sql = @"SELECT newdata.* ,
        ISNULL(( SELECT TOP 1
                        RIO.Integral
                 FROM   Gungnir.dbo.RecIntegralOperation AS RIO WITH ( NOLOCK )
                 WHERE  RIO.UserID = newdata.RecommendUserId
                        AND RIO.RecommendUserID = newdata.RecommendUserId
                        AND RIO.RecommendUserID IS NOT NULL
                        AND RIO.CreateDate>@BeginData  and RIO.Createdate<@EndData
               ), 0) AS Award,
        IsFromOldTalbe=1,
RecommendUserId UserId
 FROM   ( SELECT   Invitation_UserID RecommendUserId
          FROM      SystemLog.dbo.tbl_Recommend AS R  WITH ( NOLOCK )
          WHERE     Invitation_UserID  IN({0}) 
                    AND IsOld = 0
                    AND IsGet=1
                    AND Recommend_Date>@BeginData  and Recommend_Date<@EndData
                    
        ) AS newdata";
            System.Text.StringBuilder sbr = new System.Text.StringBuilder(256);
            userIds.ForEach(f =>
            {
                sbr.AppendFormat("'{0}',", f.UserId);
            });
            string sqlSearch = string.Format(sql, sbr.ToString().TrimEnd(new char[] { ',' }));
            using (var cmd = new SqlCommand(sqlSearch))
            {
                cmd.Parameters.AddWithValue("@BeginData", DateTime.Now.Date.AddDays(-7));
                cmd.Parameters.AddWithValue("@EndData", DateTime.Now.Date);
                return DbHelper.ExecuteSelect<UserShareAwardmodel>(true, cmd);
            }
        }

        public static IEnumerable<UserShareAwardmodel> GetShareUserDatas_New(List<ShareUserModel> userIds)
        {
            const string sql = @"SELECT newdata.* ,
                                ISNULL(( SELECT TOP 1
                                                CASE WHEN RIO.Integral = 0
                                                     THEN RIO.PromotionMoney
                                                     ELSE ( RIO.Integral
                                                            + 0.00 ) / 100
                                                END
                                         FROM   Gungnir.dbo.RecIntegralOperation
                                                AS RIO WITH ( NOLOCK )
                                         WHERE  RIO.UserID = newdata.RecommendUserID
                                                AND RIO.RecommendUserID = newdata.RegisterUserID
                                                AND RIO.RecommendUserID IS NOT NULL
                                                AND RIO.CreateDate>@BeginData  and RIO.Createdate<@EndData
                                       ),
                                       CASE WHEN newdata.IsCompleteOrder = 1
                                            THEN 20
                                            WHEN newdata.IsGetPlace = 1
                                            THEN 20
                                            ELSE 0
                                       END) AS Award,
                                RecommendUserID UserId,
                                IsFromOldTalbe=0
                         FROM   ( SELECT    RecommendUserID ,
                                            RegisterUserID,
                                            IsCompleteOrder ,
                                            IsGetPlace ,
                                            IsOldUser 
                                  FROM      Gungnir.[dbo].[RecNewUserObject] AS R WITH ( NOLOCK )
                                  WHERE     RecommendUserID  IN({0}) 
                                            AND IsOldUser = 0
                                            AND IsCompleteOrder=1
                                            AND UpdateDate>@BeginData AND UpdateDate<@EndData
                                ) AS newdata";
            System.Text.StringBuilder sbr = new System.Text.StringBuilder(256);
            userIds.ForEach(f =>
            {
                sbr.AppendFormat("'{0}',", f.UserId);
            });
            string sqlSearch = string.Format(sql, sbr.ToString().TrimEnd(new char[] { ',' }));
            using (var cmd = new SqlCommand(sqlSearch))
            {
                cmd.Parameters.AddWithValue("@BeginData", DateTime.Now.Date.AddDays(-7));
                cmd.Parameters.AddWithValue("@EndData", DateTime.Now.Date);
                return DbHelper.ExecuteSelect<UserShareAwardmodel>(true, cmd);
            }
        }

        public static bool CreateUserShareRankingData(List<UserShareRankingModel> datas)
        {
            const string sqlDelete = @"Delete from Gungnir.dbo.UserShareRanking 
  where RecommendUserID IN({0})";
            DataTable dt = new DataTable("UserShareRanking");
            DataColumn dc0 = new DataColumn("PKID", Type.GetType("System.Int32"));
            DataColumn dc1 = new DataColumn("RecommendUserID", Type.GetType("System.Guid"));
            DataColumn dc2 = new DataColumn("UserNickName", Type.GetType("System.String"));
            DataColumn dc3 = new DataColumn("UserHeadImg", Type.GetType("System.String"));
            DataColumn dc4 = new DataColumn("TotalReward", Type.GetType("System.Decimal"));
            DataColumn dc5 = new DataColumn("UpdateTime", Type.GetType("System.DateTime"));
            DataColumn dc6 = new DataColumn("CreateTime", Type.GetType("System.DateTime"));
            dt.Columns.Add(dc0);
            dt.Columns.Add(dc1);
            dt.Columns.Add(dc2);
            dt.Columns.Add(dc3);
            dt.Columns.Add(dc4);
            dt.Columns.Add(dc5);
            dt.Columns.Add(dc6);
            foreach (var item in datas)
            {
                var dr = dt.NewRow();
                dr["PKID"] = DBNull.Value;
                dr["RecommendUserID"] = item.UserId;
                dr["UserNickName"] = item.UserNickName;
                dr["UserHeadImg"] = item.UserHeadImg;
                dr["TotalReward"] = item.TotalReward;
                dr["CreateTime"] = DateTime.Now;
                dr["UpdateTime"] = DateTime.Now;
                dt.Rows.Add(dr);
            }
            using (var db = DbHelper.CreateDbHelper())
            {
                try
                {
                    System.Text.StringBuilder sbr = new System.Text.StringBuilder(256);
                    datas.ForEach(f =>
                    {
                        sbr.AppendFormat("'{0}',", f.UserId);
                    });
                    string sqlSearch = string.Format(sqlDelete, sbr.ToString().TrimEnd(new char[] { ',' }));
                    db.BeginTransaction();
                    db.ExecuteNonQuery(new SqlCommand(sqlSearch));
                    using (var cmd = new SqlBulkCopy(db.Connection.ConnectionString))
                    {
                        cmd.BatchSize = datas.Count();
                        cmd.BulkCopyTimeout = 30;
                        cmd.DestinationTableName = "dbo.UserShareRanking";
                        cmd.WriteToServer(dt);
                        db.Commit();
                        return true;
                    }

                }
                catch (Exception ex)
                {
                    db.Rollback();
                    DalShareUserLogger.Error(ex);
                    return false;
                }
            }
        }

        public static void GetUserNickNameAndHeadImg(List<UserShareRankingModel> Models)
        {
            const string sql = @"Select UserId, u_pref5 NickName,u_Imagefile HeadImg from tuhu_profiles.[dbo].[UserObject] with(nolock) where userid in({0})";
            Func<DataTable, List<UserShareRankingModel>> action = delegate (DataTable dt)
            {
                if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (Models.Where(w => w.UserId == dt.Rows[i].GetValue<Guid>("UserId")).FirstOrDefault() != null)
                        {
                            Models.Where(w => w.UserId == dt.Rows[i].GetValue<Guid>("UserId")).FirstOrDefault().UserNickName = dt.Rows[i].GetValue<string>("NickName");
                            Models.Where(w => w.UserId == dt.Rows[i].GetValue<Guid>("UserId")).FirstOrDefault().UserHeadImg = dt.Rows[i].GetValue<string>("HeadImg");
                        }
                    }
                }
                return Models;
            };
            System.Text.StringBuilder sbr = new System.Text.StringBuilder(256);
            Models.ForEach(f =>
            {
                sbr.AppendFormat("'{0}',", f.UserId);
            });
            string sqlSearch = string.Format(sql, sbr.ToString().TrimEnd(new char[] { ',' }));
            using (var cmd = new SqlCommand(sqlSearch))
            {
                DbHelper.ExecuteQuery(true, cmd, action);
            }
        }

        public static IEnumerable<SendCodeForUserGroupModel> GetNeedExportData()
        {
            const string sql = @"SELECT [ID]
      ,[GroupId]
      ,[UserId]
      ,[SendCode]
      ,[GetUserId]
      ,[GetUserName]
      ,[CreateDateTime]
      ,[UpdateDateTime]
      ,[UserPhone]
  FROM [Tuhu_bi].[dbo].[SE_SendCodeForUserGroup] with(nolock)";
            try
            {
                using (var db = DbHelper.CreateLogDbHelper())
                {
                    using (var cmd = new SqlCommand(sql))
                    {
                        return db.ExecuteSelect<SendCodeForUserGroupModel>(cmd);
                    }
                }
            }
            catch (Exception ex)
            {
                DalShareUserLogger.Error($"批量获取推荐有礼用户群体设置失败：{ex}");
                return null;
            }
        }

        public static bool GetGroupIdIsExist(int groupId)
        {
            const string sql = @"SELECT top 1 1
  FROM [Configuration].[dbo].[SE_SendCodeForUserGroup] with(nolock) where groupId=@groupId";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@groupId",groupId);
                return DbHelper.ExecuteScalar(cmd) != null;
            }

        }
        public static bool CreateSendCodeForUserGroupData(List<SendCodeForUserGroupModel> datas)
        {
            DataTable dt = new DataTable("SE_SendCodeForUserGroup");
            DataColumn dc0 = new DataColumn("ID", Type.GetType("System.Int32"));
            DataColumn dc1 = new DataColumn("GroupId", Type.GetType("System.Int32"));
            DataColumn dc2 = new DataColumn("UserId", Type.GetType("System.Guid"));
            DataColumn dc3 = new DataColumn("SendCode", Type.GetType("System.String"));
            DataColumn dc4 = new DataColumn("GetUserId", Type.GetType("System.Guid"));
            DataColumn dc5 = new DataColumn("GetUserName", Type.GetType("System.String"));
            DataColumn dc6 = new DataColumn("CreateDateTime", Type.GetType("System.DateTime"));
            DataColumn dc7 = new DataColumn("UpdateDateTime", Type.GetType("System.DateTime"));
            DataColumn dc8 = new DataColumn("UserPhone", Type.GetType("System.String"));
            dt.Columns.Add(dc0);
            dt.Columns.Add(dc1);
            dt.Columns.Add(dc2);
            dt.Columns.Add(dc3);
            dt.Columns.Add(dc4);
            dt.Columns.Add(dc5);
            dt.Columns.Add(dc6);
            dt.Columns.Add(dc7);
            dt.Columns.Add(dc8);
            foreach (var item in datas)
            {
                var dr = dt.NewRow();
                dr["ID"] = DBNull.Value;
                dr["GroupId"] = item.GroupId;
                dr["UserId"] = item.UserId;
                dr["SendCode"] = item.SendCode;
                dr["GetUserId"] = DBNull.Value;
                dr["GetUserName"] = DBNull.Value;
                dr["CreateDateTime"] = item.CreateDateTime;
                dr["UpdateDateTime"] = DateTime.Now;
                dr["UserPhone"] = item.UserPhone;
                dt.Rows.Add(dr);
            }
            using (var db = DbHelper.CreateDbHelper())
            {
                try
                {
                    using (var cmd = new SqlBulkCopy(db.Connection.ConnectionString))
                    {
                        cmd.BatchSize = datas.Count();
                        cmd.BulkCopyTimeout = 30;
                        cmd.DestinationTableName = "Configuration.dbo.SE_SendCodeForUserGroup";
                        cmd.WriteToServer(dt);
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    DalShareUserLogger.Error($"批量导入推荐有礼用户群体设置失败：GroupId={datas.FirstOrDefault()?.GroupId};error:{ex}");
                    return false;
                }
            }
        }
    }
}

