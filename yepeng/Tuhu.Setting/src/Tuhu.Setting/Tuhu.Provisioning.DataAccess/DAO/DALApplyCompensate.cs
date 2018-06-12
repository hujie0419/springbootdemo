using Microsoft.ApplicationBlocks.Data;
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
    public class DALApplyCompensate
    {
        private static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static string connectionString = SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn;
        private static SqlConnection conn = new SqlConnection(connectionString);


        private static string strConnOnRead = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
        private static string connectionStringOnRead = SecurityHelp.IsBase64Formatted(strConnOnRead) ? SecurityHelp.DecryptAES(strConnOnRead) : strConnOnRead;
        private static SqlConnection connOnRead = new SqlConnection(strConnOnRead);


        public static List<ApplyCompensate> GetApplyCompensateList(ApplyCompensate model, int pageSize, int pageIndex, out int recordCount)
        {
            string sql = @"SELECT  *
                         FROM    ( SELECT [Id]
                                      ,[UserName]
                                      ,[PhoneNumber]
                                      ,[OrderId]
                                      ,[ProductName]
                                      ,[Link]
                                      ,[DifferencePrice]
                                      ,[Images]
                                      ,[ApplyTime]
                                      ,[AuditTime]
                                      ,[Status]
                                      ,OrderChannel
                                  FROM [Configuration].[dbo].[SE_ApplyCompensateConfig] WITH ( NOLOCK )
                                  WHERE      
                                        ( @UserName = ''
                                            OR ( @UserName <> ''
                                                AND UserName = @UserName
                                                )
                                        )
									    AND 
                                        ( @PhoneNumber = ''
                                            OR ( @PhoneNumber <> ''
                                                AND PhoneNumber = @PhoneNumber
                                                )
                                        )
									    AND 
                                        ( @ProductName = ''
                                            OR ( @ProductName <> ''
                                                AND ProductName = @ProductName
                                                )
                                        )
									    AND 
                                        ( @DifferencePrice = 0
                                            OR ( @DifferencePrice <> 0
                                                AND DifferencePrice = @DifferencePrice
                                                )
                                        )
									    AND 
                                        ( @OrderId = ''
                                            OR ( @OrderId <> ''
                                                AND OrderId = @OrderId
                                                )
                                        )
                                        AND 
                                        ( @Status = -1
                                            OR ( @Status <> -1
                                                AND Status = @Status
                                                )
                                        )
                                            AND
                                          ( @OrderChannel = ''
                                                OR ( @OrderChannel <> ''
                                                    AND OrderChannel = @OrderChannel
                                                    )
                                            )

                                            AND 
                                        ( @StartApplyTime = ''
                                            OR ( @StartApplyTime <> ''
                                                AND ApplyTime >= @StartApplyTime
                                                )
                                        )
                                            AND 
                                        ( @EndApplyTime = ''
                                            OR ( @EndApplyTime <> ''
                                                AND ApplyTime <= @EndApplyTime
                                                )
                                        )

                                            AND 
                                        ( @StartAuditTime = ''
                                            OR ( @StartAuditTime <> ''
                                                AND AuditTime >= @StartAuditTime
                                                )
                                        )
                                            AND 
                                        ( @EndAuditTime = ''
                                            OR ( @EndAuditTime <> ''
                                                AND AuditTime <= @EndAuditTime
                                                )
                                        )
                                ) AS PG
                          ORDER BY PG.ApplyTime DESC
                          OFFSET ( @PageIndex - 1 ) * @PageSize ROWS  FETCH NEXT @PageSize ROWS   ONLY ";
            string sqlCount = @"SELECT 
                                COUNT(1)
                                FROM [Configuration].[dbo].[SE_ApplyCompensateConfig] WITH (NOLOCK) 
                                WHERE  ( @UserName = ''
                                            OR ( @UserName <> ''
                                                AND UserName = @UserName
                                                )
                                       )
											AND 
                                        ( @PhoneNumber = ''
                                            OR ( @PhoneNumber <> ''
                                                AND PhoneNumber = @PhoneNumber
                                                )
                                        )
											AND 
                                        ( @ProductName = ''
                                            OR ( @ProductName <> ''
                                                AND ProductName = @ProductName
                                                )
                                        )
											AND 
                                        ( @DifferencePrice = 0
                                            OR ( @DifferencePrice <> 0
                                                AND DifferencePrice = @DifferencePrice
                                                )
                                        )
											AND 
                                        ( @OrderId = ''
                                            OR ( @OrderId <> ''
                                                AND OrderId = @OrderId
                                                )
                                        )
                                            AND 
                                        ( @Status = -1
                                            OR ( @Status <> -1
                                                AND Status = @Status
                                                )
                                        )
                                         
                                            AND
                                          ( @OrderChannel = ''
                                                OR ( @OrderChannel <> ''
                                                    AND OrderChannel = @OrderChannel
                                                    )
                                            )

                                            AND 
                                        ( @StartApplyTime = ''
                                            OR ( @StartApplyTime <> ''
                                                AND ApplyTime >= @StartApplyTime
                                                )
                                        )
                                            AND 
                                        ( @EndApplyTime = ''
                                            OR ( @EndApplyTime <> ''
                                                AND ApplyTime <= @EndApplyTime
                                                )
                                        )

                                            AND 
                                        ( @StartAuditTime = ''
                                            OR ( @StartAuditTime <> ''
                                                AND AuditTime >= @StartAuditTime
                                                )
                                        )
                                            AND 
                                        ( @EndAuditTime = ''
                                            OR ( @EndAuditTime <> ''
                                                AND AuditTime <= @EndAuditTime
                                                )
                                        )";
            var sqlParameters = new SqlParameter[]
                  {
                        new SqlParameter("@PageSize",pageSize),
                        new SqlParameter("@PageIndex",pageIndex),
                        new SqlParameter("@OrderId",model.OrderId??string.Empty),
                        new SqlParameter("@DifferencePrice",model.DifferencePrice),
                        new SqlParameter("@ProductName",model.ProductName??string.Empty),
                        new SqlParameter("@PhoneNumber",model.PhoneNumber??string.Empty),
                        new SqlParameter("@UserName",model.UserName??string.Empty),
                        new SqlParameter("@Status",model.Status),
                        new SqlParameter("@OrderChannel",model.OrderChannel??string.Empty),
                        new SqlParameter("@StartApplyTime",model.StartApplyTime.HasValue?model.StartApplyTime.Value.ToString():""),
                        new SqlParameter("@StartAuditTime",model.StartAuditTime.HasValue?model.StartAuditTime.Value.ToString():""),
                        new SqlParameter("@EndApplyTime",model.EndApplyTime.HasValue?model.EndApplyTime.Value.ToString():""),
                        new SqlParameter("@EndAuditTime",model.EndAuditTime.HasValue?model.EndAuditTime.Value.ToString():"")
                  };
            recordCount = (int)SqlHelper.ExecuteScalar(connOnRead, CommandType.Text, sqlCount, sqlParameters);

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<ApplyCompensate>().ToList();

        }

        public static List<ApplyCompensate> GetApplyCompensate(ApplyCompensate model)
        {

            const string sql = @"SELECT  *
                                 FROM    ( SELECT [Id]
                                              ,[UserName]
                                              ,[PhoneNumber]
                                              ,[OrderId]
                                              ,[ProductName]
                                              ,[Link]
                                              ,[DifferencePrice]
                                              ,[Images]
                                              ,[ApplyTime]
                                              ,[AuditTime]
                                              ,[Status]
                                              ,OrderChannel
                                          FROM [Configuration].[dbo].[SE_ApplyCompensateConfig] WITH ( NOLOCK )
                                          WHERE      
                                                ( @UserName = ''
                                                    OR ( @UserName <> ''
                                                        AND UserName = @UserName
                                                        )
                                                )
									            AND 
                                                ( @PhoneNumber = ''
                                                    OR ( @PhoneNumber <> ''
                                                        AND PhoneNumber = @PhoneNumber
                                                        )
                                                )
									            AND 
                                                ( @ProductName = ''
                                                    OR ( @ProductName <> ''
                                                        AND ProductName = @ProductName
                                                        )
                                                )
									            AND 
                                                ( @DifferencePrice = 0
                                                    OR ( @DifferencePrice <> 0
                                                        AND DifferencePrice = @DifferencePrice
                                                        )
                                                )
									            AND 
                                                ( @OrderId = ''
                                                    OR ( @OrderId <> ''
                                                        AND OrderId = @OrderId
                                                        )
                                                )
                                                AND 
                                                ( @Status = -1
                                                    OR ( @Status <> -1
                                                        AND Status = @Status
                                                        )
                                                )
                                                    AND
                                                  ( @OrderChannel = ''
                                                        OR ( @OrderChannel <> ''
                                                            AND OrderChannel = @OrderChannel
                                                            )
                                                    )

                                                    AND 
                                                ( @StartApplyTime = ''
                                                    OR ( @StartApplyTime <> ''
                                                        AND ApplyTime >= @StartApplyTime
                                                        )
                                                )
                                                    AND 
                                                ( @EndApplyTime = ''
                                                    OR ( @EndApplyTime <> ''
                                                        AND ApplyTime <= @EndApplyTime
                                                        )
                                                )

                                                    AND 
                                                ( @StartAuditTime = ''
                                                    OR ( @StartAuditTime <> ''
                                                        AND AuditTime >= @StartAuditTime
                                                        )
                                                )
                                                    AND 
                                                ( @EndAuditTime = ''
                                                    OR ( @EndAuditTime <> ''
                                                        AND AuditTime <= @EndAuditTime
                                                        )
                                                )
                                        ) AS PG
                                  ORDER BY PG.ApplyTime DESC";

            var sqlParameters = new SqlParameter[]
              {
                new SqlParameter("@OrderId",model.OrderId??string.Empty),
                new SqlParameter("@DifferencePrice",model.DifferencePrice),
                new SqlParameter("@ProductName",model.ProductName??string.Empty),
                new SqlParameter("@PhoneNumber",model.PhoneNumber??string.Empty),
                new SqlParameter("@UserName",model.UserName??string.Empty),
                new SqlParameter("@Status",model.Status),
                new SqlParameter("@OrderChannel",model.OrderChannel??string.Empty),
                new SqlParameter("@StartApplyTime",model.StartApplyTime.HasValue?model.StartApplyTime.Value.ToString():""),
                new SqlParameter("@StartAuditTime",model.StartAuditTime.HasValue?model.StartAuditTime.Value.ToString():""),
                new SqlParameter("@EndApplyTime",model.EndApplyTime.HasValue?model.EndApplyTime.Value.ToString():""),
                new SqlParameter("@EndAuditTime",model.EndAuditTime.HasValue?model.EndAuditTime.Value.ToString():"")
              };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<ApplyCompensate>().ToList();

        }

        public static ApplyCompensate GetApplyCompensate(int id)
        {

            const string sql = @"SELECT [Id]
                                      ,[UserName]
                                      ,[PhoneNumber]
                                      ,[OrderId]
                                      ,[ProductName]
                                      ,[Link]
                                      ,[DifferencePrice]
                                      ,[Images]
                                      ,[ApplyTime]
                                      ,[AuditTime]
                                      ,[Status]
                                      ,[OrderChannel]
                                  FROM [Configuration].[dbo].[SE_ApplyCompensateConfig] WITH ( NOLOCK )
                                  WHERE Id=@Id";

            var sqlParameters = new SqlParameter[]
            {
               new SqlParameter("@Id",id),

            };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<ApplyCompensate>().ToList().FirstOrDefault();

        }


        public static bool UpdateApplyCompensate(ApplyCompensate model)
        {

            const string sql = @"UPDATE [Configuration].[dbo].[SE_ApplyCompensateConfig] SET 
                                       [AuditTime]=GETDATE()
                                      ,[Status]=  @Status                        
                                  WHERE Id=@Id";

            var sqlParameters = new SqlParameter[]
            {
               new SqlParameter("@Id",model.Id),
               new SqlParameter("@Status",model.Status)

            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameters) > 0;
        }
    }
}
