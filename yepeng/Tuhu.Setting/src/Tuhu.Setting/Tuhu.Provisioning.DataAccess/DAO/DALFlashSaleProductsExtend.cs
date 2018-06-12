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
    public class DALFlashSaleProductsExtend
    {
        private static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static string connectionString = SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn;
        private static SqlConnection conn = new SqlConnection(connectionString);


        private static string strConnOnRead = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
        private static string connectionStringOnRead = SecurityHelp.IsBase64Formatted(strConnOnRead) ? SecurityHelp.DecryptAES(strConnOnRead) : strConnOnRead;
        private static SqlConnection connOnRead = new SqlConnection(strConnOnRead);


        public static List<FlashSaleProductsExtend> GetFlashSaleProductsExtendList(string sqlStr, int pageSize, int pageIndex, string orderBy, out int recordCount)
        {
            string sql = @" 
                            SELECT  *
                            FROM    ( SELECT    ROW_NUMBER() OVER ( ORDER BY " + orderBy + @" ) AS ROWNUMBER ,
                                                A.[ActivityID] ,
                                                A.[PID] ,
                                                [Price] ,
                                                B.StartDateTime ,
                                                B.EndDateTime ,
                                                ISNULL([ProductName], D.DisplayName) AS ProductName ,
                                                [IsUsePCode] ,
                                                A.PKID ,
                                                B.ActivityName ,
                                                C.cost AS Costing ,
                                                C.salenum AS MonthSales ,
                                                ISNULL(CONVERT(NUMERIC(8, 2), ROUND(( ( Price - C.cost )
                                                                                      / ( CASE
                                                                                          WHEN Price = 0
                                                                                          THEN 1
                                                                                          ELSE Price
                                                                                          END ) ) * 100, 2)),
                                                       100) AS GrossProfitMargin
                                      FROM      [Activity].[dbo].[tbl_FlashSaleProducts] AS A WITH ( NOLOCK )
                                                INNER JOIN Activity..tbl_FlashSale AS B WITH ( NOLOCK ) ON B.ActivityID = A.ActivityID
                                                                                          AND B.EndDateTime > GETDATE()
                                                INNER JOIN Tuhu_productcatalog.[dbo].[CarPAR_zh-CN] AS D
                                                WITH ( NOLOCK ) ON D.PID = A.PID
                                                LEFT JOIN ProductCostSalenum AS C WITH ( NOLOCK ) ON A.PID = C.PID COLLATE Chinese_PRC_CI_AS
                                      WHERE     1 = 1 " + sqlStr + @"
                                    ) AS PG
                            WHERE   PG.ROWNUMBER BETWEEN STR(( @PageIndex - 1 ) * @PageSize + 1)
                                                 AND     STR(@PageIndex * @PageSize)";
            string sqlCount = @"SELECT COUNT(1)    FROM    [Activity].[dbo].[tbl_FlashSaleProducts] AS A  WITH(NOLOCK)
                                INNER JOIN Activity..tbl_FlashSale AS B  WITH(NOLOCK) ON B.ActivityID = A.ActivityID
                                                                            AND B.EndDateTime > GETDATE()
							    INNER JOIN Tuhu_productcatalog.[dbo].[CarPAR_zh-CN] AS D WITH (NOLOCK) ON  D.PID=A.PID

	                            LEFT JOIN ProductCostSalenum AS C  WITH(NOLOCK) ON A.PID= C.PID COLLATE Chinese_PRC_CI_AS                            
                                WHERE 1=1  " + sqlStr;
            recordCount = (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, sqlCount);

            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@strOrder",orderBy),
                new SqlParameter("@PageSize",pageSize),
                new SqlParameter("@PageIndex",pageIndex)
            };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<FlashSaleProductsExtend>().ToList();

        }

        public static bool UpdateFlashSaleProductsIsUsePCode(FlashSaleProductsExtend model)
        {
            const string sql = @"UPDATE [Activity].[dbo].[tbl_FlashSaleProducts] SET IsUsePCode=@IsUsePCode ,LastUpdateDateTime=GETDATE()
                                WHERE PKID=@PKID";
            var sqlParameter = new SqlParameter[]
               {
                    new SqlParameter("@PKID",model.PKID),
                    new SqlParameter("@IsUsePCode",model.IsUsePCode),

               };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameter) > 0;
        }
    }
}
