
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BaoYangRefreshCacheService.Model;
using Microsoft.SqlServer.Server;
using Tuhu;
using Tuhu.Service.BaoYang.Models;

namespace BaoYangRefreshCacheService.DAL
{
    public class BaoYangDal
    {
        private static readonly string tuhuBiReadConn = ConfigurationManager.ConnectionStrings["Tuhu_BI_ReadOnly"].ConnectionString;
        /// <summary>
        /// 获取所有的保养产品
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<BaoYangProductModel> SelectBaoYangProducts()
        {
            using (var cmd = new SqlCommand(@"SELECT  cp.oid ,
            cp.ProductID ,
            cp.VariantID ,
            cp.PID ,
            cp.DisplayName ,
            cp.CatalogName ,
            cp.PrimaryParentCategory ,
            ISNULL(Image_filename,
                   ISNULL(Image_filename_2,
                          ISNULL(Image_filename_3,
                                 ISNULL(Image_filename_4,
                                        ISNULL(Image_filename_5,
                                               ISNULL(Image_filename_Big,
                                                      ISNULL(Variant_Image_filename_1,
                                                             ISNULL(Variant_Image_filename_2,
                                                              Variant_Image_filename_3)))))))) AS Image_filename ,
            cp.cy_list_price ,
            cp.CP_Remark ,
            cp.CP_ShuXing1 ,
            cp.CP_ShuXing2 ,
            cp.CP_ShuXing3 ,
            cp.CP_ShuXing4 ,
            cp.CP_ShuXing5 ,
            cp.CP_ShuXing6 ,
            cp.CP_RateNumber ,
            cp.PartNo ,
            cp.CP_Brand ,
            cp.CP_Unit ,
            cp.Color ,
            cp.CP_Brake_Position ,
            ps.AvailableStockQuantity ,
            cp.stockout AS StockOut ,
            cp.OnSale AS OnSale ,
            cp.IsUsedInAdaptation AS IsUsedInAdaptation ,
            cp.CP_Wiper_Series ,
            cp.isOE
    FROM    Tuhu_productcatalog..[CarPAR_zh-CN] (NOLOCK) cp
            JOIN Tuhu_productcatalog..CarPAR_CatalogHierarchy (NOLOCK) ch ON cp.oid = ch.child_oid
            LEFT JOIN ( SELECT  PID ,
                                SUM(AvailableStockQuantity) AS AvailableStockQuantity
                        FROM    Gungnir..DataForStock (NOLOCK) opi
                                INNER JOIN BaoYang..BaoYang_WareHouse (NOLOCK) wh ON opi.WareHouseId = wh.WareHouseId
                                                              AND wh.IsDeleted = 0
                        GROUP BY PID
                      ) AS ps ON cp.PID = ps.PID COLLATE Chinese_PRC_CI_AS
    WHERE   ( ch.NodeNo = N'28656'
              OR ch.NodeNo LIKE N'28656.%'
            )
            AND cp.PrimaryParentCategory IS NOT NULL;"))
            {
                cmd.CommandType = CommandType.Text;

                return DbHelper.ExecuteSelect<BaoYangProductModel>(true, cmd);
            }
        }

        public static int InsertJobHistory(string jobName)
        {
            const string sql = @"INSERT  BaoYang.dbo.JobHistory
                                        (JobName,
                                          CreatedTime,
                                          RunStatus
                                        )
                                OUTPUT Inserted.PKID
                                VALUES  (@JobName,
                                         GETDATE(),
                                         'Running'
                                        ); ";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@JobName", jobName);

                return (int) DbHelper.ExecuteScalar(cmd);
            }
        }

        public static void UpdateJobHistory(int pkid, string status)
        {
            const string sql = @"UPDATE  BaoYang..JobHistory
                                SET     RunStatus = @Status ,
                                        UpdatedTime = GETDATE()
                                WHERE   PKID = @PKID";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@PKID", pkid);
                cmd.Parameters.AddWithValue("@Status", status);

                DbHelper.ExecuteNonQuery(cmd);
            }
        }

        public static DateTime? SelectJobSuccessTime(string jobName)
        {
            const string sql = @"SELECT  CreatedTime
                                FROM    BaoYang..JobHistory(NOLOCK)
                                WHERE   JobName = @JobName
                                        AND RunStatus = 'Success';";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@JobName", jobName);

                return DbHelper.ExecuteScalar(true, cmd) as DateTime?;
            }
        }

        public static RunJobConfig SelectRunJobConfig()
        {
            using (var cmd = new SqlCommand(@"SELECT TOP 1 * FROM BaoYang..[tbl_RunBaoYangJob](NOLOCK) WHERE RunStatus='New'")
                )
            {
                return DbHelper.ExecuteFetch<RunJobConfig>(true, cmd);
            }
        }

        public static void UpdateRunJobConfig(int id, string status)
        {
            using (var cmd = new SqlCommand(@"
                UPDATE BaoYang..[tbl_RunBaoYangJob]
                SET RunStatus = @Status, UpdatedTime = GETDATE()
                WHERE PKID = @Id"))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@Status", status);

                DbHelper.ExecuteNonQuery(cmd);
            }
        }

        public static List<string> GetAllOils()
        {
            using (
                var cmd =
                    new SqlCommand(
                        @"SELECT PID FROM Tuhu_productcatalog..[CarPAR_zh-CN] (NOLOCK) WHERE PrimaryParentCategory = 'Oil'")
                )
            {
                return DbHelper.ExecuteQuery(true, cmd, (table) =>
                {
                    List<string> pids = new List<string>();

                    foreach (DataRow row in table.Rows)
                    {
                        string pid = row["PID"].ToString();
                        pids.Add(pid);
                    }

                    return pids;
                });
            }
        }

        public static IEnumerable<OrderHistoryModel> GetOrders(string pid, int offset, int pageSize)
        {
            using (var cmd = new SqlCommand("Gungnir..BaoYangJob_SelectBaoYangOrdersByPid"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Pid", pid);
                cmd.Parameters.AddWithValue("@Offset", offset);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);

                return DbHelper.ExecuteSelect<OrderHistoryModel>(true, cmd);
            }
        }

        public static IEnumerable<OrderHistoryIncrementModel> SelectIncrementOrderHistory(DateTime time, int offset,
            int pageSize)
        {
            using (var cmd = new SqlCommand("Gungnir..BaoYangJob_SelectIncrementOrderHistory"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Time", time);
                cmd.Parameters.AddWithValue("@Offset", offset);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);

                return DbHelper.ExecuteSelect<OrderHistoryIncrementModel>(true, cmd);
            }
        }

        #region 机油推荐--用户购买过的机油
        /// <summary>
        /// 获取用户购买过的机油总数 以分批操作
        /// </summary>
        /// <returns></returns>
        public static int GetOilOrderUserDailyCount()
        {
            var sql = @"SELECT  COUNT(*)
                        FROM    Tuhu_bi..dm_oil_order_user_daily AS s WITH ( NOLOCK );";
            using (var dbHelper = DbHelper.CreateDbHelper(tuhuBiReadConn))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    var temp = dbHelper.ExecuteScalar(cmd);
                    var result = Convert.ToInt32(temp);
                    return result;
                }
            }
        }

        /// <summary>
        /// 分批获取用户购买过的机油记录
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public static IEnumerable<OilOrderUserDailyModel> SelectOilOrderUserDaily(int pageIndex, int PageSize)
        {
            var sql = @"SELECT  s.pid AS Pid ,
                                CONVERT(UNIQUEIDENTIFIER, s.userid) AS UserId ,
                                s.orderid AS OrderId ,
                                s.VehicleID AS VehicleId ,
                                s.PaiLiang ,
                                s.Nian
                        FROM    Tuhu_bi..dm_oil_order_user_daily AS s WITH ( NOLOCK )
                        ORDER BY s.orderid
                                OFFSET ( @PageIndex - 1 ) * @PageSize ROWS  FETCH NEXT @PageSize ROWS
                                        ONLY;";
            using (var dbHelper = DbHelper.CreateDbHelper(tuhuBiReadConn))
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                cmd.Parameters.AddWithValue("@PageSize", PageSize);
                return dbHelper.ExecuteSelect<OilOrderUserDailyModel>(cmd);
            }
        }
        #endregion
    }
}
