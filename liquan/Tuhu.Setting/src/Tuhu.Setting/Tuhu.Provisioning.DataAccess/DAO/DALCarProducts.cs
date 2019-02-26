using System;
using Dapper;
using System.Linq;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Collections.Generic;
using Microsoft.ApplicationBlocks.Data;

using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    /// <summary>
    /// 车品模块服务
    /// </summary>
    public class DALCarProducts
    {
        private static readonly string StrConn = SecurityHelp.IsBase64Formatted(ConfigurationManager.ConnectionStrings["Configuration"].ConnectionString) ? SecurityHelp.DecryptAES(ConfigurationManager.ConnectionStrings["Configuration"].ConnectionString) : ConfigurationManager.ConnectionStrings["Configuration"].ConnectionString;

        /// <summary>
        /// 获取Banner实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static CarProductsBanner GetCarProductsBannerEntity(int id)
        {
            using (var conn = new SqlConnection(StrConn))
            {
                string sqlString = @"SELECT * FROM Configuration.dbo.CarProductsBanner (NOLOCK) WHERE PKID=@PKID";
                return conn.Query<CarProductsBanner>(sqlString, new CarProductsBanner { PKID = id }).FirstOrDefault();
            }
        }

        /// <summary>
        /// 获取Banner实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static CarProductsBanner GetCarProductsBannerEntityByFloorId(int floorId)
        {
            using (var conn = new SqlConnection(StrConn))
            {
                string sqlString = @"SELECT * FROM Configuration.dbo.CarProductsBanner (NOLOCK) WHERE Type = 5 AND FKFloorID=@FKFloorID";
                return conn.Query<CarProductsBanner>(sqlString, new CarProductsBanner { FKFloorID = floorId }).FirstOrDefault();
            }
        }

        /// <summary>
        /// 获取banner列表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<CarProductsBanner> GetCarProductsBannerList(int type)
        {
            using (var conn = new SqlConnection(StrConn))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Type", type);
                return conn.Query<CarProductsBanner>("SELECT * FROM Configuration.dbo.CarProductsBanner WITH(NOLOCK) where Type = @Type", parameters).ToList();
            }
        }

        /// <summary>
        /// 添加banner
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int InsertCarProductsBanner(CarProductsBanner model)
        {
            using (var conn = new SqlConnection(StrConn))
            {
                const string sql = @"INSERT INTO Configuration.dbo.CarProductsBanner
                        ( FKFloorID ,
                          Name ,
                          ImgUrl ,
                          StartTime ,
                          EndTime ,
                          IsEnabled ,
                          Sort ,
                          LinkType ,
                          Link ,
                          Type
                        )
                        VALUES  ( @FKFloorID ,
                                  @Name ,
                                  @ImgUrl ,
                                  @StartTime ,
                                  @EndTime ,
                                  @IsEnabled ,
                                  @Sort ,
                                  @LinkType ,
                                  @Link ,
                                  @Type
                        );
                        SELECT ISNULL(@@IDENTITY,0)";
                var sqlPrams = new[] {
                    new SqlParameter("@FKFloorID",model.FKFloorID),
                    new SqlParameter("@Name",model.Name),
                    new SqlParameter("@ImgUrl",model.ImgUrl),
                    new SqlParameter("@StartTime",model.StartTime),
                    new SqlParameter("@EndTime",model.EndTime),
                    new SqlParameter("@IsEnabled",model.IsEnabled),
                    new SqlParameter("@Sort",model.Sort),
                    new SqlParameter("@LinkType",model.LinkType),
                    new SqlParameter("@Link",model.Link),
                    new SqlParameter("@Type",model.Type)
                };
                return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, sqlPrams));
            }
        }

        /// <summary>
        /// 删除banner
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteCarProductsBanner(int id)
        {
            using (var conn = new SqlConnection(StrConn))
            {
                const string sql = @"DELETE FROM Configuration.dbo.CarProductsBanner WHERE PKID = @PKID";
                return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, new SqlParameter("@PKID", id)) > 0;
            }
        }

        /// <summary>
        /// 更新banner
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool UpdateCarProductsBanner(CarProductsBanner model)
        {
            using (var conn = new SqlConnection(StrConn))
            {
                const string sql = @"UPDATE   CarProductsBanner
                         SET FKFloorID = @FKFloorID,
                         Name = @Name,
                         ImgUrl = @ImgUrl,
                         StartTime = @StartTime,
                         EndTime = @EndTime,
                         IsEnabled = @IsEnabled,
                         Sort = @Sort,
                         LinkType = @LinkType,
                         Link = @Link,
                         Type = @Type,
                         UpdateTime = GETDATE()
                         WHERE PKID = @PKID;";
                var sqlPrams = new[]
                {
                    new SqlParameter("@FKFloorID",model.FKFloorID),
                    new SqlParameter("@Name",model.Name),
                    new SqlParameter("@ImgUrl",model.ImgUrl),
                    new SqlParameter("@StartTime",model.StartTime),
                    new SqlParameter("@EndTime",model.EndTime),
                    new SqlParameter("@IsEnabled",model.IsEnabled),
                    new SqlParameter("@Sort",model.Sort),
                    new SqlParameter("@LinkType",model.LinkType),
                    new SqlParameter("@Link",model.Link),
                    new SqlParameter("@Type",model.Type),
                    new SqlParameter("@PKID",model.PKID)
                };
                return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlPrams) > 0;
            }
        }

        /// <summary>
        /// 是否存在相同排序
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool IsExistEqualSort(CarProductsBanner model)
        {
            using (var conn = new SqlConnection(StrConn))
            {
                var sql = @"SELECT  COUNT(0)
                            FROM    Configuration..CarProductsBanner
                            WHERE   Type = @Type
                                    AND IsEnabled = 1
		                            AND Sort = @Sort
                                    AND ( StartTime IS NULL
                                          OR EndTime IS NULL
                                          OR StartTime < GETDATE()
                                          AND EndTime > GETDATE()
                                        )";
                sql += (model.PKID.HasValue && model.PKID.Value > 0) ? $" AND PKID != {model.PKID.Value}" : "";
                var parameters = new SqlParameter[]
                {
                new SqlParameter("@Type", model.Type),
                new SqlParameter("@Sort", model.Sort)
                };
                var count = SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters);
                return Convert.ToInt32(count) > 0;
            }
        }

        /// <summary>
        /// 查询相同类型banner数量
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int SelectCarProductsBannerCount(CarProductsBanner model)
        {
            using (var conn = new SqlConnection(StrConn))
            {
                var sql = @"SELECT  COUNT(0)
                            FROM    Configuration..CarProductsBanner
                            WHERE   Type = @Type
                                    AND IsEnabled = 1
                                    AND ( StartTime IS NULL
                                          OR EndTime IS NULL
                                          OR StartTime < GETDATE()
                                          AND EndTime > GETDATE()
                                        )";
                sql += (model.PKID.HasValue && model.PKID.Value > 0) ? $" AND PKID != {model.PKID.Value}" : "";
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@Type", model.Type)
                };
                var count = SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters);
                return Convert.ToInt32(count);
            }
        }

        /// <summary>
        /// 查询模块信息
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<CarProductsHomePageModuleConfig> GetHomePageModuleConfigs(int type = 0)
        {
            using (var conn = new SqlConnection(StrConn))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Type", type);
                return conn.Query<CarProductsHomePageModuleConfig>(@"SELECT  HPM.*
                                                                FROM    Configuration..CarProductsHomePageConfig HP WITH(NOLOCK)
                                                                        JOIN Configuration..CarProductsHomePageModuleConfig HPM WITH(NOLOCK) ON HPM.FKHomePageID = HP.PKID AND HPM.ModuleType = @Type
                                                                WHERE   HP.IsEnabled = 1;", parameters).ToList();
            }
        }

        /// <summary>
        /// 修改模块信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool UpdateHomePageModuleConfig(CarProductsHomePageModuleConfig model)
        {
            using (var conn = new SqlConnection(StrConn))
            {
                const string sql = @"UPDATE Configuration..CarProductsHomePageModuleConfig
                                SET ModuleName = @ModuleName,IsEnabled = @IsEnabled,UpdateTime = GETDATE()
                                WHERE PKID = @PKID";
                var sqlPrams = new[]
                    {
                    new SqlParameter("@ModuleName",model.ModuleName),
                    new SqlParameter("@IsEnabled",model.IsEnabled),
                    new SqlParameter("@PKID",model.PKID)
                };
                return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlPrams) > 0;
            }
        }

        /// <summary>
        /// 获取楼层列表
        /// </summary>
        /// <returns></returns>
        public List<CarProductsFloor> GetFloorList()
        {
            using (var conn = new SqlConnection(StrConn))
            {
                return conn.Query<CarProductsFloor>(@"SELECT  F.*,B.ImgUrl
                                                FROM    Configuration.dbo.CarProductsFloor F WITH(NOLOCK)
                                                LEFT JOIN Configuration.dbo.CarProductsBanner B WITH(NOLOCK) ON B.FKFloorID = F.PKID AND B.IsEnabled = 1").ToList();
            }
        }

        /// <summary>
        /// 获取楼层子级列表
        /// </summary>
        /// <param name="fkFloorId"></param>
        /// <returns></returns>
        public List<CarProductsFloorConfig> GetFloorConfigList(int fkFloorId)
        {
            using (var conn = new SqlConnection(StrConn))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@FKFloorID", fkFloorId);
                return conn.Query<CarProductsFloorConfig>("SELECT * FROM Configuration.dbo.CarProductsFloorConfig WITH(NOLOCK) WHERE Code IS NOT NULL AND Code != '' AND FKFloorID = @FKFloorID", parameters).ToList();
            }
        }

        /// <summary>
        /// 获取Floor实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static CarProductsFloor GetCarProductsFloorEntity(int id)
        {
            using (var conn = new SqlConnection(StrConn))
            {
                string sqlString = @"SELECT * FROM Configuration.dbo.CarProductsFloor (NOLOCK) WHERE PKID=@PKID";
                return conn.Query<CarProductsFloor>(sqlString, new CarProductsFloor { PKID = id }).FirstOrDefault();
            }
        }

        /// <summary>
        /// 获取FloorConfig实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static CarProductsFloorConfig GetCarProductsFloorConfigEntity(int id)
        {
            using (var conn = new SqlConnection(StrConn))
            {
                string sqlString = @"SELECT * FROM Configuration.dbo.CarProductsFloorConfig (NOLOCK) WHERE PKID=@PKID";
                return conn.Query<CarProductsFloorConfig>(sqlString, new CarProductsFloorConfig { PKID = id }).FirstOrDefault();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="floorId"></param>
        /// <returns></returns>
        public static List<CarProductsFloorConfig> GetCarProductsFloorConfigsList(int floorId)
        {
            using (var conn = new SqlConnection(StrConn))
            {
                string sqlString = @"SELECT * FROM Configuration.dbo.CarProductsFloorConfig (NOLOCK) WHERE FKFloorID=@FKFloorID";
                return conn.Query<CarProductsFloorConfig>(sqlString, new CarProductsFloorConfig { FKFloorID = floorId }).ToList();
            }
        }

        /// <summary>
        /// 添加车品信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int InsertCarProductsFloor(CarProductsFloor model)
        {
            using (var conn = new SqlConnection(StrConn))
            {
                const string sql = @"INSERT INTO Configuration.dbo.CarProductsFloor
                        ( DisplayName ,
                          Name ,
                          Code ,
                          Sort ,
                          IsEnabled ,
                          CreateTime ,
                          UpdateTime
                        )
                        VALUES  ( @DisplayName ,
                                  @Name ,
                                  @Code ,
                                  @Sort ,
                                  @IsEnabled ,
                                  @CreateTime ,
                                  @UpdateTime
                        );Select @@identity;";
                var sqlPrams = new[]
                {
                    new SqlParameter("@DisplayName",model.DisplayName),
                    new SqlParameter("@Name",model.Name),
                    new SqlParameter("@Code",model.Code),
                    new SqlParameter("@IsEnabled",model.IsEnabled),
                    new SqlParameter("@Sort",model.Sort),
                    new SqlParameter("@CreateTime",DateTime.Now),
                    new SqlParameter("@UpdateTime",DateTime.Now)
                };
                return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, sqlPrams));
            }
        }

        /// <summary>
        /// 更新车品信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool UpdateCarProductsFloor(CarProductsFloor model)
        {
            using (var conn = new SqlConnection(StrConn))
            {
                const string sql = @"UPDATE   Configuration.dbo.CarProductsFloor
                         SET DisplayName = @DisplayName,
                         Name = @Name,
                         Code = @Code,
                         IsEnabled = @IsEnabled,
                         Sort = @Sort,
                         UpdateTime = GETDATE()
                         WHERE PKID = @PKID;";
                var sqlPrams = new[]
                {
                    new SqlParameter("@DisplayName",model.DisplayName),
                    new SqlParameter("@Name",model.Name),
                    new SqlParameter("@Code",model.Code),
                    new SqlParameter("@UpdateTime",DateTime.Now),
                    new SqlParameter("@IsEnabled",model.IsEnabled),
                    new SqlParameter("@Sort",model.Sort),
                    new SqlParameter("@PKID",model.PKID)
                };
                return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlPrams) > 0;
            }
        }

        /// <summary>
        /// 添加车品配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool InsertCarProductsFloorConfig(CarProductsFloorConfig model)
        {
            using (var conn = new SqlConnection(StrConn))
            {
                const string sql = @"INSERT INTO Configuration..CarProductsFloorConfig
                        ( FKFloorID ,
                          Name ,
                          Code ,
                          PidCount ,
                          PIDS ,
                          CreateTime ,
                          UpdateTime ,
                          IsEnabled
                        )
                        VALUES  ( @FKFloorID ,
                                  @Name ,
                                  @Code ,
                                  @PidCount ,
                                  @PIDS ,
                                  @CreateTime ,
                                  @UpdateTime ,
                                  @IsEnabled
                        )";
                var sqlPrams = new[]
                {
                    new SqlParameter("@FKFloorID",model.FKFloorID),
                    new SqlParameter("@Name",model.Name),
                    new SqlParameter("@Code",model.Code),
                    new SqlParameter("@PidCount",model.PidCount),
                    new SqlParameter("@PIDS",model.PIDS),
                    new SqlParameter("@CreateTime",DateTime.Now),
                    new SqlParameter("@UpdateTime",DateTime.Now),
                    new SqlParameter("@IsEnabled",model.IsEnabled)
                };
                return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlPrams) > 0;
            }
        }

        /// <summary>
        /// 更新车品配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool UpdateCarProductsFloorConfig(CarProductsFloorConfig model)
        {
            using (var conn = new SqlConnection(StrConn))
            {
                const string sql = @"UPDATE   Configuration..CarProductsFloorConfig
                         SET FKFloorID = @FKFloorID,
                         Name = @Name,
                         Code = @Code,
                         PidCount = @PidCount,
                         PIDS = @PIDS,
                         IsEnabled = @IsEnabled,
                         UpdateTime = GETDATE()
                         WHERE PKID = @PKID;";
                var sqlPrams = new[]
                {
                    new SqlParameter("@FKFloorID",model.FKFloorID),
                    new SqlParameter("@Name",model.Name),
                    new SqlParameter("@Code",model.Code),
                    new SqlParameter("@PidCount",model.PidCount),
                    new SqlParameter("@PIDS",model.PIDS),
                    new SqlParameter("@IsEnabled",model.IsEnabled),
                    new SqlParameter("@PKID",model.PKID)
                };
                return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlPrams) > 0;
            }
        }

        /// <summary>
        /// 删除车品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteCarProductsFloor(int id)
        {
            using (var conn = new SqlConnection(StrConn))
            {
                const string sql = @"DELETE FROM Configuration.dbo.CarProductsFloor WHERE PKID = @PKID";
                return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, new SqlParameter("@PKID", id)) > 0;
            }
        }

        /// <summary>
        /// 删除车品配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteCarProductsFloorConfig(int id)
        {
            using (var conn = new SqlConnection(StrConn))
            {
                const string sql = @"DELETE FROM Configuration.dbo.CarProductsFloorConfig WHERE PKID = @PKID";
                return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, new SqlParameter("@PKID", id)) > 0;
            }
        }

        /// <summary>
        /// 删除车品配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteCarProductsFloorConfigByFloorId(int floorId)
        {
            using (var conn = new SqlConnection(StrConn))
            {
                const string sql = @"DELETE FROM Configuration.dbo.CarProductsFloorConfig WHERE FKFloorID = @FKFloorID";
                return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, new SqlParameter("@FKFloorID", floorId)) > 0;
            }
        }

        /// <summary>
        /// 是否存在相同排序
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool IsExistEqualFloorSort(CarProductsFloor model)
        {
            using (var conn = new SqlConnection(StrConn))
            {
                var sql = @"SELECT  COUNT(0)
                            FROM    Configuration..CarProductsFloor
                            WHERE   IsEnabled = 1
		                            AND Sort = @Sort";
                sql += (model.PKID.HasValue && model.PKID.Value > 0) ? $" AND PKID != {model.PKID.Value}" : "";
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@Sort", model.Sort)
                };
                var count = SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters);
                return Convert.ToInt32(count) > 0;
            }
        }

        /// <summary>
        /// 是否已经存在相同楼层
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool IsExistEqualFloorName(CarProductsFloor model)
        {
            using (var conn = new SqlConnection(StrConn))
            {
                var sql = @"SELECT  COUNT(0)
                            FROM    Configuration..CarProductsFloor
                            WHERE   IsEnabled = 1
                                    AND Name = @Name
                                    AND Code = @Code";
                sql += (model.PKID.HasValue && model.PKID.Value > 0) ? $" AND PKID != {model.PKID.Value}" : "";
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@Name", model.Name),
                    new SqlParameter("@Code", model.Code)
                };
                var count = SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters);
                return Convert.ToInt32(count) > 0;
            }
        }

    }
}
