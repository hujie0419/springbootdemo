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
    public class DalPaintDiscountConfig
    {
        /// <summary>
        /// 添加喷漆打折配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int AddPaintDiscountConfig(SqlConnection conn, PaintDiscountConfigModel model)
        {
            #region SQL
            var sql = @"INSERT  INTO Configuration..PaintDiscountConfig
                                ( ServicePid ,
                                  SurfaceCount ,
                                  ActivityPrice ,
                                  ActivityName ,
                                  ActivityExplain ,
                                  ActivityImage
                                )
                        VALUES  ( @ServicePid ,
                                  @SurfaceCount ,
                                  @ActivityPrice ,
                                  @ActivityName ,
                                  @ActivityExplain ,
                                  @ActivityImage
                                )
                        SELECT  SCOPE_IDENTITY();";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@ServicePid", model.ServicePid),
                new SqlParameter("@SurfaceCount", model.SurfaceCount),
                new SqlParameter("@ActivityPrice", model.ActivityPrice),
                new SqlParameter("@ActivityName", model.ActivityName ?? string.Empty),
                new SqlParameter("@ActivityExplain", model.ActivityExplain ?? string.Empty),
                new SqlParameter("@ActivityImage", model.ActivityImage ?? string.Empty)
            };
            return Convert.ToInt32(SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters));
        }

        /// <summary>
        /// 删除喷漆打折配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static bool DeletePaintDiscountConfig(SqlConnection conn, int pkid)
        {
            #region SQL
            var sql = @"UPDATE  Configuration..PaintDiscountConfig
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
        /// 更新喷漆打折配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool UpdatePaintDiscountConfig(SqlConnection conn, PaintDiscountConfigModel model)
        {
            #region Sql
            var sql = @"UPDATE  Configuration..PaintDiscountConfig
                        SET     SurfaceCount = @SurfaceCount ,
                                ActivityPrice = @ActivityPrice ,
                                ActivityName = @ActivityName ,
                                ActivityExplain = @ActivityExplain ,
                                ActivityImage = @ActivityImage ,
                                IsDeleted = 0 ,
                                LastUpdateDateTime = GETDATE()
                        WHERE   PKID = @PKID;";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@SurfaceCount", model.SurfaceCount),
                new SqlParameter("@ActivityPrice", model.ActivityPrice),
                new SqlParameter("@ActivityName", model.ActivityName ?? string.Empty),
                new SqlParameter("@ActivityExplain", model.ActivityExplain ?? string.Empty),
                new SqlParameter("@ActivityImage", model.ActivityImage ?? string.Empty),
                new SqlParameter("@PKID", model.PKID)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
        }

        /// <summary>
        /// 查询喷漆打折配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="servicePid"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public static List<PaintDiscountConfigModel> SelectPaintDiscountConfig
            (SqlConnection conn, string servicePid, int pageIndex, int pageSize, out int totalCount)
        {
            #region Sql
            var sql = @"SELECT  @Total = COUNT(1)
                        FROM    Configuration..PaintDiscountConfig AS s WITH ( NOLOCK )
                        WHERE   ( @ServicePid IS NULL
                                  OR @ServicePid = N''
                                  OR s.ServicePid = @ServicePid
                                )
                                AND s.IsDeleted = 0; 
                        SELECT  s.PKID ,
                                s.ServicePid ,
                                s.SurfaceCount ,
                                s.ActivityPrice ,
                                s.ActivityName ,
                                s.ActivityExplain ,
                                s.ActivityImage ,
                                s.IsDeleted ,
                                s.CreateDateTime ,
                                s.LastUpdateDateTime
                        FROM    Configuration..PaintDiscountConfig AS s WITH ( NOLOCK )
                        WHERE   ( @ServicePid IS NULL
                                  OR @ServicePid = N''
                                  OR s.ServicePid = @ServicePid
                                )
                                AND s.IsDeleted = 0
                        ORDER BY s.ServicePid ,
                                s.SurfaceCount
                                OFFSET ( @PageIndex - 1 ) * @PageSize ROWS  FETCH NEXT @PageSize ROWS
                                ONLY;";
            #endregion
            totalCount = 0;
            var parameters = new[]
            {
                new SqlParameter("@ServicePid", servicePid),
                new SqlParameter("@PageIndex", pageIndex),
                new SqlParameter("@PageSize", pageSize),
                new SqlParameter("@Total", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters).ConvertTo<PaintDiscountConfigModel>().ToList();
            totalCount = Convert.ToInt32(parameters.LastOrDefault().Value.ToString());
            return result;
        }

        /// <summary>
        /// 获取所有喷漆打折服务
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static List<PaintDiscountServiceModel> GetAllPaintDiscountService(SqlConnection conn)
        {
            #region sql
            var sql = @"SELECT  s.PID AS ServicePid,
                                s.DisplayName AS ServiceName
                        FROM    Tuhu_productcatalog..[CarPAR_zh-CN] AS s WITH ( NOLOCK )
                        WHERE   s.PrimaryParentCategory = N'PQFW'
                                AND s.OrigProductID IN ( N'FU-PQXB-BZQ', N'FU-PQXB-GDQ' )
                                AND s.i_ClassType IN ( 2, 4 )
                                AND s.OnSale = 1
                        ORDER BY s.PID;";
            #endregion
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql).ConvertTo<PaintDiscountServiceModel>().ToList();
        }

        /// <summary>
        /// 获取喷漆打折配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="servicePid"></param>
        /// <param name="venderShortName"></param>
        /// <param name="carNoPrefix"></param>
        /// <returns></returns>
        public static PaintDiscountConfigModel GetPaintDiscountConfig
            (SqlConnection conn, string servicePid, int surfaceCount)
        {
            #region Sql
            var sql = @"SELECT  s.PKID ,
                                s.ServicePid ,
                                s.SurfaceCount ,
                                s.ActivityPrice ,
                                s.ActivityName ,
                                s.ActivityExplain ,
                                s.ActivityImage ,
                                s.IsDeleted ,
                                s.CreateDateTime ,
                                s.LastUpdateDateTime
                        FROM    Configuration..PaintDiscountConfig AS s WITH ( NOLOCK )
                        WHERE   s.ServicePid = @ServicePid
                                AND s.SurfaceCount = @SurfaceCount;";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@ServicePid", servicePid),
                new SqlParameter("@SurfaceCount", surfaceCount)
            };
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters)
                .ConvertTo<PaintDiscountConfigModel>().FirstOrDefault();
            return result;
        }

        /// <summary>
        /// 判断喷漆打折配置是否重复
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool IsExistPaintDiscountConfig
            (SqlConnection conn, PaintDiscountConfigModel model)
        {
            #region Sql
            var sql = @"SELECT  COUNT(1)
                        FROM    Configuration..PaintDiscountConfig AS s WITH ( NOLOCK )
                        WHERE   s.ServicePid = @ServicePid
                                AND s.SurfaceCount = @SurfaceCount
                                AND s.IsDeleted=0
                                AND s.PKID <> @PKID;";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@ServicePid", model.ServicePid),
                new SqlParameter("@SurfaceCount", model.SurfaceCount),
                new SqlParameter("@PKID", model.PKID)
            };
            var count = Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters));
            return count > 0;
        }

        /// <summary>
        /// 查询喷漆打折操作日志
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="logType"></param>
        /// <param name="identityId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static List<PaintDiscountOprLogModel> SelectPaintDiscountOprLog
            (SqlConnection conn, string logType, string identityId, int pageIndex, int pageSize)
        {
            #region Sql
            var sql = @" SELECT  log.PKID ,
                                log.LogType ,
                                log.IdentityId ,
                                log.OperationType ,
                                log.OldValue ,
                                log.NewValue ,
                                log.Operator ,
                                log.Remarks ,
                                log.CreateDateTime ,
                                log.LastUpdateDateTime
                        FROM    Tuhu_log..PaintDiscountOprLog AS log WITH ( NOLOCK )
                        WHERE   log.LogType = @LogType
                                AND ( @IdentityId IS NULL
                                      OR @IdentityId = N''
                                      OR log.IdentityId = @IdentityId
                                    )
                        ORDER BY log.PKID DESC
                                OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
                                ONLY;";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@LogType", logType),
                new SqlParameter("@IdentityId", identityId),
                new SqlParameter("@PageIndex", pageIndex),
                new SqlParameter("@PageSize", pageSize)
            };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters)
                .ConvertTo<PaintDiscountOprLogModel>().ToList();
        }
    }
}
