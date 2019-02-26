using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public static class DalActivityIntroduction
    {
        /// <summary>
        /// 获取活动信息列表
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static List<ActivityIntroductionModel> GetAllActivityIntroductionList(string activityName, int pageIndex, int pageSize)
        {
            var parameters = new[]
            {
                new SqlParameter("@pageIndex", pageIndex),
                new SqlParameter("@pageSize", pageSize),
                new SqlParameter("@activityName", string.IsNullOrWhiteSpace(activityName)?DBNull.Value:(object)activityName)
            };

            return DbHelper.ExecuteDataTable("proc_ActivityIntroduction", CommandType.StoredProcedure, parameters).ConvertTo<ActivityIntroductionModel>().ToList();
        }
        /// <summary>
        /// 新增活动信息
        /// </summary>
        /// <param name="Pay"></param>
        /// <param name="Qd"></param>
        /// <param name="Type"></param>
        public static int AddOrUpActivityIntroduction(ActivityIntroductionModel activity, string Type)
        {
            var listPar = new List<SqlParameter>();
            listPar.Add(new SqlParameter("@ActivityName", activity.ActivityName));
            listPar.Add(new SqlParameter("@ActivityChannel", activity.ActivityChannel));
            listPar.Add(new SqlParameter("@StartTime", activity.StartTime));
            listPar.Add(new SqlParameter("@EndTime", activity.EndTime));
            listPar.Add(new SqlParameter("@ActivityContent", activity.ActivityContent));

            string sSql = @"";
            if (activity.ID == 0) //新增
            {
                listPar.Add(new SqlParameter("@CreateUser", activity.CreateUser));
                sSql = @"INSERT  INTO dbo.tbl_ActivityIntroduction
                                ( ActivityName ,
                                  ActivityChannel ,
                                  StartTime ,
                                  EndTime ,
                                  ActivityContent ,
                                  CreateUser
                                )
                        VALUES  ( @ActivityName ,
                                  @ActivityChannel ,
                                  @StartTime ,
                                  @EndTime ,
                                  @ActivityContent ,
                                  @CreateUser
                                ) ";
            }
            else
            {
                listPar.Add(new SqlParameter("@ID", activity.ID));
                sSql = @"UPDATE dbo.tbl_ActivityIntroduction
                           SET ActivityName = @ActivityName
                              ,ActivityChannel =  @ActivityChannel
                              ,StartTime = @StartTime
                              ,EndTime = @EndTime
                              ,ActivityContent =   @ActivityContent 
                         WHERE ID = @ID";
            }

            return DbHelper.ExecuteNonQuery(sSql, CommandType.Text, listPar.ToArray());
        }

        /// <summary>
        /// 删除活动信息
        /// </summary>
        /// <param name="ID"></param>
        public static int DeleteActivityIntroductionById(int ID)
        {
            var listPar = new List<SqlParameter>();
            listPar.Add(new SqlParameter("@ID", ID));

            string sSql = @"delete from dbo.tbl_ActivityIntroduction where ID = @ID";
            return DbHelper.ExecuteNonQuery(sSql, CommandType.Text, listPar.ToArray());
        }

        public static ActivityIntroductionModel GetActivityIntroductionById(SqlConnection connection, int ID)
        {
            ActivityIntroductionModel activity = null;
            string sSql = @"SELECT  ID ,
                                    ActivityName ,
                                    ActivityChannel ,
                                    StartTime ,
                                    EndTime ,
                                    ActivityContent ,
                                    CreateTime ,
                                    CreateUser ,
                                    Shorder ,
                                    Status
                            FROM    dbo.tbl_ActivityIntroduction AS act WITH ( NOLOCK ) where ID = @ID";
            SqlParameter[] parameters =
                {
                    new SqlParameter("@ID", ID)
                };

            using (var reader = SqlHelper.ExecuteReader(connection, CommandType.Text, sSql, parameters))
            {
                while (reader.Read())
                {
                    activity = new ActivityIntroductionModel();

                    activity.ID = reader.GetTuhuValue<int>(0);
                    activity.ActivityName = reader.GetTuhuString(1);
                    activity.ActivityChannel = reader.GetTuhuString(2);
                    activity.StartTime = reader.GetTuhuValue<DateTime>(3);
                    activity.EndTime = reader.GetTuhuValue<DateTime>(4);
                    activity.ActivityContent = reader.GetTuhuString(5);
                }
            }
            return activity;
        }
    }
}
