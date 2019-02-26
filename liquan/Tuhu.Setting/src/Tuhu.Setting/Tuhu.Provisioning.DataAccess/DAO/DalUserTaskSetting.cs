using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Component.Common;
using System.Data;
using System;
using System.Configuration;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalUserTaskSetting
    {
        private static readonly String Gungnir_Readonly = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
        public static List<UserTaskSettingModel> GetUserTaskSettingList(int pageIndex, int pageSize)
        {
            const string sql = @"  SELECT Status,ImageUrl,PKID,TaskName,TaskDescription,StartTime,EndTime,CreateTime,UpdateTime,Tag_1,Tag_2,TargetButtonUrl  FROM Gungnir..UserTaskSetting WITH(NOLOCK) ORDER BY CreateTime Desc
 OFFSET @Begin rows Fetch Next @PageSize Rows ONLY";
            using (var cmd = new SqlCommand(sql, new SqlConnection(Gungnir_Readonly)))
            {
                cmd.Parameters.AddWithValue("@Begin", (pageIndex - 1) * pageSize);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);
                var temp = DbHelper.ExecuteDataTable(cmd);
                if (temp == null && temp.Rows.Count <= 0)
                    return null;
                return temp.Rows.OfType<DataRow>().Select(s => new UserTaskSettingModel
                {
                    PKID = Convert.ToInt32(s["PKID"]),
                    TaskName = s["TaskName"].ToString(),
                    TaskDescription = s["TaskDescription"].ToString(),
                    StartTime = Convert.ToDateTime(s["StartTime"] == DBNull.Value ? DateTime.Now : s["StartTime"]),
                    EndTime = Convert.ToDateTime(s["EndTime"] == DBNull.Value ? DateTime.Now : s["EndTime"]),
                    CreateTime = Convert.ToDateTime(s["CreateTime"] == DBNull.Value ? DateTime.Now : s["CreateTime"]),
                    UpdateTime = Convert.ToDateTime(s["UpdateTime"] == DBNull.Value ? DateTime.Now : s["UpdateTime"]),
                    Tag_1 = s["Tag_1"].ToString(),
                    Tag_2 = s["Tag_2"].ToString(),
                    TargetButtonUrl = s["TargetButtonUrl"].ToString(),
                    ImageUrl = s["ImageUrl"].ToString(),
                    Status = s["Status"].ToString()
                }).ToList();
            }
            

        }
        public static UserTaskSettingModel GetUserTaskSetting(int pkid)
        {
            const string sql = @"  SELECT *  FROM Gungnir..UserTaskSetting WITH(NOLOCK) where pkid=@pkid";
            var cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@pkid", pkid);
            var temp = DbHelper.ExecuteDataTable(cmd);
            if (temp == null && temp.Rows.Count <= 0)
                return null;
            return temp.Rows.OfType<DataRow>().Select(s => new UserTaskSettingModel
            {
                PKID = Convert.ToInt32(s["PKID"]),
                TaskName = s["TaskName"].ToString(),
                TaskDescription = s["TaskDescription"].ToString(),
                StartTime = Convert.ToDateTime(s["StartTime"] == DBNull.Value ? DateTime.Now : s["StartTime"]),
                EndTime = Convert.ToDateTime(s["EndTime"] == DBNull.Value ? DateTime.Now : s["EndTime"]),
                CreateTime = Convert.ToDateTime(s["CreateTime"] == DBNull.Value ? DateTime.Now : s["CreateTime"]),
                UpdateTime = Convert.ToDateTime(s["UpdateTime"] == DBNull.Value ? DateTime.Now : s["UpdateTime"]),
                Tag_1 = s["Tag_1"].ToString(),
                Tag_2 = s["Tag_2"].ToString(),
                TargetButtonUrl = s["TargetButtonUrl"].ToString(),
                Condition_1 = Convert.ToInt32(s["Condition_1"]),
                Condition_2 = Convert.ToInt32(s["Condition_2"]),
                Condition_3 = Convert.ToInt32(s["Condition_3"]),
                Condition_4 = Convert.ToInt32(s["Condition_4"]),
                Condition_5 = Convert.ToInt32(s["Condition_5"]),
                Condition_1_Value = s["Condition_1_Value"].ToString(),
                Condition_2_Value = s["Condition_2_Value"].ToString(),
                Condition_3_Value = s["Condition_3_Value"].ToString(),
                Condition_4_Value = s["Condition_4_Value"].ToString(),
                Condition_5_Value = s["Condition_5_Value"].ToString(),
                ConditionCountValue = s["ConditionCountValue"].ToString(),
                CouponId_1 = Convert.ToInt32(s["CouponId_1"]),
                CouponId_2 = Convert.ToInt32(s["CouponId_2"]),
                CouponId_3 = Convert.ToInt32(s["CouponId_3"]),
                CouponId_4 = Convert.ToInt32(s["CouponId_4"]),
                CouponId_5 = Convert.ToInt32(s["CouponId_5"]),
                CouponId_1_Num = Convert.ToInt32(s["CouponId_1_Num"]),
                CouponId_2_Num = Convert.ToInt32(s["CouponId_2_Num"]),
                CouponId_3_Num = Convert.ToInt32(s["CouponId_3_Num"]),
                CouponId_4_Num = Convert.ToInt32(s["CouponId_4_Num"]),
                CouponId_5_Num = Convert.ToInt32(s["CouponId_5_Num"]),
                Status = s["Status"].ToString(),
                DisplayType = s["DisplayType"].ToString(),
                TargetButtonDescription = s["TargetButtonDescription"].ToString(),
                ValidTimeType = s["ValidTimeType"].ToString(),
                ImageUrl = s["ImageUrl"].ToString()
            }).FirstOrDefault();
        }
        public static List<TaskCompleteCondition> GetTaskCompleteCondition()
        {
            const string sql = @"select * from [Gungnir].[dbo].[TaskCompleteCondition] with(nolock)";
            var cmd = new SqlCommand(sql);
            var temp = DbHelper.ExecuteDataTable(cmd);
            if (temp == null && temp.Rows.Count <= 0)
                return null;
            return temp.Rows.OfType<DataRow>().Select(s => new TaskCompleteCondition
            {
                PKID = Convert.ToInt32(s["PKID"]),
                ConditionDescription = s["ConditionDescription"].ToString(),
                ConditionName = s["ConditionName"].ToString(),
                CreateTime = Convert.ToDateTime(s["CreateTime"]),
            }).ToList();
        }
        public static bool UpdateTaskSetting(UserTaskSettingModel model)
        {
            #region sql
            const string sql = @"Update Gungnir..UserTaskSetting 
Set TaskName=ISNULL(@TaskName,TaskName),
TaskDescription=ISNULL(@TaskDescription,TaskDescription),
TargetButtonUrl=ISNULL(@TargetButtonUrl,TargetButtonUrl),
TargetButtonDescription=ISNULL(@TargetButtonDescription,TargetButtonDescription),
Condition_1=ISNULL(@Condition_1,Condition_1),
Condition_1_Value=ISNULL(@Condition_1_Value,Condition_1_Value),
Condition_2=ISNULL(@Condition_2,Condition_2),
Condition_2_Value=ISNULL(@Condition_2_Value,Condition_2_Value),
Condition_3=ISNULL(@Condition_3,Condition_3),
Condition_3_Value=ISNULL(@Condition_3_Value,Condition_3_Value),
Condition_4=ISNULL(@Condition_4,Condition_4),
Condition_4_Value=ISNULL(@Condition_4_Value,Condition_4_Value),
Condition_5=ISNULL(@Condition_5,Condition_5),
Condition_5_Value=ISNULL(@Condition_5_Value,Condition_5_Value),
ConditionCountValue=ISNULL(@ConditionCountValue,ConditionCountValue),
CouponId_1=ISNULL(@CouponId_1,CouponId_1),
CouponId_1_Num=ISNULL(@CouponId_1_Num,CouponId_1_Num),
CouponId_2=ISNULL(@CouponId_2,CouponId_2),
CouponId_2_Num=ISNULL(@CouponId_2_Num,CouponId_2_Num),
CouponId_3=ISNULL(@CouponId_3,CouponId_3),
CouponId_3_Num=ISNULL(@CouponId_3_Num,CouponId_3_Num),
CouponId_4=ISNULL(@CouponId_4,CouponId_4),
CouponId_4_Num=ISNULL(@CouponId_4_Num,CouponId_4_Num),
CouponId_5=ISNULL(@CouponId_5,CouponId_5),
CouponId_5_Num=ISNULL(@CouponId_5_Num,CouponId_5_Num),
Tag_1=ISNULL(@Tag_1,Tag_1),
Tag_2=ISNULL(@Tag_2,Tag_2),
CreateTime=ISNULL(@CreateTime,CreateTime),
UpdateTime=ISNULL(@UpdateTime,UpdateTime),
ValidTimeType=ISNULL(@ValidTimeType,ValidTimeType),
StartTime=ISNULL(@StartTime,StartTime),
EndTime=ISNULL(@EndTime,EndTime),
Status=ISNULL(@Status,Status),
DisplayType=ISNULL(@DisplayType,DisplayType),
ImageUrl=ISNULL(@ImageUrl,ImageUrl)
WHERE PKID=@PKID";
            #endregion
            var cmd = new SqlCommand(sql);
            #region parameters

            cmd.Parameters.AddWithValue("@TaskName", model.TaskName);
            cmd.Parameters.AddWithValue("@TaskDescription", model.TaskDescription);
            cmd.Parameters.AddWithValue("@TargetButtonUrl", model.TargetButtonUrl);
            cmd.Parameters.AddWithValue("@TargetButtonDescription", model.TargetButtonDescription);
            cmd.Parameters.AddWithValue("@Condition_1", model.Condition_1);
            cmd.Parameters.AddWithValue("@Condition_1_Value", model.Condition_1_Value);
            cmd.Parameters.AddWithValue("@Condition_2", model.Condition_2);
            cmd.Parameters.AddWithValue("@Condition_2_Value", model.Condition_2_Value);
            cmd.Parameters.AddWithValue("@Condition_3", model.Condition_3);
            cmd.Parameters.AddWithValue("@Condition_3_Value", model.Condition_3_Value);
            cmd.Parameters.AddWithValue("@Condition_4", model.Condition_4);
            cmd.Parameters.AddWithValue("@Condition_4_Value", model.Condition_4_Value);
            cmd.Parameters.AddWithValue("@Condition_5", model.Condition_5);
            cmd.Parameters.AddWithValue("@Condition_5_Value", model.Condition_5_Value);
            cmd.Parameters.AddWithValue("@ConditionCountValue", model.ConditionCountValue.Trim());
            cmd.Parameters.AddWithValue("@CouponId_1", model.CouponId_1);
            cmd.Parameters.AddWithValue("@CouponId_1_Num", model.CouponId_1_Num);
            cmd.Parameters.AddWithValue("@CouponId_2", model.CouponId_2);
            cmd.Parameters.AddWithValue("@CouponId_2_Num", model.CouponId_2_Num);
            cmd.Parameters.AddWithValue("@CouponId_3", model.CouponId_3);
            cmd.Parameters.AddWithValue("@CouponId_3_Num", model.CouponId_3_Num);
            cmd.Parameters.AddWithValue("@CouponId_4", model.CouponId_4);
            cmd.Parameters.AddWithValue("@CouponId_4_Num", model.CouponId_4_Num);
            cmd.Parameters.AddWithValue("@CouponId_5", model.CouponId_5);
            cmd.Parameters.AddWithValue("@CouponId_5_Num", model.CouponId_5_Num);
            cmd.Parameters.AddWithValue("@Tag_1", model.Tag_1);
            cmd.Parameters.AddWithValue("@Tag_2", model.Tag_2);
            cmd.Parameters.AddWithValue("@CreateTime", model.CreateTime);
            cmd.Parameters.AddWithValue("@UpdateTime", model.UpdateTime);
            cmd.Parameters.AddWithValue("@ValidTimeType", model.ValidTimeType);
            cmd.Parameters.AddWithValue("@StartTime", model.StartTime);
            cmd.Parameters.AddWithValue("@EndTime", model.EndTime);
            cmd.Parameters.AddWithValue("@Status", model.Status);
            cmd.Parameters.AddWithValue("@DisplayType", model.DisplayType);
            cmd.Parameters.AddWithValue("@ImageUrl", model.ImageUrl);
            cmd.Parameters.AddWithValue("@PKID", model.PKID);
            #endregion

            return DbHelper.ExecuteNonQuery(cmd) > 0;
        }
        public static bool CreateTaskSetting(UserTaskSettingModel model)
        {
            #region sql
            const string sql = @"Insert into  Gungnir..UserTaskSetting(
TaskName,
TaskDescription,
TargetButtonUrl,
TargetButtonDescription,
Condition_1,
Condition_1_Value,
Condition_2,
Condition_2_Value,
Condition_3,
Condition_3_Value,
Condition_4,
Condition_4_Value,
Condition_5,
Condition_5_Value,
ConditionCountValue,
CouponId_1,
CouponId_1_Num,
CouponId_2,
CouponId_2_Num,
CouponId_3,
CouponId_3_Num,
CouponId_4,
CouponId_4_Num,
CouponId_5,
CouponId_5_Num,
Tag_1,
Tag_2,
CreateTime,
UpdateTime,
ValidTimeType,
StartTime,
EndTime,
Status,
DisplayType,
ImageUrl)
values(
@TaskName,
@TaskDescription,
@TargetButtonUrl,
@TargetButtonDescription,
@Condition_1,
@Condition_1_Value,
@Condition_2,
@Condition_2_Value,
@Condition_3,
@Condition_3_Value,
@Condition_4,
@Condition_4_Value,
@Condition_5,
@Condition_5_Value,
@ConditionCountValue,
@CouponId_1,
@CouponId_1_Num,
@CouponId_2,
@CouponId_2_Num,
@CouponId_3,
@CouponId_3_Num,
@CouponId_4,
@CouponId_4_Num,
@CouponId_5,
@CouponId_5_Num,
@Tag_1,
@Tag_2,
@CreateTime,
@UpdateTime,
@ValidTimeType,
@StartTime,
@EndTime,
@Status,
@DisplayType,
@ImageUrl)
";
            #endregion
            var cmd = new SqlCommand(sql);
            #region parameters

            cmd.Parameters.AddWithValue("@TaskName", model.TaskName);
            cmd.Parameters.AddWithValue("@TaskDescription", model.TaskDescription);
            cmd.Parameters.AddWithValue("@TargetButtonUrl", model.TargetButtonUrl);
            cmd.Parameters.AddWithValue("@TargetButtonDescription", model.TargetButtonDescription);
            cmd.Parameters.AddWithValue("@Condition_1", model.Condition_1);
            cmd.Parameters.AddWithValue("@Condition_1_Value", model.Condition_1_Value);
            cmd.Parameters.AddWithValue("@Condition_2", model.Condition_2);
            cmd.Parameters.AddWithValue("@Condition_2_Value", model.Condition_2_Value);
            cmd.Parameters.AddWithValue("@Condition_3", model.Condition_3);
            cmd.Parameters.AddWithValue("@Condition_3_Value", model.Condition_3_Value);
            cmd.Parameters.AddWithValue("@Condition_4", model.Condition_4);
            cmd.Parameters.AddWithValue("@Condition_4_Value", model.Condition_4_Value);
            cmd.Parameters.AddWithValue("@Condition_5", model.Condition_5);
            cmd.Parameters.AddWithValue("@Condition_5_Value", model.Condition_5_Value);
            cmd.Parameters.AddWithValue("@ConditionCountValue", model.ConditionCountValue.Trim());
            cmd.Parameters.AddWithValue("@CouponId_1", model.CouponId_1);
            cmd.Parameters.AddWithValue("@CouponId_1_Num", model.CouponId_1_Num);
            cmd.Parameters.AddWithValue("@CouponId_2", model.CouponId_2);
            cmd.Parameters.AddWithValue("@CouponId_2_Num", model.CouponId_2_Num);
            cmd.Parameters.AddWithValue("@CouponId_3", model.CouponId_3);
            cmd.Parameters.AddWithValue("@CouponId_3_Num", model.CouponId_3_Num);
            cmd.Parameters.AddWithValue("@CouponId_4", model.CouponId_4);
            cmd.Parameters.AddWithValue("@CouponId_4_Num", model.CouponId_4_Num);
            cmd.Parameters.AddWithValue("@CouponId_5", model.CouponId_5);
            cmd.Parameters.AddWithValue("@CouponId_5_Num", model.CouponId_5_Num);
            cmd.Parameters.AddWithValue("@Tag_1", model.Tag_1);
            cmd.Parameters.AddWithValue("@Tag_2", model.Tag_2);
            cmd.Parameters.AddWithValue("@CreateTime", DateTime.Now);
            cmd.Parameters.AddWithValue("@UpdateTime", model.UpdateTime);
            cmd.Parameters.AddWithValue("@ValidTimeType", model.ValidTimeType);
            cmd.Parameters.AddWithValue("@StartTime", model.StartTime);
            cmd.Parameters.AddWithValue("@EndTime", model.EndTime);
            cmd.Parameters.AddWithValue("@Status", model.Status);
            cmd.Parameters.AddWithValue("@DisplayType", model.DisplayType);
            cmd.Parameters.AddWithValue("@ImageUrl", model.ImageUrl);
            #endregion

            return DbHelper.ExecuteNonQuery(cmd) > 0;
        }
    }
}
