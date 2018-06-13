using System;
using Microsoft.ApplicationBlocks.Data;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DALActivity
    {
        public static Tuple<int, List<CouponActivity>> GetActivityListForApp(SqlConnection conn, int pageSize, int pageIndex)
        {
            int totalCount;
            var parameters = new[]
            {
                new SqlParameter("@PageIndex",pageIndex),
                new SqlParameter("@PageSize",pageSize),
                new SqlParameter("@TotalCount",SqlDbType.Int) { Direction=ParameterDirection.Output}
            };
            var sql = @"DECLARE @CountId INT;
DECLARE @CountKey INT;
SELECT  @CountId = COUNT(DISTINCT ActivityId)
FROM    PromotionActivityConfig WITH ( NOLOCK )
WHERE   IsDeleted = 0
        AND ActivityId IS NOT NULL;	

SELECT  @CountKey = COUNT(DISTINCT ActivityKey)
FROM    dbo.PromotionActivityConfig WITH ( NOLOCK )
WHERE   IsDeleted = 0
        AND ActivityId IS NULL
        AND ActivityKey IS NOT NULL;

SELECT  @TotalCount = @CountId + @CountKey;

SELECT  *
FROM    ( SELECT    * ,
                    ROW_NUMBER() OVER ( ORDER BY PKID DESC ) AS rownum
          FROM      ( SELECT    pac.PKID ,
                                pac.ActivityId ,
                                pac.SmallImageUrl ,
                                pac.ActivityKey ,
                                ROW_NUMBER() OVER ( PARTITION BY pac.ActivityKey ORDER BY pac.CreateTime DESC ) rown
                      FROM      PromotionActivityConfig pac WITH ( NOLOCK )
                      WHERE     IsDeleted = 0
                                AND pac.ActivityId IS NULL
                                AND pac.ActivityKey IS NOT NULL
                      UNION
                      SELECT    pac.PKID ,
                                pac.ActivityId ,
                                pac.SmallImageUrl ,
                                pac.ActivityKey ,
                                ROW_NUMBER() OVER ( PARTITION BY pac.ActivityId ORDER BY pac.CreateTime DESC ) rown
                      FROM      PromotionActivityConfig pac WITH ( NOLOCK )
                      WHERE     IsDeleted = 0
                                AND pac.ActivityId IS NOT NULL
                    ) T
          WHERE     T.rown = 1
        ) Y
WHERE   Y.rownum BETWEEN ( @PageIndex - 1 ) * @PageSize + 1
                 AND     @PageIndex * @PageSize;";
            var activityList =
                SqlHelper.ExecuteDataTable(conn, CommandType.Text,
                sql, parameters)
                .AsEnumerable()
                .Select(x => new CouponActivity()
                {
                    ActivityId = x.IsNull(1) ? -1 : Convert.ToInt32(x[1]),
                    SmallImageUrl = (string)(x["SmallImageUrl"] ?? string.Empty),
                    ActivityKey = x.IsNull(3) ? Guid.Empty : Guid.Parse(x[3].ToString())
                }).ToList();

            int.TryParse(parameters.LastOrDefault().Value.ToString(), out totalCount);

            Tuple<int, List<CouponActivity>> result = new Tuple<int, List<CouponActivity>>(totalCount, activityList);

            return result;
        }

        public static Tuple<int, List<CouponActivity>> GetActivityListForWeb(SqlConnection conn, int pageSize, int pageIndex)
        {
            int totalCount;

            var parameters = new[]
            {
                new SqlParameter("@PageIndex",pageIndex),
                new SqlParameter("@PageSize",pageSize),
                new SqlParameter("@TotalCount",SqlDbType.Int) { Direction=ParameterDirection.Output}
            };
            var sql = @"DECLARE @CountId INT;
DECLARE @CountKey INT;

SELECT  @CountId = COUNT(DISTINCT ActivityId)
FROM    dbo.PromotionActivityWebConfig WITH ( NOLOCK )
WHERE   IsDeleted = 0
        AND ActivityId IS NOT NULL;

SELECT  @CountKey = COUNT(DISTINCT ActivityKey)
FROM    dbo.PromotionActivityWebConfig WITH ( NOLOCK )
WHERE   IsDeleted = 0
        AND ActivityId IS NULL
        AND ActivityKey IS NOT NULL;

SELECT  @TotalCount = @CountId + @CountKey;

SELECT  *
FROM    ( SELECT    * ,
                    ROW_NUMBER() OVER ( ORDER BY T.PKID DESC ) AS rownum
          FROM      ( SELECT    paw.PKID ,
                                paw.ActivityId ,
                                paw.SmallBannerImageUrl AS SmallImageUrl ,
                                paw.ActivityKey ,
                                ROW_NUMBER() OVER ( PARTITION BY paw.ActivityKey ORDER BY paw.CreateTime DESC ) rown
                      FROM      PromotionActivityWebConfig paw WITH ( NOLOCK )
                      WHERE     IsDeleted = 0
                                AND paw.ActivityId IS NULL
                                AND paw.ActivityKey IS NOT NULL
                      UNION
                      SELECT    paw.PKID ,
                                paw.ActivityId ,
                                paw.SmallBannerImageUrl AS SmallImageUrl ,
                                paw.ActivityKey ,
                                ROW_NUMBER() OVER ( PARTITION BY paw.ActivityId ORDER BY paw.CreateTime DESC ) rown
                      FROM      PromotionActivityWebConfig paw WITH ( NOLOCK )
                      WHERE     IsDeleted = 0
                                AND paw.ActivityId IS NOT NULL
                    ) T
          WHERE     T.rown = 1
        ) Y
WHERE   Y.rownum BETWEEN ( @PageIndex - 1 ) * @PageSize + 1
                 AND     @PageIndex * @PageSize;";
            var activityList =
                SqlHelper.ExecuteDataTable(conn, CommandType.Text,
                sql, parameters)
                .AsEnumerable()
                .Select(x => new CouponActivity()
                {
                    ActivityId = x.IsNull(1) ? -1 : Convert.ToInt32(x[1]),
                    SmallImageUrl = (string)(x["SmallImageUrl"] ?? string.Empty),
                    ActivityKey = x.IsNull(3) ? Guid.Empty : Guid.Parse(x[3].ToString())
                }).ToList();

            int.TryParse(parameters.LastOrDefault().Value.ToString(), out totalCount);

            Tuple<int, List<CouponActivity>> result = new Tuple<int, List<CouponActivity>>(totalCount, activityList);

            return result;
        }


        public static bool DeleteActivityConfig(SqlConnection conn, string type, string id, string userName)
        {
            var parameters = new[]
            {
                new SqlParameter("@Type",type),
                new SqlParameter("@Id",id),
                new SqlParameter("@UserName",userName)
            };

            return SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "[Gungnir].[dbo].[Activity_DeleteActivityConfig]", parameters) > 0;
        }
    }
}
