using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.Job.CommentStatisticsJob.Model;
using Tuhu.Service.Comment.Models;

namespace Tuhu.C.Job.CommentStatisticsJob.Dal
{
    class DalProductStatistics
    {

        public static bool UpdateShowCommentTimes(Dictionary<string, int> datas)
        {
            bool issuccess = true;
            foreach (var item in datas.Split(10))
            {
                string sqlformat = "UPDATE [Tuhu_productcatalog].[dbo].[tbl_ProductStatistics] WITH(ROWLOCK) SET ShowCommentTimes={0} WHERE ProductID='{1}'";
                var sqls = string.Join(";", item.Select(x => string.Format(sqlformat, x.Value, x.Key)));
                using (var helper = DbHelper.CreateDbHelper(false))
                {
                    var result = helper.ExecuteNonQuery(sqls);
                    issuccess &= result >= 0;
                }
            }
            return issuccess;
        }
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

        /// <summary>
        /// 获取评论的最大id
        /// </summary>
        /// <returns></returns>
        public static int  SelectCommentsMaxID()
        {
            using (var cmd = new SqlCommand(CommentSql_MaxID))
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 120;
                var result = DbHelper.ExecuteScalar(true, cmd);
                int.TryParse(result?.ToString(), out var value);
                return value;
            }
        }

        /// <summary>
        /// 批量 获取 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static async Task<EsProductCommentModel[]> SelectCommentsByPage(int pageIndex, int pageSize)
        {
            string sql = string.Format(CommentSql, (pageIndex-1)*pageSize, pageIndex*pageSize);
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 120;
                return await DbHelper.ExecuteSelectAsync<EsProductCommentModel>(true, cmd)
                    .ContinueWith(o => o.Result.ToArray());
            }
        }
        #region 追评相关 查询

        /// <summary>
        /// 查询 追评的 总数
        /// </summary>
        /// <returns></returns>
        public static int SelectAdditionCommentCount()
        {
            using (var cmd = new SqlCommand(AdditionCommentSql_Count))
            {
                var result = DbHelper.ExecuteScalar(true, cmd);
                int.TryParse(result?.ToString(), out var value);
                return value;
            }
        }

       /// <summary>
       /// 分页获取 追评
       /// </summary>
       /// <param name="pageIndex"></param>
       /// <param name="pageSize"></param>
       /// <returns></returns>
        public static List<int> SelectAdditionComment_Page(int pageIndex,int pageSize)
        {
            using (var cmd = new SqlCommand(AdditionCommentSql_Page))
            {
                cmd.Parameters.AddWithValue("pageIndex", pageIndex);
                cmd.AddParameter("pageSize", pageSize);

                List<int> result = DbHelper.ExecuteQuery(true, cmd, dt => dt.ToList<int>())?.ToList()??new List<int>() ;
                return result;
            }
        }

        #endregion


        #region Sql
        /// <summary>
        /// 查下评论的 详情
        /// </summary>
        private static readonly string CommentSql = @"SELECT
		C.CommentId AS Id,
		C.CommentUserId AS UserId,
		C.CommentUserName AS UserName,
		C.CommentContent AS Content,
		C.CommentImages AS Images,
		C.CommentStatus AS Status,
		C.CommentProductId AS ProductId,
		C.CommentProductFamilyId AS ProductFamilyId,
		C.CommentOrderId AS OrderId,
		C.CommentOrderListId AS OrderListId,
		C.CommentType AS Type,
		C.CommentChannel AS Channel,
		C.CommentExtAttr AS Attribute,
		C.CreateTime,
		C.UpdateTime,
        C.TotalPraise,
		C.CommentR1,
		C.CommentR2,
		C.CommentR3,
		C.CommentR4,
		C.CommentR5,
		C.CommentR6,
		C.InstallShopID,
		C.ParentComment AS ParentId,
		C.OfficialReply,
		C.SingleTitle,
        AC.AdditionCommentId,
        AC.AdditionCommentContent,
        AC.AdditionCommentImages,
        AC.AdditionCommentStatus,
        AC.CreateTime AS AdditionCommentTime,
		O.OrderDatetime,
		P.RegionName AS Province,
		R.RegionName AS City,
        car.VehicleID,
        car.PaiLiang ,
        car.Nian ,
        O.InstallType
FROM	Gungnir..tbl_Comment AS C WITH ( NOLOCK )
LEFT JOIN Gungnir..tbl_AdditionComment AS AC WITH ( NOLOCK ) ON C.CommentId=AC.CommentId
LEFT JOIN	Gungnir..tbl_Order AS O WITH ( NOLOCK ) ON C.CommentOrderId = O.PKID
LEFT JOIN Tuhu_order..tbl_OrderCar AS car WITH ( NOLOCK ) ON O.OrderNo = car.OrderNo COLLATE Chinese_PRC_CI_AS
LEFT JOIN Gungnir..tbl_Region AS R WITH ( NOLOCK ) ON O.RegionID = R.PKID
LEFT JOIN Gungnir..tbl_Region AS P WITH ( NOLOCK ) ON R.ParentID = P.PKID
WHERE	C.CommentProductFamilyId IS NOT NULL
		AND ( C.CommentType <> 1 OR C.CommentType = 1 AND C.InstallShopID IS NULL )
		AND C.ParentComment IS NULL
		AND C.CommentStatus = 2
		AND C.CommentR1 > 0 
		AND C.CommentId >{0}
        AND C.CommentId <={1}
        AND (AC.IsDel is null OR AC.IsDel=0)
		--AND C.CreateTime>'2017-06-01'
        --ORDER BY C.CommentId";

        /// <summary>
        ///获取评论的最大id
        /// </summary>
        private static readonly string CommentSql_MaxID = @"SELECT  max(c.CommentId)
                                                            FROM	Gungnir..tbl_Comment AS C WITH ( NOLOCK );";

        /// 追评 总数
        private static readonly string AdditionCommentSql_Count = @"SELECT   count(1)
                                                                from Gungnir..tbl_AdditionComment WITH(NOLOCK)
                                                                where AdditionCommentStatus =2 and  IsDel = 0 and CommentId >0;";
        //分页 获取 原评 id
        private static readonly string AdditionCommentSql_Page = @"SELECT   CommentId
                                                                from Gungnir..tbl_AdditionComment WITH(NOLOCK)
                                                                where AdditionCommentStatus =2 and  IsDel = 0 and CommentId >0
                                                                order by AdditionCommentId asc
                                                                OFFSET (@pageIndex - 1) * @pageSize ROW
                                                                FETCH NEXT @pageSize Row only";

        #endregion

    }
}
