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
    public class DalVehicleArticle
    {
        /// <summary>
        /// 添加选车攻略配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int AddVehicleArticle(SqlConnection conn, VehicleArticleModel model)
        {
            #region SQL
            var sql = @"INSERT  INTO Configuration..VehicleArticle
                                ( VehicleId ,
                                  PaiLiang ,
                                  Nian ,
                                  ArticleUrl
                                )
                        VALUES  ( @VehicleId ,
                                  @PaiLiang ,
                                  @Nian ,
                                  @ArticleUrl
                                );
                        SELECT  SCOPE_IDENTITY();";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@VehicleId", model.VehicleId),
                new SqlParameter("@PaiLiang", model.PaiLiang ),
                new SqlParameter("@Nian", model.Nian),
                new SqlParameter("@ArticleUrl", model.ArticleUrl)
            };
            return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters));
        }

        /// <summary>
        /// 删除选车攻略配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static bool DeleteVehicleArticle(SqlConnection conn, int pkid)
        {
            #region SQL
            var sql = @"DELETE  Configuration..VehicleArticle
                        WHERE   PKID = @PKID;";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@PKID", pkid)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
        }

        /// <summary>
        /// 更新选车攻略配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool UpdateVehicleArticle(SqlConnection conn, VehicleArticleModel model)
        {
            #region Sql
            var sql = @"UPDATE  Configuration..VehicleArticle
                        SET     VehicleId = @VehicleId ,
                                PaiLiang = @PaiLiang ,
                                Nian = @Nian ,
                                ArticleUrl = @ArticleUrl ,
                                LastUpdateDateTime = GETDATE()
                        WHERE   PKID = @PKID;";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@VehicleId", model.VehicleId),
                new SqlParameter("@PaiLiang", model.PaiLiang),
                new SqlParameter("@Nian", model.Nian),
                new SqlParameter("@ArticleUrl", model.ArticleUrl),
                new SqlParameter("@PKID", model.PKID)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
        }

        /// <summary>
        /// 根据车系查询
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static List<VehicleArticleViewModel> SelectVehicleArticleModelForVehicleId
            (SqlConnection conn, VehicleSearchModel model, out int totalCount)
        {
            var sql = @"SELECT  @Total = COUNT(1)
                        FROM    ( SELECT DISTINCT
                                            c.PKID ,
                                            g.ProductID AS VehicleId ,
                                            g.Brand ,
                                            g.Vehicle AS VehicleSeries ,
                                            c.ArticleUrl ,
                                            c.CreateDateTime ,
                                            c.LastUpdateDateTime
                                  FROM      ( SELECT    s.*
                                              FROM      Configuration..VehicleArticle AS s WITH ( NOLOCK )
                                              WHERE     ( s.PaiLiang IS NULL
                                                          OR s.PaiLiang = N''
                                                        )
                                                        AND ( s.Nian IS NULL
                                                              OR s.Nian = N''
                                                            )
                                            ) AS c
                                            RIGHT JOIN Gungnir..tbl_Vehicle_Type AS g WITH ( NOLOCK ) ON c.VehicleId = g.ProductID
                                                        COLLATE Chinese_PRC_CI_AS
                                  WHERE     ( @Brand IS NULL
                                              OR @Brand = N''
                                              OR g.Brand = @Brand
                                            )
                                            AND ( @VehicleId IS NULL
                                              OR @VehicleId = N''
                                              OR g.ProductID = @VehicleId
                                            )
                                            AND ( @IsOnlyConfiged = 0
                                                  OR c.PKID > 0
                                                )
                                ) AS result;
                        SELECT DISTINCT
                                c.PKID ,
                                g.ProductID AS VehicleId ,
                                g.Brand ,
                                g.Vehicle AS VehicleSeries ,
                                c.ArticleUrl ,
                                c.CreateDateTime ,
                                c.LastUpdateDateTime
                        FROM    ( SELECT    s.*
                                  FROM      Configuration..VehicleArticle AS s WITH ( NOLOCK )
                                  WHERE     ( s.PaiLiang IS NULL
                                              OR s.PaiLiang = N''
                                            )
                                            AND ( s.Nian IS NULL
                                                  OR s.Nian = N''
                                                )
                                ) AS c
                                RIGHT JOIN Gungnir..tbl_Vehicle_Type AS g WITH ( NOLOCK ) ON c.VehicleId = g.ProductID
                                                  COLLATE Chinese_PRC_CI_AS
                        WHERE   ( @Brand IS NULL
                                  OR @Brand = N''
                                  OR g.Brand = @Brand
                                )
                                AND ( @VehicleId IS NULL
                                  OR @VehicleId = N''
                                  OR g.ProductID = @VehicleId
                                )
                                AND ( @IsOnlyConfiged = 0
                                      OR c.PKID > 0
                                    )
                        ORDER BY c.PKID DESC , g.Brand
                         OFFSET ( @PageIndex - 1 ) * @PageSize ROWS  FETCH NEXT @PageSize ROWS
                                                        ONLY;";
            var parameters = new[]
            {
                new SqlParameter("@Brand", model.Brand),
                new SqlParameter("@VehicleId", model.VehicleId),
                new SqlParameter("@IsOnlyConfiged", model.IsOnlyConfiged),
                new SqlParameter("@PageIndex", model.PageIndex),
                new SqlParameter("@PageSize", model.PageSize),
                new SqlParameter("@Total", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters).ConvertTo<VehicleArticleViewModel>().ToList();
            totalCount = Convert.ToInt32(parameters.LastOrDefault().Value?.ToString());
            return result;
        }

        /// <summary>
        /// 获取单个配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static VehicleArticleModel GetVehicleArticle(SqlConnection conn, VehicleArticleModel model)
        {
            #region Sql
            var sql = @"SELECT  s.PKID ,
                                s.VehicleId ,
                                s.PaiLiang ,
                                s.Nian ,
                                s.ArticleUrl ,
                                s.CreateDateTime ,
                                s.LastUpdateDateTime
                        FROM    Configuration..VehicleArticle AS s WITH ( NOLOCK )
                        WHERE   s.VehicleId = @VehicleId
                                AND ( ( @PaiLiang IS NULL
                                        AND s.PaiLiang IS NULL
                                      )
                                      OR s.PaiLiang = @PaiLiang
                                    )
                                AND ( ( @Nian IS NULL
                                        AND s.Nian IS NULL
                                      )
                                      OR s.Nian = @Nian
                                    );";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@VehicleId", model.VehicleId),
                new SqlParameter("@PaiLiang", model.PaiLiang),
                new SqlParameter("@Nian", model.Nian)
            };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters).ConvertTo<VehicleArticleModel>().FirstOrDefault();
        }

        /// <summary>
        /// 根据Pkid查询配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static VehicleArticleModel GetVehicleArticleByPkid(SqlConnection conn, int pkid)
        {
            #region Sql
            var sql = @"SELECT  s.PKID ,
                                s.VehicleId ,
                                s.PaiLiang ,
                                s.Nian ,
                                s.ArticleUrl ,
                                s.CreateDateTime ,
                                s.LastUpdateDateTime
                        FROM    Configuration..VehicleArticle AS s WITH ( NOLOCK )
                        WHERE   s.PKID = @PKID;";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@PKID", pkid)
            };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters).ConvertTo<VehicleArticleModel>().FirstOrDefault();
        }

        /// <summary>
        /// 是否存在重复配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool IsExistVehicleArticle(SqlConnection conn, VehicleArticleModel model)
        {
            #region Sql
            var sql = @"SELECT  COUNT(1)
                        FROM    Configuration..VehicleArticle AS s WITH ( NOLOCK )
                        WHERE   s.VehicleId = @VehicleId
                                AND ( ( @PaiLiang IS NULL
                                        AND s.PaiLiang IS NULL
                                      )
                                      OR s.PaiLiang = @PaiLiang
                                    )
                                AND ( ( @Nian IS NULL
                                        AND s.Nian IS NULL
                                      )
                                      OR s.Nian = @Nian
                                    )
                                AND s.PKID <> @PKID;";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@VehicleId", model.VehicleId),
                new SqlParameter("@PaiLiang", model.PaiLiang),
                new SqlParameter("@Nian", model.Nian),
                new SqlParameter("@PKID", model.PKID)
            };
            return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters)) > 0;
        }

        /// <summary>
        /// 根据排量查询
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public static List<VehicleArticleViewModel> SelectVehicleArticleModelForPaiLiang
            (SqlConnection conn, VehicleSearchModel model, out int totalCount)
        {
            var sql = @"SELECT @Total = COUNT(1)
                         FROM   ( SELECT DISTINCT
                                            c.PKID ,
                                            g.VehicleID ,
                                            t.Brand ,
                                            g.VehicleSeries ,
                                            g.PaiLiang ,
                                            c.ArticleUrl ,
                                            c.CreateDateTime ,
                                            c.LastUpdateDateTime
                                  FROM      ( SELECT    *
                                              FROM      Configuration..VehicleArticle AS s WITH ( NOLOCK )
                                              WHERE     s.PaiLiang IS NOT NULL
                                                        AND s.PaiLiang <> N''
                                                        AND ( s.Nian IS NULL
                                                              OR s.Nian = N''
                                                            )
                                            ) AS c
                                            RIGHT JOIN Gungnir..tbl_Vehicle_Type_Timing AS g WITH ( NOLOCK ) ON c.VehicleId = g.VehicleID
                                                                                                        COLLATE Chinese_PRC_CI_AS
                                                                                      AND c.PaiLiang = g.PaiLiang COLLATE Chinese_PRC_CI_AS
                                            INNER JOIN Gungnir..tbl_Vehicle_Type AS t WITH ( NOLOCK ) ON t.ProductID = g.VehicleID
                                  WHERE     ( @Brand IS NULL
                                              OR @Brand = N''
                                              OR t.Brand = @Brand
                                            )
                                            AND ( @VehicleId IS NULL
                                                  OR @VehicleId = N''
                                                  OR g.VehicleID = @VehicleId
                                                )
                                            AND ( @PaiLiang IS NULL
                                                  OR @PaiLiang = N''
                                                  OR g.PaiLiang = @PaiLiang
                                                )
                                            AND ( @IsOnlyConfiged = 0
                                                  OR c.PKID > 0
                                                )
                                ) AS result;
                         SELECT DISTINCT
                                c.PKID ,
                                g.VehicleID ,
                                t.Brand ,
                                g.VehicleSeries ,
                                g.PaiLiang ,
                                c.ArticleUrl ,
                                c.CreateDateTime ,
                                c.LastUpdateDateTime
                         FROM   ( SELECT    s.*
                                  FROM      Configuration..VehicleArticle AS s WITH ( NOLOCK )
                                  WHERE     s.PaiLiang IS NOT NULL
                                            AND s.PaiLiang <> N''
                                            AND ( s.Nian IS NULL
                                                  OR s.Nian = N''
                                                )
                                ) AS c
                                RIGHT JOIN Gungnir..tbl_Vehicle_Type_Timing AS g WITH ( NOLOCK ) ON c.VehicleId = g.VehicleID
                                                                                                  COLLATE Chinese_PRC_CI_AS
                                                                                      AND c.PaiLiang = g.PaiLiang COLLATE Chinese_PRC_CI_AS
                                INNER JOIN Gungnir..tbl_Vehicle_Type AS t WITH ( NOLOCK ) ON t.ProductID = g.VehicleID
                         WHERE  ( @Brand IS NULL
                                  OR @Brand = N''
                                  OR t.Brand = @Brand
                                )
                                AND ( @VehicleId IS NULL
                                      OR @VehicleId = N''
                                      OR g.VehicleID = @VehicleId
                                    )
                                AND ( @PaiLiang IS NULL
                                      OR @PaiLiang = N''
                                      OR g.PaiLiang = @PaiLiang
                                    )
                                AND ( @IsOnlyConfiged = 0
                                      OR c.PKID > 0
                                    )
                         ORDER BY c.PKID DESC ,
                                t.Brand
                                OFFSET ( @PageIndex - 1 ) * @PageSize ROWS  FETCH NEXT @PageSize ROWS
                                ONLY;";
            var parameters = new[]
            {
                new SqlParameter("@Brand", model.Brand),
                new SqlParameter("@VehicleId", model.VehicleId),
                new SqlParameter("@PaiLiang", model.PaiLiang),
                new SqlParameter("@IsOnlyConfiged", model.IsOnlyConfiged),
                new SqlParameter("@PageIndex", model.PageIndex),
                new SqlParameter("@PageSize", model.PageSize),
                new SqlParameter("@Total", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters).ConvertTo<VehicleArticleViewModel>().ToList();
            totalCount = Convert.ToInt32(parameters.LastOrDefault().Value?.ToString());
            return result;
        }

        /// <summary>
        /// 根据年份查询
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public static List<VehicleArticleViewModel> SelectVehicleArticleModelForNiann
            (SqlConnection conn, VehicleSearchModel model)
        {
            var sql = @"SELECT  c.PKID ,
                                c.VehicleId ,
                                c.PaiLiang ,
                                c.Nian ,
                                c.ArticleUrl ,
                                c.CreateDateTime ,
                                c.LastUpdateDateTime
                        FROM    Configuration..VehicleArticle AS c WITH ( NOLOCK )
                        WHERE   c.VehicleId = @VehicleId
                                AND c.PaiLiang = @PaiLiang
                                AND ( @Nian IS NULL
                                      OR @Nian = N''
                                      OR c.Nian = @Nian
                                    )
                        ORDER BY c.PKID DESC;";
            var parameters = new[]
            {
                new SqlParameter("@VehicleId", model.VehicleId),
                new SqlParameter("@PaiLiang", model.PaiLiang),
                new SqlParameter("@Nian", model.Nian)
            };
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters).ConvertTo<VehicleArticleViewModel>().ToList();
            return result;
        }

        /// <summary>
        /// 根据年款查询(不分页)
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public static List<VehicleArticleViewModel> SelectVehiclesForNian
            (SqlConnection conn, VehicleSearchModel model)
        {
            var sql = @"SELECT DISTINCT
                                g.VehicleID ,
                                t.Brand ,
                                g.VehicleSeries ,
                                g.PaiLiang ,
                                g.ListedYear ,
                                ISNULL(g.StopProductionYear, YEAR(GETDATE())) AS StopProductionYear
                        FROM    Gungnir..tbl_Vehicle_Type_Timing AS g WITH ( NOLOCK )
                                INNER JOIN Gungnir..tbl_Vehicle_Type AS t WITH (NOLOCK) ON t.ProductID=g.VehicleID
                        WHERE   g.VehicleID = @VehicleId
                                AND g.PaiLiang = @PaiLiang
                                AND ( @Nian IS NULL
                                      OR @Nian = N''
                                      OR ( @Nian BETWEEN g.ListedYear
                                                 AND     ISNULL(g.StopProductionYear, YEAR(GETDATE())) )
                                    )
                       ORDER BY t.Brand;";
            var parameters = new[]
            {
                new SqlParameter("@VehicleId", model.VehicleId),
                new SqlParameter("@PaiLiang", model.PaiLiang),
                new SqlParameter("@Nian", model.Nian)
            };
            var nianConfigs = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters).ConvertTo<VehicleTypeInfoDb>().ToList();
            var result = new List<VehicleArticleViewModel>();
            foreach (var nianConfig in nianConfigs)
            {
                int start;
                int end;
                if (int.TryParse(nianConfig.ListedYear, out start) && int.TryParse(nianConfig.StopProductionYear, out end))
                {
                    for (var i = start; i <= end; i++)
                    {
                        if (!result.Any(s => s.Nian == i.ToString()))
                        {
                            result.Add(new VehicleArticleViewModel()
                            {
                                Brand = nianConfig.Brand,
                                VehicleSeries = nianConfig.VehicleSeries,
                                VehicleId = nianConfig.VehicleID,
                                PaiLiang = nianConfig.PaiLiang,
                                Nian = i.ToString()
                            });
                        }
                    }
                }
            }
            return result;
        }
    }
}
