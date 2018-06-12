using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.C.YunYing.WinService.JobSchedulerService.Model;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.DLL
{
    public static class DALVipPaintPackage
    {
        /// <summary>
        /// 根据批次号获取喷漆大客户套餐塞券记录
        /// </summary>
        /// <param name="batchCode"></param>
        /// <returns></returns>
        public static VipPaintPackagePromotionRecord SelectPaintPackagePromotionInfoByBatchCode(string batchCode)
        {
            #region Sql
            var sql = @"SELECT  ro.BatchCode ,
                                ro.RuleGUID ,
                                ro.CreateUser ,
                                ro.IsSendSms ,
                                co.SettlementMethod
                        FROM    Configuration..VipPaintPackagePromotionRecord AS ro WITH ( NOLOCK )
                                INNER JOIN Configuration..VipPaintPackageConfig AS co WITH ( NOLOCK ) ON ro.PackageId = co.PKID
                        WHERE   BatchCode = @BatchCode
                                AND co.IsDeleted =0;";
            #endregion
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@BatchCode", batchCode);
                return DbHelper.ExecuteQuery(cmd, dt =>
                {
                    return dt.Rows.OfType<DataRow>().ConvertTo<VipPaintPackagePromotionRecord>().FirstOrDefault();
                });
            };
        }

        /// <summary>
        /// 查询喷漆大客户套餐当前批次塞券失败的记录
        /// </summary>
        /// <param name="batchCode"></param>
        /// <returns></returns>
        public static List<VipPaintPackagePromotionDetail> SelectNoSuccessPromotionDetailsByBatchCode(string batchCode)
        {
            #region Sql
            var sql = @"SELECT  s.BatchCode ,
                                s.MobileNumber ,
                                s.CarNo ,
                                s.PKID
                        FROM    Configuration..VipPaintPackagePromotionDetail AS s WITH (NOLOCK)
                        WHERE   s.BatchCode = @BatchCode
                                AND s.Status <> @Status
                                AND s.IsDeleted = 0;";
            #endregion
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@BatchCode", batchCode);
                cmd.Parameters.AddWithValue("@Status", Status.SUCCESS.ToString());
                return DbHelper.ExecuteSelect<VipPaintPackagePromotionDetail>(cmd).ToList();
            };
        }

        /// <summary>
        /// 塞券成功更新塞券详情表中优惠券码
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="batchCode"></param>
        /// <param name="pkid"></param>
        /// <param name="promotionId"></param>
        /// <returns></returns>
        public static bool UpdatePaintPackagePromotionToSuccess
            (BaseDbHelper dbHelper, VipPaintPackagePromotionDetail model)
        {
            #region Sql
            var sql = @"UPDATE  Configuration..VipPaintPackagePromotionDetail
                        SET     PromotionId = @PromotionId ,
                                Status = @Status ,
                                Remarks = NULL ,
                                LastUpdateDateTime = GETDATE()
                        WHERE   PKID = @PKID
                                AND BatchCode = @BatchCode
                                AND IsDeleted = 0;";
            #endregion
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@BatchCode", model.BatchCode);
                cmd.Parameters.AddWithValue("@PromotionId", model.PromotionId);
                cmd.Parameters.AddWithValue("@Status", Status.SUCCESS.ToString());
                cmd.Parameters.AddWithValue("@PKID", model.PKID);
                return dbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        /// <summary>
        /// 更新塞券详情表中塞券记录
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="batchCode"></param>
        /// <param name="pkid"></param>
        /// <param name="status"></param>
        /// <param name="remarks"></param>
        /// <returns></returns>
        public static bool UpdatePaintPackagePromotionStatus(BaseDbHelper dbHelper, string batchCode,
            long pkid, Status status, string remarks = "")
        {
            #region Sql
            var sql = @"UPDATE  Configuration..VipPaintPackagePromotionDetail
                        SET     Status = @Status ,
                                Remarks = @Remarks ,
                                LastUpdateDateTime = GETDATE()
                        WHERE   PKID = @PKID
                                AND BatchCode = @BatchCode
                                AND IsDeleted = 0;";
            #endregion
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@BatchCode", batchCode);
                cmd.Parameters.AddWithValue("@Status", status.ToString());
                cmd.Parameters.AddWithValue("@PKID", pkid);
                cmd.Parameters.AddWithValue("@Remarks", remarks);
                return dbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        /// <summary>
        /// 批量插入塞券详情表中塞券记录
        /// </summary>
        /// <param name="info"></param>
        /// <param name="batchCode"></param>
        /// <returns></returns>
        public static bool BatchPaintPakckagePromotion(List<UploadDetails> info, string batchCode, int packageId)
        {
            using (var sbc = new SqlBulkCopy(ConfigurationManager.ConnectionStrings["Gungnir"]?.ConnectionString))
            {
                sbc.BatchSize = 1000;
                sbc.BulkCopyTimeout = 0;
                //将DataTable表名作为待导入库中的目标表名
                sbc.DestinationTableName = "Configuration..VipPaintPackagePromotionDetail";
                //将数据集合和目标服务器库表中的字段对应
                DataTable table = new DataTable();
                table.Columns.Add("BatchCode");
                table.Columns.Add("MobileNumber");
                table.Columns.Add("CarNo");
                table.Columns.Add("PromotionId");
                table.Columns.Add("Status");
                table.Columns.Add("PackageId");
                foreach (DataColumn col in table.Columns)
                {
                    //列映射定义数据源中的列和目标表中的列之间的关系
                    sbc.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                }
                foreach (var code in info)
                {
                    var row = table.NewRow();
                    row["BatchCode"] = batchCode;
                    row["MobileNumber"] = code.MobileNumber;
                    row["CarNo"] = code.Carno;
                    row["Status"] = Status.WAIT.ToString();
                    row["PackageId"] = packageId;
                    table.Rows.Add(row);
                }
                sbc.WriteToServer(table);
                return true;
            }
        }

        /// <summary>
        /// 根据塞券批次号获取大客户喷漆套餐配置
        /// </summary>
        /// <param name="batchCode"></param>
        /// <returns></returns>
        public static VipPaintPackageConfig GetVipPaintPackageConfig(string batchCode)
        {
            #region Sql
            var sql = @"SELECT
                          s.PKID,
                          s.PackagePid,
                          s.PackageName,
                          s.PackagePrice,
                          s.VipUserId,
                          s.SettlementMethod
                        FROM Configuration..VipPaintPackageConfig AS s WITH ( NOLOCK )
                          INNER JOIN Configuration..VipPaintPackagePromotionRecord AS r WITH ( NOLOCK )
                            ON s.PKID = r.PackageId
                        WHERE r.BatchCode = @BatchCode
                              AND s.IsDeleted = 0;";
            #endregion
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@BatchCode", batchCode);
                return DbHelper.ExecuteQuery(cmd, dt =>
                {
                    return dt.Rows.OfType<DataRow>().ConvertTo<VipPaintPackageConfig>().FirstOrDefault();
                });
            }
        }

        /// <summary>
        /// 根据优惠券获取大客户喷漆套餐配置
        /// </summary>
        /// <param name="promotionId"></param>
        /// <returns></returns>
        public static VipPaintPackageConfig GetVipPaintPackageConfig(long promotionId)
        {
            #region Sql
            var sql = @"SELECT
                          s.PackagePid,
                          s.PackageName,
                          s.PackagePrice,
                          s.VipUserId,
                          s.SettlementMethod
                        FROM Configuration..VipPaintPackageConfig AS s WITH ( NOLOCK )
                          INNER JOIN Configuration..VipPaintPackagePromotionDetail AS d WITH ( NOLOCK ) ON s.PKID = d.PackageId
                        WHERE d.PromotionId = @PromotionId
                              AND s.IsDeleted = 0
                              AND d.IsDeleted = 0;";
            #endregion
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@PromotionId", promotionId);
                return DbHelper.ExecuteQuery(cmd, dt =>
                {
                    return dt.Rows.OfType<DataRow>().ConvertTo<VipPaintPackageConfig>().FirstOrDefault();
                });
            }
        }

        /// <summary>
        /// 获取当前批次下所有塞券详情总数
        /// </summary>
        /// <param name="batchCode"></param>
        /// <returns></returns>
        public static int SelectPromotionDetailTotal(string batchCode)
        {
            #region Sql
            var sql = @"SELECT  COUNT(1)
                        FROM    Configuration..VipPaintPackagePromotionDetail AS s WITH ( NOLOCK )
                        WHERE   s.BatchCode = @BatchCode
                                AND s.IsDeleted = 0;";
            #endregion
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@BatchCode", batchCode);
                var count = Convert.ToInt32(DbHelper.ExecuteScalar(true, cmd));
                return count;
            }
        }

        /// <summary>
        /// 获取当前批次下塞券成功的记录
        /// </summary>
        /// <param name="batchCode"></param>
        /// <returns></returns>
        public static int SelectPromotionDetailSuccessCount(string batchCode)
        {
            #region Sql
            var sql = @"SELECT count(1)
                        FROM Configuration..VipPaintPackagePromotionDetail AS s WITH ( NOLOCK )
                        WHERE s.BatchCode = @BatchCode
                              AND Status = @Status
                              AND s.PromotionId IS NOT NULL AND s.PromotionId > 0
                              AND s.IsDeleted = 0;";
            #endregion
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@BatchCode", batchCode);
                cmd.Parameters.AddWithValue("@Status", Status.SUCCESS.ToString());
                var count = Convert.ToInt32(DbHelper.ExecuteScalar(true, cmd));
                return count;
            }
        }

        /// <summary>
        /// 买断的批次生成的ToB订单记录
        /// </summary>
        /// <param name="batchCode"></param>
        /// <param name="toBOrder"></param>
        /// <returns></returns>
        public static bool UpdatePromotionDetailToBOrder(string batchCode,string toBOrder)
        {
            #region Sql
            var sql = @"UPDATE  Configuration..VipPaintPackagePromotionRecord
                        SET     ToBOrder = @ToBOrder ,
                                LastUpdateDateTime = GETDATE()
                        WHERE   BatchCode = @BatchCode;";
            #endregion
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@BatchCode", batchCode);
                cmd.Parameters.AddWithValue("@ToBOrder", toBOrder);
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        /// <summary>
        ///  根据优惠券Id获取喷漆大客户2B买断订单
        /// </summary>
        /// <param name="promotionId"></param>
        /// <returns></returns>
        public static int GetPreSettled2BOrderIdByPromotionId(long promotionId)
        {
            #region Sql
            var sql = @"SELECT
                          r.ToBOrder
                        FROM Configuration..VipPaintPackagePromotionDetail AS d WITH ( NOLOCK )
                          INNER JOIN Configuration..VipPaintPackagePromotionRecord AS r WITH ( NOLOCK )
                            ON d.BatchCode = r.BatchCode
                        WHERE PromotionId = @PromotionId
                              AND d.IsDeleted = 0;";
            #endregion
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@PromotionId", promotionId);
                return Convert.ToInt32(DbHelper.ExecuteScalar(true, cmd));
            }
        }
    }
}
