using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Tuhu.Service.Activity.Models;
using Newtonsoft.Json;

namespace Tuhu.Service.Activity.DataAccess
{
    public class DalActivity
    {
        public static async Task<IEnumerable<TireActivityModel>> SelectTireActivity(string vehicleId, string tireSize, bool flag = true)
        {
            return await DbHelper.ExecuteSelectAsync<TireActivityModel>(flag, @"SELECT	* FROM	Activity.dbo.tbl_TireListActivity AS TLA WITH (NOLOCK) WHERE	TLA.Status = 1 AND EndTime > GETDATE() AND VehicleID = @VehicleID AND TireSize = @TireSize",
                CommandType.Text,
                new SqlParameter("@VehicleID", vehicleId),
                new SqlParameter("@tireSize", tireSize));
        }
        /// <summary>
        /// 获取所有可用活动
        /// </summary>
        /// <returns></returns>
        public static async Task<IEnumerable<T_Activity_xhrModel>> GetAllActivityAsync(int pageIndex, int pageSize)
        {
            var sql = @"SELECT 
                            [ActivityID]
                            ,[Title]
                            ,[ActivityContent]
                            ,[StartTime]
                            ,[EndTime]
                            ,[Remark]
                            ,[Picture]
                            ,[CreateTime]
                            ,[CreateUser]
                            ,[EditTime]
                            ,[EditUser]
                            ,[ActStatus]
	                        FROM  [Activity].[dbo].[T_Activity_xhr]  WITH(NOLOCK)
	                        WHERE [ActStatus]<>0
							ORDER BY [ActivityID]
							OFFSET (@pageIndex -1) * @pageSize ROWS
							FETCH NEXT @pageSize ROWS ONLY";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@pageIndex", pageIndex);
                cmd.AddParameter("@pageSize", pageSize);
                return await DbHelper.ExecuteSelectAsync<T_Activity_xhrModel>(true, cmd);
            }
        }

        /// <summary>
        /// 获取所有活动
        /// </summary>
        /// <returns></returns>
        public static async Task<IEnumerable<T_Activity_xhrModel>> GetAllActivityManagerAsync(int pageIndex, int pageSize)
        {
            var sql = @"SELECT 
                            [ActivityID]
                            ,[Title]
                            ,[ActivityContent]
                            ,[StartTime]
                            ,[EndTime]
                            ,[Remark]
                            ,[Picture]
                            ,[CreateTime]
                            ,[CreateUser]
                            ,[EditTime]
                            ,[EditUser]
                            ,[ActStatus]
	                        FROM  [Activity].[dbo].[T_Activity_xhr]  WITH(NOLOCK)
							ORDER BY [ActivityID]
							OFFSET (@pageIndex -1) * @pageSize ROWS
							FETCH NEXT @pageSize ROWS ONLY";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@startRow", (pageIndex - 1) * pageSize + 1);
                cmd.AddParameter("@endRow", pageSize * pageIndex);
                return await DbHelper.ExecuteSelectAsync<T_Activity_xhrModel>(true, cmd);
            }
        }

        /// <summary>
        /// 新增活动
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<bool> AddActivityAsync(T_Activity_xhrModel request)
        {
            string sql = @"
                        INSERT INTO Activity.dbo.T_Activity_xhr
                         (                            
                            [Title],
                            [ActivityContent],
                            [StartTime],
                            [EndTime],
	                        [Picture],
                            [CreateTime],
                            [CreateUser],
                            [ActStatus]
                        )
                        VALUES
                        (   @Title,
                            @ActivityContent,
                            @StartTime,
                            @EndTime,
                            @Picture,
                            GETDATE(),
                            'xhr',
                            1
                        );";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Title", request.Title);
                cmd.Parameters.AddWithValue("@ActivityContent", request.ActivityContent);
                cmd.Parameters.AddWithValue("@StartTime", request.StartTime);
                cmd.Parameters.AddWithValue("@EndTime", request.EndTime);
                cmd.Parameters.AddWithValue("@Picture", request.Picture);
                return (await DbHelper.ExecuteNonQueryAsync(cmd)) > 0;
            }
        }

        /// <summary>
        /// 修改活动信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<bool> UpdateActivityAsync(T_Activity_xhrModel request)
        {
                        string sql = @"
                        UPDATE [Activity].[dbo].[T_Activity_xhr]
							SET [Title] = @Title
							  ,[ActivityContent]=@ActivityContent
							  ,[StartTime]=@StartTime
							  ,[EndTime]=@EndTime
							  ,[Picture]=@Picture
							  ,[ActStatus]=@ActStatus
                              ,[EditTime]=GETDATE()
                              ,[EditUser]='xhr'
							WHERE [ActivityID] = @ActivityID;";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Title", request.Title);
                cmd.Parameters.AddWithValue("@ActivityContent", request.ActivityContent);
                cmd.Parameters.AddWithValue("@StartTime", request.StartTime);
                cmd.Parameters.AddWithValue("@EndTime", request.EndTime);
                cmd.Parameters.AddWithValue("@Picture", request.Picture);
                cmd.Parameters.AddWithValue("@ActStatus", request.ActStatus);
                cmd.Parameters.AddWithValue("@ActivityID", request.ActivityId);
                return (await DbHelper.ExecuteNonQueryAsync(cmd)) > 0;
            }
        }

            /// <summary>
            /// 活动报名
            /// </summary>
            /// <param name="request"></param>
            /// <returns></returns>
            public static async Task<bool> AddActivitiesUserAsync(ActivityUserInfo_xhrRequest request)
        {
            string sql = @"
                        INSERT INTO [Activity].[dbo].[T_ActivityUserInfo_xhr]
                        (                            
                            [UserName],
                            [UserTell],
                            [AreaID],
                            [PassStatus],
                            [ActID],
                            [CreateTime],
                            [CreateUser],
                            [UserStatus]
                        )
                        VALUES
                        (   @UserName,
                            @UserTell,
                            @AreaID,
                            @PassStatus,
                            @ActID,
                            GETDATE(),
                            'xhr',
                            1
                        );SELECT SCOPE_IDENTITY();";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@UserName", request.UserName);
                cmd.Parameters.AddWithValue("@UserTell", request.UserTell);
                cmd.Parameters.AddWithValue("@AreaID", request.AreaID);
                cmd.Parameters.AddWithValue("@PassStatus", 0);
                cmd.Parameters.AddWithValue("@ActID", request.ActID);
                return (await DbHelper.ExecuteNonQueryAsync(cmd)) > 0;
            }
        }

        /// <summary>
        ///     修改报名信息
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> UpdateActivitiesUserAsync(ActivityUserInfo_xhrRequest request)
        {
            var sql = @"

                        UPDATE [Activity].[dbo].[T_ActivityUserInfo_xhr]
                           SET [UserName] = @UserName
                              ,[UserTell] = @UserTell
                              ,[AreaID] = @AreaID
                              ,[PassStatus] = @PassStatus
                              ,[EditTime]=GETDATE()
                              ,[EditUser]='xhr'
                         WHERE [UserID] = @UserId
                ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@UserName", request.UserName);
                cmd.AddParameter("@UserId", request.UserId);
                cmd.AddParameter("@UserTell", request.UserTell);
                cmd.AddParameter("@AreaID", request.AreaID);
                cmd.AddParameter("@PassStatus", request.PassStatus);
                var result = await DbHelper.ExecuteNonQueryAsync(cmd);
                return result > 0;
            }
        }

        /// <summary>
        /// 获取所有地区
        /// </summary>
        /// <returns></returns>
        public static async Task<IEnumerable<T_ArearModel>> GetAllAreaAsync()
        {
            var sql = @"SELECT
     	               [Id]
                      ,[ArearName]
	                  FROM  Activity.[dbo].[T_Arear] WITH(NOLOCK);";
            using (var cmd = new SqlCommand(sql))
            {
                return await DbHelper.ExecuteSelectAsync<T_ArearModel>(true, cmd);
            }
        }

        /// <summary>
        /// 根据地区查询用户信息
        /// </summary>
        /// <returns></returns>
        public static async Task<IEnumerable<T_ActivityUserInfo_xhrModels>> GetActivityUserInfoByAreaAsync(int areaId, int pageIndex, int pageSize)
        {
            var sql = @"SELECT
                    [UserID]
                    ,[UserName]
                    ,[UserTell]
                    ,[AreaID]
                    ,[ActID]
                    ,[Title]
                    ,[PassStatus]
                    ,[UserStatus]
                    FROM  Activity.[dbo].[T_ActivityUserInfo_xhr] Au WITH(NOLOCK)
                    LEFT JOIN Activity.[dbo].[T_Activity_xhr]  Ac WITH(NOLOCK)
                    ON Au.[ActID]=Ac.[ActivityID] 
                    WHERE (CASE WHEN @AreaId=0 THEN 0 ELSE  Au.AreaID END)=@AreaId
                    ORDER BY [UserID]
                    OFFSET (@pageIndex -1) * @pageSize ROWS
                    FETCH NEXT @pageSize ROWS ONLY";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@AreaId", areaId);
                cmd.CommandType = CommandType.Text;
                cmd.AddParameter("@pageIndex", pageIndex);
                cmd.AddParameter("@pageSize", pageSize);
                return await DbHelper.ExecuteSelectAsync<T_ActivityUserInfo_xhrModels>(true, cmd);
            }
        }

        /// <summary>
        /// 审核活动
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> ReviewActivityTaskAsync()
        {
            const string sqlStr = @"UPDATE [Activity].[dbo].[T_ActivityUserInfo_xhr]
                           SET [PassStatus] = 1
                              ,[EditTime]=GETDATE()
                              ,[EditUser]='xhr'
                         WHERE [AreaID] = 2";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.CommandType = CommandType.Text;
                var result = await DbHelper.ExecuteNonQueryAsync(cmd);
                return result > 0;
            }
        }

        /// <summary>
        /// 管理员登录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<string> ManagerLoginAsync(T_ActivityManagerUserInfo_xhrModel request)
        {
            var sql = @"SELECT TOP 1 ManagerId
	                  FROM  Activity.[dbo].[T_ActivityManagerUserInfo_xhr] WITH(NOLOCK)
                      WHERE [Name]=@Name AND [PassWords]=@PassWords AND ManagerStatus=1;";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Name", request.Name);
                cmd.Parameters.AddWithValue("@PassWords", request.PassWords);

                var managerId = await DbHelper.ExecuteScalarAsync(true, cmd);
                var result = (managerId == null ? "" : managerId).ToString();
                return result;
            }
        }

        /// <summary>
        /// 管理员注册
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<bool> ManagerRegisterAsync(T_ActivityManagerUserInfo_xhrModel request)
        {
            string sql = @"
                        INSERT INTO Activity.dbo.T_ActivityManagerUserInfo_xhr
                         (                            
                            [Name],
                            [PassWordsSalt],
                            [PassWords],
                            [CreateTime],
                            [CreateUser],
                            [ManagerStatus]
                        )
                        VALUES
                        (   @Name,
                            @PassWordsSalt,
                            @PassWords,
                            GETDATE(),
                            'xhr',
                            1
                        );";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Name", request.Name);
                cmd.Parameters.AddWithValue("@PassWordsSalt", request.PassWordsSalt);
                cmd.Parameters.AddWithValue("@PassWords", request.PassWords);
                return (await DbHelper.ExecuteNonQueryAsync(cmd)) > 0;
            }
        }

        /// <summary>
        /// 获取密码盐
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetPassWordsSalt(T_ActivityManagerUserInfo_xhrModel request)
        {
            var sql = @"SELECT TOP 1 PassWordsSalt
	                  FROM  Activity.[dbo].[T_ActivityManagerUserInfo_xhr] WITH(NOLOCK)
                      WHERE [Name]=@Name AND ManagerStatus=1;";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Name", request.Name);
                var salt = DbHelper.ExecuteScalar(true, cmd);
                var result = (salt == null?"": salt).ToString();
                return result;
            }
        }

        /// <summary>
        /// 验证登录状态
        /// </summary>
        /// <param name="managerId"></param>
        /// <returns></returns>
        public static async Task<bool> CheckLoginAsync(int managerId)
        {
            var sql = @"SELECT TOP 1 1
	                  FROM  Activity.[dbo].[T_ActivityManagerUserInfo_xhr] WITH(NOLOCK)
                      WHERE [ManagerId]=@ManagerId AND ManagerStatus=1;";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ManagerId", managerId);
                return (Convert.ToInt32(await DbHelper.ExecuteScalarAsync(true, cmd))) > 0;
            }
        }

    }
}

