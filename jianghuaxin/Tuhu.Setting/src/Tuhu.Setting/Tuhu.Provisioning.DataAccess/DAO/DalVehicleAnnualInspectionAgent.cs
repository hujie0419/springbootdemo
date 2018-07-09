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
    public class DalVehicleAnnualInspectionAgent
    {
        /// <summary>
        /// 添加年检代办配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int AddVehicleAnnualInspectionAgent(SqlConnection conn, VehicleAnnualInspectionAgentModel model)
        {
            #region SQL
            var sql = @"INSERT  INTO Configuration..VehicleAnnualInspectionAgent
                                ( ServicePid ,
                                  CarNoPrefix ,
                                  VenderShortName ,
                                  SalePrice ,
                                  CostPrice ,
                                  IsEnabled ,
                                  Contact ,
                                  TelNum ,
                                  OfficeAddress,
                                  Remarks
                                )
                        VALUES  ( @ServicePid ,
                                  @CarNoPrefix ,
                                  @VenderShortName ,
                                  @SalePrice ,
                                  @CostPrice ,
                                  @IsEnabled ,
                                  @Contact ,
                                  @TelNum ,
                                  @OfficeAddress,
                                  @Remarks
                                )
                        SELECT  SCOPE_IDENTITY();";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@ServicePid", model.ServicePid),
                new SqlParameter("@CarNoPrefix", model.CarNoPrefix),
                new SqlParameter("@VenderShortName", model.VenderShortName),
                new SqlParameter("@SalePrice", model.SalePrice),
                new SqlParameter("@CostPrice", model.CostPrice),
                new SqlParameter("@IsEnabled", model.IsEnabled),
                new SqlParameter("@Contact", model.Contact),
                new SqlParameter("@TelNum", model.TelNum),
                new SqlParameter("@OfficeAddress", model.OfficeAddress),
                new SqlParameter("@Remarks", model.Remarks)
            };
            return Convert.ToInt32(SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters));
        }

        /// <summary>
        /// 删除年检代办配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static bool DeleteVehicleAnnualInspectionAgent(SqlConnection conn, int pkid)
        {
            #region SQL
            var sql = @"UPDATE  Configuration..VehicleAnnualInspectionAgent
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
        /// 更新年检代办配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool UpdateVehicleAnnualInspectionAgent(SqlConnection conn, VehicleAnnualInspectionAgentModel model)
        {
            #region Sql
            var sql = @"UPDATE  Configuration..VehicleAnnualInspectionAgent
                        SET     VenderShortName = @VenderShortName ,
                                SalePrice = @SalePrice ,
                                CostPrice = @CostPrice ,
                                IsEnabled = @IsEnabled ,
                                IsDeleted = @IsDeleted ,
                                Contact = @Contact ,
                                TelNum = @TelNum ,
                                OfficeAddress = @OfficeAddress ,
                                Remarks = @Remarks ,
                                LastUpdateDateTime = GETDATE()
                        WHERE   PKID = @PKID;";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@VenderShortName", model.VenderShortName),
                new SqlParameter("@SalePrice", model.SalePrice),
                new SqlParameter("@CostPrice", model.CostPrice),
                new SqlParameter("@IsEnabled", model.IsEnabled),
                new SqlParameter("@IsDeleted", model.IsDeleted),
                new SqlParameter("@Contact", model.Contact),
                new SqlParameter("@TelNum", model.TelNum),
                new SqlParameter("@OfficeAddress", model.OfficeAddress),
                new SqlParameter("@PKID", model.PKID),
                new SqlParameter("@Remarks", model.Remarks)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
        }

        /// <summary>
        /// 查询年检代办配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="servicePid"></param>
        /// <param name="carNoPrefix"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public static List<VehicleAnnualInspectionAgentModel> SelectVehicleAnnualInspectionAgent
            (SqlConnection conn, string servicePid, string carNoPrefix, int pageIndex, int pageSize, out int totalCount)
        {
            #region Sql
            var sql = @"SELECT  @Total = COUNT(1)
                        FROM    Configuration..VehicleAnnualInspectionAgent AS s WITH ( NOLOCK )
                        WHERE   ( @ServicePid IS NULL
                                  OR @ServicePid = N''
                                  OR s.ServicePid = @ServicePid
                                )
                                AND ( @CarNoPrefix IS NULL
                                      OR @CarNoPrefix = N''
                                      OR s.CarNoPrefix = @CarNoPrefix
                                    )
                                AND s.IsDeleted = 0; 
                        SELECT  s.PKID ,
                                s.ServicePid ,
                                s.CarNoPrefix ,
                                s.VenderShortName ,
                                s.SalePrice ,
                                s.CostPrice ,
                                s.IsEnabled ,
                                s.IsDeleted ,
                                s.Contact ,
                                s.TelNum ,
                                s.OfficeAddress ,
                                s.Remarks ,
                                s.CreateDateTime ,
                                s.LastUpdateDateTime
                        FROM    Configuration..VehicleAnnualInspectionAgent AS s WITH ( NOLOCK )
                        WHERE   ( @ServicePid IS NULL
                                  OR @ServicePid = N''
                                  OR s.ServicePid = @ServicePid
                                )
                                AND ( @CarNoPrefix IS NULL
                                      OR @CarNoPrefix = N''
                                      OR s.CarNoPrefix = @CarNoPrefix
                                    )
                                AND s.IsDeleted = 0
                        ORDER BY s.CarNoPrefix ,
                                s.IsEnabled DESC ,
                                s.ServicePid
                                OFFSET ( @PageIndex - 1 ) * @PageSize ROWS  FETCH NEXT @PageSize ROWS
                                ONLY;";
            #endregion
            totalCount = 0;
            var parameters = new[]
            {
                new SqlParameter("@ServicePid", servicePid),
                new SqlParameter("@CarNoPrefix", carNoPrefix),
                new SqlParameter("@PageIndex", pageIndex),
                new SqlParameter("@PageSize", pageSize),
                new SqlParameter("@Total", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters).ConvertTo<VehicleAnnualInspectionAgentModel>().ToList();
            totalCount = Convert.ToInt32(parameters.LastOrDefault().Value.ToString());
            return result;
        }

        /// <summary>
        /// 获取所有年检代办服务
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static List<AnnualInspectionServiceModel> GetAllAnnualInspectionService(SqlConnection conn)
        {
            #region sql
            var sql = @"SELECT  p.PID AS ServicePid ,
                                p.DisplayName AS ServiceName
                        FROM    Tuhu_productcatalog..[CarPAR_zh-CN] AS p WITH ( NOLOCK )
                        WHERE   p.PrimaryParentCategory = N'CWFW'
                                AND p.OrigProductID = N'FU-NJDB'
                                AND p.i_ClassType IN ( 2, 4 )
                        ORDER BY p.PID DESC;";
            #endregion
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql).ConvertTo<AnnualInspectionServiceModel>().ToList();
        }

        /// <summary>
        /// 获取所有车牌前缀
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static List<LicensePlateLocalModel> GetAllCarNoPrefix(SqlConnection conn)
        {
            #region Sql
            var sql = @"SELECT  s.PlatePrefix ,
                                s.Province AS ProvinceName
                        FROM    Gungnir..LicensePlateLocale AS s WITH ( NOLOCK );";
            #endregion
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql).ConvertTo<LicensePlateLocalModel>().ToList();
        }

        /// <summary>
        /// 获取年检代办配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="servicePid"></param>
        /// <param name="telNum"></param>
        /// <param name="carNoPrefix"></param>
        /// <returns></returns>
        public static VehicleAnnualInspectionAgentModel GetAnnualInspectionAgent
            (SqlConnection conn, string servicePid, string telNum, string carNoPrefix)
        {
            #region Sql
            var sql = @"SELECT  s.PKID ,
                                s.ServicePid ,
                                s.CarNoPrefix ,
                                s.VenderShortName ,
                                s.SalePrice ,
                                s.CostPrice ,
                                s.Contact ,
                                s.TelNum ,
                                s.OfficeAddress ,
                                s.Remarks ,
                                s.IsEnabled ,
                                s.IsDeleted ,
                                s.CreateDateTime ,
                                s.LastUpdateDateTime
                        FROM    Configuration..VehicleAnnualInspectionAgent AS s WITH ( NOLOCK )
                        WHERE   s.ServicePid = @ServicePid
                                AND s.TelNum = @TelNum
                                AND s.CarNoPrefix = @CarNoPrefix
                        ORDER BY s.IsDeleted;";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@ServicePid", servicePid),
                new SqlParameter("@TelNum", telNum),
                new SqlParameter("@CarNoPrefix", carNoPrefix)
            };
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters)
                .ConvertTo<VehicleAnnualInspectionAgentModel>().FirstOrDefault();
            return result;
        }

        /// <summary>
        /// 判断年检配置是否重复
        /// 同PID同城市(车牌前缀)，只能有一条启用的数据
        /// PID、车牌前缀、联系电话 作唯一性约束
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool IsExistAnnualInspectionAgent
            (SqlConnection conn, VehicleAnnualInspectionAgentModel model)
        {
            #region Sql
            var sql = @"SELECT  COUNT(1)
                        FROM    Configuration..VehicleAnnualInspectionAgent AS s WITH ( NOLOCK )
                        WHERE   s.ServicePid = @ServicePid
                                AND s.CarNoPrefix = @CarNoPrefix
                                AND s.IsDeleted=0
                                AND ( ( s.IsEnabled = 1
                                        AND s.IsEnabled = @IsEnabled
                                      )
                                      OR s.TelNum = @TelNum
                                    )
                                AND s.PKID <> @PKID;";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@ServicePid", model.ServicePid),
                new SqlParameter("@CarNoPrefix", model.CarNoPrefix),
                new SqlParameter("@IsEnabled", model.IsEnabled),
                new SqlParameter("@TelNum", model.TelNum),
                new SqlParameter("@PKID", model.PKID)
            };
            var count = Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters));
            return count > 0;
        }

        /// <summary>
        /// 查询年检代办操作日志
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="logType"></param>
        /// <param name="identityId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static List<AnnualInspectionOprLogModel> SelectAnnualInspectionOprLog
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
                        FROM    Tuhu_log..AnnualInspectionOprLog AS log WITH ( NOLOCK )
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
                .ConvertTo<AnnualInspectionOprLogModel>().ToList();
        }
    }
}
