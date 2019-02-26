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

namespace Tuhu.Provisioning.DataAccess
{
    public class DalTipBannerConfig
    {
        /// <summary>
        /// 添加新的提示条类型
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int AddTipBannerTypeConfig(SqlConnection conn, TipBannerTypeConfigModel model)
        {
            var sql = @"INSERT  Configuration..TipBannerTypeConfig
                                ( TypeName )
                        VALUES  ( @TypeName )
                        SELECT  SCOPE_IDENTITY();";
            var parameters = new[]
            {
                new SqlParameter("@TypeName", model.TypeName)
            };
            return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters));
        }

        /// <summary>
        /// 获取所有提示条类型
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static List<TipBannerTypeConfigModel> GetAllTipBannerTypeConfig(SqlConnection conn)
        {
            #region SQL
            var sql = @"SELECT  t.PKID AS TypeId ,
                                t.TypeName
                        FROM    Configuration..TipBannerTypeConfig AS t WITH ( NOLOCK )
                        ORDER BY t.TypeName;";
            #endregion
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql).ConvertTo<TipBannerTypeConfigModel>().ToList();
            return result;
        }

        /// <summary>
        /// 添加提示条配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int AddTipBannerDetailConfig(SqlConnection conn, TipBannerConfigDetailModel model)
        {
            #region SQL
            var sql = @"INSERT  INTO Configuration..TipBannerDetailConfig
                                ( BannerTypeId ,
                                  IsEnabled ,
                                  Icon ,
                                  Content ,
                                  Url ,
                                  BackgroundColor ,
                                  BgTransparent ,
                                  ContentColor ,
                                  Title
                                )
                        VALUES  ( @BannerTypeId ,
                                  @IsEnabled ,
                                  @Icon ,
                                  @Content ,
                                  @Url ,
                                  @BackgroundColor ,
                                  @BgTransparent ,
                                  @ContentColor ,
                                  @Title
                                );
                        SELECT  SCOPE_IDENTITY();";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@BannerTypeId", model.TypeId),
                new SqlParameter("@IsEnabled", model.IsEnabled),
                new SqlParameter("@Icon", model.Icon),
                new SqlParameter("@Content", model.Content),
                new SqlParameter("@Url", model.Url),
                new SqlParameter("@BackgroundColor", model.BackgroundColor),
                new SqlParameter("@BgTransparent", model.BgTransparent),
                new SqlParameter("@ContentColor", model.ContentColor),
                new SqlParameter("@Title", model.Title)
            };
            return Convert.ToInt32(SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters));
        }

        public static bool DeleteTipBannerDetailConfigByPKID(SqlConnection conn, int pkid)
        {
            #region SQL
            var sql = @"DELETE  Configuration..TipBannerDetailConfig
                        WHERE   PKID = @PKID;";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@PKID", pkid)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
        }

        /// <summary>
        /// 更新提示条配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool UpdateTipBannerDetailConfig(SqlConnection conn, TipBannerConfigDetailModel model)
        {
            var sql = @"UPDATE  Configuration..TipBannerDetailConfig
                        SET     IsEnabled = @IsEnabled ,
                                Icon = @Icon ,
                                Content = @Content ,
                                Url = @Url ,
                                BackgroundColor = @BackgroundColor ,
                                BgTransparent = @BgTransparent ,
                                ContentColor = @ContentColor ,
                                Title = @Title ,
                                LastUpdateDateTime = GETDATE()
                        WHERE   PKID = @PKID;";
            var parameters = new[]
            {
                new SqlParameter("@IsEnabled", model.IsEnabled),
                new SqlParameter("@Icon", model.Icon),
                new SqlParameter("@Content", model.Content),
                new SqlParameter("@Url", model.Url),
                new SqlParameter("@BackgroundColor", model.BackgroundColor),
                new SqlParameter("@BgTransparent", model.BgTransparent),
                new SqlParameter("@ContentColor", model.ContentColor),
                new SqlParameter("@Title", model.Title),
                new SqlParameter("@PKID", model.PKID)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
        }

        public static List<TipBannerConfigDetailModel> SelectTipBannerDetailConfig(SqlConnection conn, int typeId, int pageIndex, int pageSize, out int totalCount)
        {
            var sql = @"SELECT  @Total = COUNT(1)
                        FROM    Configuration..TipBannerDetailConfig AS b WITH ( NOLOCK )
                        WHERE   @BannerTypeId < 1 OR b.BannerTypeId = @BannerTypeId;
                        SELECT  b.PKID ,
                                b.BannerTypeId AS TypeId ,
                                t.TypeName ,
                                b.IsEnabled ,
                                b.Icon ,
                                b.Content ,
                                b.Url ,
                                b.BackgroundColor ,
                                b.BgTransparent ,
                                b.ContentColor ,
                                b.Title ,
                                b.CreateDateTime ,
                                b.LastUpdateDateTime
                        FROM    Configuration..TipBannerDetailConfig AS b WITH ( NOLOCK )
                                JOIN Configuration..TipBannerTypeConfig AS t WITH ( NOLOCK ) ON b.BannerTypeId = t.PKID
                        WHERE   @BannerTypeId < 1
                                OR b.BannerTypeId = @BannerTypeId
                        ORDER BY b.BannerTypeId ,
                                 b.CreateDateTime DESC
                                 OFFSET ( @PageIndex - 1 ) * @PageSize ROWS  FETCH NEXT @PageSize ROWS
                                 ONLY;";
            totalCount = 0;
            var parameters = new[]
            {
                new SqlParameter("@BannerTypeId", typeId),
                new SqlParameter("@PageIndex", pageIndex),
                new SqlParameter("@PageSize", pageSize),
                new SqlParameter("@Total", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters).ConvertTo<TipBannerConfigDetailModel>().ToList();
            totalCount = Convert.ToInt32(parameters.LastOrDefault().Value.ToString());
            return result;
        }

        /// <summary>
        /// 获取提示条配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static TipBannerConfigDetailModel GetTipBannerDetailConfigByPKID(SqlConnection conn, int pkid)
        {
            var sql = @"SELECT  b.PKID ,
                                b.BannerTypeId AS TypeId ,
                                t.TypeName ,
                                b.IsEnabled ,
                                b.Icon ,
                                b.Content ,
                                b.Url ,
                                b.BackgroundColor ,
                                b.BgTransparent ,
                                b.ContentColor ,
                                b.Title ,
                                b.CreateDateTime ,
                                b.LastUpdateDateTime
                        FROM    Configuration..TipBannerDetailConfig AS b WITH ( NOLOCK )
                                JOIN Configuration..TipBannerTypeConfig AS t WITH ( NOLOCK ) ON b.BannerTypeId = t.PKID
                        WHERE   b.PKID = @PKID;";
            var parameters = new[]
            {
                new SqlParameter("@PKID", pkid)
            };
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters).ConvertTo<TipBannerConfigDetailModel>().FirstOrDefault();
            return result;
        }

        public static bool IsRepeatTipBannerTypeConfig(SqlConnection conn, string typeName)
        {
            var sql = @"SELECT  COUNT(1)
                        FROM    Configuration..TipBannerTypeConfig AS b WITH ( NOLOCK )
                        WHERE   b.TypeName = @TypeName;";
            var parameters = new[]
            {
                new SqlParameter("@TypeName", typeName)
            };
            return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters)) > 0;
        }

        public static bool IsRepeatTipBannerDetailConfig(SqlConnection conn, TipBannerConfigDetailModel model)
        {
            var sql = @"SELECT  COUNT(1)
                        FROM    Configuration..TipBannerDetailConfig AS d WITH ( NOLOCK )
                        WHERE   d.BannerTypeId = @BannerTypeId
                                AND ( d.IsEnabled = 1
                                      AND @IsEnabled = 1
                                    )
                                AND d.PKID <> @PKID;";
            var parameters = new[]
            {
                new SqlParameter("@BannerTypeId", model.TypeId),
                new SqlParameter("@IsEnabled", model.IsEnabled),
                new SqlParameter("@PKID", model.PKID)
            };
            return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters)) > 0;
        }
    }
}
