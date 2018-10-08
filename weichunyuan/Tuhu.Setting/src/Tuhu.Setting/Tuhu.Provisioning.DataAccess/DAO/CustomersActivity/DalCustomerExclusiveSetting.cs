using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity.CustomersActivity;

namespace Tuhu.Provisioning.DataAccess.DAO.CustomersActivity
{
    public class DalCustomerExclusiveSetting
    {
        #region 大客户活动专享配置
        /// <summary>
        /// 查询大客户专享活动配置列表
        /// </summary>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="pageSize">pageSize</param>
        /// <returns></returns>
        public static IEnumerable<CustomerExclusiveSettingModel> SelectCustomerExclusives(SqlConnection conn, int pageIndex, int pageSize)
        {

            string sqlCustomerExclusiveSetting = @"SELECT [PKID]
                      ,[ActivityExclusiveId]
                      ,[OrderChannel]
                      ,[LargeCustomersID]
                      ,[LargeCustomersName]
                      ,[EventLink]
                      ,[ActivityId]
                      ,[ImageUrl]
                      ,[BusinessHotline]
                      ,[IsEnable]
                      ,[IsDelete]
                      ,[CreateTime]
                      ,[CreateBy]
                      ,[UpdateDatetime]
                      ,[UpdateBy]
                FROM
                (
                    SELECT 
     	                [PKID],
	                    ROW_NUMBER() OVER (PARTITION BY [PKID] ORDER BY [CreateTime]) AS RowNum
                      ,[ActivityExclusiveId]
                      ,[OrderChannel]
                      ,[LargeCustomersID]
                      ,[LargeCustomersName]
                      ,[EventLink]
                      ,[ActivityId]
                      ,[ImageUrl]
                      ,[BusinessHotline]
                      ,[IsEnable]
                      ,[IsDelete]
                      ,[CreateTime]
                      ,[CreateBy]
                      ,[UpdateDatetime]
                      ,[UpdateBy]
	                  FROM  Activity.[dbo].[CustomerExclusiveSetting] WITH(NOLOCK)
	                WHERE [IsDelete] = 0 
                ) AS t
                WHERE [t].[RowNum] = 1
                ORDER BY [t].[PKID] DESC
                OFFSET @offset ROWS
                FETCH NEXT @size ROWS ONLY";


            return conn.Query<CustomerExclusiveSettingModel>(sqlCustomerExclusiveSetting,
                new
                {
                    size = pageSize,
                    offset = (pageIndex - 1) * pageSize
                });
        }

        /// <summary>
        /// 查询大客户专享活动配置列表总数
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int SelectCustomerExclusiveCount(SqlConnection conn)
        {
            string sqlCustomerExclusiveSettingCount = $@"SELECT COUNT(1)
                                    FROM
                                    (
                                        SELECT 
     	                                    [PKID],
	                                        ROW_NUMBER() OVER (PARTITION BY [PKID] ORDER BY [CreateTime]) AS RowNum
                                          ,[ActivityExclusiveId]
                                          ,[OrderChannel]
                                          ,[LargeCustomersID]
                                          ,[LargeCustomersName]
                                          ,[EventLink]
                                          ,[ActivityId]
                                          ,[ImageUrl]
                                          ,[BusinessHotline]
                                          ,[IsEnable]
                                          ,[IsDelete]
                                          ,[CreateTime]
                                          ,[CreateBy]
                                          ,[UpdateDatetime]
                                          ,[UpdateBy]
	                                      FROM  Activity.[dbo].[CustomerExclusiveSetting] WITH(NOLOCK)
	                                    WHERE [IsDelete] = 0 
                                    ) AS t
                                    WHERE [t].[RowNum] = 1";
            return (int)conn.ExecuteScalar(sqlCustomerExclusiveSettingCount);
        }

        /// <summary>
        /// 新增时验证限时抢购ID是否唯一
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="ActivityId"></param>
        /// <returns></returns>
        public static IEnumerable<CustomerExclusiveSettingModel> SelectCustomerExclusive(SqlConnection conn, string ActivityId)
        {
            string sqlCustomerExclusiveSetting = @"SELECT 
     	               [PKID]
                      ,[ActivityExclusiveId]
                      ,[OrderChannel]
                      ,[LargeCustomersID]
                      ,[LargeCustomersName]
                      ,[EventLink]
                      ,[ActivityId]
                      ,[ImageUrl]
                      ,[BusinessHotline]
                      ,[IsEnable]
                      ,[IsDelete]
	                  FROM  Activity.[dbo].[CustomerExclusiveSetting] WITH(NOLOCK)
	                WHERE [IsDelete] = 0 AND ActivityId = @buyid";

            return conn.Query<CustomerExclusiveSettingModel>(sqlCustomerExclusiveSetting,
                new
                {
                    buyid = ActivityId
                });
        }

        /// <summary>
        /// 编辑时验证限时抢购ID是否唯一
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="ActivityId"></param>
        /// <param name="PkId"></param>
        /// <returns></returns>
        public static IEnumerable<CustomerExclusiveSettingModel> SelectCustomerExclusive(SqlConnection conn, string ActivityId, int PkId)
        {
            string sqlCustomerExclusiveSetting = @"SELECT 
     	               [PKID]
                      ,[ActivityExclusiveId]
                      ,[OrderChannel]
                      ,[LargeCustomersID]
                      ,[LargeCustomersName]
                      ,[EventLink]
                      ,[ActivityId]
                      ,[ImageUrl]
                      ,[BusinessHotline]
                      ,[IsEnable]
                      ,[IsDelete]
	                  FROM  Activity.[dbo].[CustomerExclusiveSetting] WITH(NOLOCK)
	                WHERE [IsDelete] = 0 AND ActivityId = @buyid AND PKID !=@pkid";

            return conn.Query<CustomerExclusiveSettingModel>(sqlCustomerExclusiveSetting,
                new
                {
                    buyid = ActivityId,
                    pkid = PkId
                });
        }

        /// <summary>
        /// 查询客户活动专享配置单个实体信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="ActivityId"></param>
        /// <param name="PkId"></param>
        /// <returns></returns>
        public static IEnumerable<CustomerExclusiveSettingModel> SelectCustomerExclusive(SqlConnection conn, int PkId)
        {
            string sqlCustomerExclusiveSetting = @"SELECT 
     	               [PKID]
                      ,[ActivityExclusiveId]
                      ,[OrderChannel]
                      ,[LargeCustomersID]
                      ,[LargeCustomersName]
                      ,[EventLink]
                      ,[ActivityId]
                      ,[ImageUrl]
                      ,[BusinessHotline]
                      ,[IsEnable]
                      ,[IsDelete]
	                  FROM  Activity.[dbo].[CustomerExclusiveSetting] WITH(NOLOCK)
	                WHERE [IsDelete] = 0 AND PKID =@pkid";

            return conn.Query<CustomerExclusiveSettingModel>(sqlCustomerExclusiveSetting,
                new
                {
                    pkid = PkId
                });
        }

        /// <summary>
        /// 客户专享活动配置编辑
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="customerExclusiveSettingModel">实体</param>
        /// <returns></returns>
        public static int UpdateCustomerExclusiveSetting(SqlConnection conn, CustomerExclusiveSettingModel customerExclusiveSettingModel)
        {

            const string sqlUpdateCustomerExclusiveSettin = @"UPDATE  Activity.[dbo].[CustomerExclusiveSetting] WITH(ROWLOCK)
                                       SET [OrderChannel] = @OrderChannel
                                          ,[LargeCustomersID] = @LargeCustomersID
                                          ,[LargeCustomersName] = @LargeCustomersName
                                          ,[EventLink] = @EventLink
                                          ,[ActivityId] = @ActivityId
                                          ,[ImageUrl] = @ImageUrl
                                          ,[BusinessHotline] = @BusinessHotline
                                          ,[IsEnable] =@IsEnable
                                          ,[IsDelete] = 0
                                          ,[CreateTime] = GETDATE()
                                     WHERE PKID =@PKID";
            return conn.Execute(sqlUpdateCustomerExclusiveSettin, customerExclusiveSettingModel, commandType: CommandType.Text);
        }

        /// <summary>
        /// 客户专享活动配置新增
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="customerExclusiveSettingModel">实体</param>
        /// <returns></returns>
        public static int InsertCustomerExclusiveSetting(SqlConnection conn, CustomerExclusiveSettingModel customerExclusiveSettingModel)
        {

            const string sqlInsertCustomerExclusiveSettin = @"INSERT INTO  Activity.[dbo].[CustomerExclusiveSetting]
                                                               ([ActivityExclusiveId]
                                                               ,[OrderChannel]
                                                               ,[LargeCustomersID]
                                                               ,[LargeCustomersName]
                                                               ,[EventLink]
                                                               ,[ActivityId]
                                                               ,[ImageUrl]
                                                               ,[BusinessHotline]
                                                               ,[IsEnable]
                                                               ,[IsDelete]
                                                               ,[CreateTime]
                                                               ,[CreateBy])
                                                         VALUES
                                                               (@ActivityExclusiveId
                                                               ,@OrderChannel
                                                               ,@LargeCustomersID
                                                               ,@LargeCustomersName
                                                               ,@EventLink
                                                               ,@ActivityId
                                                               ,@ImageUrl
                                                               ,@BusinessHotline
                                                               ,@IsEnable
                                                               ,0
                                                               ,GETDATE()
                                                               ,@CreateBy)";
            return conn.Execute(sqlInsertCustomerExclusiveSettin, customerExclusiveSettingModel, commandType: CommandType.Text);
        }

        /// <summary>
        /// 获取客户配置专项表最大的主键ID
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int GetCustomerExclusiveSettingMaxPkId(SqlConnection conn)
        {
            string sqlGetCustomerExclusiveSettingMaxPkId = "SELECT MAX(PKID) FROM Activity.[dbo].[CustomerExclusiveSetting] WITH(NOLOCK)";

            return (int)conn.ExecuteScalar(sqlGetCustomerExclusiveSettingMaxPkId);
        }

        #endregion


        #region 大客户活动专享券码

        /// <summary>
        /// 查询大客户专享活动券码列表
        /// </summary>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="pageSize">pageSize</param>
        /// <param name="queryString">查询条件</param>
        /// <param name="customersSettingId">活动专享配置表ID</param>
        /// <param name="activityExclusiveId">活动专享ID</param>
        /// <returns></returns>
        public static IEnumerable<CustomerExclusiveCouponModel> SelectCustomerCoupons(SqlConnection conn, string queryString, string customersSettingId, string activityExclusiveId, int pageIndex, int pageSize)
        {

            string sqlCustomerCoupons = @"SELECT [PKID]
                          ,[CustomerExclusiveSettingPkId]
                          ,[ActivityExclusiveId]
                          ,[CouponCode]
                          ,[UserName]
                          ,[UserId]
                          ,[Phone]
                          ,[Status]
                          ,[IsDelete]
                          ,[CreateTime]
                          ,[CreateBy]
                          ,[UpdateDateTime]
                          ,[UpdateBy]
                                        FROM
                                        (
                                            SELECT 
     	                                        [PKID],
	                                            ROW_NUMBER() OVER (PARTITION BY [PKID] ORDER BY                               [CreateTime]) AS RowNum
                                              ,[CustomerExclusiveSettingPkId]
					                          ,[ActivityExclusiveId]
					                          ,[CouponCode]
					                          ,[UserName]
					                          ,[UserId]
					                          ,[Phone]
					                          ,[Status]
					                          ,[IsDelete]
					                          ,[CreateTime]
					                          ,[CreateBy]
					                          ,[UpdateDateTime]
					                          ,[UpdateBy]
	                                          FROM  Activity.[dbo].[CustomerExclusiveCoupon] WITH(NOLOCK)
	                                        WHERE [IsDelete] = 0  AND (
                                            (@queryString=''  OR Phone LIKE @queryString)
                                            OR (@queryString='' OR CouponCode LIKE @queryString)) 
                                            AND CustomerExclusiveSettingPkId = @csId
                                            AND ActivityExclusiveId = @atyNo
                                        ) AS t
                                        WHERE [t].[RowNum] = 1
                                        ORDER BY [t].[PKID] DESC
                                        OFFSET @offset ROWS
                                        FETCH NEXT @size ROWS ONLY";


            return conn.Query<CustomerExclusiveCouponModel>(sqlCustomerCoupons,
                new
                {
                    queryString = "%" + queryString + "%",
                    csId = customersSettingId,
                    atyNo = activityExclusiveId,
                    size = pageSize,
                    offset = (pageIndex - 1) * pageSize
                });
        }

        /// <summary>
        /// 查询大客户专享活动券码总数
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="queryString">查询条件</param>
        /// <param name="customersSettingId">活动专享配置表PKID</param>
        /// <param name="activityExclusiveId">活动专享ID</param>
        /// <returns></returns>
        public static int SelectCustomerCouponCount(SqlConnection conn, string queryString, string customersSettingId, string activityExclusiveId)
        {
            string sqlCustomerCouponCount = $@"SELECT  COUNT(1)
                FROM
                (
                    SELECT 
     	                [PKID],
	                    ROW_NUMBER() OVER (PARTITION BY [PKID] ORDER BY [CreateTime]) AS RowNum
                      ,[CustomerExclusiveSettingPkId]
					  ,[ActivityExclusiveId]
					  ,[CouponCode]
					  ,[UserName]
					  ,[UserId]
					  ,[Phone]
					  ,[Status]
					  ,[IsDelete]
					  ,[CreateTime]
					  ,[CreateBy]
					  ,[UpdateDateTime]
					  ,[UpdateBy]
	                  FROM  Activity.[dbo].[CustomerExclusiveCoupon] WITH(NOLOCK)
	                WHERE [IsDelete] = 0 
                    AND ((@queryString=''  OR Phone LIKE @queryString)
                        OR (@queryString='' OR CouponCode LIKE @queryString)) 
                    AND CustomerExclusiveSettingPkId = @csId
                    AND ActivityExclusiveId = @atyNo
                ) AS t
                WHERE [t].[RowNum] = 1";
            return (int)conn.ExecuteScalar(sqlCustomerCouponCount,
                new
                {
                    queryString = "%" + queryString + "%",
                    csId = customersSettingId,
                    atyNo = activityExclusiveId
                });
        }


        /// <summary>
        /// 验证券码是否已经存在
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="CouponCode">券码</param>
        /// <param name="ActivityExclusiveId">活动专享ID</param>
        /// <returns></returns>
        public static IEnumerable<CustomerExclusiveSettingModel> SelectCustomerCouponInfo(SqlConnection conn, string CouponCode, string ActivityExclusiveId)
        {
            string sqlCustomerCouponInfo = @"SELECT [PKID]
                                                  ,[CustomerExclusiveSettingPkId]
                                                  ,[ActivityExclusiveId]
                                                  ,[CouponCode]
                                                  ,[UserName]
                                                  ,[UserId]
                                                  ,[Phone]
                                                  ,[Status]
                                                  ,[IsDelete]
                                                  ,[CreateTime]
                                                  ,[CreateBy]
                                                  ,[UpdateDateTime]
                                                  ,[UpdateBy]
                                              FROM Activity.[dbo].[CustomerExclusiveCoupon] WITH(NOLOCK)
                                              WHERE IsDelete = 0 AND CouponCode =@couponCode
                                              AND ActivityExclusiveId = @activityExclusiveId";

            return conn.Query<CustomerExclusiveSettingModel>(sqlCustomerCouponInfo,
                new
                {
                    couponCode = CouponCode,
                    activityExclusiveId = ActivityExclusiveId
                });
        }

        /// <summary>
        /// 客户专享活动券码新增
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="customerExclusiveCouponModel">实体</param>
        /// <returns></returns>
        public static int InsertCustomerCoupon(SqlConnection conn, CustomerExclusiveCouponModel customerExclusiveCouponModel)
        {

            string sqlInsertCustomerCoupon = @"INSERT INTO 
                         Activity.[dbo].[CustomerExclusiveCoupon]
                               ([CustomerExclusiveSettingPkId]
                               ,[ActivityExclusiveId]
                               ,[CouponCode]
                               ,[Status]
                               ,[IsDelete]
                               ,[CreateTime]
                               ,[CreateBy])
                         VALUES
                               (@CustomerExclusiveSettingPkId
                               ,@ActivityExclusiveId
                               ,@CouponCode
                               ,0
                               ,0
                               ,getdate()
                               ,@CreateBy)";
            return conn.Execute(sqlInsertCustomerCoupon, customerExclusiveCouponModel, commandType: CommandType.Text);
        }

        /// <summary>
        /// 获取客户专享券码表最大的主键ID
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int GetCustomerCouponMaxPkId(SqlConnection conn)
        {
            string sqlGetCustomerCouponMaxPkId = "SELECT MAX(PKID) FROM Activity.[dbo].[CustomerExclusiveCoupon] WITH(NOLOCK)";

            return (int)conn.ExecuteScalar(sqlGetCustomerCouponMaxPkId);
        }

        /// <summary>
        /// 客户专享活动券码状态修改
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="status">状态</param>
        /// <param name="pkId">主键</param>
        /// <param name="user">用户名称</param>
        /// <returns></returns>
        public static int UpdateCustomerCouponStatus(SqlConnection conn, CustomerExclusiveCouponModel customerExclusiveCouponModel)
        {

            string sqlCustomerCouponStatus = @"UPDATE Activity.[dbo].[CustomerExclusiveCoupon] WITH(ROWLOCK)
                                       SET [Status] = @status
                                          ,[UpdateDateTime] = GETDATE()
                                          ,[UpdateBy] = @updateBy
                                     WHERE PKID = @pkId";
            return conn.Execute(sqlCustomerCouponStatus,
                new
                {
                    status = customerExclusiveCouponModel.Status,
                    updateBy = customerExclusiveCouponModel.UpdateBy,
                    pkId = customerExclusiveCouponModel.PKID
                }, commandType: CommandType.Text);
        }


        /// <summary>
        /// 查询客户活动专享券码单个实体信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="ActivityId"></param>
        /// <param name="PkId"></param>
        /// <returns></returns>
        public static IEnumerable<CustomerExclusiveCouponModel> SelectCustomerCoupon(SqlConnection conn, int PkId)
        {
            string sqlSelectCustomerCoupon = @"SELECT [PKID]
                                    ,[CustomerExclusiveSettingPkId]
                                    ,[ActivityExclusiveId]
                                    ,[CouponCode]
                                    ,[UserName]
                                    ,[UserId]
                                    ,[Phone]
                                    ,[Status]
                                    ,[IsDelete]
                                    ,[CreateTime]
                                    ,[CreateBy]
                                    ,[UpdateDateTime]
                                    ,[UpdateBy]
                                    FROM Activity.[dbo].[CustomerExclusiveCoupon] WITH(NOLOCK)
	                           WHERE [IsDelete] = 0 AND PKID =@pkid";

            return conn.Query<CustomerExclusiveCouponModel>(sqlSelectCustomerCoupon,
                new
                {
                    pkid = PkId
                });
        }

        #endregion

        /// <summary>
        /// 查询客户专享活动单条日志信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="ojbetId"></param>
        /// <returns></returns>
        public static List<CustomerExclusiveSettingLogModel> GetCustomerExclusiveSettingLog(string ojbetId, string source)
        {
            const string sqlGetCustomerEsLog = @"SELECT [PKID]
                              ,[ObjectId]
                              ,[ObjectType]
                              ,[BeforeValue]
                              ,[AfterValue]
                              ,[Remark]
                              ,[Creator]
                              ,[CreateDateTime]
                              ,[LastUpdateDateTime]
                              ,[IsDeleted]
                          FROM Tuhu_log.[dbo].[CustomerExclusiveSettingLog] WITH(NOLOCK)
                          WHERE IsDeleted = 0 
                          AND ObjectId = @objectId
                          AND Source = @source
                           ORDER BY PKID DESC;";

            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Tuhu_log")))
            {
                using (var cmd = new SqlCommand(sqlGetCustomerEsLog))
                {
                    cmd.Parameters.AddWithValue("@objectId", ojbetId);
                    cmd.Parameters.AddWithValue("@source", source);
                    return dbHelper.ExecuteDataTable(cmd)?.ConvertTo<CustomerExclusiveSettingLogModel>()?.AsList() ??
                           new List<CustomerExclusiveSettingLogModel>();
                }
            }

        }
    }
}
