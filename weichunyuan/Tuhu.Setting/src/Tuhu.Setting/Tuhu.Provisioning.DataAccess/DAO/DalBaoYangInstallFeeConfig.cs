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
    public class DalBaoYangInstallFeeConfig
    {
        /// <summary>
        /// 获取所有保养服务
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static List<BaoYangService> GetAllBaoYangServices(SqlConnection conn)
        {
            var sql = @"SELECT  s.ProductID AS ServiceId ,
                                s.ServersName AS ServiceName
                        FROM    Gungnir..ShopCosmetologyServers AS s WITH ( NOLOCK )
                        WHERE   s.ServersType = N'MaintenanceServices'
                                AND s.IsActive = 1
                                AND s.IsActive = 1
                        ORDER BY s.ServersType;";
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql).ConvertTo<BaoYangService>().ToList();
        }

        /// <summary>
        /// 添加保养项目加价配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int AddBaoYangInstallFeeConfig(SqlConnection conn, BaoYangInstallFeeConfigModel model)
        {
            var sql = @"INSERT  BaoYang..BaoYangInstallFeeConfig
                                ( ServiceId ,
                                  CarMinPrice ,
                                  CarMaxPrice ,
                                  AddInstallFee
                                )
                        VALUES  ( @ServiceId ,
                                  @CarMinPrice ,
                                  @CarMaxPrice ,
                                  @AddInstallFee 
                                );
                        SELECT  SCOPE_IDENTITY()";
            var parameters = new[]
            {
                new SqlParameter("@ServiceId", model.ServiceId),
                 new SqlParameter("@CarMinPrice", model.CarMinPrice),
                  new SqlParameter("@CarMaxPrice", model.CarMaxPrice),
                   new SqlParameter("@AddInstallFee", model.AddInstallFee)
            };
            return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters));
        }

        /// <summary>
        /// 删除保养项目加价配置 --只做逻辑删除
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static bool DeleteBaoYangInstallFeeConfigByPKID(SqlConnection conn, int pkid)
        {
            #region SQL
            var sql = @"UPDATE  BaoYang..BaoYangInstallFeeConfig
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
        /// 恢复被逻辑删除的配置 --添加数据等同已被删除数据时,恢复该数据
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static bool BackUpBaoYangInstallFeeConfigByPKID(SqlConnection conn, int pkid)
        {
            #region SQL
            var sql = @"UPDATE  BaoYang..BaoYangInstallFeeConfig
                        SET     IsDeleted = 0 ,
                                LastUpdateDateTime = GETDATE()
                        WHERE   PKID = @PKID;";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@PKID", pkid)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
        }

        public static BaoYangInstallFeeConfigModel GetDeletedBaoYangInstallFeeConfig(SqlConnection conn, BaoYangInstallFeeConfigModel model)
        {
            #region SQL 
            var sql = @"SELECT  PKID ,
                                s.ServiceId ,
                                s.CarMinPrice ,
                                s.CarMaxPrice ,
                                s.AddInstallFee ,
                                s.IsDeleted ,
                                s.CreateDateTime ,
                                s.LastUpdateDateTime
                        FROM    BaoYang..BaoYangInstallFeeConfig AS s WITH ( NOLOCK )
                        WHERE   s.ServiceId = @ServiceId
                                AND s.CarMinPrice = @CarMinPrice
                                AND s.CarMaxPrice = @CarMaxPrice
                                AND s.AddInstallFee = @AddInstallFee
                                AND s.IsDeleted = 1;";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@ServiceId", model.ServiceId),
                new SqlParameter("@CarMinPrice", model.CarMinPrice),
                new SqlParameter("@CarMaxPrice", model.CarMaxPrice),
                new SqlParameter("@AddInstallFee", model.AddInstallFee)
            };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters).ConvertTo<BaoYangInstallFeeConfigModel>().ToList().FirstOrDefault();
        }

        /// <summary>
        /// 获取保养项目加价配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="serviceId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public static List<BaoYangInstallFeeConfigModel> SelectBaoYangInstallFeeConfig(SqlConnection conn, string serviceId)
        {
            var sql = @"SELECT  b.PKID ,
                                s.ProductID AS ServiceId ,
                                s.ServersName AS ServiceName ,
                                b.CarMinPrice ,
                                b.CarMaxPrice ,
                                b.AddInstallFee ,
                                b.IsDeleted ,
                                b.CreateDateTime ,
                                b.LastUpdateDateTime
                        FROM    Gungnir..ShopCosmetologyServers AS s WITH ( NOLOCK )
                                LEFT JOIN BaoYang..BaoYangInstallFeeConfig AS b WITH ( NOLOCK ) ON s.ProductID COLLATE Chinese_PRC_CI_AS = b.ServiceId
                                                                                     AND b.IsDeleted = 0
                        WHERE   s.ServersType = N'MaintenanceServices'
                                AND s.IsActive = 1
                                AND s.IsActive = 1
                                AND ( @ServiceId IS NULL
                                      OR @ServiceId = N''
                                      OR s.ProductID = @ServiceId
                                    )
                                AND ( b.IsDeleted IS NULL
                                              OR b.IsDeleted = 0
                                            )
                        ORDER BY s.ServersType ,
                                 CASE WHEN b.CarMinPrice IS NULL THEN 1
                                     ELSE 0
                                END ASC ,
                                b.CarMinPrice ASC ;";
            var parameters = new[]
            {
                new SqlParameter("@ServiceId", serviceId)
            };
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters).ConvertTo<BaoYangInstallFeeConfigModel>().ToList();
            return result;
        }

        #region 特殊车型附加安装费开关

        /// <summary>
        /// 添加开关
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="isOpen"></param>
        /// <returns></returns>
        public static bool AddVehicleAdditionalPriceSwitch(SqlConnection conn, bool isOpen)
        {
            #region SQL
            var sql = @"INSERT  INTO Configuration..SimpleConfig
                                ( ConfigName ,
                                  ConfigContent ,
                                  Description ,
                                  ConfigUrl ,
                                  CreatedTime ,
                                  UpdatedTime
                                )
                        VALUES  ( N'VehicleAdditionalPriceSwitch' ,
                                  @ConfigContent ,
                                  N'特殊车型附加安装费开关' ,
                                  N'' ,
                                  GETDATE() ,
                                  GETDATE()
                                );";
            #endregion
            var parameters = new[]
             {
                new SqlParameter("@ConfigContent", $"<Switch>{isOpen}</Switch>")
            };
            return Convert.ToInt32(SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters)) > 0;
        }

        /// <summary>
        /// 更新开关状态
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="isOpen"></param>
        /// <returns></returns>
        public static bool UpdateVehicleAdditionalPriceSwitch(SqlConnection conn, bool isOpen)
        {
            #region SQL
            var sql = @"UPDATE  Configuration..SimpleConfig
                        SET     ConfigContent = @ConfigContent ,
                                UpdatedTime = GETDATE()
                        WHERE   ConfigName = N'VehicleAdditionalPriceSwitch';";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@ConfigContent", $"<Switch>{isOpen}</Switch>")
            };
            return Convert.ToInt32(SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters)) > 0;
        }

        /// <summary>
        /// 获取开关状态
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static string IsExistVehicleAdditionalPriceSwitch(SqlConnection conn)
        {
            #region SQL
            var sql = @"SELECT  s.ConfigContent
                        FROM    Configuration..SimpleConfig AS s WITH ( NOLOCK )
                        WHERE   s.ConfigName = N'VehicleAdditionalPriceSwitch';";
            #endregion
            var dt= SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql);
            return (from DataRow row in dt.Rows select row[0].ToString()).ToList().FirstOrDefault();
        }
        #endregion
    }
}
