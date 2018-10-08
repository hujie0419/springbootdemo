using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalActivitySchedule
    {
        /// <summary>
        /// 查询活动安排数据
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static List<ActivitySchedule> SelectActivitySchedule(SqlConnection connection)
        {
            var activityList = new List<ActivitySchedule>();

            using (var reader = SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure, "Activity_SelectActivityScheduleData"))
            {
                while (reader.Read())
                {
                    var activity = new ActivitySchedule();
                    activity.start = reader.GetTuhuValue<DateTime>(0);
                    activity.end = reader.GetTuhuValue<DateTime>(1);
                    activity.title = reader.GetTuhuString(2);
                    activity.allDay = true;
                    activityList.Add(activity);
                }
            }
            return activityList;
        }

        /// <summary>
        /// 根据活动标题查询活动具体安排内容
        /// </summary>
        /// <param name="title">活动标题</param>
        public static List<ActivityCalendar> SelectActivityDetailData(SqlConnection connection, string title, string date)
        {
            var activityDetailList = new List<ActivityCalendar>();
            var parameters = new[]
            {
                new SqlParameter("@Title",title),
                new SqlParameter("@Date",date)
            };

            using (
                var reader = SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure,
                    "Activity_SelectActivityDetailDataByTitle", parameters))
            {
                while (reader.Read())
                {
                    var activityDetail = new ActivityCalendar();
                    activityDetail.Owner = reader.GetTuhuString(0);
                    activityDetail.BeginDate = reader.GetTuhuValue<DateTime>(1);
                    activityDetail.EndDate = reader.GetTuhuValue<DateTime>(2);
                    activityDetail.ActivityTitle = reader.GetTuhuString(3);
                    activityDetail.ActivityContent = reader.GetTuhuString(4);
                    activityDetail.ActivityUrl = reader.GetTuhuString(5);
                    activityDetailList.Add(activityDetail);
                }
            }
            return activityDetailList;
        }


        public static List<ActivityCalendar> SelectCurrentDayActivity(SqlConnection connection)
        {
            var activityList = new List<ActivityCalendar>();
            var sql = @"  SELECT DISTINCT
                                    AD.Owner ,
                                    AD.BeginDate ,
                                    AD.EndDate ,
                                    AD.ActivityTitle 
                            FROM    dbo.Activity_Detail AS AD WITH ( NOLOCK )
                            WHERE   DATEDIFF(dd, AD.BeginDate, GETDATE()) >= 0
                                    AND DATEDIFF(dd, AD.EndDate, GETDATE()) <= 0";
            using (var reader = SqlHelper.ExecuteReader(connection, CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    var activity = new ActivityCalendar();
                    activity.Owner = reader.GetTuhuString(0);
                    activity.BeginDate = reader.GetTuhuValue<DateTime>(1);
                    activity.EndDate = reader.GetTuhuValue<DateTime>(2);
                    activity.ActivityTitle = reader.GetTuhuString(3);
                    activityList.Add(activity);
                }
            }
            return activityList;
        }
    }
}
