using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalCityActivityPageConfig
    {

        #region vehicle
        public static List<VehicleTypeModel> SelectAllBrand(SqlConnection conn)
        {
            var sql = @"SELECT DISTINCT Brand
                        FROM    Gungnir.dbo.tbl_Vehicle_Type WITH ( NOLOCK );";
            var brandList = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql).ConvertTo<VehicleTypeModel>().ToList();
            return brandList;
        }

        public static List<VehicleTypeModel> SelectVehicleByBrand(SqlConnection conn, string brand)
        {
            var sql = @"SELECT  ProductID ,
                                Brand ,
                                Vehicle
                        FROM    Gungnir.dbo.tbl_Vehicle_Type WITH ( NOLOCK )
                        WHERE   Brand = @Brand";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Brand", brand)
            };
            var vehicleList = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters).ConvertTo<VehicleTypeModel>().ToList();
            return vehicleList;
        }

        public static List<VehicleTypeModel> SelectVehicleByProductID(SqlConnection conn, string productIDs)
        {
            var sql = @"SELECT  ProductID ,
                                Brand ,
                                Vehicle
                        FROM    Gungnir.dbo.tbl_Vehicle_Type as t WITH ( NOLOCK )
                        WHERE  Exists(select 1 from Gungnir..SplitString(@ProductIDs, N',',1) where t.ProductID = item)";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@ProductIDs", productIDs)
            };
            var vehicles = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters).ConvertTo<VehicleTypeModel>().ToList();
            return vehicles;
        }

        #endregion

        #region RegionVehicleId
        public static List<RegionVehicleIdActivityConfig> SelectAllActivity(SqlConnection conn, RegionVehicleIdActivityConfig filter, int pageIndex, int pageSize)
        {
            var sql = @"SELECT  PKID ,
                                ActivityId ,
                                ActivityName ,
                                ActivityType ,
                                StartTime ,
                                EndTime ,
                                IsEnabled ,
                                CreateUser ,
                                UpdateUser
                        FROM    Configuration..RegionVehicleIdActivityConfig WITH ( NOLOCK )
                        WHERE   ( @ActivityName IS NULL 
                                    OR ActivityName LIKE N'%' + @ActivityName + N'%' )
                                AND ( @Status IS NULL  
                                    OR  @Status = N'所有'
                                    OR ( @Status = N'未开始'
                                        AND StartTime >= GETDATE()
                                        )
                                    OR ( @Status = N'进行中'
                                        AND StartTime <= GETDATE()
                                        AND EndTime >= GETDATE()
                                        )
                                    OR ( @Status = N'已结束'
                                        AND EndTime <= GETDATE()
                                        )
                                    )
                                AND ( @StartTime IS NULL
                                      OR  EndTime > @StartTime
                                    )
                                AND ( @EndTime IS NULL
                                    OR StartTime < @EndTime
                                    )
                                AND ( @CreateUser IS NULL
                                    OR CreateUser = @CreateUser
                                    )
                                AND ( @UpdateUser IS NULL
                                    OR UpdateUser = @UpdateUser
                                    )
                         ORDER BY PKID DESC
                                OFFSET ( @PageIndex - 1 ) * @PageSize ROWS  FETCH NEXT @PageSize ROWS
                                ONLY;";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Status", filter.Status),
                new SqlParameter("@StartTime", filter.StartTime),
                new SqlParameter("@EndTime", filter.EndTime),
                new SqlParameter("@CreateUser", filter.CreateUser),
                new SqlParameter("@UpdateUser", filter.UpdateUser),
                new SqlParameter("@ActivityName", filter.ActivityName),
                new SqlParameter("@PageIndex", pageIndex),
                new SqlParameter("@PageSize", pageSize)
            };
            var activityList = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters).ConvertTo<RegionVehicleIdActivityConfig>().ToList();
            return activityList;
        }

        public static int SelectActivityCount(SqlConnection conn, RegionVehicleIdActivityConfig filter)
        {
            var sql = @"SELECT  COUNT(1)
                        FROM    Configuration..RegionVehicleIdActivityConfig WITH ( NOLOCK )
                        WHERE   ( @ActivityName IS NULL 
                                    OR ActivityName LIKE N'%' + @ActivityName + N'%' )
                                AND ( @Status IS NULL  
                                    OR  @Status = N'所有'
                                    OR ( @Status = N'未开始'
                                        AND StartTime >= GETDATE()
                                        )
                                    OR ( @Status = N'进行中'
                                        AND StartTime <= GETDATE()
                                        AND EndTime >= GETDATE()
                                        )
                                    OR ( @Status = N'已结束'
                                        AND EndTime <= GETDATE()
                                        )
                                    )
                                AND ( @StartTime IS NULL
                                      OR  EndTime > @StartTime
                                    )
                                AND ( @EndTime IS NULL
                                    OR StartTime < @EndTime
                                    )
                                AND ( @CreateUser IS NULL
                                    OR CreateUser = @CreateUser
                                    )
                                AND ( @UpdateUser IS NULL
                                    OR UpdateUser = @UpdateUser
                                    )";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Status", filter.Status),
                new SqlParameter("@StartTime", filter.StartTime),
                new SqlParameter("@EndTime", filter.EndTime),
                new SqlParameter("@CreateUser", filter.CreateUser),
                new SqlParameter("@UpdateUser", filter.UpdateUser),
                new SqlParameter("@ActivityName", filter.ActivityName)
            };
            var activityCount = SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters);
            return (int)activityCount;
        }

        public static bool DeleteActivityByActivityId(SqlConnection conn, Guid activityId)
        {
            var sql = @"DELETE  FROM Configuration..RegionVehicleIdActivityConfig
                        WHERE   ActivityId = @ActivityId;";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@ActivityId", activityId)
            };
            var count = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);
            return count > 0;
        }

        public static bool UpdateActivity(SqlConnection conn, RegionVehicleIdActivityConfig model)
        {
            var sql = @"UPDATE  Configuration..RegionVehicleIdActivityConfig
                        SET     ActivityName = @ActivityName ,
                                StartTime = @StartTime ,
                                EndTime = @EndTime ,
                                IsEnabled = @IsEnabled,
                                UpdateUser = @UpdateUser,
                                LastUpdateDateTime = GETDATE()
                        WHERE   ActivityId = @ActivityId;";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@ActivityId", model.ActivityId),
                new SqlParameter("@ActivityName", model.ActivityName),
                new SqlParameter("@StartTime", model.StartTime),
                new SqlParameter("@EndTime", model.EndTime),
                new SqlParameter("@UpdateUser", model.UpdateUser),
                new SqlParameter("@IsEnabled", model.IsEnabled)
            };
            var count = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);
            return count > 0;
        }

        public static bool CreateActivity(SqlConnection conn, RegionVehicleIdActivityConfig model)
        {
            var sql = @"INSERT  INTO Configuration..RegionVehicleIdActivityConfig
                                ( ActivityId ,
                                ActivityName ,
                                ActivityType ,
                                StartTime ,
                                EndTime ,
                                IsEnabled ,
                                CreateUser ,
                                UpdateUser ,
                                CreateDateTime ,
                                LastUpdateDateTime
                                )
                        VALUES  ( @ActivityId ,
                                @ActivityName ,
                                @ActivityType ,
                                @StartTime ,
                                @EndTime ,
                                @IsEnabled ,
                                @CreateUser ,
                                @UpdateUser ,
                                GETDATE() ,
                                GETDATE()
                                );";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@ActivityId", model.ActivityId),
                new SqlParameter("@ActivityName", model.ActivityName),
                new SqlParameter("@ActivityType", model.ActivityType),
                new SqlParameter("@StartTime", model.StartTime),
                new SqlParameter("@EndTime", model.EndTime),
                new SqlParameter("@IsEnabled", model.IsEnabled),
                new SqlParameter("@CreateUser", model.CreateUser),
                new SqlParameter("@UpdateUser", model.UpdateUser)
            };
            var count = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);
            return count > 0;
        }

        public static bool UpdateActivityIsEnabled(SqlConnection conn, int pkid, int isEnabled)
        {
            var sql = @"UPDATE  Configuration..RegionVehicleIdActivityConfig
                        SET     IsEnabled = @IsEnabled
                        WHERE   PKID = @PKID;";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@PKID", pkid),
                new SqlParameter("@IsEnable", isEnabled)
            };
            var count = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);
            return count > 0;
        }

        #endregion

        #region RegionVehicleIdURL

        /// <summary>
        /// 获取该活动配置的活动页
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static List<RegionVehicleIdActivityUrlConfig> SelectActivityUrlByActivityId(SqlConnection conn, Guid activityId)
        {
            var sql = @"SELECT  DISTINCT
                                TargetUrl ,
                                WxappUrl ,
                                IsDefault  
                        FROM    Configuration..RegionVehicleIdActivityUrlConfig WITH ( NOLOCK )
                        WHERE   ActivityId = @ActivityId
                        ORDER BY IsDefault DESC";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@ActivityId", activityId)
            };
            var urlList = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters).ConvertTo<RegionVehicleIdActivityUrlConfig>().ToList();
            return urlList;
        }

        /// <summary>
        /// 获取活动页配置的地区/车型
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="activityId"></param>
        /// <param name="targetUrl"></param>
        /// <param name="wxappUrl"></param>
        /// <returns></returns>
        public static List<RegionVehicleIdActivityUrlConfig> SelectRegionIdVehicleIdByActivityIdUrl(SqlConnection conn, Guid activityId, string targetUrl, string wxappUrl)
        {
            var sql = @"SELECT  RegionId ,
                                VehicleId
                        FROM    Configuration..RegionVehicleIdActivityUrlConfig AS s WITH ( NOLOCK )
                        WHERE   ActivityId = @ActivityId
                                AND ( @WxappUrl IS NULL
                                  OR @WxappUrl = N''
                                  OR WxappUrl = @WxappUrl
                                )
                                AND ( @TargetUrl IS NULL
                                     OR @TargetUrl = N''
                                     OR TargetUrl = @TargetUrl
                                   );";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@ActivityId", activityId),
                new SqlParameter("@TargetUrl", targetUrl),
                new SqlParameter("@WxappUrl", wxappUrl)
            };
            var urlList = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters).ConvertTo<RegionVehicleIdActivityUrlConfig>().ToList();
            return urlList;
        }

        /// <summary>
        /// 删除该活动下所有活动页
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static bool DeleteActivityUrlByActivityId(SqlConnection conn, Guid activityId)
        {
            var sql = @"DELETE  FROM Configuration..RegionVehicleIdActivityUrlConfig
                        WHERE   ActivityId = @ActivityId;";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@ActivityId", activityId)
            };
            var count = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);
            return count > 0;
        }

        /// <summary>
        /// 删除活动页
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="activityId"></param>
        /// <param name="targetUrl"></param>
        /// <param name="wxUrl"></param>
        /// <returns></returns>
        public static bool DeleteActivityUrlByActivityIdUrl(SqlConnection conn, Guid activityId, string targetUrl, string wxUrl)
        {
            var sql = @"DELETE  FROM Configuration..RegionVehicleIdActivityUrlConfig
                        WHERE ActivityId = @ActivityId
                              AND ( @TargetUrl IS NULL
                                    OR @TargetUrl = N''
                                    OR TargetUrl = @TargetUrl
                                  )
                              AND ( @WxappUrl IS NULL
                                    OR @WxappUrl = N''
                                    OR WxappUrl = @WxappUrl
                                  );";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@ActivityId", activityId),
                new SqlParameter("@TargetUrl", targetUrl),
                new SqlParameter("@WxappUrl", wxUrl)
            };
            var count = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);
            return count > 0;
        }

        /// <summary>
        /// 创建活动页
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool CreateActivityUrl(SqlConnection conn, RegionVehicleIdActivityUrlConfig model)
        {
            var sql = @"INSERT  INTO Configuration..RegionVehicleIdActivityUrlConfig
                              ( ActivityId ,
                                TargetUrl ,
                                WxappUrl ,
                                IsDefault ,
                                VehicleId ,
                                RegionId ,
                                CreateDateTime ,
                                LastUpdateDateTime
                               )
                        VALUES  ( @ActivityId ,
                                  @TargetUrl,
                                  @WxappUrl ,
                                  @IsDefault,
                                  @VehicleId,
                                  @RegionId,
                                  GETDATE(),
                                  GETDATE()
                                );";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@ActivityId", model.ActivityId),
                new SqlParameter("@TargetUrl", model.TargetUrl ?? string.Empty),
                new SqlParameter("@WxappUrl", model.WxappUrl),
                new SqlParameter("@IsDefault", model.IsDefault),
                new SqlParameter("@VehicleId", model.VehicleId),
                new SqlParameter("@RegionId", model.RegionId)
            };
            var count = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);
            return count > 0;
        }

        /// <summary>
        /// 一个活动只允许一个默认页
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static bool IsExistDefaultUrl(SqlConnection conn, Guid activityId)
        {
            var sql = @"SELECT  COUNT(0)
                        FROM    Configuration..RegionVehicleIdActivityUrlConfig WITH ( NOLOCK )
                        WHERE   ActivityId = @ActivityId
                                AND IsDefault = 1;";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@ActivityId", activityId)
            };
            var count = SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters);
            return (int)count > 0;
        }

        /// <summary>
        /// 一个活动下相同H5活动链接只允许配置一次
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="activityId"></param>
        /// <param name="targetUrl"></param>
        /// <returns></returns>
        public static bool IsExistH5Url(SqlConnection conn, Guid activityId, string targetUrl)
        {
            var sql = @"SELECT  COUNT(1)
                        FROM    Configuration.dbo.RegionVehicleIdActivityUrlConfig WITH ( NOLOCK )
                        WHERE   ActivityId = @ActivityId
                                AND TargetUrl = @TargetUrl;";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@ActivityId", activityId),
                new SqlParameter("@TargetUrl", targetUrl)
            };
            var count = SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters);
            return Convert.ToInt32(count) > 0;
        }

        /// <summary>
        /// 一个活动下相同小程序活动链接只允许配置一次
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="activityId"></param>
        /// <param name="wxappUrl"></param>
        /// <returns></returns>
        public static bool IsExistWxUrl(SqlConnection conn, Guid activityId, string wxappUrl)
        {
            var sql = @"SELECT  COUNT(1)
                        FROM    Configuration.dbo.RegionVehicleIdActivityUrlConfig WITH ( NOLOCK )
                        WHERE   ActivityId = @ActivityId
                                AND WxappUrl = @WxappUrl;";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@ActivityId", activityId),
                new SqlParameter("@WxappUrl", wxappUrl)
            };
            var count = SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters);
            return Convert.ToInt32(count) > 0;
        }

        #endregion


        #region 活动标题
        /// <summary>
        /// ActivePageList中获取活动标题
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="hashKey"></param>
        /// <returns></returns>
        public static string GetActivePageListTitle(SqlConnection conn, string hashKey)
        {
            var sql = @"SELECT  Title
                        FROM    Configuration..[ActivePageList] WITH ( NOLOCK )
                        WHERE   HashKey =  @HashKey;";
            var parameters = new SqlParameter[]
           {
                new SqlParameter("@HashKey", hashKey),
           };
            var result = SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters);
            return result == null ? null : (string)result;
        }

        /// <summary>
        /// ActivityBuild表中获取活动标题
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetActivityBuildTitle(SqlConnection conn, int id)
        {
            var sql = @"SELECT  Title
                        FROM    [Activity].[dbo].[ActivityBuild] WITH ( NOLOCK )
                        WHERE   id = @id ;";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@id", id)
            };
            var result = SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters);
            return result == null ? null : (string)result;
        }
        #endregion

        /// <summary>
        /// 根据活动Id获取记录的PKID
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static int GetActivityPKIDByActivityId(SqlConnection conn, Guid activityId)
        {
            var sql = @"SELECT  PKID
                        FROM    Configuration.dbo.RegionVehicleIdActivityConfig AS s WITH ( NOLOCK )
                        WHERE   s.ActivityId = @ActivityId;";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@ActivityId", activityId)
            };
            var pkid = SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters);
            return Convert.ToInt32(pkid);
        }
    }
}
