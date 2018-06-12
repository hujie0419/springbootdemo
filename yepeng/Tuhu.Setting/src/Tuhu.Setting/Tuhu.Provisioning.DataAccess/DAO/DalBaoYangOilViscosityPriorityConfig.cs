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
   public class DalBaoYangOilViscosityPriorityConfig
    {
        #region 机油粘度优先级配置
        /// <summary>
        /// 查询机油粘度优先级配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="originViscosity"></param>
        /// <param name="configType"></param>
        /// <returns></returns>
        public static List<BaoYangOilViscosityPriorityConfigModel> SelectBaoYangOilViscosityPriorityConfig(SqlConnection conn,string originViscosity,string configType)
        {
            var sql = @"SELECT  s.PKID ,
                                s.OriginViscosity ,
                                s.ViscosityPriority ,
                                s.CreateDateTime ,
                                s.LastUpdateDateTime ,
                                s.ConfigType
                        FROM    BaoYang..BaoYangOilViscosityPriorityConfig AS s WITH ( NOLOCK )
                        WHERE   ( @OriginViscosity IS NULL
                                  OR @OriginViscosity = N''
                                  OR s.OriginViscosity = @OriginViscosity
                                )
                                AND s.IsDeleted = 0
                                AND s.ConfigType = @ConfigType;";
            var parameters = new[]
            {
                new SqlParameter("@OriginViscosity", originViscosity),
                new SqlParameter("@ConfigType", configType)
            };
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters).ConvertTo<BaoYangOilViscosityPriorityConfigModel>().ToList();
            return result;
        }

        /// <summary>
        /// 获取当前粘度优先级配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="originViscosity"></param>
        /// <param name="configType"></param>
        /// <returns></returns>
        public static BaoYangOilViscosityPriorityConfigModel GetBaoYangOilViscosityPriorityConfig(SqlConnection conn,string originViscosity,string configType)
        {
            var sql = @"SELECT  s.PKID ,
                                s.OriginViscosity ,
                                s.ViscosityPriority ,
                                s.CreateDateTime ,
                                s.LastUpdateDateTime ,
                                s.IsDeleted ,
                                s.ConfigType
                        FROM    BaoYang..BaoYangOilViscosityPriorityConfig AS s WITH ( NOLOCK )
                        WHERE   s.OriginViscosity = @OriginViscosity
                                AND s.ConfigType = @ConfigType;";
            var parameters = new[]
            {
                new SqlParameter("@OriginViscosity", originViscosity),
                new SqlParameter("@ConfigType", configType)
            };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters)
                .ConvertTo<BaoYangOilViscosityPriorityConfigModel>().ToList().FirstOrDefault();
        }

        /// <summary>
        /// 更新机油粘度优先级配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool UpdateBaoYangOilViscosityPriorityConfig(SqlConnection conn, BaoYangOilViscosityPriorityConfigModel model)
        {
            var sql = @"UPDATE  BaoYang..BaoYangOilViscosityPriorityConfig
                        SET     ViscosityPriority = @ViscosityPriority ,
                                IsDeleted = 0
                        WHERE   OriginViscosity = @OriginViscosity
                                AND ConfigType = @ConfigType;";
            var parameters = new[]
            {
                new SqlParameter("@OriginViscosity", model.OriginViscosity),
                new SqlParameter("@ViscosityPriority",model.ViscosityPriority),
                new SqlParameter("@ConfigType",model.ConfigType)
            };
            return Convert.ToInt32(SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters)) > 0;
        }

        /// <summary>
        /// 添加机油粘度优先级配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int AddBaoYangOilViscosityPriorityConfig(SqlConnection conn, BaoYangOilViscosityPriorityConfigModel model)
        {
            var sql = @"INSERT  BaoYang..BaoYangOilViscosityPriorityConfig
                                ( OriginViscosity ,
                                  ViscosityPriority ,
                                  ConfigType
                                )
                        VALUES  ( @OriginViscosity ,
                                  @ViscosityPriority ,
                                  @ConfigType
                                );
                        SELECT  SCOPE_IDENTITY();";
            var parameters = new[]
            {
                new SqlParameter("@OriginViscosity", model.OriginViscosity),
                new SqlParameter("@ViscosityPriority", model.ViscosityPriority),
                new SqlParameter("@ConfigType",model.ConfigType)
            };
            return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters));
        }

        /// <summary>
        /// 删除机油粘度优先级配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static bool DeleteBaoYangOilViscosityPriorityConfig(SqlConnection conn, int pkid)
        {
            var sql = @"UPDATE  BaoYang..BaoYangOilViscosityPriorityConfig
                        SET     IsDeleted = 1
                        WHERE   PKID = @PKID;";
            var parameters = new[]
            {
                new SqlParameter("@PKID", pkid)
            };
            return Convert.ToInt32(SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters)) > 0;
        }
        #endregion

        #region 机油粘度特殊地区配置
        /// <summary>
        /// 获取机油粘度特殊地区配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="regionIds"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static List<OilViscosityRegionModel> SelectOilViscosityRegionConfig(SqlConnection conn,
            List<int> regionIds, int pageIndex, int pageSize)
        {
            var result = null as List<OilViscosityRegionModel>;
            #region SQL
            var sql = @"SELECT  s.PKID ,
                                CONVERT(INT, r.Item) AS RegionId ,
                                s.OilViscosity ,
                                s.CreateDateTime ,
                                s.LastUpdateDateTime
                        FROM    BaoYang..OilViscosityRegionPriorityConfig AS s WITH ( NOLOCK )
                                RIGHT JOIN ( SELECT Item
                                             FROM   Gungnir..SplitString(@RegionId, N',', 1)
                                           ) AS r ON r.Item = s.RegionId
                                                     AND s.IsDeleted = 0
                        ORDER BY RegionId
                                OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
                                ONLY;";
            #endregion
            var parameters = new SqlParameter[] {
                    new SqlParameter("@PageIndex", pageIndex),
                    new SqlParameter("@PageSize", pageSize),
                    new SqlParameter("@RegionId", string.Join(",",regionIds))
            };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters).ConvertTo<OilViscosityRegionModel>().ToList();
        }

        /// <summary>
        /// 添加机油粘度特殊地区配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int AddOilViscosityRegionConfig(SqlConnection conn, OilViscosityRegionModel model)
        {
            var sql = @"INSERT  INTO BaoYang..OilViscosityRegionPriorityConfig
                                ( RegionId, OilViscosity )
                        VALUES  ( @RegionId, @OilViscosity )
                        SELECT  SCOPE_IDENTITY();";
            var parameters = new SqlParameter[] {
                new SqlParameter("@RegionId", model.RegionId),
                new SqlParameter("@OilViscosity", model.OilViscosity)
            };
            return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters));
        }

        /// <summary>
        /// 更新机油粘度特殊地区配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool UpdateOilViscosityRegionConfig(SqlConnection conn, OilViscosityRegionModel model)
        {
            var sql = @"UPDATE  BaoYang..OilViscosityRegionPriorityConfig
                        SET     OilViscosity = @OilViscosity ,
                                IsDeleted = 0 ,
                                LastUpdateDateTime = GETDATE()
                        WHERE   RegionId = @RegionId;";
            var parameters = new SqlParameter[] {
                new SqlParameter("@RegionId", model.RegionId),
                new SqlParameter("@OilViscosity", model.OilViscosity)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
        }

        /// <summary>
        /// 删除机油粘度特殊地区配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static bool DeleteOilViscosityRegionConfig(SqlConnection conn, int pkid)
        {
            var sql = @"UPDATE  BaoYang..OilViscosityRegionPriorityConfig
                        SET     IsDeleted = 1 ,
                                LastUpdateDateTime = GETDATE()
                        WHERE   PKID = @PKID;";
            var parameters = new SqlParameter[] {
                new SqlParameter("@PKID", pkid)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
        }

        /// <summary>
        /// 根据地区Id查询机油粘度特殊地区配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="regionId"></param>
        /// <returns></returns>
        public static OilViscosityRegionModel GetOilViscosityRegionConfigByRegionId(SqlConnection conn, int regionId)
        {
            var sql = @"SELECT  s.PKID ,
                                s.RegionId ,
                                s.OilViscosity ,
                                s.IsDeleted ,
                                s.CreateDateTime ,
                                s.LastUpdateDateTime
                        FROM    BaoYang..OilViscosityRegionPriorityConfig AS s WITH ( NOLOCK )
                        WHERE   s.RegionId = @RegionId;";
            var parameters = new SqlParameter[] {
                new SqlParameter("@RegionId", regionId)
            };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters).ConvertTo<OilViscosityRegionModel>().FirstOrDefault();
        }
        #endregion
    }
}
