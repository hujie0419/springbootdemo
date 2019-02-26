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
    public class DalSprayPaintVehicle
    {
        /// <summary>
        /// 判断对应车型是否在其他类型中配置
        /// </summary>
        /// <param name="vehicleIds"></param>
        /// <returns></returns>
        public static bool IsRepeatPaintVehicleLevelConfig(SqlConnection conn, string vehicleIds, string type)
        {
            var sql = @"WITH T
                        AS (SELECT *
                            FROM Configuration..SplitString(@vehicleIds, N',', 1) )
                        SELECT COUNT(1)
                        FROM Configuration..PaintVehicleLevelConfig AS SC WITH (NOLOCK)
                            INNER JOIN T
                                ON SC.VehicleId = T.Item
                        WHERE SC.Type <> @type;
                        ";
            var parameters = new[]
            {
                new  SqlParameter("@vehicleIds", vehicleIds),
                new  SqlParameter("@type", type)
            };
            var obj = SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters);
            return obj != null && Convert.ToInt32(obj) > 0;
        }
        /// <summary>
        /// 根据车型品牌首字母获取车型档次配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="initalWord"></param>
        /// <returns></returns>
        public static List<VehicleLevelViewModel> SelectVehicleLevel(SqlConnection conn, string initalWord)
        {
            var sql = @"SELECT     vt.Brand,
                                   vt.ProductID AS VehicleId,
                                   a.VehicleLevel,
                                   a.Type,
                                   vt.Vehicle AS VehicleSeries
                            FROM Configuration..PaintVehicleLevelConfig AS a (NOLOCK)
                                RIGHT JOIN Gungnir..tbl_Vehicle_Type AS vt (NOLOCK)
                                    ON a.VehicleId = vt.ProductID COLLATE Chinese_PRC_CI_AS
                            WHERE  (
                                      vt.Brand LIKE @initalWord
                                      OR @initalWord IS NULL
                                      OR @initalWord = N''
                                  );";
            var parameters = new[]
            {
                new  SqlParameter("@initalWord",string.IsNullOrEmpty(initalWord)?string.Empty:$"{initalWord}%")
            };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters).ConvertTo<VehicleLevelViewModel>().ToList();
        }

        /// <summary>
        /// 当前等级和类型的车型档次配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="type"></param>
        /// <param name="vehicleLevel"></param>
        /// <returns></returns>
        public static List<VehicleLevelViewModel> SelectSprayPaintVehicleByLevel(SqlConnection conn, string type, string vehicleLevel)
        {
            var sql = @"SELECT
                      sc.VehicleId,
                      sc.VehicleLevel,
                      sc.Type,
                      vt.Vehicle AS VehicleSeries
                    FROM Configuration..PaintVehicleLevelConfig
                      AS sc ( NOLOCK ) INNER JOIN Gungnir..tbl_Vehicle_Type AS vt ( NOLOCK )
                        ON sc.VehicleId = vt.ProductID
                      COLLATE Chinese_PRC_CI_AS
                    WHERE Type = @Type 
                          AND sc.VehicleLevel = @VehicleLevel";
            var parameters = new SqlParameter[] {
            new SqlParameter("@Type", type),
            new SqlParameter("@VehicleLevel", vehicleLevel)};
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters).ConvertTo<VehicleLevelViewModel>().ToList();
        }

        /// <summary>
        /// 根据品牌首字母查询车型信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="initalWord"></param>
        /// <returns></returns>
        public static List<TreeItem> SelectVehicleInfo(SqlConnection conn, string initalWord)
        {
            var result = new List<TreeItem>();
            var sql = @"SELECT vt.ProductID, vt.Vehicle, vt.Brand
                        FROM Gungnir..tbl_Vehicle_Type (NOLOCK) AS vt 
                        WHERE vt.Brand LIKE @Alpha
                        ORDER BY vt.Brand ";
            SqlParameter parameter = new SqlParameter("@Alpha", initalWord+'%');
            var dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameter);
            if (dt == null || dt.Rows.Count == 0)
                return result;
            else
            {
                var data = dt.AsEnumerable().GroupBy(p => p["Brand"].ToString()).ToDictionary(o => o.Key, o => o.Select(t => t));
                foreach (var item in data)
                {
                    var vehicle = new TreeItem
                    {
                        id = item.Key,
                        name = item.Key,
                        check = "false",
                        disabled = string.Empty
                    };
                    foreach (var node in item.Value)
                    {
                        var child = new TreeItem
                        {
                            id = node["ProductID"].ToString(),
                            name = node["Vehicle"].ToString(),
                            check = "false",
                            disabled = string.Empty
                        };
                        vehicle.children.Add(child);
                    }
                    result.Add(vehicle);
                }
                return result;        
            }               
        }

        /// <summary>
        /// 新增喷漆车型档次配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="vehicleLevel"></param>
        /// <param name="vehicleIds"></param>
        /// <returns></returns>
        public static bool AddPaintVehicleLevel(SqlConnection conn, string vehicleLevel, string vehicleIds,string type)
        {
            var sql = @"WITH VehicleIds
            AS ( SELECT   *
               FROM  Gungnir..SplitString(@VehicleIds, ',', 1)
             )
            INSERT  INTO Configuration..PaintVehicleLevelConfig
            ( VehicleId ,
              VehicleLevel ,
              type,
              CreateTime ,
              UpdateTime
            )
            SELECT  Item ,
                    @VehicleLevel ,
                    @type,
                    GETDATE() ,
                    GETDATE()
            FROM  VehicleIds
            WHERE VehicleIds.Item IS NOT NULL AND VehicleIds.Item <> ''";
            SqlParameter[] parameters = {
                new SqlParameter("@VehicleLevel", vehicleLevel),
                new SqlParameter("@type", type),
                new SqlParameter("@VehicleIds", vehicleIds)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
        }

        /// <summary>
        /// 根据传入车型档次和车型品牌首字母删除喷漆车型档次
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="vehicleLevel"></param>
        /// <param name="initalWord"></param>
        /// <returns></returns>
        public static bool DeletePaintVehicleDataByInitalWord(SqlConnection conn, string vehicleLevel, string initalWord ,string type)
        {
            var sql = @"DELETE A
                        FROM Configuration..PaintVehicleLevelConfig AS A
                            JOIN Gungnir..tbl_Vehicle_Type AS B
                                ON A.VehicleId = B.ProductID COLLATE Chinese_PRC_CI_AS
                        WHERE A.VehicleLevel = @VehicleLevel
                              AND B.Brand LIKE @Alpha
                              AND
                              (
                                  A.Type = @type
                                  OR @type IS NULL
                                  OR @type = N''
                              );";
            SqlParameter[] parameters = {
                new SqlParameter("@VehicleLevel", vehicleLevel),
                new SqlParameter("@Alpha", initalWord + '%'),
                new SqlParameter("@type", string.IsNullOrWhiteSpace(type)?string.Empty:type)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
        }
        /// <summary>
        /// 根据类型删除对应的车型喷漆配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="vehicleIds"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool DeletePaintVehicleDataByType(SqlConnection conn,  string vehicleIds,string type)
        {
            var sql = @"WITH T
                        AS (SELECT *
                            FROM Configuration..SplitString(@vehicleIds, N',', 1) )
                        DELETE SC
                        FROM Configuration..PaintVehicleLevelConfig AS SC
                            INNER JOIN T
                                ON T.Item = SC.VehicleId
                        WHERE SC.Type = @type;";
            SqlParameter[] parameters = {
                new SqlParameter("@vehicleIds", vehicleIds),
                new SqlParameter("@type" ,type)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
        }

        /// <summary>
        /// 根据传入车型档次和车型品牌首字母查询喷漆车型档次
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="vehicleLevel"></param>
        /// <param name="initalWord"></param>
        /// <returns></returns>
        public static List<VehicleLevelLog> SeletePaintVehicleByInitalWord(SqlConnection conn, string vehicleLevel, string initalWord , string type)
        {
            var result = new List<VehicleLevelLog>();
            var sql = @"SELECT A.*
            FROM 
            Configuration..PaintVehicleLevelConfig AS A (NOLOCK)
            JOIN Gungnir..tbl_Vehicle_Type AS B (NOLOCK)
            ON A.VehicleId = B.ProductID COLLATE Chinese_PRC_CI_AS
            WHERE A.VehicleLevel = @VehicleLevel 
                AND B.Brand LIKE @Alpha
                AND A.Type=@Type
                ";
            SqlParameter[] parameters = {
                new SqlParameter("@VehicleLevel", vehicleLevel),
                new SqlParameter("@Alpha", initalWord + '%'),
                new SqlParameter("@Type", type),
            };
            var data = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters);
            if (data != null && data.Rows.Count > 0)
            {
                result = data.ConvertTo<VehicleLevelLog>().ToList();
            }
            return result;
        }

        /// <summary>
        /// 查询喷漆车型服务
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="vehicleLevel"></param>
        /// <returns></returns>
        public static List<PaintServiceLog> SelectPaintServiceInfo(SqlConnection conn, string vehicleLevel)
        {
            var result = new List<PaintServiceLog>();
            var sql = @"SELECT  *
            FROM  Configuration..PaintVehicleServiceConfig (NOLOCK)
            WHERE  VehicleLevel = @VehicleLevel;";
            SqlParameter parameter = new SqlParameter("@VehicleLevel", vehicleLevel);
            var data = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameter);
            if (data != null && data.Rows.Count > 0)
            {
                result = data.ConvertTo<PaintServiceLog>().ToList();
            }
            return result;
        }

        /// <summary>
        /// 删除喷漆车型服务
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="vehicleLevel"></param>
        /// <returns></returns>
        public static bool DeletePaintService(SqlConnection conn, string vehicleLevel)
        {
            var sql = @"DELETE FROM Configuration..PaintVehicleServiceConfig WHERE VehicleLevel = @VehicleLevel";
            SqlParameter parameter = new SqlParameter("@VehicleLevel", vehicleLevel);
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameter) > 0;
        }

        /// <summary>
        /// 新增喷漆车型服务
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="paintService"></param>
        /// <returns></returns>
        public static bool AddPaintVehicleServiceConfig(SqlConnection conn, PaintService paintService)
        {
            var sql = @"INSERT INTO Configuration..PaintVehicleServiceConfig
            ( VehicleLevel ,
              ServiceId ,
              DisplayIndex ,
              CreateTime ,
              UpdateTime
            )
            VALUES  ( @VehicleLevel , -- VehicleLevel - nvarchar(50)
            @ServiceId , -- ServiceId - nvarchar(50)
            @DisplayIndex , -- DisplayIndex - int
            GETDATE() , -- CreateTime - datetime
            GETDATE()  -- UpdateTime - datetime
            )";
            SqlParameter[] parameters = {
                new SqlParameter("@VehicleLevel", paintService.VehicleLevel),
                new SqlParameter("@ServiceId", paintService.ServiceId),
                new SqlParameter("@DisplayIndex", paintService.DisplayIndex) 
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
        }


        #region 根据vin码导出数据
        /// <summary>
        /// 根据tids查询车型
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="tids"></param>
        /// <returns></returns>
        public static List<VehicleInfoDetail> SelectVehicleInfoByTids(SqlConnection conn, IEnumerable<int> tids)
        {
            var sql = @"WITH    tids
          AS ( SELECT   Item AS tid
               FROM     Gungnir.dbo.SplitString(@Tids, N',', 1)
             )
    SELECT  ti.VehicleID ,
            ti.TID ,
            vt.Brand ,
            vt.Vehicle
    FROM    Gungnir..tbl_Vehicle_Type_Timing AS ti WITH ( NOLOCK )
            INNER JOIN Gungnir..tbl_Vehicle_Type AS vt WITH ( NOLOCK ) ON ti.VehicleID = vt.ProductID
    WHERE   EXISTS ( SELECT 1
                     FROM   tids
                     WHERE  tids.tid = ti.TID );";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Tids", string.Join(",", tids ?? new List<int>())),
            };
            var dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters);
            var result = dt.Rows.Cast<DataRow>().Select(row => new VehicleInfoDetail
            {
                Vehicle = row["Vehicle"].ToString(),
                VehicleID = row["VehicleID"].ToString(),
                Brand = row["Brand"].ToString(),
                TID = row["TID"].ToString(),
            }).ToList();
            return result;
        }

        /// <summary>
        /// 获取喷漆等级
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="vehicleIds"></param>
        /// <returns></returns>
        public static IEnumerable<Tuple<string, string>> SelectSprayPaintLevelsByVehicleIds(SqlConnection conn, IEnumerable<string> vehicleIds)
        {
            var sql = @"WITH    vehicleIds
          AS ( SELECT   Item AS vehicleId
               FROM     Gungnir.dbo.SplitString(@VehicleIds, N',', 1)
             )
    SELECT  t.VehicleId ,
            t.VehicleLevel
    FROM    Configuration..PaintVehicleLevelConfig AS t ( NOLOCK )
            INNER JOIN vehicleIds AS v WITH ( NOLOCK ) ON t.VehicleId = v.vehicleId COLLATE Chinese_PRC_CI_AS;";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@VehicleIds", string.Join(",", vehicleIds ?? new List<string>())),
            };
            var dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters);
            var result = dt.Rows.Cast<DataRow>().Select(row => Tuple.Create(row["VehicleId"].ToString(), row["VehicleLevel"].ToString())).ToList();
            return result;
        }

        #endregion

    }
}
