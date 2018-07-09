using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DALCommonConfigLog
    {
        /// <summary>
        /// 获取通用日志列表
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pagination"></param>
        /// <param name="objectType"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public List<CommonConfigLogModel> GetCommonConfigLogList(SqlConnection conn, Pagination pagination, string objectId, string objectType, DateTime startTime, DateTime endTime)
        {
            #region SQL
            string sql = @"
                            SELECT  [PKID] ,
                                    [ObjectId] ,
                                    [ObjectType] ,
                                    [BeforeValue] ,
                                    [AfterValue] ,
                                    [Remark] ,
                                    [Creator] ,
                                    [CreateDateTime] ,
                                    [LastUpdateDateTime] ,
                                    [IsDeleted]
                            FROM    [Tuhu_log].[dbo].[CommonConfigLog]
                            WHERE   IsDeleted = 0
                                    AND ObjectID=ISNULL(@ObjectID,ObjectID)
                                    AND ObjectType=@ObjectType
                                    AND CreateDateTime >= @StartTime
                                    AND CreateDateTime <= @EndTime
                            ORDER BY CreateDateTime DESC
                                    OFFSET ( @PageIndex - 1 ) * @PageSize ROWS  FETCH NEXT @PageSize ROWS
                                    ONLY;";

            string sqlCount = @"
                                SELECT  COUNT(*)
                                FROM    [Tuhu_log].[dbo].[CommonConfigLog]
                                WHERE   IsDeleted = 0
                                        AND ObjectID=ISNULL(@ObjectID,ObjectID)
                                        AND ObjectType=@ObjectType
                                        AND CreateDateTime >= @StartTime
                                        AND CreateDateTime <= @EndTime";
            #endregion
            var dp = new DynamicParameters();
            if (!string.IsNullOrEmpty(objectId))
                dp.Add("@ObjectID", objectId);
            else
                dp.Add("@ObjectID", null);
            dp.Add("@ObjectType", objectType);
            dp.Add("@StartTime", startTime);
            dp.Add("@EndTime", endTime);
            dp.Add("@PageSize", pagination.rows);
            dp.Add("@PageIndex", pagination.page);

            pagination.records = (int)conn.ExecuteScalar(sqlCount, dp);

            if (pagination.records > 0)
                return conn.Query<CommonConfigLogModel>(sql, dp).ToList();
            return null;
        }
        /// <summary>
        /// 获取通用日志列表
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pagination"></param>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public List<CommonConfigLogModel> GetCommonConfigLogList(SqlConnection conn, Pagination pagination, string objectId, string objectType)
        {
            #region SQL
            string sql = @"
                            SELECT  [PKID] ,
                                    [ObjectId] ,
                                    [ObjectType] ,
                                    [BeforeValue] ,
                                    [AfterValue] ,
                                    [Remark] ,
                                    [Creator] ,
                                    [CreateDateTime] ,
                                    [LastUpdateDateTime] ,
                                    [IsDeleted]
                            FROM    [Tuhu_log].[dbo].[CommonConfigLog]
                            WHERE   IsDeleted = 0
                                    AND ObjectID=ISNULL(@ObjectID,ObjectID)
                                    AND ObjectType=@ObjectType
                            ORDER BY CreateDateTime DESC
                                    OFFSET ( @PageIndex - 1 ) * @PageSize ROWS  FETCH NEXT @PageSize ROWS
                                    ONLY;";

            string sqlCount = @"
                                SELECT  COUNT(*)
                                FROM    [Tuhu_log].[dbo].[CommonConfigLog]
                                WHERE   IsDeleted = 0
                                        AND ObjectID=ISNULL(@ObjectID,ObjectID)
                                        AND ObjectType=@ObjectType";
            #endregion
            var dp = new DynamicParameters();
            if (!string.IsNullOrEmpty(objectId))
                dp.Add("@ObjectID", objectId);
            else
                dp.Add("@ObjectID", null);
            dp.Add("@ObjectType", objectType);
            dp.Add("@PageSize", pagination.rows);
            dp.Add("@PageIndex", pagination.page);

            pagination.records = (int)conn.ExecuteScalar(sqlCount, dp);

            if (pagination.records > 0)
                return conn.Query<CommonConfigLogModel>(sql, dp).ToList();
            return null;
        }
        /// <summary>
        /// 获取通用日志信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public CommonConfigLogModel GetCommonConfigLogInfo(SqlConnection conn, int id)
        {
            #region SQL
            string sql = @"SELECT [PKID]
                          ,[ObjectId]
                          ,[ObjectType]
                          ,[BeforeValue]
                          ,[AfterValue]
                          ,[Remark]
                          ,[Creator]
                          ,[CreateDateTime]
                          ,[LastUpdateDateTime]
                          ,[IsDeleted]
                      FROM [Tuhu_log].[dbo].[CommonConfigLog]
                      WHERE PKID=@PKID";
            #endregion

            DynamicParameters dp = new DynamicParameters();
            dp.Add("@PKID", id);
            return conn.Query<CommonConfigLogModel>(sql, dp).FirstOrDefault();
        }


        /// <summary>
        /// ADD 日志
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ADDCommonConfigLogInfo(SqlConnection conn, CommonConfigLogModel model)
        {
            #region SQL
            string sql = @"insert into   [Tuhu_log].[dbo].[CommonConfigLog]
                                  ([ObjectId]
                                  ,[ObjectType]
                                  ,[BeforeValue]
                                  ,[AfterValue]
                                  ,[Remark]
                                  ,[Creator]
                                  ,[CreateDateTime]
                                  ,[LastUpdateDateTime])
                           Values( @ObjectId
                                  ,@ObjectType
                                  ,@BeforeValue
                                  ,@AfterValue
                                  ,@Remark
                                  ,@Creator
                                  ,@CreateDateTime
                                  ,@LastUpdateDateTime)";
            #endregion);
            return conn.Execute(sql, model) > 0;
        }

    }
}
