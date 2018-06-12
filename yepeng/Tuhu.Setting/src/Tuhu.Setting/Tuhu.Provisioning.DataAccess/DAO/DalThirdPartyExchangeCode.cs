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
    public static class DalThirdPartyExchangeCode
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
        /// 搜索兑换码批次信息
        /// </summary>
        /// <returns></returns>
        public static List<ThirdPartyCodeBatch> SelectBatches(SerchElement serchElement)
        {
            string sql1 = @"SELECT  PKID ,
                                        BatchGuid ,
                                        BatchName ,
                                        LimitQty ,
                                        BatchQty ,
                                        StockQty ,
                                        StartDateTime ,
                                        EndDateTime ,
                                        Creator ,
                                        CreateDateTime ,
                                        Modifier ,
                                        UpdateDateTime
                                FROM    ( SELECT    PKID ,BatchGuid ,
                                                    BatchName ,
                                                    LimitQty ,
                                                    BatchQty ,
                                                    StockQty ,
                                                    StartDateTime ,
                                                    EndDateTime ,
                                                    Creator ,
                                                    CreateDateTime ,
                                                    Modifier ,
                                                    UpdateDateTime ,
                                         ";
            string sql2 = @"FROM      Configuration.dbo.ThirdPartyCodeBatchConfig WITH ( NOLOCK )
                                          WHERE     ( @BatchGuid IS  NULL--批次ID
                                                      OR @BatchGuid = BatchGuid
                                                    )
                                                    AND ( @BatchName IS NULL--批次名称
                                                          OR BatchName LIKE '%' + @BatchName + '%'
                                                        )
                                                    AND ( @Creator IS NULL--创建人
                                                          OR Creator LIKE '%' + @Creator + '%'
                                                        )
                                                    AND ( @Modifier IS NULL--最近更新人
                                                          OR Modifier LIKE '%' + @Modifier + '%'
                                                        )
                                        ) AS T
                                WHERE   T.RowNumber BETWEEN ( ( @PageNumber - 1 ) * @PageSize + 1 )
                                                    AND     ( @PageNumber * @PageSize ) ;";
            string sqlSort = @" ROW_NUMBER() OVER ( ORDER BY UpdateDateTime DESC ) AS RowNumber ";
            #region 排序方式
            switch (serchElement.Sort)
            {
                case 10://批次数量升序
                    sqlSort = @" ROW_NUMBER() OVER ( ORDER BY BatchQty ) AS RowNumber ";
                    break;
                case 11://批次数量降序
                    sqlSort = @"ROW_NUMBER() OVER ( ORDER BY BatchQty DESC ) AS RowNumber ";
                    break;
                case 20://库存数量升序
                    sqlSort = @" ROW_NUMBER() OVER ( ORDER BY StockQty ) AS RowNumber ";
                    break;
                case 21://库存数量降序
                    sqlSort = @" ROW_NUMBER() OVER ( ORDER BY StockQty  DESC ) AS RowNumber ";
                    break;
                case 30://兑换开始日期升序
                    sqlSort = @" ROW_NUMBER() OVER ( ORDER BY StartDateTime  ) AS RowNumber ";
                    break;
                case 31://兑换开始日期降序
                    sqlSort = @" ROW_NUMBER() OVER ( ORDER BY StartDateTime DESC ) AS RowNumber ";
                    break;
                case 40://兑换结束日期升序
                    sqlSort = @" ROW_NUMBER() OVER ( ORDER BY EndDateTime ) AS RowNumber ";
                    break;
                case 41://兑换结束日期降序
                    sqlSort = @" ROW_NUMBER() OVER ( ORDER BY EndDateTime DESC) AS RowNumber ";
                    break;
                case 50://创建日期升序
                    sqlSort = @" ROW_NUMBER() OVER ( ORDER BY CreateDateTime ) AS RowNumber ";
                    break;
                case 51://创建日期降序
                    sqlSort = @" ROW_NUMBER() OVER ( ORDER BY CreateDateTime DESC ) AS RowNumber ";
                    break;
                case 60://更新日期升序
                    sqlSort = @" ROW_NUMBER() OVER ( ORDER BY UpdateDateTime ) AS RowNumber ";
                    break;
                case 61://更新日期降序
                    sqlSort = @" ROW_NUMBER() OVER ( ORDER BY UpdateDateTime DESC) AS RowNumber ";
                    break;

            }
            #endregion

            var sqlParameters = new[]
            {
                new SqlParameter("@PageSize", serchElement.PageSize),
                new SqlParameter("@PageNumber", serchElement.PageNumber),
                new SqlParameter("@BatchGuid", serchElement.BatchGuid),
                new SqlParameter("@Instructions", serchElement.Instructions),
                new SqlParameter("@BatchName", serchElement.BatchName),
                new SqlParameter("@Creator", serchElement.Creator),
                new SqlParameter("@Modifier", serchElement.Modifier)
            };
            return
                SqlHelper.ExecuteDataTable(Connfig, CommandType.Text, sql1 + sqlSort + sql2, sqlParameters)
                    .ConvertTo<ThirdPartyCodeBatch>()
                    .ToList();
        }
        /// <summary>
        /// 增加兑换码批次
        /// </summary>
        /// <param name="codeBatch"></param>
        /// <returns></returns>
        public static int InserBatches(ThirdPartyCodeBatch codeBatch)
        {
            const string sql = @"INSERT INTO Configuration.dbo.ThirdPartyCodeBatchConfig
                                      ( BatchGuid ,
                                        BatchName ,
                                        BatchQty ,
                                        StockQty ,
                                        LimitQty ,
                                        Instructions ,
                                        StartDateTime ,
                                        EndDateTime ,
                                        CreateDateTime ,
                                        Creator ,
                                        UpdateDateTime ,
                                        Modifier
                                      )
                              VALUES  ( @BatchGuid , -- BatchGuid - uniqueidentifier
                                        @BatchName , -- BatchName - nvarchar(100)
                                        @BatchQty , -- BatchQty - int
                                        @StockQty , -- StockQty - int
                                        @LimitQty , -- LimitQty - int
                                        @Instructions , -- Instructions - nvarchar(1000)
                                        @StartDateTime , -- StartDateTime - datetime
                                        @EndDateTime , -- EndDateTime - datetime
                                        GETDATE() , -- CreateDateTime - datetime
                                        @Creator , -- Creator - nvarchar(100)
                                        GETDATE() , -- UpdateDateTime - datetime
                                        @Modifier  -- Modifier - nvarchar(100)
                                      )
                                    SELECT @@IDENTITY
                                            ";

            var sqlParameter = new[]
            {
                new SqlParameter("@BatchGuid", codeBatch.BatchGuid),
                new SqlParameter("@BatchName",codeBatch.BatchName),
                new SqlParameter("@BatchQty", codeBatch.BatchQty),
                new SqlParameter("@StockQty", codeBatch.StockQty),
                new SqlParameter("@LimitQty", codeBatch.LimitQty),
                new SqlParameter("@Instructions", codeBatch.Instructions),
                new SqlParameter("@StartDateTime", codeBatch.StartDateTime),
                new SqlParameter("@EndDateTime", codeBatch.EndDateTime),
                new SqlParameter("@Creator", codeBatch.Creator),
                new SqlParameter("@Modifier", codeBatch.Modifier)
            };

            var result = SqlHelper.ExecuteScalar(Connfig, CommandType.Text, sql, sqlParameter);
            return Convert.ToInt32(result);
        }
        /// <summary>
        /// 编辑兑换码批次记录
        /// </summary>
        /// <param name="codeBatch"></param>
        /// <returns></returns>
        public static int UpdateBatches(ThirdPartyCodeBatch codeBatch)
        {
            const string sql = @"UPDATE  Configuration.dbo.ThirdPartyCodeBatchConfig
                                 SET     BatchName = @BatchName ,
                                         LimitQty = @LimitQty ,
                                         StartDateTime = @StartDateTime ,
                                         EndDateTime = @EndDateTime ,
                                         Instructions = @Instructions ,
                                         UpdateDateTime = @UpdateDateTime ,
                                         Modifier = @Modifier
                                         WHERE BatchGuid=@BatchGuid
                                    ;"; 
            int result = -1;
            using (var dbhelper = new SqlDbHelper(ConnectionStringConfig))
            {
                try
                {
                    dbhelper.BeginTransaction();
                    var cmd = new SqlCommand(sql);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@BatchGuid", codeBatch.BatchGuid);
                    cmd.Parameters.AddWithValue("@BatchName", codeBatch.BatchName);
                    cmd.Parameters.AddWithValue("@LimitQty", codeBatch.LimitQty);
                    cmd.Parameters.AddWithValue("@Instructions", codeBatch.Instructions);
                    cmd.Parameters.AddWithValue("@StartDateTime", codeBatch.StartDateTime);
                    cmd.Parameters.AddWithValue("@EndDateTime", codeBatch.EndDateTime);
                    cmd.Parameters.AddWithValue("@Modifier", codeBatch.Modifier);
                    cmd.Parameters.AddWithValue("@UpdateDateTime", codeBatch.UpdateDateTime);
                    var i = dbhelper.ExecuteNonQuery(cmd);
                    if (i > 0)
                    {
                        DalThirdPartyExchangeCode.UpdateExchangeCodeTime(codeBatch.StartDateTime, codeBatch.EndDateTime, codeBatch.BatchGuid);
                    }
                    dbhelper.Commit();
                    result = 1;
                }
                catch (Exception ex)
                {
                    dbhelper.Rollback();
                    return -1;
                }
            }
          
            return result;
        }
        /// <summary>
        /// 批次导入兑换码
        /// </summary>
        /// <param name="exchangeCode"></param>
        /// <returns></returns>
        public static int InsertExchangeCode(ThirdPartyExchangeCode exchangeCode)
        {
            const string sql = @"INSERT INTO Configuration.dbo.ThirdPartyExchangeCodeConfig
                                        ( BatchGuid ,
                                          IsEnabled ,
                                          IsGain ,
                                          UserId ,
                                          ExchangeCode ,
                                          StartDateTime ,
                                          EndDateTime ,
                                          ImportDateTime ,
                                          GainDateTime
                                        )
                                VALUES  ( @BatchGuid , -- BatchGuid - uniqueidentifier
                                          @IsEnabled , -- IsEnabled - bit
                                          0 , -- IsGain - bit
                                          NULL , -- UserId - uniqueidentifier
                                          @ExchangeCode , -- ExchangeCode - nvarchar(100)
                                          @StartDateTime , -- StartDateTime - datetime
                                          @EndDateTime , -- EndDateTime - datetime
                                          GETDATE() , -- ImportDateTime - datetime
                                          NULL  -- GainDateTime - datetime
                                        )";

            var sqlParameter = new[]
            {
                new SqlParameter("@BatchGuid", exchangeCode.BatchGuid),
                new SqlParameter("@IsEnabled",exchangeCode.IsEnabled),
                new SqlParameter("@ExchangeCode", exchangeCode.ExchangeCode),
                new SqlParameter("@StartDateTime", exchangeCode.StartDateTime),
                new SqlParameter("@EndDateTime", exchangeCode.EndDateTime)
                
            };
            return SqlHelper.ExecuteNonQuery(Connfig, CommandType.Text, sql, sqlParameter);
        }

        /// <summary>
        /// 搜索兑换码批次信息
        /// </summary>
        /// <returns></returns>
        public static List<ThirdPartyExchangeCode> SelectExchangCode(SerchCodeElement serchElement)
        {
            string sql1 = @"SELECT  IsEnabled ,
                                    IsGain ,
                                    ExchangeCode ,
                                    StartDateTime ,
                                    EndDateTime ,
                                    ImportDateTime ,
                                    UserId ,
                                    GainDateTime
                            FROM    ( SELECT    IsEnabled ,
                                                IsGain ,
                                                ExchangeCode ,
                                                StartDateTime ,
                                                EndDateTime ,
                                                ImportDateTime ,
                                                UserId ,
                                                GainDateTime ,
                                          ";
            string sql2 = @" FROM  Configuration.dbo.ThirdPartyExchangeCodeConfig WITH ( NOLOCK )
                                      WHERE     ( @IsGain IS NULL--是否领取
                                                  OR IsGain = @IsGain
                                                )
                                                AND ( @OutTime IS NULL--已过期
                                                      OR EndDateTime < @OutTime
                                                    )
                            						 AND ( @OnTime IS NULL--未过期
                                                      OR EndDateTime > @OnTime
                                                    )
                                                AND ( @IsEnabled IS NULL--是否禁用
                                                      OR IsEnabled = @IsEnabled
                                                    )
                                                AND ( @BatchId IS NULL--批次ID
                                                      OR BatchGuid = @BatchId
                                                )
                                    ) AS T
                            WHERE   T.RowNumber BETWEEN ( ( @PageNumber - 1 ) * @PageSize + 1 )
                                                AND     ( @PageNumber * @PageSize );
                            ";
            string sqlSort = @" ROW_NUMBER() OVER ( ORDER BY ImportDateTime DESC ) AS RowNumber ";
            #region 排序方式

            switch (serchElement.Sort)
            {
                case 10: //兑换开始日期升序
                    sqlSort = @"ROW_NUMBER() OVER ( ORDER BY StartDateTime ) AS RowNumber ";
                    break;
                case 11: //兑换开始日期降序
                    sqlSort = @"ROW_NUMBER() OVER ( ORDER BY StartDateTime DESC ) AS RowNumber ";
                    break;
                case 20: //兑换结束日期升序
                    sqlSort = @"ROW_NUMBER() OVER ( ORDER BY EndDateTime  ) AS RowNumber ";
                    break;
                case 21: //兑换结束日期降序
                    sqlSort = @"ROW_NUMBER() OVER ( ORDER BY EndDateTime DESC ) AS RowNumber ";
                    break;
                case 30: //创建日期升序
                    sqlSort = @"ROW_NUMBER() OVER ( ORDER BY ImportDateTime ) AS RowNumber ";
                    break;
                case 31: //创建日期降序
                    sqlSort = @"ROW_NUMBER() OVER ( ORDER BY ImportDateTime DESC ) AS RowNumber ";
                    break;
                case 40: //领取时间升序
                    sqlSort = @"ROW_NUMBER() OVER ( ORDER BY GainDateTime ) AS RowNumber ";
                    break;
                case 41: //领取时间降序
                    sqlSort = @"ROW_NUMBER() OVER ( ORDER BY GainDateTime  DESC ) AS RowNumber ";
                    break;
            }

            #endregion

            var sqlParameters = new[]
            {
                new SqlParameter("@PageSize", serchElement.PageSize),
                new SqlParameter("@PageNumber", serchElement.PageNumber),
                new SqlParameter("@IsGain", serchElement.IsGain),
                new SqlParameter("@OutTime", serchElement.OutTime),
                new SqlParameter("@OnTime", serchElement.OnTime),
                new SqlParameter("@IsEnabled", serchElement.IsEnabled),
                new SqlParameter("@BatchId", serchElement.BatchId),
            };
            return
                SqlHelper.ExecuteDataTable(Connfig, CommandType.Text, sql1 + sqlSort + sql2, sqlParameters)
                    .ConvertTo<ThirdPartyExchangeCode>()
                    .ToList();
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
        /// <summary>
        /// 查询总行数
        /// </summary>
        /// <returns></returns>
        public static int SelectCount(SerchElement serchElement)
        {
            string sql = @"SELECT  COUNT(1)
                            FROM    Configuration.dbo.ThirdPartyCodeBatchConfig WITH ( NOLOCK )
                            WHERE   ( @BatchGuid IS  NULL--批次ID
                                      OR @BatchGuid = BatchGuid
                                    )
                                    AND ( @BatchName IS NULL--批次名称
                                          OR BatchName LIKE '%' + @BatchName + '%'
                                        )
                                    AND ( @Creator IS NULL--创建人
                                          OR Creator LIKE '%' + @Creator + '%'
                                        )
                                    AND ( @Modifier IS NULL--最近更新人
                                          OR Modifier LIKE '%' + @Modifier + '%'
                                        );";
            var sqlParameters = new[]
            {
                new SqlParameter("@PageSize", serchElement.PageSize),
                new SqlParameter("@PageNumber", serchElement.PageNumber),
                new SqlParameter("@BatchGuid", serchElement.BatchGuid),
                new SqlParameter("@Instructions", serchElement.Instructions),
                new SqlParameter("@BatchName", serchElement.BatchName),
                new SqlParameter("@Creator", serchElement.Creator),
                new SqlParameter("@Modifier", serchElement.Modifier)
            };
            return Convert.ToInt32(SqlHelper.ExecuteScalar(Connection, CommandType.Text, sql, sqlParameters));
                    
        }

        public static int UpdateQty(int pkid,int batchQty, int stockQty)
        {
            string sql = @"UPDATE Configuration.dbo.ThirdPartyCodeBatchConfig SET StockQty=@StockQty ,BatchQty=@BatchQty WHERE PKID=@PKID";
            var sqlParameters = new[]
           {
                new SqlParameter("@StockQty", stockQty),
                new SqlParameter("@BatchQty", batchQty),
                new SqlParameter("@PKID", pkid)
            };
            return SqlHelper.ExecuteNonQuery(Connfig, CommandType.Text, sql, sqlParameters);
        }
        public static int SortChange(int sort)
        {
            string sql = @"SELECT COUNT(1)  FROM Configuration..ThirdPartyMallConfig WITH ( NOLOCK) WHERE Sort=@sort";
            var sqlParameters = new[]
           {
              
                new SqlParameter("@sort", sort)
            };
            return Convert.ToInt32(SqlHelper.ExecuteScalar(Connection, CommandType.Text, sql, sqlParameters));
        }
     

        public static int UpdateExchangeCodeTime(DateTime StartDateTime, DateTime EndDateTime, Guid BatchGuid)
        {
            var sql = @"UPDATE Configuration..ThirdPartyExchangeCodeConfig SET StartDateTime=@StartDateTime ,EndDateTime=@EndDateTime WHERE BatchGuid=@BatchGuid ";
            var sqlParameters = new[]
           {
                new SqlParameter("@StartDateTime", StartDateTime),
                new SqlParameter("@EndDateTime", EndDateTime),
                new SqlParameter("@BatchGuid", BatchGuid)
            };
            return SqlHelper.ExecuteNonQuery(Connfig, CommandType.Text, sql, sqlParameters);
        }
    }
}
