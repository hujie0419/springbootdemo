using Dapper;
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
    public class DALAdvertisingConfig
    {
        /// <summary>
        /// 获取下单完成页广告配置列表
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public List<AdvertisingConfigModel> GetAdvertisingConfigList(SqlConnection conn, Pagination pagination)
        {
            #region SQL
            string sql = @" 
                            SELECT  [PKID] ,
                            [Title] ,
                            [AdLocation] ,
                            [AdType] ,
                            [ProvinceID] ,
                            [ProvinceName] ,
                            [CityID] ,
                            [CityName] ,
                            [ProductLine] ,
                            [SupportedChannels] ,
                            [PcIconUrl] ,
                            [MobileIconUrl] ,
                            [Slogan] ,
                            [OperatingButtonText] ,
                            [PcImageUrl] ,
                            [MobileImageUrl] ,
                            [PcRoute] ,
                            [MiniProgramRoute] ,
                            [IosRoute] ,
                            [AndroidRoute] ,
                            [WapRoute] ,
                            [HuaweiRoute] ,
                            [StartVersion] ,
                            [EndVersion] ,
                            [OnlineTime] ,
                            [OfflineTime] ,
                            [Status] ,
                            [Creator] ,
                            [CreateDateTime] ,
                            [LastUpdateDateTime] ,
                            [IsDeleted]
                    FROM    [Configuration].[dbo].[AdvertisingConfig] WITH(NOLOCK)
                    WHERE   IsDeleted = 0
                            AND ProductLine!='' --过滤掉默认配置

                    ORDER BY CreateDateTime DESC
                            OFFSET ( @PageIndex - 1 ) * @PageSize ROWS  FETCH NEXT @PageSize ROWS
                            ONLY; ";
            string sqlCount = @"
                                SELECT  COUNT(*)
                                FROM    [Configuration].[dbo].[AdvertisingConfig] WITH(NOLOCK)
                                WHERE   IsDeleted = 0
                                        AND ProductLine!='' --过滤掉默认配置";
            #endregion

            var dp = new DynamicParameters();
            dp.Add("@PageSize", pagination.rows);
            dp.Add("@PageIndex", pagination.page);

            pagination.records = (int)conn.ExecuteScalar(sqlCount, dp);

            if (pagination.records > 0)
                return conn.Query<AdvertisingConfigModel>(sql, dp).ToList();
            return null;
        }
        /// <summary>
        /// 获取下单完成页广告配置详细信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public AdvertisingConfigModel GetAdvertisingConfigInfo(SqlConnection conn, long id)
        {
            #region SQL
            string sql = @" SELECT [PKID]
                                  ,[Title]
                                  ,[AdLocation]
                                  ,[AdType]
                                  ,[ProvinceID]
                                  ,[ProvinceName]
                                  ,[CityID]
                                  ,[CityName]
                                  ,[ProductLine]
                                  ,[SupportedChannels]
                                  ,[PcIconUrl]
                                  ,[MobileIconUrl]
                                  ,[Slogan]
                                  ,[OperatingButtonText]
                                  ,[PcImageUrl]
                                  ,[MobileImageUrl]
                                  ,[PcRoute]
                                  ,[MiniProgramRoute]
                                  ,[IosRoute]
                                  ,[AndroidRoute]
                                  ,[WapRoute]
                                  ,[HuaweiRoute]
                                  ,[StartVersion]
                                  ,[EndVersion]
                                  ,[OnlineTime]
                                  ,[OfflineTime]
                                  ,[Status]
                                  ,[Creator]
                                  ,[CreateDateTime]
                                  ,[LastUpdateDateTime]
                                  ,[IsDeleted]
                              FROM [Configuration].[dbo].[AdvertisingConfig] WITH(NOLOCK)
                              WHERE PKID=@PKID ";
            #endregion

            DynamicParameters dp = new DynamicParameters();
            dp.Add("@PKID", id);
            return conn.Query<AdvertisingConfigModel>(sql, dp).FirstOrDefault();
        }
        /// <summary>
        /// 新增下单完成页广告配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddAdvertisingConfig(SqlConnection conn, AdvertisingConfigModel model)
        {
            #region SQL
            string sql = @"
                            INSERT INTO [Configuration].[dbo].[AdvertisingConfig] WITH(ROWLOCK)
                                       ([Title]
                                       ,[AdLocation]
                                       ,[AdType]
                                       ,[ProvinceID]
                                       ,[ProvinceName]
                                       ,[CityID]
                                       ,[CityName]
                                       ,[ProductLine]
                                       ,[SupportedChannels]
                                       ,[PcIconUrl]
                                       ,[MobileIconUrl]
                                       ,[Slogan]
                                       ,[OperatingButtonText]
                                       ,[PcImageUrl]
                                       ,[MobileImageUrl]
                                       ,[PcRoute]
                                       ,[MiniProgramRoute]
                                       ,[IosRoute]
                                       ,[AndroidRoute]
                                       ,[WapRoute]
                                       ,[HuaweiRoute]
                                       ,[StartVersion]
                                       ,[EndVersion]
                                       ,[OnlineTime]
                                       ,[OfflineTime]
                                       ,[Status]
                                       ,[Creator])
                                 VALUES
                                       (@Title
                                       ,@AdLocation
                                       ,@AdType
                                       ,@ProvinceID
                                       ,@ProvinceName
                                       ,@CityID
                                       ,@CityName
                                       ,@ProductLine
                                       ,@SupportedChannels
                                       ,@PcIconUrl
                                       ,@MobileIconUrl
                                       ,@Slogan
                                       ,@OperatingButtonText
                                       ,@PcImageUrl
                                       ,@MobileImageUrl
                                       ,@PcRoute
                                       ,@MiniProgramRoute
                                       ,@IosRoute
                                       ,@AndroidRoute
                                       ,@WapRoute
                                       ,@HuaweiRoute
                                       ,@StartVersion
                                       ,@EndVersion
                                       ,@OnlineTime
                                       ,@OfflineTime
                                       ,@STATUS
                                       ,@Creator);
                            SELECT CAST(SCOPE_IDENTITY() as int);";
            #endregion
            model.PKID = conn.Query<int>(sql, model).FirstOrDefault();
            if (model.PKID > 0)
                return true;
            else return false;
        }
        /// <summary>
        /// 删除下单完成页广告配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteAdvertisingConfig(SqlConnection conn, long id)
        {
            string sql = " UPDATE [Configuration].[dbo].[AdvertisingConfig] WITH(ROWLOCK) SET IsDeleted=1,LastUpdateDateTime=GETDATE() WHERE PKID=@PKID ";
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@PKID", id);
            return conn.Execute(sql, dp) > 0;
        }
        /// <summary>
        /// 修改下单完成页广告配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateAdvertisingConfig(SqlConnection conn, AdvertisingConfigModel model)
        {
            #region SQL
            string sql = @" UPDATE [Configuration].[dbo].[AdvertisingConfig] WITH(ROWLOCK)
                               SET [Title] = @Title
                                  ,[AdLocation] = @AdLocation
                                  ,[AdType] = @AdType
                                  ,[ProvinceID] = @ProvinceID
                                  ,[ProvinceName] = @ProvinceName
                                  ,[CityID] = @CityID
                                  ,[CityName] = @CityName
                                  ,[ProductLine] = @ProductLine
                                  ,[SupportedChannels] = @SupportedChannels
                                  ,[PcIconUrl] = @PcIconUrl
                                  ,[MobileIconUrl] = @MobileIconUrl
                                  ,[Slogan] = @Slogan
                                  ,[OperatingButtonText] = @OperatingButtonText
                                  ,[PcImageUrl] = @PcImageUrl
                                  ,[MobileImageUrl] = @MobileImageUrl
                                  ,[PcRoute] = @PcRoute
                                  ,[MiniProgramRoute] = @MiniProgramRoute
                                  ,[IosRoute] = @IosRoute
                                  ,[AndroidRoute] = @AndroidRoute
                                  ,[WapRoute] = @WapRoute
                                  ,[HuaweiRoute]=@HuaweiRoute
                                  ,[StartVersion] = @StartVersion
                                  ,[EndVersion] = @EndVersion
                                  ,[OnlineTime]=@OnlineTime
                                  ,[OfflineTime]=@OfflineTime
                                  ,[Status] = @Status
                                  ,[Creator] = @Creator
                                  ,[LastUpdateDateTime] = GETDATE()
                             WHERE PKID=@PKID ";
            #endregion

            return conn.Execute(sql, model) > 0;
        }
        /// <summary>
        /// 获取下单完成页广告位配置详细信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="provinceId"></param>
        /// <param name="cityId"></param>
        /// <param name="productLine"></param>
        /// <param name="AdType"></param>
        /// <returns></returns>
        public AdvertisingConfigModel GetAdvertisingConfigInfo(SqlConnection conn, int provinceId, int cityId, string productLine,int adType)
        {
            #region SQL
            string sql = @"
                            SELECT [PKID]
                                  ,[Title]
                                  ,[AdLocation]
                                  ,[AdType]
                                  ,[ProvinceID]
                                  ,[ProvinceName]
                                  ,[CityID]
                                  ,[CityName]
                                  ,[ProductLine]
                                  ,[SupportedChannels]
                                  ,[PcIconUrl]
                                  ,[MobileIconUrl]
                                  ,[Slogan]
                                  ,[OperatingButtonText]
                                  ,[PcImageUrl]
                                  ,[MobileImageUrl]
                                  ,[PcRoute]
                                  ,[MiniProgramRoute]
                                  ,[IosRoute]
                                  ,[AndroidRoute]
                                  ,[WapRoute]
                                  ,[HuaweiRoute]
                                  ,[StartVersion]
                                  ,[EndVersion]
                                  ,[OnlineTime]
                                  ,[OfflineTime]
                                  ,[Status]
                                  ,[Creator]
                                  ,[CreateDateTime]
                                  ,[LastUpdateDateTime]
                                  ,[IsDeleted]
                              FROM [Configuration].[dbo].[AdvertisingConfig] WITH(NOLOCK)
                    WHERE   IsDeleted = 0
                            AND Status = 1
                            AND AdType=@AdType
                            AND ProvinceID = @ProvinceID
                            AND CityID=@CityID
                            AND ProductLine =ISNULL(@ProductLine,ProductLine);";
            #endregion

            DynamicParameters dp = new DynamicParameters();
            dp.Add("@AdType", adType);
            dp.Add("@ProvinceID", provinceId);
            dp.Add("@CityID", cityId);
            dp.Add("@ProductLine", productLine);

            return conn.Query<AdvertisingConfigModel>(sql, dp).FirstOrDefault();
        }
    }
}
