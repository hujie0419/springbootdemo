using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.Sql;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.CouponActivityConfigV2;
using Microsoft.ApplicationBlocks.Data;
using System.Data;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Request;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalCouponActivityConfigV2
    {
        /// <summary>
        /// 获取蓄电池/加油卡浮动配置分页列表
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static Tuple<int, List<CouponActivityConfigV2Model>> GetCouponActivityConfigs(SqlConnection conn, CouponActivityConfigPageRequestModel model)
        {
            int totalCount = 0;
            var sql = @"SELECT @totalCount = COUNT(1)
                        FROM [Configuration].[dbo].[SE_CouponActivityConfig] AS a
                        WHERE a.Type = @Type  or @Type =0; 
                        SELECT A.[Id],
                               [ActivityNum],
                               [ActivityName],
                               [ActivityStatus],
                               [CheckStatus],
                               [LayerImage],
                               [CouponId],
                               [ButtonChar],
                               ActivityImage,
                               A.GetRuleGUID,
                               [CreateTime],
                                A.Type,
                                A.Channel,
                               [UpdateTime]
                        FROM [Configuration].[dbo].[SE_CouponActivityConfig] AS A WITH (NOLOCK)
                        WHERE A.Type = @Type or @Type =0
                        ORDER BY A.[UpdateTime] DESC OFFSET (@PageIndex - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;";
            var parameters = new SqlParameter[] {
                new SqlParameter("@PageIndex",model.PageIndex),
               new SqlParameter("@PageSize",model.PageSize),
               new SqlParameter("@Type",model.Type),
               new SqlParameter("@totalCount",System.Data.SqlDbType.Int){  Direction=System.Data.ParameterDirection.Output}

          };
            var configs = SqlHelper.ExecuteDataTable2(conn, CommandType.Text, sql, parameters).ConvertTo<CouponActivityConfigV2Model>().ToList();
            int.TryParse(parameters.Last().Value.ToString(), out totalCount);
            return Tuple.Create(totalCount, configs);
        }
        /// <summary>
        /// 根据PKID 查询单个实体
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static CouponActivityConfigV2Model GetCouponActivityConfigById(SqlConnection conn, int id)
        {
            const string sql = @"SELECT [Id]
                                      ,[ActivityNum]
                                      ,[ActivityName]
                                      ,[ActivityStatus]
                                      ,[CheckStatus]                                  
                                      ,[LayerImage]
                                      ,[CouponId]
                                      ,[ButtonChar]                                                        
                                      ,[CreateTime] 
                                      ,[UpdateTime]
                                      ,ActivityImage 
                                      ,GetRuleGUID 
                                      ,Type   
                              FROM [Configuration].[dbo].[SE_CouponActivityConfig] WITH (NOLOCK)  WHERE Id = @Id ";
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, new SqlParameter("@Id", id)).ConvertTo<CouponActivityConfigV2Model>().ToList().FirstOrDefault();
        }
        /// <summary>
        /// 添加蓄电池/加油卡浮动配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int InsertCouponActivityConfig(SqlConnection conn, CouponActivityConfig model)
        {
            const string sql = @"INSERT INTO [Configuration].[dbo].[SE_CouponActivityConfig]
                                        ( 
                                           [ActivityNum]
                                          ,[ActivityName]
                                          ,[ActivityStatus]
                                          ,[CheckStatus]
                                          ,Type
                                          ,[LayerImage]
                                          ,[CouponId]     
                                          ,[CreateTime]
                                          ,[UpdateTime]
                                          ,ActivityImage 
                                            ,Channel 
                                        )
                                VALUES  ( @ActivityNum
                                          ,@ActivityName
                                          ,@ActivityStatus
                                          ,@CheckStatus
                                          ,@Type
                                          ,@LayerImage 
                                          ,@ButtonChar                                    
                                          ,GETDATE()
                                          ,GETDATE()
                                          ,@ActivityImage 
                                          ,@Channel 
                                        )SELECT @@IDENTITY
                                ";
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@ActivityName",model.ActivityName),
                new SqlParameter("@ActivityNum",model.ActivityNum),
                new SqlParameter("@ActivityStatus",model.ActivityStatus),
                new SqlParameter("@ButtonChar",model.ButtonChar),
                new SqlParameter("@CheckStatus",model.CheckStatus),
                new SqlParameter("@CouponId",model.CouponId),
                new SqlParameter("@LayerImage",model.LayerImage),
                new SqlParameter("@ActivityImage",model.ActivityImage),
                new SqlParameter("@Type",model.Type),
                new SqlParameter("@Channel",model.Channel),
            };
            return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, sqlParameters));
        }
        /// <summary>
        /// 修改蓄电池/加油卡浮动配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool UpdateCouponActivityConfig(SqlConnection conn, CouponActivityConfigV2Model model)
        {
            const string sql = @"UPDATE  [Configuration].[dbo].[SE_CouponActivityConfig]
                                SET    
                                        ActivityName = @ActivityName ,
                                        ActivityStatus = @ActivityStatus ,
                                        CheckStatus = @CheckStatus ,
                                        Type=@Type,
                                        LayerImage = @LayerImage ,
                                        CouponId = @CouponId ,
                                        ButtonChar = @ButtonChar ,    
                                        ActivityImage = @ActivityImage,
                                        UpdateTime = GETDATE(),
                                        Channel=@Channel,
                                        GetRuleGUID=@GetRuleGUID
                                WHERE   Id = @Id";

            var sqlParameters = new SqlParameter[]
           {
                new SqlParameter("@ActivityName",model.ActivityName),
                new SqlParameter("@ActivityNum",model.ActivityNum),
                new SqlParameter("@ActivityStatus",model.ActivityStatus),
                new SqlParameter("@ButtonChar",model.ButtonChar),
                new SqlParameter("@CheckStatus",model.CheckStatus),
                new SqlParameter("@CouponId",model.CouponId),
                new SqlParameter("@LayerImage",model.LayerImage),
                new SqlParameter("@Id",model.Id),
                new SqlParameter("@ActivityImage",model.ActivityImage),
                new SqlParameter("@GetRuleGUID",model.GetRuleGUID??string.Empty),
                new SqlParameter("@Type",model.Type),
                new SqlParameter("@Channel",model.Channel)
           };

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameters) > 0;

        }
        /// <summary>
        /// 蓄电池/加油卡浮动配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteCouponActivityConfig(SqlConnection conn, int id)
        {
            const string sql = @"DELETE FROM [Configuration].[dbo].[SE_CouponActivityConfig] WHERE Id =@Id";
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, new SqlParameter("@Id", id)) > 0;
        }
        #region [SE_CouponActivityChannelConfig]
        /// <summary>
        /// 根据活动ID查出平台渠道配置信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="configIds"></param>
        /// <returns></returns>
        public static List<CouponActivityChannelConfigModel> GetChannelConfigs(SqlConnection conn, IEnumerable<int> configIds)
        {
            var sql = @"WITH T
                        AS (SELECT *
                            FROM Configuration..SplitString(@ConfigIds, N',', 1) )
                        SELECT a.[PKID],
                               a.[ConfigId],
                               a.[Channel],
                               a.[GetRuleGUID],
                               a.[Url],
                               a.[Type],
                               a.[CreateDateTime],
                               a.[LastUpdateDateTime]
                        FROM [Configuration].[dbo].[SE_CouponActivityChannelConfig] AS a (NOLOCK)
                            INNER JOIN T
                                ON T.Item = a.ConfigId;";
            var parameters = new SqlParameter[]
            {
               new SqlParameter("@ConfigIds",string.Join(",",configIds))
            };
            return SqlHelper.ExecuteDataTable2(conn, CommandType.Text, sql, parameters).ConvertTo<CouponActivityChannelConfigModel>().ToList();
        }
        /// <summary>
        /// 根据配置ID和类型拿到渠道配置信息列表
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="configId">SE_CouponActivityConfig  PKIDid</param>
        /// <param name="type">可空</param>
        /// <returns></returns>
        public static List<CouponActivityChannelConfigModel> GetChannelConfigs(SqlConnection conn, int configId, string type)
        {
            var sql = @"SELECT a.[PKID],
                       a.[ConfigId],
                       a.[Channel],
                       a.[GetRuleGUID],
                       a.[Url],
                       a.[Type],
                       a.[CreateDateTime],
                       a.[LastUpdateDateTime]
                FROM [Configuration].[dbo].[SE_CouponActivityChannelConfig] AS a (NOLOCK)
                WHERE a.ConfigId = @configId
                      AND
                      (
                          a.type = @Type
                          OR @Type = N''
                          OR @Type IS NULL
                      );";
            var parameters = new SqlParameter[]
            {
               new SqlParameter("@configId",configId),
               new SqlParameter("@Type",type),
            };
            return SqlHelper.ExecuteDataTable2(conn, CommandType.Text, sql, parameters).ConvertTo<CouponActivityChannelConfigModel>().ToList();
        }
        /// <summary>
        /// 根据configid删除对应渠道配置信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="configIds">SE_CouponActivityConfig  PKIDid</param>
        /// <returns></returns>
        public static bool DeleteChannelConfigsByConfigId(SqlConnection conn, IEnumerable<int> configIds)
        {
            var sql = @"WITH T
                        AS (SELECT *
                            FROM Configuration..SplitString(@ConfigIds, N',', 1) )
                        DELETE a
                        FROM [Configuration].[dbo].[SE_CouponActivityChannelConfig] AS a
                            INNER JOIN T
                                ON a.ConfigId = T.Item;";
            var parameters = new SqlParameter[]
            {
                 new SqlParameter("@ConfigIds",string.Join(",",configIds))
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
        }

        /// <summary>
        /// 添加渠道配置信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool InsertChannelConfigsById(SqlConnection conn, CouponActivityChannelConfigModel model)
        {
            var sql = @"INSERT INTO [Configuration].[dbo].[SE_CouponActivityChannelConfig]
                        (
                            ConfigId,
                            Channel,
                            GetRuleGUID,
                            Url,
                            Type
                        )
                        VALUES
                        (@ConfigId, @Channel, @GetRuleGUID, @Url, @Type);";
            var parameters = new SqlParameter[]
             {
               new SqlParameter("@GetRuleGUID",model.GetRuleGUID),
               new SqlParameter("@Url",model.Url),
               new SqlParameter("@Type",model.Type),
               new SqlParameter("@Channel",model.Channel),
               new SqlParameter("@ConfigId",model.ConfigId),
             };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
        }
        #endregion

    }
}
