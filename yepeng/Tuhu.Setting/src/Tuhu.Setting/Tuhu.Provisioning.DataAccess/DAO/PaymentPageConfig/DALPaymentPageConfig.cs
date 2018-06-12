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
    public class DALPaymentPageConfig
    {
        /// <summary>
        /// 获取下单完成页配置列表
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public List<PaymentPageConfigModel> GetPaymentPageConfigList(SqlConnection conn, Pagination pagination)
        {
            #region SQL
            string sql = @" 
                            SELECT  [PKID] ,
                                    [Title] ,
                                    [ProvinceID] ,
                                    [ProvinceName] ,
                                    [CityID] ,
                                    [CityName] ,
                                    [ProductLine] ,
                                    [IsShowConsigneeInfo] ,
                                    [IsShowAddress] ,
                                    [IsShowShop] ,
                                    [IsShowButton] ,
                                    [ButtonText] ,
                                    [ButtonUrl],
                                    [MiniProgramButtonUrl],
                                    [IosButtonUrl],
                                    [AndroidButtonUrl],
                                    [WapButtonUrl],
                                    [HuaweiButtonUrl],
                                    [Remark] ,
                                    [Status] ,
                                    [Creator] ,
                                    [CreateDateTime] ,
                                    [LastUpdateDateTime]
                            FROM    [Configuration].[dbo].[PaymentPageConfig] WITH(NOLOCK)
                            WHERE   IsDeleted = 0
                                    AND ProductLine!='' --过滤掉默认配置 
                            ORDER BY CreateDateTime DESC
                                    OFFSET ( @PageIndex - 1 ) * @PageSize ROWS  FETCH NEXT @PageSize ROWS
                                    ONLY;";
            string sqlCount = @"
                                SELECT  COUNT(*)
                                FROM    [Configuration].[dbo].[PaymentPageConfig] WITH(NOLOCK)
                                WHERE   IsDeleted = 0
                                        AND ProductLine!='' --过滤掉默认配置 ";
            #endregion

            var dp = new DynamicParameters();
            dp.Add("@PageSize", pagination.rows);
            dp.Add("@PageIndex", pagination.page);

            pagination.records = (int)conn.ExecuteScalar(sqlCount, dp);

            if (pagination.records > 0)
                return conn.Query<PaymentPageConfigModel>(sql, dp).ToList();
            return null;
        }
        /// <summary>
        /// 获取下单完成页配置详细信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public PaymentPageConfigModel GetPaymentPageConfigInfo(SqlConnection conn, long id)
        {
            #region SQL
            string sql = @" SELECT  [PKID] ,
                                    [Title] ,
                                    [ProvinceID] ,
                                    [ProvinceName] ,
                                    [CityID] ,
                                    [CityName] ,
                                    [ProductLine] ,
                                    [IsShowConsigneeInfo] ,
                                    [IsShowAddress] ,
                                    [IsShowShop] ,
                                    [IsShowButton] ,
                                    [ButtonText] ,
                                    [ButtonUrl] ,
                                    [MiniProgramButtonUrl],
                                    [IosButtonUrl],
                                    [AndroidButtonUrl],
                                    [WapButtonUrl],
                                    [HuaweiButtonUrl],
                                    [Remark] ,
                                    [Status] ,
                                    [Creator] ,
                                    [CreateDateTime] ,
                                    [LastUpdateDateTime]
                            FROM    [Configuration].[dbo].[PaymentPageConfig] WITH(NOLOCK)
                            WHERE PKID=@PKID ";
            #endregion

            DynamicParameters dp = new DynamicParameters();
            dp.Add("@PKID", id);
            return conn.Query<PaymentPageConfigModel>(sql, dp).FirstOrDefault();
        }
        /// <summary>
        /// 新增下单完成页配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddPaymentPageConfig(SqlConnection conn, PaymentPageConfigModel model)
        {
            #region SQL
            string sql = @"
                            INSERT INTO [Configuration].[dbo].[PaymentPageConfig] WITH(ROWLOCK)
                                       ([Title]
                                       ,[ProvinceID]
                                       ,[ProvinceName]
                                       ,[CityID]
                                       ,[CityName]
                                       ,[ProductLine]
                                       ,[IsShowConsigneeInfo]
                                       ,[IsShowAddress]
                                       ,[IsShowShop]
                                       ,[IsShowButton]
                                       ,[ButtonText]
                                       ,[ButtonUrl]
                                       ,[MiniProgramButtonUrl]
                                       ,[IosButtonUrl]
                                       ,[AndroidButtonUrl]
                                       ,[WapButtonUrl]
                                       ,[HuaweiButtonUrl]
                                       ,[Remark]
                                       ,[Status]
                                       ,[Creator])
                                 VALUES
                                       (@Title
                                       ,@ProvinceID
                                       ,@ProvinceName
                                       ,@CityID
                                       ,@CityName
                                       ,@ProductLine
                                       ,@IsShowConsigneeInfo
                                       ,@IsShowAddress
                                       ,@IsShowShop
                                       ,ISNULL(@IsShowButton,0)
                                       ,ISNULL(@ButtonText,'')
                                       ,ISNULL(@ButtonUrl,'')
                                       ,ISNULL(@MiniProgramButtonUrl,'')
                                       ,ISNULL(@IosButtonUrl,'')
                                       ,ISNULL(@AndroidButtonUrl,'')
                                       ,ISNULL(@WapButtonUrl,'')
                                       ,ISNULL(@HuaweiButtonUrl,'')
                                       ,ISNULL(@Remark,'')
                                       ,@STATUS
                                       ,@Creator);
                           SELECT CAST(SCOPE_IDENTITY() as int);";
            #endregion
            model.PKID= conn.Query<int>(sql, model).FirstOrDefault();
            if (model.PKID > 0)
                return true;
            else return false;
        }
        /// <summary>
        /// 删除下单完成页配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeletePaymentPageConfig(SqlConnection conn, long id)
        {
            string sql = " UPDATE [Configuration].[dbo].[PaymentPageConfig] WITH(ROWLOCK) SET IsDeleted=1,LastUpdateDateTime=GETDATE() WHERE PKID=@PKID ";
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
        public bool UpdatePaymentPageConfig(SqlConnection conn, PaymentPageConfigModel model)
        {
            #region SQL
            string sql = @" UPDATE [dbo].[PaymentPageConfig] WITH(ROWLOCK)
                               SET [Title] = @Title
                                  ,[ProvinceID] = @ProvinceID
                                  ,[ProvinceName] = @ProvinceName
                                  ,[CityID] = @CityID
                                  ,[CityName] = @CityName
                                  ,[ProductLine] = @ProductLine
                                  ,[IsShowConsigneeInfo] = @IsShowConsigneeInfo
                                  ,[IsShowAddress] = @IsShowAddress
                                  ,[IsShowShop] = @IsShowShop
                                  ,[IsShowButton] = @IsShowButton
                                  ,[ButtonText] = ISNULL(@ButtonText,'')
                                  ,[ButtonUrl] = ISNULL(@ButtonUrl,'')
                                  ,[MiniProgramButtonUrl]=ISNULL(@MiniProgramButtonUrl,'')
                                  ,[IosButtonUrl]=ISNULL(@IosButtonUrl,'')
                                  ,[AndroidButtonUrl]=ISNULL(@AndroidButtonUrl,'')
                                  ,[WapButtonUrl]=ISNULL(@WapButtonUrl,'')
                                  ,[HuaweiButtonUrl]=ISNULL(@HuaweiButtonUrl,'')
                                  ,[Remark] = ISNULL(@Remark,'')
                                  ,[Status] = @STATUS
                                  ,[Creator] = @Creator
                                  ,[LastUpdateDateTime] = GETDATE()
                             WHERE PKID=@PKID ";
            #endregion

            return conn.Execute(sql, model) > 0;
        }
        /// <summary>
        /// 获取下单完成页配置详细信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="provinceId"></param>
        /// <param name="cityId"></param>
        /// <param name="productLine"></param>
        /// <returns></returns>
        public PaymentPageConfigModel GetPaymentPageConfigInfo(SqlConnection conn, int provinceId, int cityId, string productLine)
        {
            #region SQL
            string sql = @"
                            SELECT  [PKID] ,
                            [Title] ,
                            [ProvinceID] ,
                            [ProvinceName] ,
                            [CityID] ,
                            [CityName] ,
                            [ProductLine] ,
                            [IsShowConsigneeInfo] ,
                            [IsShowAddress] ,
                            [IsShowShop] ,
                            [IsShowButton] ,
                            [ButtonText] ,
                            [ButtonUrl] ,
                            [MiniProgramButtonUrl],
                            [IosButtonUrl],
                            [AndroidButtonUrl],
                            [WapButtonUrl],
                            [HuaweiButtonUrl],
                            [Remark] ,
                            [Status] ,
                            [Creator] ,
                            [CreateDateTime] ,
                            [LastUpdateDateTime] ,
                            [IsDeleted]
                    FROM    [Configuration].[dbo].[PaymentPageConfig] WITH ( NOLOCK )
                    WHERE   IsDeleted = 0
                            AND Status = 1
                            AND ProvinceID = @ProvinceID
                            AND CityID=@CityID
                            AND ProductLine =ISNULL(@ProductLine,ProductLine);";
            #endregion

            DynamicParameters dp = new DynamicParameters();
            dp.Add("@ProvinceID", provinceId);
            dp.Add("@CityID", cityId);
            dp.Add("@ProductLine", productLine);

            return conn.Query<PaymentPageConfigModel>(sql, dp).FirstOrDefault();
        }
    }
}
