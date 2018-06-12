using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Tuhu.WebSite.Component.SystemFramework;
using Tuhu.WebSite.Web.Activity.Models;
using Tuhu.WebSite.Web.Activity.Models.Activity;
using Tuhu.WebSite.Web.Activity.Models.Enum;

namespace Tuhu.WebSite.Web.Activity.DataAccess
{
    public class Activity
    {
        private static readonly string ActivityConnectionString = System.Configuration.ConfigurationManager
            .ConnectionStrings["Tuhu_Activity_Db"].ConnectionString;
        private static readonly string ActivityConnectionStringReadOnly = System.Configuration.ConfigurationManager
            .ConnectionStrings["Tuhu_Activity_Db_ReadOnly"].ConnectionString;

        public static DataTable GetBatteryBanner()
        {
            string sql = @"SELECT TOP 1 [Id]
                                      ,[SmallImage]
                                      ,[Image]
                                      ,[ShowTime]
                                      ,[Province]
                                      ,[City]
                                      ,[Display]
                                      ,[Available] 
                           FROM [Tuhu_huodong].[dbo].[BatteryBanner] WHERE  ShowTime <= GETDATE() ORDER BY  ShowTime DESC";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                return Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(cmd);
            }
        }

        public static DataTable GetActivityBuildById(int id)
        {
            string sql = @"SELECT [id]
                          ,[Title]
                          ,[Content]
                          ,[ActivityUrl]
                          ,[CreateTime]
                          ,[UpdateTime]
                      FROM [HuoDong].[dbo].[ActivityBuild] WITH(NOLOCK) WHERE id=@id";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@id", id);
                return Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(cmd);
            }
        }

        public static DataTable GeDownloadAppById(int id)
        {
            string sql = @"SELECT *  FROM [Tuhu_huodong].[dbo].[DownloadApp] WHERE Id = @id";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@id", id);
                return Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(cmd);
            }
        }

        public static CouponActivity GetCurrentCouponActivity()
        {
            using (var cmd = new SqlCommand("HuoDong..Activity_GetCurrentCouponActivity"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                var result =
                    Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(cmd).Rows.OfType<DataRow>().Select(
                        x => new CouponActivity()
                        {
                            SmallImageUrl = (string) (x["smallImageUrl"] ?? string.Empty),
                            BigImageUrl = (string) (x["bigImageUrl"] ?? string.Empty),
                            ButtonBackGroundColor = (string) (x["buttonBackGroundColor"] ?? string.Empty),
                            ButtonTextColor = (string) (x["buttonTextColor"] ?? string.Empty),
                            ButtonText = (string) (x["buttonText"] ?? string.Empty),
                            Url = (string) (x["Url"] ?? string.Empty),
                            IosJson = (string) (x["IosJson"] ?? string.Empty),
                            AndroidJson = (string) (x["AndroidJson"] ?? string.Empty),
                            IsSendCoupon = (bool) (x["isSendCoupon"] ?? false),
                            LayerButtonText = (string) (x["layerButtonText"] ?? string.Empty),
                            LayerButtonBackGroundColor = (string) (x["layerButtonBackGroundColor"] ?? string.Empty),
                            LayerButtonTextColor = (string) (x["layerButtonTextColor"] ?? string.Empty),
                            PromotionType = (int) (x["promotionType"] ?? 0),
                            CouponRuleId = (int) (x["couponRuleId"] ?? 0),
                            CouponUseMoney = (int) (x["couponUseMoney"] ?? 0),
                            CouponDisCountMoney = (int) (x["couponDisCountMoney"] ?? 0),
                            ValidityPeriod = (int) (x["couponValidityPeriod"] ?? 0),
                            CouponDescription = (string) (x["CouponDescription"] ?? string.Empty),
                            CodeChannel = (string) (x["CouponCodeChannel"] ?? string.Empty)
                        }).ToList();
                return result[0];
            }
        }

        public static int InsertPromotionFromActivity(CouponActivity couponDetail, string user)
        {
            using (var cmd = new SqlCommand("Gungnir..Activity_InsertPromotionData"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PromotionType", couponDetail.PromotionType);
                cmd.Parameters.AddWithValue("@CouponRuleId", couponDetail.CouponRuleId);
                cmd.Parameters.AddWithValue("@CouponDisCountMoney", couponDetail.CouponDisCountMoney);
                cmd.Parameters.AddWithValue("@CouponUseMoney", couponDetail.CouponUseMoney);
                cmd.Parameters.AddWithValue("@CouponValidityPeriod", couponDetail.ValidityPeriod);
                cmd.Parameters.AddWithValue("@CouponDescription", couponDetail.CouponDescription);
                cmd.Parameters.AddWithValue("@CodeChannel", couponDetail.CodeChannel);
                cmd.Parameters.AddWithValue("@User", user);
                return DbHelper.ExecuteNonQuery(cmd);

            }
        }

        public static int UserIsClaimedCoupon(CouponActivity couponDetail, string userId)
        {
            using (var cmd = new SqlCommand("Gungnir..Activity_IsUserClaimedCoupon"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PromotionType", couponDetail.PromotionType);
                cmd.Parameters.AddWithValue("@CouponRuleId", couponDetail.CouponRuleId);
                cmd.Parameters.AddWithValue("@CouponDisCountMoney", couponDetail.CouponDisCountMoney);
                cmd.Parameters.AddWithValue("@CouponUseMoney", couponDetail.CouponUseMoney);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.Add(new SqlParameter("@Result", SqlDbType.Int) {Direction = ParameterDirection.Output});
                DbHelper.ExecuteNonQuery(cmd);
                var result = Convert.ToInt32(cmd.Parameters["@Result"].Value);
                return result;

            }
        }

        /// <summary>
        /// 获取活动列表
        /// </summary>
        /// <returns></returns>
        public static async Task<Tuple<DataTable,int>> GetActivityModelsAsync(int pageIndex,int pageSize)
        {
            string sql = @"
SET @TotalCount = ( SELECT  COUNT(1)
                    FROM    Activity..T_Activity WITH ( NOLOCK )
                  );
SELECT PKID,
                           Name,
                           ActivityId,
                           StartTime,
                           EndTime,
                           ActivityType,
                           ActivityUrl,
                           Quota,
                           CreateUser,
                           CreatedDateTime,
                           UpdatedDateTime
                    FROM Activity..T_Activity WITH (NOLOCK)
                    ORDER BY CreatedDateTime DESC
                    OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize
                    ROWS ONLY";
            using (var dbHelper = new SqlDbHelper(ActivityConnectionStringReadOnly))
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                var sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@PageIndex", pageIndex),
                    new SqlParameter("@PageSize", pageSize),
                    new SqlParameter
                    {
                        DbType = DbType.Int32,
                        ParameterName = "@TotalCount",
                        Direction = ParameterDirection.Output
                    }
                };
                cmd.Parameters.AddRange(sqlParameters);
                var result = await dbHelper.ExecuteDataTableAsync(cmd);
                return new Tuple<DataTable, int>(result, (int)sqlParameters[2].Value);
            }
        }

        /// <summary>
        /// 根据pkid获取活动
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static async Task<DataTable> GetActivityModelByActivityIdAsync(Guid activityId)
        {
            string sql = @"SELECT TOP 1
                           PKID,
                           ActivityId,
                           Name,
                           StartTime,
                           EndTime,
                           ActivityType,
                           ActivityUrl,
                           Quota,
                           CreateUser,
                           CreatedDateTime,
                           UpdatedDateTime
                    FROM Activity..T_Activity WITH (NOLOCK) WHERE ActivityId = @ActivityId";
            using (var dbHelper = new SqlDbHelper(ActivityConnectionStringReadOnly))
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ActivityId", activityId);
                return await dbHelper.ExecuteDataTableAsync(cmd);
            }
        }

        /// <summary>
        /// 创建活动返回主键
        /// </summary>
        /// <param name="activityModel"></param>
        /// <returns></returns>
        public static async Task<int> InsertActivityModelAsync(ActivityModel activityModel)
        {
            string sql = @"INSERT Activity..T_Activity
                            (
                                Name,
                                ActivityId,
                                StartTime,
                                EndTime,
                                ActivityType,
                                ActivityUrl,
                                Quota,
                                CreateUser,
                                CreatedDateTime,
                                UpdatedDateTime
                            )
                            VALUES
                            (@Name, @ActivityId, @StartTime, @EndTime, @ActivityType, @ActivityUrl, @Quota, @CreateUser, GETDATE(), GETDATE());SET @PKID = (SELECT @@IDENTITY)";
            using (var dbHelper = new SqlDbHelper(ActivityConnectionString))
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@Name", activityModel.Name),
                    new SqlParameter("@ActivityId", activityModel.ActivityId),
                    new SqlParameter("@StartTime", activityModel.StartTime),
                    new SqlParameter("@EndTime", activityModel.EndTime),
                    new SqlParameter("@ActivityType", activityModel.ActivityType),
                    new SqlParameter("@ActivityUrl", activityModel.ActivityUrl),
                    new SqlParameter("@Quota", activityModel.Quota),
                    new SqlParameter("@CreateUser", activityModel.CreateUser),
                    new SqlParameter
                    {
                        DbType = DbType.Int32,
                        ParameterName = "@PKID",
                        Direction = ParameterDirection.Output
                    }
                };
                cmd.Parameters.AddRange(parameters);
                await dbHelper.ExecuteNonQueryAsync(cmd);
                return (int) parameters[8].Value;
            }
        }

        /// <summary>
        /// 更新活动
        /// </summary>
        /// <param name="activityModel"></param>
        /// <returns></returns>
        public static async Task<int> UpdateActivityModelAsync(ActivityModel activityModel)
        {
            string sql = @"UPDATE Activity..T_Activity
                            SET Name = @Name,
                            SET ActivityId = @ActivityId,
                            StartTime = @StartTime,
                            EndTime = @EndTime,
                            ActivityType = @ActivityType,
                            ActivityUrl = @ActivityUrl,
                            Quota = @Quota,
                            UpdatedDateTime = GETDATE() WHERE PKID = @PKID";
            using (var dbHelper = new SqlDbHelper(ActivityConnectionString))
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@Name", activityModel.Name),
                    new SqlParameter("@ActivityId", activityModel.ActivityId),
                    new SqlParameter("@StartTime", activityModel.StartTime),
                    new SqlParameter("@EndTime", activityModel.EndTime),
                    new SqlParameter("@ActivityType", activityModel.ActivityType),
                    new SqlParameter("@ActivityUrl", activityModel.ActivityUrl),
                    new SqlParameter("@Quota", activityModel.Quota),
                    new SqlParameter("@PKID", activityModel.PKID)
                };
                cmd.Parameters.AddRange(parameters);
                return await dbHelper.ExecuteNonQueryAsync(cmd);
            }
        }

        /// <summary>
        /// 删除活动
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static async Task<bool> DeleteActivityModelByPKIDAsync(int pkid)
        {
            string sql = @"DELETE Activity..T_Activity
                    WHERE PKID = @PKID";
            using (var dbHelper = new SqlDbHelper(ActivityConnectionString))
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@PKID", pkid);
                return await dbHelper.ExecuteNonQueryAsync(cmd) > 0;
            }
        }

        /// <summary>
        /// 用户报名
        /// </summary>
        /// <param name="userActivityModel"></param>
        /// <returns></returns>
        public static async Task<bool> InsertUserActivityModelAsync(UserActivityModel userActivityModel)
        {
            string sql = @"INSERT Activity..T_UserApplyActivity
                        (
                            UserId,
                            ActivityId,
                            Mobile,
                            CarNum,
                            DriverNum,
                            Ip,
                            Region,
                            Status,
                            ExpirationTime,
                            CreatedDateTime,
                            UpdatedDateTime
                        )
                        VALUES
                        (@UserId, @ActivityId, @Mobile, @CarNum, @DriverNum, @Ip, @Region, @Status, NULL, GETDATE(), GETDATE())";
            using (var dbHelper = new SqlDbHelper(ActivityConnectionString))
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@UserId", userActivityModel.UserId),
                    new SqlParameter("@ActivityId", userActivityModel.ActivityId),
                    new SqlParameter("@Mobile", userActivityModel.Mobile),
                    new SqlParameter("@CarNum", userActivityModel.CarNum),
                    new SqlParameter("@DriverNum", userActivityModel.DriverNum),
                    new SqlParameter("@Ip", userActivityModel.Ip),
                    new SqlParameter("@Region", userActivityModel.Region),
                    new SqlParameter("@Status", userActivityModel.Status)
                };
                cmd.Parameters.AddRange(parameters);
                return await dbHelper.ExecuteNonQueryAsync(cmd) > 0;
            }
        }

        /// <summary>
        /// 审核用户报名活动
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> UpdateUserActivityStatusByPKIDAsync(UserActivityModel userActivityModel)
        {
            string sql = @"UPDATE Activity..T_UserApplyActivity
                            SET Status = @Status,
                                Remark = @Remark,
                                ServiceCode = @ServiceCode
                            WHERE PKID = @PKID";
            using (var dbHelper = new SqlDbHelper(ActivityConnectionString))
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@Status", userActivityModel.Status),
                    new SqlParameter("@Remark", userActivityModel.Remark),
                    new SqlParameter("@ServiceCode", userActivityModel.ServiceCode),
                    new SqlParameter("@PKID", userActivityModel.PKID)
                };
                cmd.Parameters.AddRange(parameters);
                return await dbHelper.ExecuteNonQueryAsync(cmd) > 0;
            }
        }

        /// <summary>
        /// 分页获取用户报名列表
        /// </summary>
        /// <returns></returns>
        public static async Task<Tuple<DataTable, int>> GetUserActivityModelsAsync(Guid activityId, AuditStatus auditStatus,int pageIndex, int pageSize)
        {
            string auditStatusFilter =
                auditStatus == AuditStatus.None ? "" : " AND Status = " + (int)auditStatus;
            string sql = $@"
SET @TotalCount = ( SELECT  COUNT(1)
                    FROM    Activity..T_UserApplyActivity WITH ( NOLOCK )
                    WHERE   ActivityId = @ActivityId{auditStatusFilter}
                  );

SELECT PKID,
UserId,
ActivityId,
Mobile,
CarNum,
DriverNum,
Ip,
Region,
Status,
Remark,
ServiceCode,
ExpirationTime,
CreatedDateTime,
UpdatedDateTime
FROM Activity..T_UserApplyActivity WITH (NOLOCK)
WHERE ActivityId = @ActivityId{auditStatusFilter}
ORDER BY CreatedDateTime DESC
            OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize
            ROWS ONLY";
            using (var dbHelper = new SqlDbHelper(ActivityConnectionStringReadOnly))
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                var sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@ActivityId", activityId),
                    new SqlParameter("@PageIndex", pageIndex),
                    new SqlParameter("@PageSize", pageSize),
                    new SqlParameter
                    {
                        DbType = DbType.Int32,
                        ParameterName = "@TotalCount",
                        Direction = ParameterDirection.Output
                    }
                };
                cmd.Parameters.AddRange(sqlParameters);
                var result = await dbHelper.ExecuteDataTableAsync(cmd);
                return new Tuple<DataTable, int>(result, (int) sqlParameters[3].Value);
            }
        }
        /// <summary>
        /// 根据活动id获取报名人员数量
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static async Task<int> GetActivityApplyUserCountByActivityIdAsync(Guid activityId)
        {
            string sql = @"SELECT COUNT(1)
FROM Activity..T_UserApplyActivity WITH (NOLOCK)
WHERE ActivityId = @ActivityId";
            using (var dbHelper = new SqlDbHelper(ActivityConnectionStringReadOnly))
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(new SqlParameter("@ActivityId", activityId));
                return (int)await dbHelper.ExecuteScalarAsync(cmd);
            }
        }
        /// <summary>
        /// 根据活动id获取报名人员审核通过数量
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static async Task<int> GetActivityApplyUserPassCountByActivityIdAsync(Guid activityId)
        {
            string sql = @"SELECT COUNT(1)
FROM Activity..T_UserApplyActivity WITH (NOLOCK)
WHERE ActivityId = @ActivityId AND Status = 1";
            using (var dbHelper = new SqlDbHelper(ActivityConnectionStringReadOnly))
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(new SqlParameter("@ActivityId", activityId));
                return (int)await dbHelper.ExecuteScalarAsync(cmd);
            }
        }
        /// <summary>
        /// 根据pkid获取报名人员
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static async Task<DataTable> GetUserActivityByPKIDAsync(int pkid)
        {
            string sql = @"SELECT TOP 1
       PKID,
       UserId,
       ActivityId,
       Mobile,
       CarNum,
       DriverNum,
       Ip,
       Region,
       Status,
       Remark,
       ServiceCode,
       ExpirationTime,
       CreatedDateTime,
       UpdatedDateTime
FROM Activity..T_UserApplyActivity WITH (NOLOCK)
WHERE PKID = @PKID";
            using (var dbHelper = new SqlDbHelper(ActivityConnectionStringReadOnly))
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(new SqlParameter("@PKID", pkid));
                return await dbHelper.ExecuteDataTableAsync(cmd);
            }
        }
        /// <summary>
        /// 删除用户报名
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static async Task<bool> DeleteUserActivityModelByPKIDAsync(int pkid)
        {
            string sql = @"DELETE Activity..T_UserApplyActivity
                    WHERE PKID = @PKID";
            using (var dbHelper = new SqlDbHelper(ActivityConnectionString))
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@PKID", pkid);
                return await dbHelper.ExecuteNonQueryAsync(cmd) > 0;
            }
        }
        /// <summary>
        /// 检查用户报名活动手机号、车牌号、驾驶证号是否重复
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="carNum"></param>
        /// <param name="driverNum"></param>
        /// <returns></returns>
        public static async Task<bool> CheckUserActivityDupAsync(string mobile, string carNum, string driverNum)
        {
            string sql = @"SELECT COUNT(1)
FROM Activity..T_UserApplyActivity WITH (NOLOCK)
WHERE Mobile = @Mobile
      OR CarNum = @CarNum
      OR DriverNum = @DriverNum";
            using (var dbHelper = new SqlDbHelper(ActivityConnectionStringReadOnly))
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                var sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@Mobile", mobile),
                    new SqlParameter("@CarNum", carNum),
                    new SqlParameter("@DriverNum", driverNum)
                };
                cmd.Parameters.AddRange(sqlParameters);
                return (int) await dbHelper.ExecuteScalarAsync(cmd) > 0;
            }
        }
    }
}