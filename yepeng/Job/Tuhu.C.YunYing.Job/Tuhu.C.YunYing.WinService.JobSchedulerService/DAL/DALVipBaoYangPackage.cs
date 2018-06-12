using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.YunYing.WinService.JobSchedulerService.Model;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.DLL
{
    public static class DALVipBaoYangPackage
    {
        public static BaoYangPackagePromotionRecord SelectBaoYangPackagePromotionInfoByBatchCode(string batchCode)
        {
            using (var cmd = new SqlCommand(@"SELECT  ro.BatchCode ,
        ro.RulesGUID ,
        ro.CreateUser ,
        ro.IsSendSms ,
        co.SettlementMethod
FROM    BaoYang..VipBaoYangPackagePromotionRecord AS ro WITH ( NOLOCK )
        INNER JOIN BaoYang..VipBaoYangPackageConfig AS co WITH ( NOLOCK ) ON ro.PackageId = co.PKID
WHERE   BatchCode = @BatchCode;"))
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.AddWithValue("@BatchCode", batchCode);
                return DbHelper.ExecuteQuery<BaoYangPackagePromotionRecord>(cmd, dt =>
                {
                    return dt.Rows.OfType<DataRow>().ConvertTo<BaoYangPackagePromotionRecord>().FirstOrDefault();
                });
            };
        }

        public static List<BaoYangPackagePromotionDetail> SelectNoSuccessPromotionDetailsByBatchCode(string batchCode)
        {
            using (var cmd = new SqlCommand(@"SELECT  BatchCode ,
        MobileNumber ,
        Carno ,
        PKID,
        StartTime,
        EndTime 
FROM    BaoYang..VipBaoYangPackagePromotionDetail (NOLOCK)
WHERE   BatchCode = @BatchCode
        AND Status <> 'SUCCESS';"))
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.AddWithValue("@BatchCode", batchCode);
                return DbHelper.ExecuteSelect<BaoYangPackagePromotionDetail>(cmd).ToList();
            };
        }

        public static bool UpdateBaoYangPackagePromotionToSuccess(BaseDbHelper dbHelper, string batchCode, long pkid, int promotionId)
        {
            using (var cmd = new SqlCommand(@"UPDATE  BaoYang..VipBaoYangPackagePromotionDetail
SET     PromotionId = @PromotionId ,
        Status = 'SUCCESS' ,
        Remarks = NULL ,
        LastUpdateDateTime = GETDATE()
WHERE   PKID = @PKID
        AND BatchCode = @BatchCode;
"))
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.AddWithValue("@BatchCode", batchCode);
                cmd.Parameters.AddWithValue("@PromotionId", promotionId);
                cmd.Parameters.AddWithValue("@PKID", pkid);
                return dbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        public static bool UpdateBaoYangPackagePromotionStatus(BaseDbHelper dbHelper, string batchCode,
            long pkid, Status status, string remarks = "")
        {
            using (var cmd = new SqlCommand(@"UPDATE  BaoYang..VipBaoYangPackagePromotionDetail
SET     Status = @Status ,
        Remarks = @Remarks ,
        LastUpdateDateTime = GETDATE()
WHERE   PKID = @PKID
        AND BatchCode = @BatchCode
        AND Status <> 'SUCCESS';"))
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.AddWithValue("@BatchCode", batchCode);
                cmd.Parameters.AddWithValue("@Status", status.ToString());
                cmd.Parameters.AddWithValue("@PKID", pkid);
                cmd.Parameters.AddWithValue("@Remarks", remarks);
                return dbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        public static bool BatchBaoYangPakckagePromotion(List<UploadDetails> info, string batchCode)
        {
            using (var sbc = new SqlBulkCopy(ConfigurationManager.ConnectionStrings["Gungnir"]?.ConnectionString))
            {
                sbc.BatchSize = 1000;
                sbc.BulkCopyTimeout = 0;
                //将DataTable表名作为待导入库中的目标表名
                sbc.DestinationTableName = "BaoYang..VipBaoYangPackagePromotionDetail";
                //将数据集合和目标服务器库表中的字段对应
                DataTable table = new DataTable();
                table.Columns.Add("BatchCode");
                table.Columns.Add("MobileNumber");
                table.Columns.Add("Carno");
                table.Columns.Add("PromotionId");
                table.Columns.Add("Status");
                table.Columns.Add("StartTime");
                table.Columns.Add("EndTime");
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
                    row["Carno"] = code.Carno;
                    row["Status"] = "WAIT";
                    row["StartTime"] = code.StartTime;
                    row["EndTime"] = code.EndTime;
                    table.Rows.Add(row);
                }
                sbc.WriteToServer(table);
                return true;
            }
        }

        public static int GetUserHasGetPromotionCount(Guid userId, int getRuleId)
        {
            using (var cmd = new SqlCommand(@"SELECT	COUNT(1)
                                FROM	Gungnir..tbl_PromotionCode  WITH ( NOLOCK )
                                WHERE	UserId = @UserID
		                        AND GetRuleID = @GetRuleID
		                        AND (Status = 0	 OR Status = 1	 AND OrderId > 0 )"))
            {
                cmd.Parameters.AddWithValue("@GetRuleID", getRuleId);
                cmd.Parameters.AddWithValue("@UserID", userId);
                return Convert.ToInt32(DbHelper.ExecuteScalar(cmd));
            }
        }

        /// <summary>
        /// 记录买断的批次生成的ToB订单
        /// </summary>
        /// <param name="batchCode"></param>
        /// <param name="toBOrder"></param>
        /// <returns></returns>
        public static bool UpdatePromotionDetailToBOrder(string batchCode, int toBOrder)
        {
            #region Sql
            var sql = @"UPDATE  BaoYang..VipBaoYangPackagePromotionRecord
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
    }
}
