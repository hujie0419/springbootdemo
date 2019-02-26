using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalBaoYangPackageTypeRelation
    {
        /// <summary>
        /// 添加保养关联项目配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int AddBaoYangPackageTypeRelation(SqlConnection conn, BaoYangPackageTypeRelationViewModel model)
        {
            #region SQL
            var sql = @"INSERT INTO BaoYang..BaoYangPackageTypeRelation
                        (
                            MainPackageType,
                            RelatedPackageTypes,
                            Content,
                            Highlights,
                            IsEnabled,
                            IsStrongRelated,
                            CancelContent
                        )
                        VALUES
                        (@MainPackageType, @RelatedPackageTypes, @Content, @Highlights, @IsEnabled, @IsStrongRelated, @CancelContent);
                        SELECT SCOPE_IDENTITY();";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@MainPackageType", model.MainPackageType),
                new SqlParameter("@RelatedPackageTypes", model.RelatedPackageTypes),
                new SqlParameter("@Content", model.Content),
                new SqlParameter("@Highlights", model.Highlights??string.Empty),
                new SqlParameter("@IsEnabled", model.IsEnabled),
                new SqlParameter("@IsStrongRelated", model.IsStrongRelated),
                new SqlParameter("@CancelContent", string.IsNullOrWhiteSpace(model.CancelContent)?string.Empty:model.CancelContent)
            };
            return Convert.ToInt32(SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters));
        }

        /// <summary>
        /// 删除保养关联项目配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static bool DeleteBaoYangPackageTypeRelation(SqlConnection conn, int pkid)
        {
            #region SQL
            var sql = @"UPDATE  BaoYang..BaoYangPackageTypeRelation
                        SET     IsDeleted = 1 ,
                                LastUpdateDateTime = GETDATE()
                        WHERE   PKID = @PKID;";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@PKID", pkid)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
        }

        /// <summary>
        /// 更新保养关联项目配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool UpdateBaoYangPackageTypeRelation(SqlConnection conn, BaoYangPackageTypeRelationViewModel model)
        {
            #region Sql
            var sql = @"UPDATE BaoYang..BaoYangPackageTypeRelation
                        SET RelatedPackageTypes = @RelatedPackageTypes,
                            Content = @Content,
                            Highlights = @Highlights,
                            IsEnabled = @IsEnabled,
                            IsDeleted = @IsDeleted,
                            IsStrongRelated = @IsStrongRelated,
                            CancelContent = @CancelContent,
                            LastUpdateDateTime = GETDATE()
                        WHERE PKID = @PKID;";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@RelatedPackageTypes", model.RelatedPackageTypes),
                new SqlParameter("@Content", model.Content),
                new SqlParameter("@Highlights", model.Highlights??string.Empty),
                new SqlParameter("@IsEnabled", model.IsEnabled),
                new SqlParameter("@IsDeleted", model.IsDeleted),
                new SqlParameter("@IsStrongRelated", model.IsStrongRelated),
                new SqlParameter("@CancelContent", string.IsNullOrWhiteSpace(model.CancelContent)?string.Empty:model.CancelContent),
                new SqlParameter("@PKID", model.PKID)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
        }

        /// <summary>
        /// 查询保养关联项目配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="servicePid"></param>
        /// <param name="carNoPrefix"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public static List<BaoYangPackageTypeRelationViewModel> SelectBaoYangPackageTypeRelation
            (SqlConnection conn, string mainPackageType, int pageIndex, int pageSize, out int totalCount)
        {
            #region Sql
            var sql = @"SELECT @Total = COUNT(1)
                        FROM BaoYang..BaoYangPackageTypeRelation AS s WITH (NOLOCK)
                        WHERE (
                                  @MainPackageType IS NULL
                                  OR @MainPackageType = N''
                                  OR s.MainPackageType = @MainPackageType
                              )
                              AND s.IsDeleted = 0;
                        SELECT PKID,
                               MainPackageType,
                               RelatedPackageTypes,
                               Content,
                               IsEnabled,
                               IsDeleted,
                               CreateDateTime,
                               LastUpdateDateTime,
                               IsStrongRelated,
                               CancelContent,
                               Highlights
                        FROM BaoYang..BaoYangPackageTypeRelation AS s WITH (NOLOCK)
                        WHERE (
                                  @MainPackageType IS NULL
                                  OR @MainPackageType = N''
                                  OR s.MainPackageType = @MainPackageType
                              )
                              AND s.IsDeleted = 0
                        ORDER BY s.PKID DESC OFFSET (@PageIndex - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;";
            #endregion
            totalCount = 0;
            var parameters = new[]
            {
                new SqlParameter("@MainPackageType", mainPackageType),
                new SqlParameter("@PageIndex", pageIndex),
                new SqlParameter("@PageSize", pageSize),
                new SqlParameter("@Total", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters).ConvertTo<BaoYangPackageTypeRelationViewModel>().ToList();
            totalCount = Convert.ToInt32(parameters.LastOrDefault().Value.ToString());
            return result;
        }

        /// <summary>
        /// 获取单个保养关联项目配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="mainPackageType"></param>
        /// <returns></returns>
        public static BaoYangPackageTypeRelationViewModel GetBaoYangPackageTypeRelation(SqlConnection conn, string mainPackageType)
        {
            #region Sql
            var sql = @"SELECT PKID,
                       MainPackageType,
                       RelatedPackageTypes,
                       Content,
                       IsEnabled,
                       IsDeleted,
                       CreateDateTime,
                       LastUpdateDateTime,
                       CancelContent,
                       IsStrongRelated,
                       Highlights
                FROM BaoYang..BaoYangPackageTypeRelation AS s WITH (NOLOCK)
                WHERE s.MainPackageType = @MainPackageType;";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@MainPackageType", mainPackageType)
            };
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters).ConvertTo<BaoYangPackageTypeRelationViewModel>().FirstOrDefault();
            return result;
        }

        /// <summary>
        /// 主项目对应的配置是否存在
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool IsExistBaoYangPackageTypeRelation
           (SqlConnection conn, BaoYangPackageTypeRelationViewModel model)
        {
            #region Sql
            var sql = @"SELECT  COUNT(1)
                        FROM    BaoYang..BaoYangPackageTypeRelation AS s WITH ( NOLOCK )
                        WHERE   s.MainPackageType = @MainPackageType
                                AND s.IsDeleted=0
                                AND s.PKID <> @PKID;";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@MainPackageType", model.MainPackageType),
                new SqlParameter("@PKID", model.PKID)
            };
            var count = Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters));
            return count > 0;
        }
        /// <summary>
        ///  检查辅项目是否存在重复
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static bool IsExistRelatedPackageTypes(SqlConnection conn, int pkid, string types)
        {
            var sql = @"SELECT COUNT(1)
                        FROM BaoYang..BaoYangPackageTypeRelation AS s WITH (NOLOCK)
                        WHERE EXISTS
                        (
                            SELECT *
                            FROM Configuration.dbo.SplitString(s.RelatedPackageTypes, ',', 1) AS a
                                JOIN Configuration.dbo.SplitString(@Types, ',', 1) AS b
                                    ON a.Item = b.Item
                        )
                              AND s.IsDeleted = 0
                              AND s.IsEnabled = 1
                              AND IsStrongRelated = 1
                              AND s.PKID <> @pkid;";
            var parameters = new[]
            {
                new SqlParameter("@PKID",pkid),
                new SqlParameter("@Types" , types)
            };
            return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters)) > 0;
        }
    }
}
