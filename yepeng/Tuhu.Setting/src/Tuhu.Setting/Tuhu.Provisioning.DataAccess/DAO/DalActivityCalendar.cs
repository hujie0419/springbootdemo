using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalActivityCalendar
    {
        public static void Add(SqlConnection connection, ActivityCalendar activityDetail)
        {
            var parameters = new[]
            {
                 new SqlParameter("@Owner", activityDetail.Owner?? string.Empty),
                 new SqlParameter("@OwnerType", activityDetail.OwnerType?? string.Empty),
                 new SqlParameter("@BeginDate", activityDetail.BeginDate),
                 new SqlParameter("@EndDate", activityDetail.EndDate),
                 new SqlParameter("@ActivityTitle", activityDetail.ActivityTitle?? string.Empty),
                 new SqlParameter("@ActivityContent", activityDetail.ActivityContent?? string.Empty),
                 new SqlParameter("@ActivityUrl", activityDetail.ActivityUrl?? string.Empty),
                 new SqlParameter("@ScheduleType", activityDetail.ScheduleType?? string.Empty),
                 new SqlParameter("@CreateDate", activityDetail.CreateDate),
                 new SqlParameter("@CreateBy", activityDetail.CreateBy?? string.Empty),
                 new SqlParameter("@DataFrom", activityDetail.DataFrom?? string.Empty),
                 new SqlParameter("@DataFromID", activityDetail.DataFromId.HasValue? (object)activityDetail.DataFromId.Value : DBNull.Value),
                 new SqlParameter("@IsActive",activityDetail.IsActive)
            };

            SqlHelper.ExecuteNonQuery(connection, CommandType.StoredProcedure, "ActivityDetail_InsertActivityDetail", parameters);
        }

        public static ActivityCalendar GetActivity_Detail(SqlConnection connection, int pkid)
        {
            ActivityCalendar activityDetail = null;

            var parameters = new[]
            {
                new SqlParameter("@PKID", pkid)
            };

            using (var dataReader = SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure, "procSelectActivity_Detail", parameters))
            {
                if (dataReader.Read())
                {
                    activityDetail = new ActivityCalendar();

                    activityDetail.Pkid = dataReader.GetTuhuValue<int>(0);
                    activityDetail.Owner = dataReader.GetTuhuString(1);
                    activityDetail.OwnerType = dataReader.GetTuhuString(2);
                    activityDetail.BeginDate = dataReader.GetTuhuValue<System.DateTime>(3);
                    activityDetail.EndDate = dataReader.GetTuhuNullableValue<DateTime>(4);
                    activityDetail.ActivityTitle = dataReader.GetTuhuString(5);
                    activityDetail.ActivityContent = dataReader.GetTuhuString(6);
                    activityDetail.ActivityUrl = dataReader.GetTuhuString(7);
                    activityDetail.ScheduleType = dataReader.GetTuhuString(8);
                    activityDetail.CreateDate = dataReader.GetTuhuValue<System.DateTime>(9);
                    activityDetail.CreateBy = dataReader.GetTuhuString(10);
                    activityDetail.DataFrom = dataReader.GetTuhuString(11);
                    activityDetail.DataFromId = dataReader.GetTuhuNullableValue<int>(12);
                    activityDetail.IsActive = dataReader.GetTuhuValue<bool>(13);
                }
            }

            return activityDetail;
        }
        public static List<ActivityCalendar> SelectActivityByCondition(SqlConnection connection, string sqlWhere)
        {
            var activityList = new List<ActivityCalendar>();

            var parameters = new[]
            {
                new SqlParameter("@sqlWhere", sqlWhere)
            };

            using (var dataReader = SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure, "Actovity_SelectActivityDetail", parameters))
            {
                while (dataReader.Read())
                {
                    var activityDetail = new ActivityCalendar
                    {
                        Pkid = dataReader.GetTuhuValue<int>(0),
                        Owner = dataReader.GetTuhuString(1),
                        OwnerType = dataReader.GetTuhuString(2),
                        BeginDate = dataReader.GetTuhuValue<System.DateTime>(3),
                        EndDate = dataReader.GetTuhuNullableValue<DateTime>(4),
                        ActivityTitle = dataReader.GetTuhuString(5),
                        ActivityContent = dataReader.GetTuhuString(6),
                        ActivityUrl = dataReader.GetTuhuString(7),
                        ScheduleType = dataReader.GetTuhuString(8),
                        CreateDate = dataReader.GetTuhuValue<System.DateTime>(9),
                        CreateBy = dataReader.GetTuhuString(10),
                        DataFrom = dataReader.GetTuhuString(11),
                        DataFromId = dataReader.GetTuhuNullableValue<int>(12),
                        IsActive = dataReader.GetTuhuValue<bool>(13),
                        LastUpdateDate = dataReader.GetTuhuValue<DateTime>(14)
                    };
                    activityList.Add(activityDetail);
                }
            }

            return activityList;
        }

        public static void UpdateIsActivity(SqlConnection conn, int datafromId, string dataFrom, bool status)
        {
            var parameters = new[]
            {
                 new SqlParameter("@dataFromID", datafromId),
                 new SqlParameter("@dataFrom", dataFrom),
                 new SqlParameter("@Status", status)

            };

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "Actovity_UpdateIsActive", parameters);
        }
        public static void UpdateIsActivity(SqlConnection conn, int datafromId, string dataFrom, bool status, DateTime lastUpdatetime)
        {
            var parameters = new[]
            {
                 new SqlParameter("@dataFromID", datafromId),
                 new SqlParameter("@dataFrom", dataFrom),
                 new SqlParameter("@Status", status),
                 new SqlParameter("@LastUpdateDate",lastUpdatetime),

            };

            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "Actovity_UpdateIsActive_V2", parameters);
        }
    }
}
