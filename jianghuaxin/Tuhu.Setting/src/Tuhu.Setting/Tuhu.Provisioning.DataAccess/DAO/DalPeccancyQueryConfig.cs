using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;
using static Tuhu.Provisioning.DataAccess.Entity.PeccancyQueryModel;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalPeccancyQueryConfig
    {
        private static readonly string grReadconnectionString;
        private static readonly string tuhu_logConnString;
        static DalPeccancyQueryConfig()
        {
            var gungnirConnRead = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
            grReadconnectionString = SecurityHelp.IsBase64Formatted(gungnirConnRead) ? SecurityHelp.DecryptAES(gungnirConnRead) : gungnirConnRead;
            var tuhu_logConnRead= ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            tuhu_logConnString = SecurityHelp.IsBase64Formatted(tuhu_logConnRead) ? SecurityHelp.DecryptAES(tuhu_logConnRead) : tuhu_logConnRead;
        }
        #region 违章查询省份配置

        /// <summary>
        /// 添加违章查询省份配置
        /// </summary>
        /// <param name="dbhelper"></param>
        /// <param name="peccancyQueryProvinceModel"></param>
        /// <returns></returns>
        public static bool AddPeccancyProvinceConfig(SqlDbHelper dbhelper, PeccancyQueryProvinceModel peccancyQueryProvinceModel)
        {
            var sql = @"INSERT  INTO Gungnir..tbl_peccancyProvince
                                ( ID ,
                                Province ,
                                ProvinceSimple
                                )
                        VALUES  ( @ProvinceId ,
                                @ProvinceName ,
                                @ProvinceSimpleName
                                );";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@ProvinceId", peccancyQueryProvinceModel.ProvinceId);
                cmd.Parameters.AddWithValue("@ProvinceName", peccancyQueryProvinceModel.ProvinceName);
                cmd.Parameters.AddWithValue("@ProvinceSimpleName", peccancyQueryProvinceModel.ProvinceSimpleName);
                var count = dbhelper.ExecuteNonQuery(cmd);
                return count > 0;
            }
        }

        /// <summary>
        /// 删除违章查询省份配置
        /// </summary>
        /// <param name="dbhelper"></param>
        /// <param name="provinceId"></param>
        /// <returns></returns>
        public static bool DeletePeccancyProvinceConfig(SqlDbHelper dbhelper, int provinceId)
        {
            var sql = @"DELETE  FROM Gungnir..tbl_peccancyProvince
                        WHERE   ID = @ProvinceId;";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@ProvinceId", provinceId);
                var count = dbhelper.ExecuteNonQuery(cmd);
                return count > 0;
            }
        }

        /// <summary>
        /// 更新违章查询省份配置
        /// </summary>
        /// <param name="dbhelper"></param>
        /// <param name="peccancyQueryProvinceModel"></param>
        /// <returns></returns>
        public static bool UpdatePeccancyProvinceConfig(SqlDbHelper dbhelper, PeccancyQueryProvinceModel peccancyQueryProvinceModel)
        {
            var sql = @"UPDATE  Gungnir..tbl_peccancyProvince
                        SET     Province = @ProvinceName ,
                                ProvinceSimple = @ProvinceSimpleName
                        WHERE   ID = @ProvinceId;";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@ProvinceName", peccancyQueryProvinceModel.ProvinceName);
                cmd.Parameters.AddWithValue("@ProvinceSimpleName", peccancyQueryProvinceModel.ProvinceSimpleName);
                cmd.Parameters.AddWithValue("@ProvinceId", peccancyQueryProvinceModel.ProvinceId);
                var count = dbhelper.ExecuteNonQuery(cmd);
                return count > 0;
            }
        }

        /// <summary>
        /// 查询违章查询省份配置
        /// </summary>
        /// <param name="provinceId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static Tuple<List<PeccancyQueryProvinceModel>, int> SelectPeccancyProvinceConfig(int provinceId, int pageIndex, int pageSize)
        {
            var sql = @"SELECT  @Total = COUNT(1)
                        FROM Gungnir..tbl_PeccancyProvince AS s WITH (NOLOCK ) 
                        WHERE (@ProvinceId < 1 OR s.ID= @ProvinceId);
                        SELECT  ID AS ProvinceId ,
                                Province AS ProvinceName ,
                                ProvinceSimple AS ProvinceSimpleName
                        FROM    Gungnir..tbl_PeccancyProvince WITH ( NOLOCK )
                        WHERE   @ProvinceId < 1 OR ID= @ProvinceId
                        ORDER BY ID
                        OFFSET ( @PageIndex - 1 ) * @PageSize ROWS  FETCH NEXT @PageSize ROWS
                        ONLY;";
            int totalCount = 0;
            using (var dbhelper = new SqlDbHelper(grReadconnectionString))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", pageSize);
                    cmd.Parameters.AddWithValue("@ProvinceId", provinceId);
                    cmd.Parameters.Add(new SqlParameter("@Total", SqlDbType.Int) { Direction = ParameterDirection.Output });
                    var result = dbhelper.ExecuteDataTable(cmd).ConvertTo<PeccancyQueryProvinceModel>().ToList();
                    totalCount = (int)cmd.Parameters["@Total"].Value;
                    return Tuple.Create(result, totalCount);
                }
            }
        }

        /// <summary>
        /// 获取所有配置的省份
        /// </summary>
        /// <returns></returns>
        public static List<PeccancyRegionMiniModel> GetAllPeccancyProvinces()
        {
            #region sql
            var sql = @"SELECT  s.ID AS RegionId ,
                               s.Province AS RegionName 
                       FROM    Gungnir..tbl_PeccancyProvince AS s WITH ( NOLOCK );";
            #endregion
            using (var dbhelper = new SqlDbHelper(grReadconnectionString))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    var result = dbhelper.ExecuteDataTable(cmd).ConvertTo<PeccancyRegionMiniModel>().ToList();
                    return result;
                }
            }
        }

        /// <summary>
        /// 根据省份Id获取该省份下的城市
        /// </summary>
        /// <param name="provinceId"></param>
        /// <returns></returns>
        public static List<PeccancyQueryCityModel> GetPeccancyCitiesByProvinceId(int provinceId)
        {
            #region sql
            var sql = @"SELECT
                          s.ID   AS CityId,
                          s.provinceID,
                          s.cityCode,
                          s.city AS CityName,
                          s.needEngine,
                          s.engineLen,
                          s.needFrame,
                          s.frameLen,
                          s.isEnabled
                        FROM Gungnir..tbl_PeccancyCity AS s WITH ( NOLOCK )
                        WHERE s.provinceID = @ProvinceId;";
            #endregion
            using (var dbhelper = new SqlDbHelper(grReadconnectionString))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@ProvinceId", provinceId);
                    var result = dbhelper.ExecuteDataTable(cmd).ConvertTo<PeccancyQueryCityModel>().ToList();
                    return result;
                }
            }
        }

        /// <summary>
        /// 判断省份配置是否重复
        /// </summary>
        /// <param name="peccancyQueryModel"></param>
        /// <returns></returns>
        public static PeccancyQueryProvinceModel GetRepeatPeccancyProvinceConfig(PeccancyQueryProvinceModel peccancyQueryModel)
        {
            var sql = @"SELECT  ID AS ProvinceId ,
                                Province AS ProvinceName ,
                                ProvinceSimple AS ProvinceSimpleName
                        FROM    Gungnir..tbl_PeccancyProvince WITH ( NOLOCK )
                        WHERE   ID = @ProvinceId OR Province=@ProvinceName OR ProvinceSimple=@ProvinceSimpleName;";
            using (var dbhelper = new SqlDbHelper(grReadconnectionString))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@ProvinceId", peccancyQueryModel.ProvinceId);
                    cmd.Parameters.AddWithValue("@ProvinceName", peccancyQueryModel.ProvinceName);
                    cmd.Parameters.AddWithValue("@ProvinceSimpleName", peccancyQueryModel.ProvinceSimpleName);
                    var result = dbhelper.ExecuteDataTable(cmd).ConvertTo<PeccancyQueryProvinceModel>().FirstOrDefault();
                    return result;
                }
            }
        }

        /// <summary>
        /// 根据省份Id获取省份配置
        /// </summary>
        /// <param name="provinceId"></param>
        /// <returns></returns>
        public static PeccancyQueryProvinceModel GetPeccancyProvinceConfigByProvinceId(int provinceId)
        {
            var sql = @"SELECT  ID AS ProvinceId ,
                                Province AS ProvinceName ,
                                ProvinceSimple AS ProvinceSimpleName
                        FROM    Gungnir..tbl_PeccancyProvince WITH ( NOLOCK )
                        WHERE   ID = @ProvinceId;";
            using (var dbhelper = new SqlDbHelper(grReadconnectionString))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@ProvinceId", provinceId);
                    var result = dbhelper.ExecuteDataTable(cmd).ConvertTo<PeccancyQueryProvinceModel>().FirstOrDefault();
                    return result;
                }
            }
        }
        #endregion


        #region 违章查询城市配置

        /// <summary>
        /// 添加违章查询城市配置
        /// </summary>
        /// <param name="dbhelper"></param>
        /// <param name="peccancyQueryCityModel"></param>
        /// <returns></returns>
        public static bool AddPeccancyCityConfig(SqlDbHelper dbhelper, PeccancyQueryCityModel peccancyQueryCityModel)
        {
            #region SQL
            var sql = @"INSERT  INTO Gungnir..tbl_PeccancyCity
                                ( ID ,
                                provinceID ,
                                cityCode ,
                                city ,
                                engineLen ,
                                frameLen ,
                                needEngine,
                                needFrame,
                                isEnabled
                                )
                        VALUES  ( @CityId , 
                                @ProvinceId ,
                                @CityCode ,
                                @CityName ,
                                @EngineLen ,
                                @FrameLen ,
                                @NeedEngine ,
                                @NeedFrame ,
                                @IsEnabled 
                                );";
            #endregion
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@CityId", peccancyQueryCityModel.CityId);
                cmd.Parameters.AddWithValue("@ProvinceId", peccancyQueryCityModel.ProvinceId);
                cmd.Parameters.AddWithValue("@CityCode", peccancyQueryCityModel.CityCode);
                cmd.Parameters.AddWithValue("@CityName", peccancyQueryCityModel.CityName);
                cmd.Parameters.AddWithValue("@NeedEngine", peccancyQueryCityModel.NeedEngine);
                cmd.Parameters.AddWithValue("@NeedFrame", peccancyQueryCityModel.NeedFrame);
                cmd.Parameters.AddWithValue("@EngineLen", peccancyQueryCityModel.EngineLen);
                cmd.Parameters.AddWithValue("@FrameLen", peccancyQueryCityModel.FrameLen);
                cmd.Parameters.AddWithValue("@IsEnabled", peccancyQueryCityModel.IsEnabled);
                var count = dbhelper.ExecuteNonQuery(cmd);
                return count > 0;
            };
        }

        /// <summary>
        /// 删除违章查询城市配置
        /// </summary>
        /// <param name="dbhelper"></param>
        /// <param name="cityId"></param>
        /// <returns></returns>
        public static bool DeletePeccancyCityConfigByCityId(SqlDbHelper dbhelper, int cityId)
        {
            var sql = @"DELETE  FROM Gungnir..tbl_PeccancyCity
                        WHERE   ID = @CityId;";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@CityId", cityId);
                var count = dbhelper.ExecuteNonQuery(cmd);
                return count > 0;
            }
        }

        /// <summary>
        /// 删除省份下的城市配置
        /// </summary>
        /// <param name="dbhelper"></param>
        /// <param name="provinceId"></param>
        /// <returns></returns>
        public static bool DeletePeccancyCityConfigByProvinceId(SqlDbHelper dbhelper, int provinceId)
        {
            var sql = @"DELETE  FROM Gungnir..tbl_PeccancyCity
                        WHERE   provinceID = @ProvinceId;";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@ProvinceId", provinceId);
                var count = dbhelper.ExecuteNonQuery(cmd);
                return count > 0;
            }
        }

        /// <summary>
        /// 更新违章查询城市配置
        /// </summary>
        /// <param name="dbhelper"></param>
        /// <param name="peccancyQueryCityModel"></param>
        /// <returns></returns>
        public static bool UpdatePeccancyCityConfig(SqlDbHelper dbhelper, PeccancyQueryCityModel peccancyQueryCityModel)
        {
            #region sql
            var sql = @"UPDATE  Gungnir..tbl_PeccancyCity
                        SET     provinceId = @ProvinceId ,
                                cityCode = @CityCode ,
                                city = @CityName ,
                                needEngine = @NeedEngine ,
                                needFrame = @NeedFrame ,
                                engineLen = @EngineLen ,
                                frameLen = @FrameLen ,
                                isEnabled = @IsEnabled                                
                        WHERE   ID = @CityId;";
            #endregion
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@ProvinceId", peccancyQueryCityModel.ProvinceId);
                cmd.Parameters.AddWithValue("@CityId", peccancyQueryCityModel.CityId);
                cmd.Parameters.AddWithValue("@CityCode", peccancyQueryCityModel.CityCode);
                cmd.Parameters.AddWithValue("@CityName", peccancyQueryCityModel.CityName);
                cmd.Parameters.AddWithValue("@NeedEngine", peccancyQueryCityModel.NeedEngine);
                cmd.Parameters.AddWithValue("@NeedFrame", peccancyQueryCityModel.NeedFrame);
                cmd.Parameters.AddWithValue("@EngineLen", peccancyQueryCityModel.EngineLen);
                cmd.Parameters.AddWithValue("@FrameLen", peccancyQueryCityModel.FrameLen);
                cmd.Parameters.AddWithValue("@IsEnabled", peccancyQueryCityModel.IsEnabled);
                var count = dbhelper.ExecuteNonQuery(cmd);
                return count > 0;
            }
        }

        /// <summary>
        /// 查询违章查询城市配置
        /// </summary>
        /// <param name="provinceId"></param>
        /// <param name="cityId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static Tuple<List<PeccancyQueryCityModel>, int> SelectPeccancyCityConfig(int provinceId, int cityId, int pageIndex, int pageSize)
        {
            var sql = @"SELECT  @Total = COUNT(1)
                        FROM Gungnir..tbl_PeccancyCity AS s WITH (NOLOCK ) 
                        WHERE (@ProvinceId < 1 OR  s.ProvinceId = @ProvinceId) AND (@CityId < 1 OR s.ID=@CityId);
                        SELECT  p.ID AS ProvinceId,
		                        p.Province AS ProvinceName,
		                        p.ProvinceSimple AS ProvinceSimpleName,
                                c.ID AS CityId ,
                                c.cityCode AS CityCode ,
                                c.city AS CityName ,
                                c.needEngine AS NeedEngine ,
                                c.needFrame AS NeedFrame ,
                                c.engineLen AS EngineLen ,
                                c.frameLen AS FrameLen ,
                                c.isEnabled AS IsEnabled
                                FROM Gungnir..tbl_PeccancyCity AS c WITH (NOLOCK)
                                LEFT JOIN  Gungnir..tbl_PeccancyProvince AS p WITH ( NOLOCK ) ON c.ProvinceId = p.ID
                                WHERE   (@ProvinceId < 1 OR  c.ProvinceId = @ProvinceId) AND (@CityId < 1 OR c.ID=@CityId)
                                ORDER BY c.provinceID
                                OFFSET ( @PageIndex - 1 ) * @PageSize ROWS  FETCH NEXT @PageSize ROWS
                                ONLY;";
            int totalCount = 0;
            using (var dbhelper = new SqlDbHelper(grReadconnectionString))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@ProvinceId", provinceId);
                    cmd.Parameters.AddWithValue("@CityId", cityId);
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", pageSize);
                    cmd.Parameters.Add(new SqlParameter("@Total", SqlDbType.Int) { Direction = ParameterDirection.Output });
                    var result = dbhelper.ExecuteDataTable(cmd).ConvertTo<PeccancyQueryCityModel>().ToList();
                    totalCount = (int)cmd.Parameters["@Total"].Value;
                    return Tuple.Create(result, totalCount);
                }
            }
        }

        /// <summary>
        /// 获取省份下的城市数量
        /// </summary>
        /// <param name="provinceId"></param>
        /// <returns></returns>
        public static int GetPeccancyCityConfigCountByPrvinceId(int provinceId)
        {
            var sql = @"SELECT  COUNT(1)
                        FROM    Gungnir..tbl_PeccancyCity WITH ( NOLOCK )
                        WHERE   ProvinceId = @ProvinceId";
            using (var dbhelper = new SqlDbHelper(grReadconnectionString))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@ProvinceId", provinceId);
                    return Convert.ToInt32(dbhelper.ExecuteScalar(cmd));
                }
            }
        }

        /// <summary>
        /// 根据城市Id获取城市配置
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        public static PeccancyQueryCityModel GetPeccancyCityConfigByCityId(int cityId)
        {
            var sql = @" SELECT  p.ID AS ProvinceId ,
                                 p.Province AS ProvinceName ,
                                 p.ProvinceSimple AS ProvinceSimpleName ,
                                 c.ID AS CityId ,
                                 c.cityCode AS CityCode ,
                                 c.city AS CityName ,
                                 c.needEngine AS NeedEngine ,
                                 c.needFrame AS NeedFrame ,
                                 c.engineLen AS EngineLen ,
                                 c.frameLen AS FrameLen ,
                                 c.IsEnabled
                                 FROM    Gungnir..tbl_PeccancyProvince AS p WITH ( NOLOCK )
                                 RIGHT JOIN Gungnir..tbl_PeccancyCity AS c WITH ( NOLOCK ) ON c.provinceID = p.ID
                                 WHERE   c.ID = @CityId;";
            using (var dbhelper = new SqlDbHelper(grReadconnectionString))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@CityId", cityId);
                    var cityConfig = dbhelper.ExecuteDataTable(cmd).ConvertTo<PeccancyQueryCityModel>().SingleOrDefault();
                    return cityConfig;
                }
            }
        }

        #endregion

        #region 查看操作日志
        /// <summary>
        /// 查看违章配置操作日志
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="identityId"></param>
        /// <returns></returns>
        public static List<PeccancyConfigOprLogModel> SelectPeccancyConfigOprLog(string logType,string identityId)
        {
            var sql = @"SELECT
                          s.PKID,
                          s.IdentityId,
                          s.OperationType,
                          s.Operator,
                          s.Remarks,
                          s.CreateDateTime
                        FROM Tuhu_log..PeccancyConfigOprLog AS s WITH ( NOLOCK )
                        WHERE s.LogType = @LogType
                              AND s.IdentityId = @IdentityId
                        ORDER BY  s.CreateDateTime DESC";
            using (var dbhelper = new SqlDbHelper(tuhu_logConnString))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@LogType", logType);
                    cmd.Parameters.AddWithValue("@IdentityId", identityId);
                    var result = dbhelper.ExecuteDataTable(cmd).ConvertTo<PeccancyConfigOprLogModel>().ToList();
                    return result;
                }
            }
        }

        /// <summary>
        /// 查看违章配置操作记录详情
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static PeccancyConfigOprLogModel GetPeccancyConfigOprLog(int pkid)
        {
            var sql = @"SELECT
                          s.OldValue,
                          s.NewValue
                        FROM Tuhu_log..PeccancyConfigOprLog AS s WITH ( NOLOCK )
                        WHERE s.PKID = @PKID;";
            using (var dbhelper = new SqlDbHelper(tuhu_logConnString))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@PKID", pkid);
                    var result = dbhelper.ExecuteDataTable(cmd).ConvertTo<PeccancyConfigOprLogModel>().FirstOrDefault();
                    return result;
                }
            }
        }
        #endregion
    }
}
