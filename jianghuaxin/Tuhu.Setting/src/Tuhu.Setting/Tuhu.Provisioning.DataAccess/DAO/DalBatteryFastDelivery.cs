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

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalBatteryFastDelivery
    {
        public static int AddBatteryFastDelivery(SqlConnection conn, BatteryFastDeliveryModel model)
        {
            var sql = @"INSERT  Configuration..BatteryFastDelivery
                     ( ServiceTypePid ,
                       RegionId ,
                       StartTime ,
                       EndTime ,
                       Remark
                     )
                    VALUES  ( @ServiceTypePid ,
                              @RegionId ,
                              @StartTime ,
                              @EndTime ,
                              @Remark
                    )
                    SELECT  SCOPE_IDENTITY();";
            var parameters = new[]
            {
                new SqlParameter("@ServiceTypePid", model.ServiceTypePid),
                new SqlParameter("@RegionId", model.RegionId),
                new SqlParameter("@StartTime", model.StartTime == null ? DBNull.Value : (object)model.StartTime.Value),
                new SqlParameter("@EndTime", model.EndTime == null ? DBNull.Value : (object)model.EndTime.Value),
                new SqlParameter("@Remark", model.Remark)
            };
            var pkid = Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters));
            return pkid;
        }

        public static bool DeleteBatteryFastDelivery(SqlConnection conn, int pkid)
        {
            var sql = @"DELETE  FROM Configuration..BatteryFastDelivery
                        WHERE   PKID = @PKID;";
            var parameters = new[]
            {
                new SqlParameter("@PKID", pkid)
            };
            var count = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);
            return count > 0;
        }

        public static bool UpdateBatteryFastDelivery(SqlConnection conn, BatteryFastDeliveryModel batteryFastDeliveryModel)
        {
            var sql = @"UPDATE  Configuration..BatteryFastDelivery
                        SET     StartTime = @StartTime,
                                EndTime=@EndTime,
                                Remark=@Remark,
                                LastUpdateDateTime=@LastUpdateDateTime
                        WHERE   PKID = @PKID;";
            var parameters = new[]
            {
                new SqlParameter("@StartTime", batteryFastDeliveryModel.StartTime),
                new SqlParameter("@EndTime", batteryFastDeliveryModel.EndTime),
                new SqlParameter("@Remark", batteryFastDeliveryModel.Remark),
                new SqlParameter("@PKID", batteryFastDeliveryModel.PKID),
                new SqlParameter("@LastUpdateDateTime", DateTime.Now)
            };
            var count = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);
            return count > 0;
        }

        public static bool UpdateBatteryFastDeliveryStatus(SqlConnection conn, int pkid, bool isEnabled)
        {
            var sql = @"UPDATE  Configuration..BatteryFastDelivery
SET     IsEnabled = @IsEnabled ,
        LastUpdateDateTime = GETDATE()
WHERE   PKID = @PKID;";
            var parameters = new[]
            {
                new SqlParameter("@IsEnabled", isEnabled),
                new SqlParameter("@PKID", pkid),
            };
            var count = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);
            return count > 0;
        }

        public static List<BatteryFastDeliveryModel> SelectBatteryFastDelivery(SqlConnection conn, string serviceTypePid, int pageIndex, int pageSize, out int totalCount)
        {
            var sql = @"SELECT  @Total = COUNT(1)
                        FROM Configuration..BatteryFastDelivery AS b WITH (NOLOCK ) 
                        WHERE   b.ServiceTypePid =@ServiceTypePid;
                        SELECT  b.PKID ,
                                b.ServiceTypePid ,
                                b.RegionId ,
                                b.StartTime ,
                                b.EndTime ,
                                b.Remark ,
                                b.CreateDateTime ,
                                b.LastUpdateDateTime,
                                b.IsEnabled
                        FROM    Configuration..BatteryFastDelivery AS b WITH ( NOLOCK )
                        WHERE   b.ServiceTypePid = @ServiceTypePid
                                ORDER BY b.LastUpdateDateTime DESC
                                OFFSET ( @PageIndex - 1 ) * @PageSize ROWS  FETCH NEXT @PageSize ROWS
                                ONLY;";
            totalCount = 0;
            var parameters = new[]
            {
                new SqlParameter("@ServiceTypePid", serviceTypePid),
                new SqlParameter("@PageIndex", pageIndex),
                new SqlParameter("@PageSize", pageSize),
                new SqlParameter("@Total", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters).ConvertTo<BatteryFastDeliveryModel>().ToList();
            totalCount = Convert.ToInt32(parameters.LastOrDefault().Value.ToString());
            return result;
        }

        public static BatteryFastDeliveryModel SelectBatteryFastDeliveryById(SqlConnection conn, int pkid)
        {
            const string sql = @"SELECT  b.PKID ,
        b.ServiceTypePid ,
        b.RegionId ,
        b.StartTime ,
        b.EndTime ,
        b.Remark ,
        b.CreateDateTime ,
        b.LastUpdateDateTime ,
        b.IsEnabled
FROM    Configuration..BatteryFastDelivery AS b WITH ( NOLOCK )
WHERE   b.PKID = @PKID;";
            var parameters = new[]
            {
                new SqlParameter("@PKID", pkid),
            };
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters).ConvertTo<BatteryFastDeliveryModel>().FirstOrDefault();
            return result;
        }

        public static bool IsRepeatBatteryFastDelivery(SqlConnection conn, string serviceTypePid, int regionId)
        {
            var sql = @"SELECT  COUNT(1)
                        FROM    Configuration..BatteryFastDelivery AS b WITH ( NOLOCK )
                        WHERE   b.ServiceTypePid = @ServiceTypePid
                                AND b.RegionId = @RegionId;";
            var parameters = new[]
            {
                new SqlParameter("@ServiceTypePid", serviceTypePid),
                new SqlParameter("@RegionId", regionId)
            };
            return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters)) > 0;
        }

        public static List<BatteryFastDeliveryProductsModel> GetBatteryFastDeliveryProductsByFastDeliveryId(SqlConnection conn, string fastDeliveryIdsStr)
        {
            var sql = @"SELECT  bp.FastDeliveryId ,
                                bp.Brand ,
                                bp.ProductPid
                                FROM    Configuration..BatteryFastDeliveryProducts AS bp WITH ( NOLOCK )
                                WHERE   bp.FastDeliveryId IN (
                                                         SELECT    *
                                                         FROM      Configuration..SplitString(@FastDeliveryId, N',', 1) );";
            var parameters = new[]
            {
                new SqlParameter("@FastDeliveryId", fastDeliveryIdsStr)
            };
            var batteryList = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters).ConvertTo<BatteryFastDeliveryProductsModel>().ToList();
            return batteryList;
        }

        public static int GetBatteryFastDeliveryProductsCountByFastDeliveryId(SqlConnection conn, int fastDeliveryId)
        {
            var sql = @"SELECT  COUNT(1)
                                FROM    Configuration..BatteryFastDeliveryProducts AS bp WITH ( NOLOCK )
                                WHERE   bp.FastDeliveryId =@FastDeliveryId;";
            var parameters = new[]
            {
                new SqlParameter("@FastDeliveryId", fastDeliveryId)
            };
            return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters));
        }

        public static List<BatteryServiceTypeModel> GetAllBatteryServiceType(SqlConnection conn)
        {
            #region SQL
            var sql = @"SELECT  p.PID,
                                p.DisplayName
                        FROM    Tuhu_productcatalog..[CarPAR_zh-CN] AS p WITH ( NOLOCK )
                        WHERE   p.PrimaryParentCategory = N'XDCFW'
                                AND p.i_ClassType IN ( 2, 4 )
			            ORDER BY p.cy_list_price DESC;";
            #endregion
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql).ConvertTo<BatteryServiceTypeModel>().ToList();
            return result;
        }

        public static List<BatteryFastDeliveryForViewModel> GetAllBattery(SqlConnection conn)
        {
            #region SQL
            var sql = @"SELECT  p.CP_Brand AS Brand ,
                                p.PID ,
                                p.Name AS DisplayName
                        FROM    Tuhu_productcatalog..[CarPAR_zh-CN] AS p WITH ( NOLOCK )
                        WHERE   p.PrimaryParentCategory = N'battery'
                                AND p.i_ClassType IN ( 2, 4 )
                        ORDER BY p.PID;";
            #endregion
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql).ConvertTo<BatteryFastDeliveryForViewModel>().ToList();
            return result;
        }

        public static bool DeleteBatteryProductsByFastDeliveryId(SqlConnection conn, int fastDeliveryId)
        {
            #region SQL
            var sql = @"DELETE  Configuration..BatteryFastDeliveryProducts
                        WHERE   FastDeliveryId = @FastDeliveryId;";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@FastDeliveryId", fastDeliveryId)
            };
            var count = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);
            return count > 0;
        }

        public static bool DeleteBatteryFastDeliveryProducts(SqlConnection conn, BatteryFastDeliveryProductsModel model)
        {
            #region SQL
            var sql = @"DELETE  Configuration..BatteryFastDeliveryProducts
                        WHERE   FastDeliveryId = @FastDeliveryId
                                AND ProductPid = @ProductPid
                                AND Brand = @Brand;";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@FastDeliveryId", model.FastDeliveryId),
            new SqlParameter("@ProductPid", model.ProductPid),
            new SqlParameter("@Brand", model.Brand)

            };
            var count = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);
            return count > 0;
        }

        public static bool AddBatteryFastDeliveryProducts(SqlConnection conn, BatteryFastDeliveryProductsModel model)
        {
            #region SQL
            var sql = @"INSERT  Configuration..BatteryFastDeliveryProducts
                                ( FastDeliveryId, Brand, ProductPid )
                        VALUES  ( @FastDeliveryId, @Brand, @ProductPid );";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@FastDeliveryId", model.FastDeliveryId),
                new SqlParameter("@Brand", model.Brand),
                new SqlParameter("@ProductPid", model.ProductPid)
            };
            var count = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);
            return count > 0;
        }

        #region 服务类蓄电池产品排序

        /// <summary>
        /// 获取蓄电池品牌
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static List<string> SelectBatteryBrands(SqlConnection conn)
        {
            const string sql = @"SELECT DISTINCT
        vp.CP_Brand
FROM    Tuhu_productcatalog..vw_Products AS vp WITH ( NOLOCK )
WHERE   vp.Category = N'battery';";
            var dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql);
            var result = dt.Rows.OfType<DataRow>().Select(row => row["CP_Brand"].ToString()).ToList();
            return result;
        }

        /// <summary>
        /// 获取服务类蓄电池产品配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="index"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Tuple<int, List<BatteryFastDeliveryPriority>> SelectBatteryFastDeliveryPriorities(SqlConnection conn,
            int? province, int? city, int index, int size)
        {
            var countparam = new SqlParameter("@Total", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var parameters = new[]
            {
                new SqlParameter("@ProvinceId", province),
                new SqlParameter("@CityId", city),
                new SqlParameter("@PageIndex", index),
                new SqlParameter("@PageSize", size),
                countparam
            };
            const string sql = @"SELECT  @Total = COUNT(t.PKID)
FROM    BaoYang..BatteryFastDeliveryPriority AS t WITH ( NOLOCK )
WHERE   ( @ProvinceId IS NULL
          OR t.ProvinceId = @ProvinceId
        )
        AND ( @CityId IS NULL
              OR @CityId = @CityId
            );
SELECT  t.PKID ,
        t.ProvinceId ,
        t.ProvinceName ,
        t.CityId ,
        t.CityName ,
        t.Priorities ,
        t.IsEnabled ,
        t.CreateDateTime ,
        t.LastUpdateDateTime
FROM    BaoYang..BatteryFastDeliveryPriority AS t WITH ( NOLOCK )
WHERE   ( @ProvinceId IS NULL
          OR t.ProvinceId = @ProvinceId
        )
        AND ( @CityId IS NULL
              OR @CityId = @CityId
            )
ORDER BY t.IsEnabled DESC ,
        t.ProvinceId ,
        t.CityId
        OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
        ONLY;";
            var dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters);
            var result = dt.Rows.OfType<DataRow>().Select(row => new BatteryFastDeliveryPriority
            {
                PKID = (int)row["PKID"],
                ProvinceId = (int)row["ProvinceId"],
                ProvinceName = row["ProvinceName"].ToString(),
                CityId = (int)row["CityId"],
                CityName = row["CityName"].ToString(),
                Priorities = row["Priorities"].ToString().Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList(),
                IsEnabled = (bool)row["IsEnabled"],
                CreateDateTime = (DateTime)row["CreateDateTime"],
                LastUpdateDateTime = row["LastUpdateDateTime"] == DBNull.Value ? (DateTime?)null : (DateTime)row["LastUpdateDateTime"],
            }).ToList();
            var total = (int)countparam.Value;
            return Tuple.Create(total, result);
        }

        /// <summary>
        /// 根据Id获取配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static BatteryFastDeliveryPriority GetBatteryFastDeliveryPriorities(SqlConnection conn, int id)
        {
            const string sql = @"SELECT  t.PKID ,
        t.ProvinceId ,
        t.ProvinceName ,
        t.CityId ,
        t.CityName ,
        t.Priorities ,
        t.IsEnabled ,
        t.CreateDateTime ,
        t.LastUpdateDateTime
FROM    BaoYang..BatteryFastDeliveryPriority AS t WITH ( NOLOCK )
WHERE   t.PKID = @PKID";
            var parameters = new[]
            {
                new SqlParameter("@PKID", id),
            };
            var dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters);
            var result = dt.Rows.OfType<DataRow>().Select(row => new BatteryFastDeliveryPriority
            {
                PKID = (int)row["PKID"],
                ProvinceId = (int)row["ProvinceId"],
                ProvinceName = row["ProvinceName"].ToString(),
                CityId = (int)row["CityId"],
                CityName = row["CityName"].ToString(),
                Priorities = row["Priorities"].ToString().Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList(),
                IsEnabled = (bool)row["IsEnabled"],
                CreateDateTime = (DateTime)row["CreateDateTime"],
                LastUpdateDateTime = row["LastUpdateDateTime"] == DBNull.Value ? (DateTime?)null : (DateTime)row["LastUpdateDateTime"],
            }).FirstOrDefault();
            return result;
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="priority"></param>
        /// <returns></returns>
        public static bool IsExistsBatteryFastDeliveryPriority(SqlConnection conn, BatteryFastDeliveryPriority priority)
        {
            const string sql = @"SELECT  COUNT(t.PKID)
FROM    BaoYang..BatteryFastDeliveryPriority AS t WITH ( NOLOCK )
WHERE   t.PKID <> @PKID
        AND t.ProvinceId = @ProvinceId
        AND t.CityId = @CityId;";
            var parameters = new[]
            {
                new SqlParameter("@PKID", priority.PKID),
                new SqlParameter("@ProvinceId", priority.ProvinceId),
                new SqlParameter("@ProvinceName", priority.ProvinceName),
                new SqlParameter("@CityId", priority.CityId),
                new SqlParameter("@CityName", priority.CityName),
            };
            var countObj = SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters);
            var count = Convert.ToInt32(countObj);
            return count > 0;
        }

        /// <summary>
        /// 删除配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteBatteryFastDeliveryPriority(SqlConnection conn, int id)
        {
            const string sql = @"DELETE  FROM BaoYang..BatteryFastDeliveryPriority   WHERE   PKID = @PKID;";
            var parameters = new[]
            {
                new SqlParameter("@PKID", id),
            };
            var rows = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);
            return rows > 0;
        }

        /// <summary>
        /// 修改配置
        /// </summary>
        /// <param name="priority"></param>
        /// <returns></returns>
        public static bool UpdateBatteryFastDeliveryPriority(SqlConnection conn, BatteryFastDeliveryPriority priority)
        {
            const string sql = @"UPDATE  t
SET     t.Priorities = @Priorities ,
        t.LastUpdateDateTime = GETDATE() ,
        t.IsEnabled = @IsEnabled
FROM    BaoYang..BatteryFastDeliveryPriority AS t
WHERE   t.PKID = @PKID;";
            var parameters = new[]
            {
                new SqlParameter("@PKID", priority.PKID),
                new SqlParameter("@Priorities", string.Join(";", priority.Priorities ?? new List<string>())),
                new SqlParameter("@IsEnabled", priority.IsEnabled),
            };
            var rows = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);
            return rows > 0;
        }

        /// <summary>
        /// 添加配置
        /// </summary>
        /// <param name="priority"></param>
        /// <returns></returns>
        public static int AddBatteryFastDeliveryPriority(SqlConnection conn, BatteryFastDeliveryPriority priority)
        {
            const string sql = @"INSERT  INTO BaoYang..BatteryFastDeliveryPriority
        ( ProvinceId ,
          ProvinceName ,
          CityId ,
          CityName ,
          Priorities ,
          IsEnabled ,
          CreateDateTime ,
          LastUpdateDateTime
        )
OUTPUT  Inserted.PKID
VALUES  ( @ProvinceId ,
          @ProvinceName ,
          @CityId ,
          @CityName ,
          @Priorities ,
          @IsEnabled ,
          GETDATE() ,
          GETDATE()
        );";
            var parameters = new[]
            {
                new SqlParameter("@Priorities", string.Join(";", priority.Priorities ?? new List<string>())),
                new SqlParameter("@IsEnabled", priority.IsEnabled),
                new SqlParameter("@ProvinceId", priority.ProvinceId),
                new SqlParameter("@ProvinceName", priority.ProvinceName),
                new SqlParameter("@CityId", priority.CityId),
                new SqlParameter("@CityName", priority.CityName),
            };
            var result = SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters);
            var pkid = Convert.ToInt32(result);
            return pkid;
        }

        #endregion
    }
}
