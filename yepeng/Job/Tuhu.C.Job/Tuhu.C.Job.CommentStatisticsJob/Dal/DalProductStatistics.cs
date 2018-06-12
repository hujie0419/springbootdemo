using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.Job.CommentStatisticsJob.Model;

namespace Tuhu.C.Job.CommentStatisticsJob.Dal
{
    class DalProductStatistics
    {
        public static IEnumerable<ProductStatisticsModel> GetProductStatisticsByPage()
        {
            #region sql
            const string sql = @"WITH    T AS ( SELECT   CP.oid ,
                            CP.ProductID COLLATE Chinese_PRC_CI_AS AS ProductID ,
                            CP.VariantID ,
                            OL.PID ,
                            OL.Num
                   FROM     Gungnir..tbl_Order AS O WITH ( NOLOCK )
                            JOIN Gungnir..tbl_OrderList AS OL WITH ( NOLOCK ) ON O.PKID = OL.OrderID
                            JOIN Tuhu_productcatalog..CarPAR_CatalogProducts AS CP WITH ( NOLOCK ) ON OL.PID = CP.Pid2 COLLATE Chinese_PRC_CI_AS
                   WHERE    O.Status <> '7Canceled'
                            AND O.Status <> '0New'
                            AND OL.Deleted = 0
                            AND CP.i_ClassType IN ( 2 , 4 )
                 )
    SELECT  CP.ProductID ,
            CP.VariantID ,
            PS.OrderQuantity ,
            PS.SalesQuantity ,
            CS.CommentTimes ,
            CS.CommentR1 ,
            ( CASE WHEN PS.ProductID LIKE 'TR-%' THEN CS.CommentR2
                   ELSE NULL
              END ) AS CommentR2 ,
            ( CASE WHEN PS.ProductID LIKE 'TR-%' THEN CS.CommentR3
                   ELSE NULL
              END ) AS CommentR3 ,
            ( CASE WHEN PS.ProductID LIKE 'TR-%' THEN CS.CommentR4
                   ELSE NULL
              END ) AS CommentR4 ,
            ( CASE WHEN PS.ProductID LIKE 'TR-%' THEN CS.CommentR5
                   ELSE NULL
              END ) AS CommentR5 ,
            ( CASE WHEN PS.ProductID LIKE 'TR-%'
                   THEN 0.2 * ( CS.CommentR1 + CS.CommentR2 + CS.CommentR3
                                + CS.CommentR4 + CS.CommentR5 )
                   ELSE 1.0 * CS.CommentR1
              END ) / CS.CommentTimes AS CommentRate
    FROM    ( SELECT    T.ProductID ,
                        COUNT(1) AS OrderQuantity ,
                        SUM(T.Num) AS SalesQuantity
              FROM      T
              GROUP BY  T.ProductID
            ) AS PS
            LEFT JOIN ( SELECT  C.CommentProductFamilyId ,
                                COUNT(C.CommentId) AS CommentTimes ,
                                SUM(C.CommentR1) AS CommentR1 ,
                                SUM(C.CommentR2) AS CommentR2 ,
                                SUM(C.CommentR3) AS CommentR3 ,
                                SUM(C.CommentR4) AS CommentR4 ,
                                SUM(C.CommentR5) AS CommentR5
                        FROM    Gungnir..tbl_Comment AS C WITH ( NOLOCK )
                        WHERE   C.CommentR1 > 0
                                AND C.CommentStatus IN ( 2 , 3 )
                        GROUP BY C.CommentProductFamilyId
                      ) AS CS ON PS.ProductID = CS.CommentProductFamilyId
            JOIN Tuhu_productcatalog..CarPAR_CatalogProducts AS CP WITH ( NOLOCK ) ON PS.ProductID = CP.ProductID
                                                              AND CP.i_ClassType IN (
                                                              2 , 4 );
";
            #endregion
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandTimeout = 10 * 60;
                return DbHelper.ExecuteSelect<ProductStatisticsModel>(true, cmd);
            }

        }

        public static bool UpdateProductStatistics(ProductStatisticsModel model)
        {
            #region sql
            const string sql = @"  UPDATE [Tuhu_productcatalog].[dbo].[tbl_ProductStatistics] WITH(ROWLOCK)
  SET OrderQuantity=@OrderQuantity,
      SalesQuantity=@SalesQuantity,
	  CommentTimes=@CommentTimes,
	  CommentR1=@CommentR1,
	  CommentR2=@CommentR2,
	  CommentR3=@CommentR3,
	  CommentR4=@CommentR4,
	  CommentR5=@CommentR5,
	  CommentRate=@CommentRate
WHERE ProductID=@ProductID AND VariantID=@VariantID";
            #endregion
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@OrderQuantity", model.OrderQuantity);
                cmd.Parameters.AddWithValue("@SalesQuantity", model.SalesQuantity);
                cmd.Parameters.AddWithValue("@CommentTimes", model.CommentTimes);
                cmd.Parameters.AddWithValue("@CommentR1", model.CommentR1);
                cmd.Parameters.AddWithValue("@CommentR2", model.CommentR2);
                cmd.Parameters.AddWithValue("@CommentR3", model.CommentR3);
                cmd.Parameters.AddWithValue("@CommentR4", model.CommentR4);
                cmd.Parameters.AddWithValue("@CommentR5", model.CommentR5);
                cmd.Parameters.AddWithValue("@CommentRate", model.CommentRate);
                cmd.Parameters.AddWithValue("@ProductID", model.ProductID);
                cmd.Parameters.AddWithValue("@VariantID", model.VariantID);
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }

        }

        public static IEnumerable<ProductStatisticsModel> GetProductStatisticsByProductId(string pid)
        {
            #region sql
            const string sql = @"SELECT *  FROM [Tuhu_productcatalog].[dbo].[tbl_ProductStatistics] WITH(NOLOCK)
  WHERE ProductID=@ProductID";
            #endregion
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@ProductID", pid);
                return DbHelper.ExecuteSelect<ProductStatisticsModel>(cmd);
            }
        }

        public static bool InsertProductStatistics(ProductStatisticsModel model)
        {
            #region sql
            const string sql = @"Insert Into [Tuhu_productcatalog].[dbo].[tbl_ProductStatistics]
 ([ProductID]
,[VariantID]
,[OrderQuantity]
,[SalesQuantity]
,[CommentTimes]
,[CommentR1]
,[CommentR2]
,[CommentR3]
,[CommentR4]
,[CommentR5]
,[CommentRate])
Values(@ProductID,
@VariantID,
@OrderQuantity,
@SalesQuantity,
@CommentTimes,
@CommentR1,
@CommentR2,
@CommentR3,
@CommentR4,
@CommentR5,
@CommentRate);
";
            #endregion
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@OrderQuantity", model.OrderQuantity);
                cmd.Parameters.AddWithValue("@SalesQuantity", model.SalesQuantity);
                cmd.Parameters.AddWithValue("@CommentTimes", model.CommentTimes);
                cmd.Parameters.AddWithValue("@CommentR1", model.CommentR1);
                cmd.Parameters.AddWithValue("@CommentR2", model.CommentR2);
                cmd.Parameters.AddWithValue("@CommentR3", model.CommentR3);
                cmd.Parameters.AddWithValue("@CommentR4", model.CommentR4);
                cmd.Parameters.AddWithValue("@CommentR5", model.CommentR5);
                cmd.Parameters.AddWithValue("@CommentRate", model.CommentRate);
                cmd.Parameters.AddWithValue("@ProductID", model.ProductID);
                cmd.Parameters.AddWithValue("@VariantID", model.VariantID);
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }

        }

        public static bool UpdatetCarPAR_CatalogProducts()
        {
            #region sql
            const string sql = @"UPDATE	CP
SET		CP_RateNumber = PS.OrderQuantity,
		CP_RateSource = PS.SalesQuantity
FROM		Tuhu_productcatalog..CarPAR_CatalogProducts AS CP WITH ( NOLOCK )
JOIN		Tuhu_productcatalog..tbl_ProductStatistics AS PS
				ON PS.ProductID = CP.ProductID
				   AND PS.VariantID = CP.VariantID;";
            #endregion
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandTimeout = 5 * 60;
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }

        }


        public static bool MergeIntoProductStatistics(List<ProductStatisticsModel> models)
        {
            if (models == null || !models.Any())
                return true;

            #region sql
            var sql = @"MERGE [Tuhu_productcatalog].[dbo].[tbl_ProductStatistics] WITH ( ROWLOCK )  AS A
USING
    ( {models}
    ) AS B
ON A.ProductID = B.ProductID
    AND A.VariantID = B.VariantID
WHEN NOT MATCHED THEN
    INSERT ( ProductID ,
             VariantID ,
             OrderQuantity ,
             SalesQuantity ,
             CommentTimes ,
             CommentR1 ,
             CommentR2 ,
             CommentR3 ,
             CommentR4 ,
             CommentR5 ,
             CommentRate
           )
    VALUES ( B.ProductID ,
             B.VariantID ,
             B.OrderQuantity ,
             B.SalesQuantity ,
             B.CommentTimes ,
             B.CommentR1 ,
             B.CommentR2 ,
             B.CommentR3 ,
             B.CommentR4 ,
             B.CommentR5 ,
             B.CommentRate
           )
WHEN MATCHED THEN
    UPDATE SET OrderQuantity = B.OrderQuantity ,
               SalesQuantity = B.SalesQuantity ,
               CommentTimes = B.CommentTimes ,
               CommentR1 = B.CommentR1 ,
               CommentR2 = B.CommentR2 ,
               CommentR3 = B.CommentR3 ,
               CommentR4 = B.CommentR4 ,
               CommentR5 = B.CommentR5 ,
               CommentRate = B.CommentRate;";
            #endregion
            using (var cmd = new SqlCommand())
            {
                var modelSqlList = new List<string>();
                models.ForEach(p => {
                    modelSqlList.Add($@"
SELECT    N'{p.ProductID}' ProductID , N'{p.VariantID}' VariantID , N'{p.OrderQuantity}' OrderQuantity , N'{p.SalesQuantity}' SalesQuantity ,
          N'{p.CommentTimes}' CommentTimes , N'{p.CommentR1}' CommentR1 , N'{p.CommentR2}' CommentR2 , N'{p.CommentR3}' CommentR3 , N'{p.CommentR4}' CommentR4 , N'{p.CommentR5}' CommentR5 , N'{p.CommentRate}' CommentRate");
                });
                cmd.CommandText = sql.Replace("{models}", string.Join(" UNION ALL ", modelSqlList));
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }
    }
}
