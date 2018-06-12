using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
  public  class DalThirdPartyMall
    {
        private static readonly string StrConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;

        private static readonly string ConnectionString = SecurityHelp.IsBase64Formatted(StrConn)
            ? SecurityHelp.DecryptAES(StrConn)
            : StrConn;

        private static readonly SqlConnection Conn = new SqlConnection(ConnectionString);


        private static readonly string StrConnconfig = ConfigurationManager.ConnectionStrings["Configuration"].ConnectionString;

        private static readonly string ConnectionStringConfig = SecurityHelp.IsBase64Formatted(StrConnconfig)
            ? SecurityHelp.DecryptAES(StrConnconfig)
            : StrConnconfig;

        private static readonly SqlConnection Connfig = new SqlConnection(ConnectionStringConfig);

        private static readonly string StrConnOnRead =
            ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;

        private static readonly string ConnectionStringOnRead = SecurityHelp.IsBase64Formatted(StrConnOnRead)
            ? SecurityHelp.DecryptAES(StrConnOnRead)
            : StrConnOnRead;

        private static readonly SqlConnection Connection = new SqlConnection(ConnectionStringOnRead);

        /// <summary>
        /// 搜索三方商城记录
        /// </summary>
        /// <returns></returns>
        public static List<ThirdPartyMallModel> SelectThirdMall(SerchThirdPartyMallModel serchMall)
        {
            string sql1 = @"SELECT  B.PKID ,B.UpdateDateTime,
        B.IsEnabled ,
        B.Sort ,
        B.ImageUrl ,
        B.Description ,
        B.StartDateTime ,
        B.EndDateTime ,
        B.BatchGuid ,
        B.BatchName ,
        B.BatchQty ,
        B.StockQty ,
        B.LimitQty ,
        B.Instructions ,
        B.Visible
FROM    ( SELECT    T.PKID ,
                    T.IsEnabled ,
                    T.Sort ,
                    T.ImageUrl ,
                    T.Description ,
                    T.StartDateTime ,
                    T.EndDateTime ,
                    T.BatchGuid ,
                    T.BatchName ,
                    T.BatchQty ,
                    T.StockQty ,
                    T.LimitQty ,
                    T.Instructions ,T.UpdateDateTime,
                    T.Visible,
                                         ";
            string sql2 = @"   FROM      ( SELECT    a.PKID ,
                                a.IsEnabled ,
                                a.Sort ,
                                a.ImageUrl ,
                                a.Description ,
                                a.StartDateTime ,
                                a.EndDateTime ,
                                B.BatchGuid ,
                                B.BatchName ,
                                B.BatchQty ,
                                B.StockQty ,
                                B.LimitQty ,
                                B.Instructions ,a.UpdateDateTime,
                                CASE WHEN ( a.StartDateTime < GETDATE()
                                            AND a.EndDateTime > GETDATE()
                                            AND B.StockQty > 0
                                          ) THEN 0
                                     ELSE 1
                                END AS Visible
                      FROM      Configuration.dbo.ThirdPartyMallConfig AS a
                                WITH ( NOLOCK )
                                JOIN Configuration..ThirdPartyCodeBatchConfig
                                AS B WITH ( NOLOCK ) ON B.BatchGuid = a.BatchGuid
                      WHERE     ( @IsEnabled IS NULL
                                  OR a.IsEnabled = @IsEnabled
                                )
                                AND ( @BatchName IS NULL
                                      OR B.BatchName LIKE '%' + @BatchName
                                      + '%'
                                    )
                                AND ( @BatchGuid IS NULL
                                      OR B.BatchGuid = @BatchGuid
                                    )
                    ) T
          WHERE     ( @Visible IS NULL
                      OR T.Visible = @Visible
                    )
        ) AS B
WHERE   B.RowNumber BETWEEN ( ( @PageNumber - 1 ) * @PageSize + 1 )
                    AND     ( @PageNumber * @PageSize );";
            string sqlSort = @" ROW_NUMBER() OVER ( ORDER BY T.UpdateDateTime DESC ) AS RowNumber ";
            #region 排序方式
            switch (serchMall.Sort)
            {
                case 10://批次数量升序
                    sqlSort = @" ROW_NUMBER() OVER ( ORDER BY T.BatchQty ) AS RowNumber ";
                    break;
                case 11://批次数量降序
                    sqlSort = @"ROW_NUMBER() OVER ( ORDER BY T.BatchQty  DESC ) AS RowNumber";
                    break;
                case 20://库存数量升序
                    sqlSort = @" ROW_NUMBER() OVER ( ORDER BY T.StockQty ) AS RowNumber ";
                    break;
                case 21://库存数量降序
                    sqlSort = @" ROW_NUMBER() OVER ( ORDER BY T.StockQty DESC ) AS RowNumber ";
                    break;
                case 30://兑换开始日期升序
                    sqlSort = @" ROW_NUMBER() OVER ( ORDER BY T.StartDateTime ) AS RowNumber ";
                    break;
                case 31://兑换开始日期降序
                    sqlSort = @" ROW_NUMBER() OVER ( ORDER BY T.StartDateTime DESC ) AS RowNumber ";
                    break;
                case 40://兑换结束日期升序
                    sqlSort = @" ROW_NUMBER() OVER ( ORDER BY T.EndDateTime ) AS RowNumber ";
                    break;
                case 41://兑换结束日期降序
                    sqlSort = @" ROW_NUMBER() OVER ( ORDER BY T.EndDateTime DESC ) AS RowNumber "; break;
                case 50://兑换结束日期升序
                    sqlSort = @" ROW_NUMBER() OVER ( ORDER BY T.Sort ) AS RowNumber ";
                    break;
                case 51://兑换结束日期降序
                    sqlSort = @" ROW_NUMBER() OVER ( ORDER BY T.Sort DESC ) AS RowNumber ";
                    break;

            }
            #endregion

            var sqlParameters = new[]
            {
                new SqlParameter("@PageSize", serchMall.PageSize),
                new SqlParameter("@PageNumber", serchMall.PageNumber),
                new SqlParameter("@IsEnabled", serchMall.IsEnabled),
                new SqlParameter("@Visible", serchMall.Visible),
                new SqlParameter("@BatchName", serchMall.BatchName),
                new SqlParameter("@BatchGuid", serchMall.BatchGuid)
            };
            return
                SqlHelper.ExecuteDataTable(Connfig, CommandType.Text, sql1 + sqlSort + sql2, sqlParameters)
                    .ConvertTo<ThirdPartyMallModel>()
                    .ToList();
        }

        /// <summary>
        /// 增加三方商城记录
        /// </summary>
        /// <param name="thirdMall"></param>
        /// <returns></returns>
        public static int InserThirdMall(ThirdPartyMallModel thirdMall)
        {
            const string sql = @"INSERT  INTO Configuration.dbo.ThirdPartyMallConfig
                                        ( BatchGuid ,
                                          BatchName ,
                                          IsEnabled ,
                                          Sort ,
                                          
                                        
                                          ImageUrl ,
                                          Description ,
                                          StartDateTime ,
                                          EndDateTime ,
                                          CreateDateTime ,
                                          UpdateDateTime
                                        )
                                VALUES  ( @BatchGuid , -- BatchGuid - uniqueidentifier
                                          @BatchName , -- BatchName - nvarchar(100)
                                          @IsEnabled , -- IsEnabled - bit
                                          @Sort , -- Sort - int
                                         
                                         
                                          @ImageUrl , -- ImageUrl - nvarchar(1000)
                                          @Description , -- Description - nvarchar(1000)
                                          @StartDateTime , -- StartDateTime - datetime
                                          @EndDateTime , -- EndDateTime - datetime
                                          GETDATE() , -- CreateDateTime - datetime
                                          GETDATE()  -- UpdateDateTime - datetime
                                        )
                                 SELECT @@IDENTITY ";

            var sqlParameter = new[]
            {
                new SqlParameter("@BatchGuid", thirdMall.BatchGuid),
                new SqlParameter("@BatchName",thirdMall.BatchName),

                new SqlParameter("@IsEnabled",thirdMall.IsEnabled),
                new SqlParameter("@Sort",thirdMall.Sort),

               
               
                new SqlParameter("@ImageUrl", thirdMall.ImageUrl),
                new SqlParameter("@StartDateTime", thirdMall.StartDateTime),
                new SqlParameter("@EndDateTime", thirdMall.EndDateTime),
                new SqlParameter("@Description", thirdMall.Description)
            };
            var result = SqlHelper.ExecuteScalar(Connfig, CommandType.Text, sql, sqlParameter);
            return Convert.ToInt32(result);
        }

        /// <summary>
        /// 编辑三方商城记录
        /// </summary>
        /// <param name="thirdMall"></param>
        /// <returns></returns>
        public static int EditThirdMall(ThirdPartyMallModel thirdMall)
        {
            const string sql = @"UPDATE  Configuration.dbo.ThirdPartyMallConfig
                                SET     IsEnabled = @IsEnabled ,
                                        Sort = @Sort ,
                                        ImageUrl = @ImageUrl ,
                                        Description = @Description ,
                                        StartDateTime = @StartDateTime ,
                                        EndDateTime = @EndDateTime ,
                                        UpdateDateTime = GETDATE()
                                WHERE   PKID = @PKID;";
                                
            var sqlParameter = new[]
            {
                new SqlParameter("@PKID",thirdMall.PKID),
                new SqlParameter("@Description",thirdMall.Description),
                new SqlParameter("@IsEnabled", thirdMall.IsEnabled),
                new SqlParameter("@Sort", thirdMall.Sort),
                new SqlParameter("@ImageUrl", thirdMall.ImageUrl),
                new SqlParameter("@StartDateTime", thirdMall.StartDateTime),
                new SqlParameter("@EndDateTime", thirdMall.EndDateTime)
            };
            return SqlHelper.ExecuteNonQuery(Connfig, CommandType.Text, sql, sqlParameter);
        }

        /// <summary>
        /// 根据PKID查询具体的信息
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        public static ThirdPartyMallModel SelectThirdMall(int pkid)
        {
            string sql1 = @"SELECT  a.PKID ,
                                    a.IsEnabled ,
                                    a.Sort ,
                                    a.ImageUrl ,
                                    a.Description ,
                                    a.StartDateTime ,
                                    a.EndDateTime ,
                                    b.BatchGuid ,
                                    b.BatchName ,
                                    b.BatchQty ,
                                    b.StockQty ,
                                    b.LimitQty ,
                                    b.Instructions ,
                                    CASE WHEN ( b.StartDateTime < GETDATE()
                                                AND b.EndDateTime > GETDATE()
                                                AND b.StockQty > 0
                                              ) THEN 1
                                         ELSE 2
                                    END AS Visible ,
                                    ROW_NUMBER() OVER ( ORDER BY a.EndDateTime DESC ) AS RowNumber
                            FROM    Configuration.dbo.ThirdPartyMallConfig AS a WITH ( NOLOCK )
                                    JOIN Configuration..ThirdPartyCodeBatchConfig AS b WITH ( NOLOCK ) ON b.BatchGuid = a.BatchGuid
                            WHERE   a.PKID = @PKID;";
                            
            return
                SqlHelper.ExecuteDataTable(Connection, CommandType.Text, sql1, new SqlParameter("@PKID", pkid))
                    .ConvertTo<ThirdPartyMallModel>().FirstOrDefault();

        }

        /// <summary>
        /// 根据总行数
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        public static int SelectCout(SerchThirdPartyMallModel serchMall)
        {
            string sql1 = @" SELECT  COUNT(1)
									 FROM    ( SELECT    a.PKID ,
                                                a.IsEnabled ,
                                                a.Sort ,
                                                a.ImageUrl ,
                                                a.Description ,
                                                a.StartDateTime ,
                                                a.EndDateTime ,
                                                B.BatchGuid ,
                                                B.BatchName ,
                                                B.BatchQty ,
                                                B.StockQty ,
                                                B.LimitQty ,
                                                B.Instructions ,
                                                CASE WHEN ( a.StartDateTime < GETDATE()
                                                        AND a.EndDateTime > GETDATE()
                                                        AND B.StockQty > 0
                                                          ) THEN 0
                                                     ELSE 1
                                                END AS Visible
                                      FROM      Configuration.dbo.ThirdPartyMallConfig AS a WITH ( NOLOCK )
                                                JOIN Configuration..ThirdPartyCodeBatchConfig AS B WITH ( NOLOCK ) ON B.BatchGuid = a.BatchGuid
                                      WHERE     ( @IsEnabled IS NULL
                                                  OR a.IsEnabled = @IsEnabled
                                                )
                                                AND ( @BatchName IS NULL
                                                      OR B.BatchName LIKE '%' + @BatchName + '%'
                                                    )
                                                AND ( @BatchGuid IS NULL
                                                      OR B.BatchGuid = @BatchGuid
                                                    )
                                    ) T
                            WHERE  (@Visible IS NULL
                                    OR T.Visible = @Visible)		
                  ";
            var sqlParameters = new[]
           {
                new SqlParameter("@IsEnabled", serchMall.IsEnabled),
                new SqlParameter("@Visible", serchMall.Visible),
                new SqlParameter("@BatchName", serchMall.BatchName),
                new SqlParameter("@BatchGuid", serchMall.BatchGuid)
            };
            return (int)SqlHelper.ExecuteScalar(Connection, CommandType.Text, sql1, sqlParameters);

        }


        /// <summary>
        /// 根据BatchGuid查询具体的批次信息
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        public static ThirdPartyCodeBatch SelectBatch(Guid batchId)
        {
            string sql1 = @"SELECT TOP 1
                                    PKID ,
                                    BatchGuid ,
                                    BatchName ,
                                    BatchQty ,
                                    StockQty ,
                                    LimitQty ,
                                    Instructions ,
                                    StartDateTime ,
                                    EndDateTime
                            FROM    Configuration.dbo.ThirdPartyCodeBatchConfig WITH ( NOLOCK )
                            WHERE   BatchGuid = @batchId;";

            return
                SqlHelper.ExecuteDataTable(Connection, CommandType.Text, sql1, new SqlParameter("@batchId", batchId))
                    .ConvertTo<ThirdPartyCodeBatch>().FirstOrDefault();

        }



    }
}
