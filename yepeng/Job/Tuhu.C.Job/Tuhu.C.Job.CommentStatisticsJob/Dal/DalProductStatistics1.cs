using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.Job.CommentStatisticsJob.Model;
using Tuhu.Service.Config;

namespace Tuhu.C.Job.CommentStatisticsJob.Dal
{
    class DalProductStatistics1
    {

        private static string DBName_ProductStatistics = "tbl_ProductStatistics";

        public static IEnumerable<ProductStatisticsModel> GetProductStatisticsByPage()
        {
            //产品 好评 的 规则   评分R1>=4.0  
            //产品 默认好评 的 规则      时间 6个月 统一变量管理
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
	        CS.FavourableCount ,
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
                                SUM(C.CommentR5) AS CommentR5 ,
                                sum(case when C.CommentR1>4.0 then 1 else 0 end )  as FavourableCount
                        FROM    Gungnir..tbl_Comment AS C WITH ( NOLOCK )
                        WHERE   C.CommentR1 > 0
                                AND C.CommentStatus IN ( 2 , 3 )
                        GROUP BY C.CommentProductFamilyId
                      ) AS CS ON PS.ProductID = CS.CommentProductFamilyId
            JOIN Tuhu_productcatalog..CarPAR_CatalogProducts AS CP WITH ( NOLOCK ) ON PS.ProductID = CP.ProductID
                                                              AND CP.i_ClassType IN (2 , 4 );
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
            string DBName = GetSwitchValue() ? DBName_ProductStatistics + "_temp" : DBName_ProductStatistics;
            #region sql
            string sql = @"UPDATE [Tuhu_productcatalog].[dbo].[" + DBName + @"] WITH(ROWLOCK)
                          SET OrderQuantity=@OrderQuantity,
                              SalesQuantity=@SalesQuantity,
	                          CommentTimes=@CommentTimes,
	                          CommentR1=@CommentR1,
	                          CommentR2=@CommentR2,
	                          CommentR3=@CommentR3,
	                          CommentR4=@CommentR4,
	                          CommentR5=@CommentR5,
	                          CommentRate=@CommentRate,
                            FavourableCount=@FavourableCount,
                            DefaultFavourableCount=@DefaultFavourableCount,
                            Score=@Score,
                            CommentTimesB=@CommentTimesB,
                            CommentR1B=@CommentR1B,
                            CommentR2B=@CommentR2B,
                            CommentR3B=@CommentR3B,
                            CommentR4B=@CommentR4B,
                            CommentR5B=@CommentR5B
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

                cmd.Parameters.AddWithValue("@FavourableCount", model.FavourableCount);
                cmd.Parameters.AddWithValue("@DefaultFavourableCount", model.DefaultFavourableCount);
                cmd.Parameters.AddWithValue("@Score", model.Score);
                cmd.Parameters.AddWithValue("@CommentTimesB", model.CommentTimesB);
                cmd.Parameters.AddWithValue("@CommentR1B", model.CommentR1B);
                cmd.Parameters.AddWithValue("@CommentR2B", model.CommentR2B);
                cmd.Parameters.AddWithValue("@CommentR3B", model.CommentR3B);
                cmd.Parameters.AddWithValue("@CommentR4B", model.CommentR4B);
                cmd.Parameters.AddWithValue("@CommentR5B", model.CommentR5B);

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


        /// <summary>
        /// 获取 所以产品的 默认好评数
        /// </summary>
        /// <returns></returns>
        public static List<ProductCommentStatusModel> GetAllProductDefaultFavourableStatistics()
        {
            List<ProductCommentStatusModel> data = new List<ProductCommentStatusModel>();

            string sql = @"SELECT ProductId, COUNT(1) as DefaultFavourableCount FROM Tuhu_comment..ProductCommentStatus WITH (NOLOCK)
WHERE CanComment = 1 AND CreateDateTime<DATEADD(MONTH, 6, GETDATE()) and ProductId not like 'temp-%'
GROUP BY ProductId";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandTimeout = 10 * 60;
                cmd.Parameters.AddWithValue("@month", -1 * CommonUtilsModel.CanComment);
                var temp = DbHelper.ExecuteSelect<ProductCommentStatusModel>(cmd).ToList();
                data.AddRange(temp);
                return data;
            }

            ////查询总数
            //int productIdCount = QueryProductIdCount();
            //int pageSize = 100;
            //int pageCount = (productIdCount - 1) / pageSize + 1;
            //for (int pageIndex = 1; pageIndex <= pageCount; pageIndex++)
            //{
            //    List<string> ProductIds = QueryProductIdListByPage(pageIndex,pageSize);
            //    data.AddRange(QueryByProductIds(ProductIds));
            //}
            //return data;
        }



        public static bool InsertProductStatistics(ProductStatisticsModel model)
        {

            string DBName = GetSwitchValue() ? DBName_ProductStatistics + "_temp" : DBName_ProductStatistics;
            #region sql
            string sql = @"Insert Into [Tuhu_productcatalog].[dbo].[" + DBName + @"]
(ProductID,VariantID,OrderQuantity,SalesQuantity,CommentTimes,CommentR1,CommentR2,CommentR3,CommentR4,CommentR5,CommentRate,
FavourableCount,DefaultFavourableCount,Score,CommentTimesB,CommentR1B,CommentR2B,CommentR3B,CommentR4B,CommentR5B)
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
@CommentRate,
@FavourableCount,@DefaultFavourableCount,@Score,@CommentTimesB,@CommentR1B，@CommentR2B，@CommentR3B，@CommentR4B，@CommentR5B
);
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

                cmd.Parameters.AddWithValue("@FavourableCount", model.FavourableCount);
                cmd.Parameters.AddWithValue("@DefaultFavourableCount", model.DefaultFavourableCount);
                cmd.Parameters.AddWithValue("@Score", model.Score);
                cmd.Parameters.AddWithValue("@CommentTimesB", model.CommentTimesB);
                cmd.Parameters.AddWithValue("@CommentR1B", model.CommentR1B);
                cmd.Parameters.AddWithValue("@CommentR2B", model.CommentR2B);
                cmd.Parameters.AddWithValue("@CommentR3B", model.CommentR3B);
                cmd.Parameters.AddWithValue("@CommentR4B", model.CommentR4B);
                cmd.Parameters.AddWithValue("@CommentR5B", model.CommentR5B);

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


        public static bool MergeIntoProductStatistics(List<ProductStatisticsModel> models, IEnumerable<ProductDefaultFavourableStatisticsModel> defaultFavourableStatisticsList)
        {
            if (models == null || !models.Any())
                return true;

            string DBName = GetSwitchValue() ? DBName_ProductStatistics + "_temp" : DBName_ProductStatistics;
            #region sql
            var sql = @"MERGE [Tuhu_productcatalog].[dbo].[" + DBName + @"] WITH ( ROWLOCK )  AS A
USING
    ( {models}
    ) AS B
ON A.ProductID = B.ProductID
    AND A.VariantID = B.VariantID
WHEN NOT MATCHED THEN
    INSERT ( ProductID ,VariantID ,OrderQuantity , SalesQuantity ,CommentTimes ,CommentR1 , CommentR2 , CommentR3 ,CommentR4 ,CommentR5 , CommentRate ,
             FavourableCount ,DefaultFavourableCount ,Score,CommentTimesB,CommentR1B,CommentR2B,CommentR3B,CommentR4B,CommentR5B,CreateTime
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
             B.CommentRate ,
             B.FavourableCount ,B.DefaultFavourableCount ,B.Score,B.CommentTimesB,B.CommentR1B,B.CommentR2B,B.CommentR3B,B.CommentR4B,B.CommentR5B,getdate()
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
               CommentRate = B.CommentRate ,
                FavourableCount = B.FavourableCount ,
                DefaultFavourableCount = B.DefaultFavourableCount ,
                Score = B.Score,
                CommentTimesB=B.CommentTimesB,
                CommentR1B=B.CommentR1B,
                CommentR2B=B.CommentR2B,
                CommentR3B=B.CommentR3B,
                CommentR4B=B.CommentR4B,
                CommentR5B=B.CommentR5B,
                LastUpdateTime=getdate()
;";
            #endregion
            using (var cmd = new SqlCommand())
            {
                var modelSqlList = new List<string>();
                models.ForEach(p => {
                    //获取产品  默认好评个数
                    ProductDefaultFavourableStatisticsModel _ProductDefaultFavourableStatisticsModel = defaultFavourableStatisticsList.Where(l => l.ProductFamilyId == p.ProductID).FirstOrDefault() ?? new ProductDefaultFavourableStatisticsModel();
                    p.DefaultFavourableCount = _ProductDefaultFavourableStatisticsModel.DefaultFavourableCount;

                    

                    //字段 备份
                    p.CommentTimesB = p.CommentTimes;
                    p.CommentR1B = p.CommentR1;
                    p.CommentR2B = p.CommentR2;
                    p.CommentR3B = p.CommentR3;
                    p.CommentR4B = p.CommentR4;
                    p.CommentR5B = p.CommentR5;

                    p.Score = p.CommentRate;

                    //重新赋值
                    p.CommentTimes = p.CommentTimes + p.DefaultFavourableCount;
                    p.CommentR1 = p.CommentR1 + p.DefaultFavourableCount * 5;
                    p.CommentR2 = p.CommentR2 + p.DefaultFavourableCount * 5;
                    p.CommentR3 = p.CommentR3 + p.DefaultFavourableCount * 5;
                    p.CommentR4 = p.CommentR4 + p.DefaultFavourableCount * 5;
                    p.CommentR5 = p.CommentR5 + p.DefaultFavourableCount * 5;

                    //产品的平均分（带上默认好评的） score 的分数需要重新 计算
                    if (p.CommentTimes > 0)
                    {
                        if (p.ProductID.ToUpper().IndexOf("TR") == 0)//轮胎算法
                        {
                            p.CommentRate = float.Parse(((p.CommentR1 + p.CommentR2 + p.CommentR3 + p.CommentR4 + p.CommentR5) / (p.CommentTimes * 5.0)).ToString("#0.00000"));
                        }
                        else
                        {
                            p.CommentRate = float.Parse((p.CommentR1 / (p.CommentTimes * 1.0)).ToString("#0.00000"));
                        }
                    }


                    modelSqlList.Add($@"
                    SELECT    N'{p.ProductID}' ProductID , N'{p.VariantID}' VariantID , N'{p.OrderQuantity}' OrderQuantity , N'{p.SalesQuantity}' SalesQuantity ,
                              N'{p.CommentTimes}' CommentTimes , N'{p.CommentR1}' CommentR1 , N'{p.CommentR2}' CommentR2 , N'{p.CommentR3}' CommentR3 ,
                              N'{p.CommentR4}' CommentR4 , N'{p.CommentR5}' CommentR5 , N'{p.CommentRate}' CommentRate,
                              N'{p.FavourableCount}' FavourableCount , N'{p.DefaultFavourableCount}' DefaultFavourableCount , N'{p.Score}' Score ,
                              N'{p.CommentTimesB}' CommentTimesB , N'{p.CommentR1B}' CommentR1B , N'{p.CommentR2B}' CommentR2B ,
                              N'{p.CommentR3B}' CommentR3B , N'{p.CommentR4B}' CommentR4B , N'{p.CommentR5B}' CommentR5B 
                    ");
                });
                cmd.CommandText = sql.Replace("{models}", string.Join(" UNION ALL ", modelSqlList));
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        #region 内部方法
        private static bool GetSwitchValue()
        {
            using (var client = new ConfigClient())
            {
                var result = client.GetOrSetRuntimeSwitch("UpdateProductStatisticsIsTest");
                return result.Success ? result.Result.Value : false;
            }

        }

        #endregion



    }
}
