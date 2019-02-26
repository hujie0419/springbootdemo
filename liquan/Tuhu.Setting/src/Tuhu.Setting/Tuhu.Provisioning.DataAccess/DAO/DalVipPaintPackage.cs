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
using Tuhu.Provisioning.DataAccess.Entity.VipBaoYangPackage;
using static Tuhu.Provisioning.DataAccess.Entity.VipPaintPackageModel;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalVipPaintPackage
    {
        #region 喷漆大客户套餐配置

        /// <summary>
        /// 添加喷漆大客户套餐
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int AddPackageConfig(SqlConnection conn, VipPaintPackageConfigModel model)
        {
            #region Sql
            var sql = @"INSERT  INTO Configuration..VipPaintPackageConfig
                                ( PackagePid ,
                                  PackageName ,
                                  PackagePrice ,
                                  VipUserId ,
                                  ServicePids ,
                                  SettlementMethod ,
                                  Operator 
          
                                )
                        OUTPUT  Inserted.PKID
                        VALUES  ( @PackagePid ,
                                  @PackageName ,
                                  @PackagePrice ,
                                  @VipUserId ,
                                  @ServicePids ,
                                  @SettlementMethod ,
                                  @Operator
                                );";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@PackagePid", model.PackagePid ?? string.Empty),
                new SqlParameter("@PackageName", model.PackageName),
                new SqlParameter("@PackagePrice", model.PackagePrice),
                new SqlParameter("@VipUserId", model.VipUserId),
                new SqlParameter("@ServicePids", model.ServicePids),
                new SqlParameter("@SettlementMethod", model.SettlementMethod),
                new SqlParameter("@Operator", model.Operator)
            };
            return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters));
        }

        /// <summary>
        /// 更新喷漆大客户套餐Pid
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pkid"></param>
        /// <param name="packagePid"></param>
        /// <returns></returns>
        public static bool UpdatePackagePID(SqlConnection conn, int pkid, string packagePid)
        {
            var sql = @"UPDATE  Configuration..VipPaintPackageConfig
                        SET     PackagePid = @PackagePid
                        WHERE   PKID = @PKID;";
            var parameters = new[]
            {
                new SqlParameter("@PKID", pkid),
                new SqlParameter("@PackagePid", packagePid)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
        }

        /// <summary>
        /// 删除喷漆大客户套餐配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pkid"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool DeletePackageConfig(SqlConnection conn, int pkid, string user)
        {
            #region Sql
            var sql = @"UPDATE  Configuration..VipPaintPackageConfig
                        SET     IsDeleted = 1 ,
                                Operator = @Operator ,
                                LastUpdateDateTime = GETDATE()
                        WHERE   PKID = @PKID;";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@PKID", pkid),
                new SqlParameter("@Operator", user)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
        }

        /// <summary>
        /// 更新喷漆大客户套餐配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool UpdatePackageConfig(SqlConnection conn, VipPaintPackageConfigModel model)
        {
            #region Sql
            var sql = @"UPDATE  Configuration..VipPaintPackageConfig
                        SET     PackagePrice = @PackagePrice ,
                                VipUserId = @VipUserId ,
                                ServicePids = @ServicePids ,
                                SettlementMethod = @SettlementMethod ,
                                Operator = @Operator ,
                                IsDeleted = 0,
                                LastUpdateDateTime=GETDATE()
                        WHERE   PKID = @PKID;";
            var parameters = new[]
            {
                new SqlParameter("@PackagePrice", model.PackagePrice),
                new SqlParameter("@VipUserId", model.VipUserId),
                new SqlParameter("@ServicePids", model.ServicePids),
                new SqlParameter("@SettlementMethod", model.SettlementMethod),
                new SqlParameter("@Operator", model.Operator),
                new SqlParameter("@PKID", model.PKID)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
            #endregion
        }

        /// <summary>
        /// 查询喷漆大客户套餐
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="packagePid"></param>
        /// <param name="vipUserId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static List<VipPaintPackageConfigModel> SelectPackageConfig
            (SqlConnection conn, string packagePid, Guid vipUserId, int pageIndex, int pageSize, out int totalCount)
        {
            #region Sql
            var sql = @"SELECT  @Total = COUNT(1)
                        FROM    Configuration..VipPaintPackageConfig AS s WITH ( NOLOCK )
                        WHERE   ( @PackagePid IS NULL
                                  OR @PackagePid = N''
                                  OR s.PackagePid = @PackagePid
                                )
                                AND ( @VipUserId IS NULL
                                      OR @VipUserId = N'00000000-0000-0000-0000-000000000000'
                                      OR s.VipUserId = @VipUserId
                                    )
                                AND s.IsDeleted = 0;
                        SELECT  s.PKID ,
                                s.PackagePid ,
                                s.PackageName ,
                                s.PackagePrice ,
                                s.VipUserId ,
                                s.ServicePids ,
                                s.SettlementMethod ,
                                s.Operator ,
                                s.CreateDateTime
                        FROM    Configuration..VipPaintPackageConfig AS s WITH ( NOLOCK )
                        WHERE   ( @PackagePid IS NULL
                                  OR @PackagePid = N''
                                  OR s.PackagePid = @PackagePid
                                )
                                AND ( @VipUserId IS NULL
                                      OR @VipUserId = N'00000000-0000-0000-0000-000000000000'
                                      OR s.VipUserId = @VipUserId
                                    )
                                AND s.IsDeleted = 0
                        ORDER BY s.PKID DESC
                                OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
                                ONLY;";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@PackagePid", packagePid),
                new SqlParameter("@VipUserId", vipUserId),
                new SqlParameter("@PageIndex", pageIndex),
                new SqlParameter("@PageSize", pageSize),
                new SqlParameter("@Total", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters).ConvertTo<VipPaintPackageConfigModel>().ToList();
            totalCount = Convert.ToInt32(parameters.LastOrDefault().Value.ToString());
            return result;
        }

        /// <summary>
        /// 获取大客户喷漆套餐配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="packageName"></param>
        /// <returns></returns>
        public static VipPaintPackageConfigModel GetPackageConfig(SqlConnection conn, string packageName)
        {
            #region Sql
            var sql = @"SELECT  s.PKID ,
                                s.PackagePid ,
                                s.PackageName ,
                                s.PackagePrice ,
                                s.VipUserId ,
                                s.ServicePids ,
                                s.SettlementMethod ,
                                s.Operator ,
                                s.IsDeleted ,
                                s.CreateDateTime ,
                                s.LastUpdateDateTime
                        FROM    Configuration..VipPaintPackageConfig AS s WITH ( NOLOCK )
                        WHERE   s.PackageName = @PackageName
                                AND s.IsDeleted= 0;";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@PackageName", packageName)
            };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters)
                .ConvertTo<VipPaintPackageConfigModel>().FirstOrDefault();
        }

        /// <summary>
        /// 大客户保养套餐是否存在
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="packageName"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static bool IsExistPackageConfig(SqlConnection conn, string packageName, int pkid)
        {
            #region Sql
            var sql = @"SELECT  COUNT(1)
                        FROM    Configuration..VipPaintPackageConfig AS s WITH ( NOLOCK )
                        WHERE   s.PackageName = @PackageName
                                AND s.IsDeleted = 0
                                AND s.PKID <> @PKID;";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@PackageName", packageName),
                new SqlParameter("@PKID", pkid)
            };
            return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters)) > 0;
        }
        #endregion

        #region 给用户塞券
        /// <summary>
        /// 该套餐是否存在塞券记录
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="packageId"></param>
        /// <returns></returns>
        public static bool IsExistPromotionRecord(SqlConnection conn, int packageId)
        {
            #region Sql
            var sql = @"SELECT  COUNT(1)
                        FROM    Configuration..VipPaintPackagePromotionRecord AS s WITH ( NOLOCK )
                        WHERE   s.PackageId = @PackageId;";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@PackageId", packageId)
            };
            return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters)) > 0;
        }

        /// <summary>
        /// 获取配置了套餐的喷漆大客户
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static List<string> GetPaintVipUsers(SqlConnection conn)
        {
            #region Sql
            var sql = @"SELECT DISTINCT
                                s.VipUserId
                        FROM    Configuration..VipPaintPackageConfig AS s WITH ( NOLOCK )
                        WHERE   s.IsDeleted = 0;";
            #endregion
            var dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql);
            var result = new List<string>();
            if (dt != null && dt.Rows.Count > 0)
            {
                result = (from DataRow row in dt.Rows select row[0].ToString()).ToList();
            }
            return result;
        }

        /// <summary>
        /// 获取该用户下的大客户喷漆套餐
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="vipUserId"></param>
        /// <returns></returns>
        public static List<VipPaintPackageSimpleModel> GetVipPaintPackages(SqlConnection conn, string vipUserId)
        {
            #region Sql
            var sql = @"SELECT  s.PKID AS PackageId ,
                                s.PackagePid ,
                                s.PackageName ,
                                s.PackagePrice ,
                                s.SettlementMethod
                        FROM    Configuration..VipPaintPackageConfig AS s WITH ( NOLOCK )
                        WHERE   s.VipUserId = @VipUserId
                                AND s.IsDeleted = 0;";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@VipUserId", vipUserId)
            };
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters)
                .ConvertTo<VipPaintPackageSimpleModel>().ToList();
            return result;
        }

        /// <summary>
        /// 文件是否导入过
        /// 失败和取消的任务可再次导入
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool IsExistVipPaintFile(SqlConnection conn, string fileName)
        {
            var type = FileType.VipPaintPackage.ToString();
            #region Sql
            var sql = @" SELECT  COUNT(1)
                        FROM    Tuhu_log..UploadFileTaskLog AS s WITH ( NOLOCK )
                        WHERE   s.Type = @Type
                                AND s.Status NOT IN ( SELECT    *
                                                  FROM      Tuhu_log..SplitString(@Status, ',', 1) )
                                AND s.FileName = @FileName;";

            #endregion
            var status = new List<string>()
            {
                FileStatus.Cancel.ToString(),
                FileStatus.Failed.ToString()
            };
            var parameters = new[]
            {
                new SqlParameter("@Type",type),
                new SqlParameter("@Status",string.Join(",", status)),
                new SqlParameter("@FileName",fileName)
            };
            return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters)) > 0;
        }

        /// <summary>
        /// 插入到塞券记录
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="packageId"></param>
        /// <param name="batchCode"></param>
        /// <param name="ruleGUID"></param>
        /// <param name="isSendSms"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static int InsertPromotionRecord
            (SqlConnection conn, VipPaintPackagePromotionRecordModel model)
        {
            #region Sql
            var sql = @"INSERT  INTO Configuration..VipPaintPackagePromotionRecord
                                        ( PackageId ,
                                          BatchCode ,
                                          RuleGUID ,
                                          IsSendSms ,
                                          ToBOrder ,
                                          CreateUser 
                                        )
                                OUTPUT  Inserted.PKID
	                            VALUES  ( @PackageId ,
	                                      @BatchCode ,
	                                      @RulesGUID ,
	                                      @IsSendSms ,
                                          @ToBOrder ,
	                                      @CreateUser
	                                    )";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@PackageId",model.PackageId),
                new SqlParameter("@BatchCode",model.BatchCode),
                new SqlParameter("@RulesGUID",model.RuleGUID),
                new SqlParameter("@IsSendSms",model.IsSendSms),
                new SqlParameter("@ToBOrder",model.ToBOrder),
                new SqlParameter("@CreateUser",model.CreateUser)
            };
            return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters));
        }

        /// <summary>
        /// 塞过券的记录
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="ruleGUID"></param>
        /// <param name="mobileNumbers"></param>
        /// <returns></returns>
        public static List<VipPaintPromotionTemplateModel> SelectImportedPromotionDetail(SqlConnection conn,
            Guid ruleGUID, IEnumerable<string> mobileNumbers)
        {
            var sql = @"SELECT  d.MobileNumber ,
                                COUNT(1) AS PromotionCount
                        FROM    Configuration..VipPaintPackagePromotionRecord AS r WITH ( NOLOCK )
                                INNER JOIN Configuration..VipPaintPackagePromotionDetail AS d WITH ( NOLOCK ) ON r.BatchCode = d.BatchCode
                        WHERE   r.RuleGUID = @RuleGuid
                                AND d.MobileNumber IN (
                                SELECT  s.Item
                                FROM    Configuration..SplitString(@MobileNumbers, N',', 1) AS s )
                                AND d.IsDeleted = 0
                                AND (d.Remarks IS NULL OR d.Remarks = N'' OR d.Remarks NOT LIKE '%作废%')
                        GROUP BY d.MobileNumber;";
            var parameters = new[]
            {
                new SqlParameter("@RuleGuid",ruleGUID),
                new SqlParameter("@MobileNumbers",string.Join(",", mobileNumbers ?? new List<string>()))
            };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters).ConvertTo<VipPaintPromotionTemplateModel>().ToList();
        }
        #endregion

        #region 塞券记录

        /// <summary>
        /// 查询塞券记录
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="batchCode"></param>
        /// <param name="mobileNumber"></param>
        /// <param name="pid"></param>
        /// <param name="vipUserId"></param>
        /// <param name="packagePid"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public static List<VipPaintPromotionRecordViewModel> SelectPromotionRecord
            (SqlConnection conn, string batchCode, string mobileNumber,
            string packagePid, Guid vipUserId, int pageIndex, int pageSize, out int totalCount)
        {
            #region Sql
            var sql = @"SELECT  @Total = COUNT(1)
                        FROM    Configuration..VipPaintPackageConfig (NOLOCK) AS c
                                RIGHT JOIN Configuration..VipPaintPackagePromotionRecord (NOLOCK) AS r ON c.PKID = r.PackageId
                        WHERE   ( @BatchCode IS NULL
                                  OR @BatchCode = N''
                                  OR r.BatchCode = @BatchCode
                                )
                                AND ( @MobilePhone IS NULL
                                      OR @MobilePhone = N''
                                      OR EXISTS ( SELECT    1
                                                  FROM      Configuration..VipPaintPackagePromotionDetail
                                                            AS dd WITH ( NOLOCK )
                                                  WHERE     dd.MobileNumber = @MobilePhone
                                                            AND r.BatchCode = dd.BatchCode 
                                                            AND dd.IsDeleted =0)
                                    )
                                AND ( @PackagePid = ''
                                      OR c.PackagePid = @PackagePid
                                    )
                                AND ( @VipUserId = '00000000-0000-0000-0000-000000000000'
                                      OR c.VipUserId = @VipUserId
                                    )
                                AND c.IsDeleted =0;
                        SELECT  r.PackageId ,
                                r.BatchCode ,
                                c.PackagePid ,
                                c.PackageName ,
                                c.VipUserId ,
                                ( SELECT    COUNT(1)
                                  FROM      Configuration..VipPaintPackagePromotionDetail (NOLOCK) AS d
                                  WHERE     r.BatchCode = d.BatchCode
                                            AND d.IsDeleted =0
                                ) AS UploadCount ,
                                ( SELECT    COUNT(1)
                                  FROM      Configuration..VipPaintPackagePromotionDetail (NOLOCK) AS d
                                  WHERE     r.BatchCode = d.BatchCode
                                            AND d.Status = N'Success'
                                            AND d.IsDeleted =0
                                ) AS SuccessCount ,
                                r.CreateUser ,
                                r.CreateDateTime ,
                                r.IsSendSms
                        FROM    Configuration..VipPaintPackageConfig (NOLOCK) AS c
                                RIGHT JOIN Configuration..VipPaintPackagePromotionRecord (NOLOCK) AS r ON c.PKID = r.PackageId
                        WHERE   ( @BatchCode IS NULL
                                  OR @BatchCode = N''
                                  OR r.BatchCode = @BatchCode
                                )
                                AND ( @MobilePhone IS NULL
                                      OR @MobilePhone = N''
                                      OR EXISTS ( SELECT    1
                                                  FROM      Configuration..VipPaintPackagePromotionDetail
                                                            AS dd WITH ( NOLOCK )
                                                  WHERE     dd.MobileNumber = @MobilePhone
                                                            AND r.BatchCode = dd.BatchCode
                                                            AND dd.IsDeleted =0)
                                    )
                                AND ( @PackagePid = ''
                                      OR c.PackagePid = @PackagePid
                                    )
                                AND ( @VipUserId = '00000000-0000-0000-0000-000000000000'
                                      OR c.VipUserId = @VipUserId
                                    )
                                AND c.IsDeleted =0
                        ORDER BY r.CreateDateTime DESC
                                OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
                                ONLY;";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@BatchCode",batchCode),
                new SqlParameter("@MobilePhone",mobileNumber),
                new SqlParameter("@PackagePid",packagePid),
                new SqlParameter("@VipUserId",vipUserId.ToString()),
                new SqlParameter("@PageIndex",pageIndex),
                new SqlParameter("@PageSize",pageSize),
                new SqlParameter("@Total", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters)
                .ConvertTo<VipPaintPromotionRecordViewModel>().ToList();
            totalCount = Convert.ToInt32(parameters.LastOrDefault().Value.ToString());
            return result;
        }

        /// <summary>
        /// 获取当前批次相关信息及对应套餐配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="batchCode"></param>
        /// <returns></returns>
        public static VipPaintPackageConfigForDetail GetPackageConfigByBatchCode(SqlConnection conn, string batchCode)
        {
            #region Sql
            var sql = @"SELECT  r.BatchCode ,
                                s.PackagePrice ,
                                s.VipUserId ,
                                s.PackagePid ,
                                s.PackageName ,
                                s.SettlementMethod ,
                                r.RuleGUID ,
                                r.CreateUser ,
                                r.IsSendSms
                        FROM    Configuration..VipPaintPackageConfig AS s WITH ( NOLOCK )
                                LEFT JOIN Configuration..VipPaintPackagePromotionRecord AS r WITH ( NOLOCK ) ON s.PKID = r.PackageId
                        WHERE   r.BatchCode = @BatchCode
                                AND s.IsDeleted = 0;";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@BatchCode",batchCode)
            };
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters)
                .ConvertTo<VipPaintPackageConfigForDetail>().FirstOrDefault();
            return result;
        }

        /// <summary>
        /// 根据批次号获取塞券记录
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="batchCode"></param>
        /// <returns></returns>
        public static VipPaintPackagePromotionRecordModel GetPromotionRecord(SqlConnection conn, string batchCode)
        {
            var sql = @"SELECT
                          r.PKID,
                          r.PackageId,
                          r.BatchCode,
                          r.RuleGUID,
                          r.IsSendSms,
                          r.ToBOrder,
                          r.CreateUser
                        FROM Configuration..VipPaintPackagePromotionRecord AS r WITH ( NOLOCK )
                        WHERE r.BatchCode = @BatchCode;";
            var parameters = new[]
            {
                new SqlParameter("@BatchCode",batchCode)
            };
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters)
               .ConvertTo<VipPaintPackagePromotionRecordModel>().FirstOrDefault();
            return result;
        }

        #endregion

        #region 塞券详情

        /// <summary>
        /// 塞券详情
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="batchCode"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static List<VipPaintPromotionDetailViewModel> SelectPromotionDetail
        (SqlConnection conn, VipPaintPackagePromotionDetail model, int pageIndex, int pageSize, out int totalCount)
        {
            #region Sql
            var sql = @"SELECT  @Total = COUNT(1)
                        FROM    Configuration..VipPaintPackagePromotionDetail AS s WITH ( NOLOCK )
                        WHERE   s.BatchCode = @BatchCode
                        AND ( @MobileNumber IS NULL
                              OR @MobileNumber = N''
                              OR s.MobileNumber = @MobileNumber
                            )
                        AND ( @PromotionId < 1
                              OR s.PromotionId = @PromotionId
                            )
                        AND ( @Status IS NULL
                              OR @Status = N''
                              OR s.Status = @Status
                            )
                        AND s.IsDeleted =0;
                        SELECT  s.PKID ,
                                s.BatchCode ,
                                s.MobileNumber ,
                                s.PromotionId ,
                                s.CarNo ,
                                s.Status ,
                                s.Remarks ,
                                s.CreateDateTime ,
                                s.StartDateTime ,
                                s.EndDateTime 
                        FROM    Configuration..VipPaintPackagePromotionDetail AS s WITH ( NOLOCK )
                        WHERE   s.BatchCode = @BatchCode
                        AND ( @MobileNumber IS NULL
                              OR @MobileNumber = N''
                              OR s.MobileNumber = @MobileNumber
                            )
                        AND ( @PromotionId < 1
                              OR s.PromotionId = @PromotionId
                            )
                        AND ( @Status IS NULL
                              OR @Status = N''
                              OR s.Status = @Status
                            )
                        AND s.IsDeleted =0
                        ORDER BY s.CreateDateTime DESC
                                OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
                                ONLY;;";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@BatchCode",model.BatchCode),
                new SqlParameter("@MobileNumber",model.MobileNumber),
                new SqlParameter("@PromotionId",model.PromotionId),
                new SqlParameter("@Status",model.Status),
                new SqlParameter("@PageIndex",pageIndex),
                new SqlParameter("@PageSize",pageSize),
                new SqlParameter("@Total", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters)
                .ConvertTo<VipPaintPromotionDetailViewModel>().ToList();
            totalCount = Convert.ToInt32(parameters.LastOrDefault().Value.ToString());
            return result;
        }

        /// <summary>
        /// 作废优惠券备注
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="promotionId"></param>
        /// <param name="remarks"></param>
        /// <returns></returns>
        public static bool UpdatePromotionDetailRemark(SqlConnection conn, int promotionId, string remarks)
        {
            var sql = @"UPDATE  Configuration..VipPaintPackagePromotionDetail
                        SET     Remarks = @Remarks ,
                                LastUpdateDateTime = GETDATE()
                        WHERE   PromotionId = @PromotionId;";
            var parameters = new[]
            {
                new SqlParameter("@PromotionId",promotionId),
                new SqlParameter("@Remarks",remarks),
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
        }

        /// <summary>
        /// 该批次下塞券不成功的记录
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="batchCode"></param>
        /// <returns></returns>
        public static int GetNotSuccessPromotionDtailCount(SqlConnection conn, string batchCode)
        {
            var sql = @"SELECT  COUNT(1)
                        FROM    Configuration..VipPaintPackagePromotionDetail AS s WITH ( NOLOCK )
                        WHERE   s.BatchCode = @BatchCode
                                AND s.Status <> @Status
                                AND s.IsDeleted =0;";
            var parameters = new[]
            {
                new SqlParameter("@BatchCode",batchCode),
                new SqlParameter("@Status",Status.SUCCESS.ToString())
            };
            return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters));
        }

        /// <summary>
        /// 更新塞券详情记录
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool UpdatePromotionDetail(SqlConnection conn, long pkid, string mobilePhone, string remark)
        {
            var sql = @"UPDATE Configuration..VipPaintPackagePromotionDetail
                        SET MobileNumber = @MobileNumber,
                            LastUpdateDateTime = GETDATE(),
                            Status = @Status,
                            Remarks = @Remarks
                        WHERE PKID = @PKID;";
            var parameters = new[]
            {
                new SqlParameter("@MobileNumber",mobilePhone),
                new SqlParameter("@Status",Status.WAIT.ToString()),
                new SqlParameter("@Remarks",remark),
                new SqlParameter("@PKID",pkid)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
        }

        /// <summary>
        /// 删除塞券详情记录
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static bool DeletePromotionDetail(SqlConnection conn, long pkid)
        {
            var sql = @"UPDATE Configuration..VipPaintPackagePromotionDetail
                        SET IsDeleted = 1,
                            LastUpdateDateTime = getdate()
                        WHERE PKID = @PKID 
                              AND IsDeleted = 0";
            var parameters = new[]
            {
                new SqlParameter("@PKID",pkid)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
        }

        /// <summary>
        /// 获取塞券详情记录
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static VipPaintPackagePromotionDetail GetPromotionDetail(SqlConnection conn, long pkid)
        {
            var sql = @"SELECT
                          s.PKID,
                          s.BatchCode,
                          s.MobileNumber,
                          s.CarNo,
                          s.PromotionId,
                          s.Status,
                          s.Remarks,
                          s.CreateDateTime,
                          s.LastUpdateDateTime
                        FROM Configuration..VipPaintPackagePromotionDetail AS s WITH ( NOLOCK )
                        WHERE s.PKID = @PKID
                                AND s.IsDeleted = 0;";
            var parameters = new[]
            {
                new SqlParameter("@PKID",pkid)
            };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters)
               .ConvertTo<VipPaintPackagePromotionDetail>().FirstOrDefault();
        }


        #endregion

        #region 短信配置

        /// <summary>
        /// 查看短信配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public static List<VipPaintPackageSmsConfig> SelectPackageSmsConfig
        (SqlConnection conn, VipPaintPackageSmsConfig model, int pageIndex, int pageSize, out int totalCount)
        {
            #region Sql
            var sql = @"SELECT  @Total = COUNT(1)
                        FROM    Configuration..VipPaintPackageSmsConfig AS s WITH ( NOLOCK )
                                RIGHT  JOIN Configuration..VipPaintPackageConfig AS c WITH ( NOLOCK ) ON s.PackageId = c.PKID
                        WHERE   c.IsDeleted = 0
                                AND ( @VipUserId IS NULL
                                      OR @VipUserId = N'00000000-0000-0000-0000-000000000000'
                                      OR c.VipUserId = @VipUserId
                                    )
                                AND ( @PackageId < 1
                                      OR c.PKID = @PackageId
                                    );
                        SELECT  c.VipUserId ,
                                c.PKID AS PackageId ,
                                c.PackagePid ,
                                c.PackageName ,
                                ISNULL(s.IsSendSms, 0) AS IsSendSms
                        FROM    Configuration..VipPaintPackageSmsConfig AS s WITH ( NOLOCK )
                                RIGHT  JOIN Configuration..VipPaintPackageConfig AS c WITH ( NOLOCK ) ON s.PackageId = c.PKID
                        WHERE   c.IsDeleted = 0
                                AND ( @VipUserId IS NULL
                                      OR @VipUserId = N'00000000-0000-0000-0000-000000000000'
                                      OR c.VipUserId = @VipUserId
                                    )
                                AND ( @PackageId < 1
                                      OR c.PKID = @PackageId
                                    )
                        ORDER BY c.PKID DESC
                                OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
                                ONLY;";
            #endregion
            var parameters = new[]
            {
                new SqlParameter("@VipUserId",model.VipUserId),
                new SqlParameter("@PackageId",model.PackageId),
                new SqlParameter("@PageIndex",pageIndex),
                new SqlParameter("@PageSize",pageSize),
                new SqlParameter("@Total", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters)
                .ConvertTo<VipPaintPackageSmsConfig>().ToList();
            totalCount = Convert.ToInt32(parameters.LastOrDefault().Value.ToString());
            return result;
        }

        /// <summary>
        /// 配置是否存在
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="packageId"></param>
        /// <returns></returns>
        public static bool IsPackageSmsConfigExist(SqlConnection conn, int packageId)
        {
            var sql = @"SELECT COUNT(1)
                        FROM Configuration..VipPaintPackageSmsConfig AS s WITH ( NOLOCK )
                        WHERE PackageId = @PackageId;";
            var parameters = new[]
            {
                new SqlParameter("@PackageId",packageId)
            };
            var result = Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters));
            return result > 0;
        }

        /// <summary>
        /// 读取套餐是否发送短信配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="packageId"></param>
        /// <returns></returns>
        public static VipPaintPackageSmsConfig GetPackageSendSmsConfig(SqlConnection conn, int packageId)
        {
            var sql = @"SELECT  s.PKID ,
                                s.PackageId ,
                                s.IsSendSms ,
                                s.Operator ,
                                s.CreateDateTime ,
                                s.LastUpdateDateTime
                        FROM    Configuration..VipPaintPackageSmsConfig AS s WITH ( NOLOCK )
                        WHERE   PackageId = @PackageId;";
            var parameters = new[]
            {
                new SqlParameter("@PackageId",packageId)
            };
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters)
                .ConvertTo<VipPaintPackageSmsConfig>().FirstOrDefault();
            return result;
        }

        /// <summary>
        /// 插入短信配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int InsertPackageSmsConfig(SqlConnection conn, VipPaintPackageSmsConfig model)
        {
            var sql = @"INSERT  INTO Configuration..VipPaintPackageSmsConfig
                                ( PackageId, IsSendSms, Operator )
                        OUTPUT  inserted.PKID
                        VALUES  ( @PackageId, @IsSendSms, @Operator );";
            var parameters = new[]
            {
                new SqlParameter("@PackageId",model.PackageId),
                new SqlParameter("@IsSendSms",model.IsSendSms),
                new SqlParameter("@Operator",model.Operator)
            };
            var result = Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters));
            return result;
        }

        /// <summary>
        /// 更新短信配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="packageId"></param>
        /// <returns></returns>
        public static bool UpdatePackageSendSms(SqlConnection conn, VipPaintPackageSmsConfig model)
        {
            var sql = @"UPDATE  Configuration..VipPaintPackageSmsConfig
                        SET     IsSendSms = @IsSendSms ,
                                Operator = @Operator ,
                                LastUpdateDateTime = GETDATE()
                        WHERE   PackageId = @PackageId;";
            var parameters = new[]
            {
                new SqlParameter("@IsSendSms",model.IsSendSms),
                new SqlParameter("@Operator",model.Operator),
                new SqlParameter("@PackageId",model.PackageId)
            };
            var result = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
            return result;
        }

        #endregion
    }
}
