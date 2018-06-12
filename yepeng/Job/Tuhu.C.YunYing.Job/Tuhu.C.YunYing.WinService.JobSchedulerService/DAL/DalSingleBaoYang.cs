using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.YunYing.WinService.JobSchedulerService.Model;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.DAL
{
    public class DalSingleBaoYang
    {
        public static IEnumerable<int> GetOrderIds(string orderType, DateTime startTime, DateTime endTime)
        {
            using (var cmd = new SqlCommand(@"SELECT  o.PKID AS OrderId
FROM    Gungnir..tbl_Order AS o WITH ( NOLOCK )
WHERE   o.LastUpdateTime < @EndTime
        AND o.LastUpdateTime >= @StartTime
        AND o.OrderType = @OrderType;"))
            {
                cmd.Parameters.AddWithValue("@OrderType", orderType);
                cmd.Parameters.AddWithValue("@StartTime", startTime);
                cmd.Parameters.AddWithValue("@EndTime", endTime);
                return DbHelper.ExecuteQuery(true, cmd, dt => dt.Rows.Cast<DataRow>().Select(x => (int)x["OrderId"]));
            }
        }

        public static SingleBaoYangPackage SelectSingleBaoYangPackage(long promotionId)
        {
            using (var cmd = new SqlCommand(@"SELECT  t.VipUserId ,
        t.PackageName ,
        t.Price ,
        t.PID ,
        t.SettlementMethod ,
        N'' AS RedemptionCode
FROM    BaoYang..VipBaoYangPackageConfig AS t WITH ( NOLOCK )
        INNER JOIN BaoYang..VipBaoYangPackagePromotionRecord AS r WITH ( NOLOCK ) ON r.PackageId = t.PKID
        INNER JOIN BaoYang..VipBaoYangPackagePromotionDetail AS d WITH ( NOLOCK ) ON r.BatchCode = d.BatchCode
WHERE   d.PromotionId = @PromotionId
UNION ALL
SELECT  t.VipUserId ,
        t.PackageName ,
        t.Price ,
        t.PID ,
        t.SettlementMethod ,
        d.RedemptionCode
FROM    BaoYang..VipBaoYangPackageConfig AS t WITH ( NOLOCK )
        INNER JOIN BaoYang..VipBaoYangGiftPackCouponDetail AS d WITH ( NOLOCK ) ON d.PackageId = t.PKID
WHERE   d.PromotionId = @PromotionId;"))
            {
                cmd.Parameters.AddWithValue("@PromotionId", promotionId);
                return DbHelper.ExecuteFetch<SingleBaoYangPackage>(true, cmd);
            }
        }

        public static IEnumerable<int> GetOrderIds(IEnumerable<int> orderIds, string orderType)
        {
            using (var cmd = new SqlCommand(@"WITH    OrderIds
          AS ( SELECT   Item AS OrderId
               FROM     Gungnir..SplitString(@OrderIds, N',', 1)
             )
    SELECT  o.PKID AS OrderId
    FROM    Gungnir..tbl_Order AS o WITH ( NOLOCK )
    WHERE   EXISTS ( SELECT 1
                     FROM   OrderIds
                     WHERE  o.PKID = OrderId )
            AND o.OrderType = @OrderType
            AND o.Status <> @Status;"))
            {
                cmd.Parameters.AddWithValue("@OrderIds", string.Join(",", orderIds ?? new List<int>()));
                cmd.Parameters.AddWithValue("@OrderType", orderType);
                cmd.Parameters.AddWithValue("@Status", "7Canceled");
                return DbHelper.ExecuteQuery(true, cmd, dt => dt.Rows.Cast<DataRow>().Select(x => (int)x["OrderId"]));
            }
        }

        public static SingleBaoYangPackage SelectSingleBaoYangPackage(string batchCode)
        {
            using (var cmd = new SqlCommand(@"SELECT  co.VipUserId ,
        co.PackageName ,
        co.Price ,
        co.PID ,
        co.SettlementMethod
FROM    BaoYang..VipBaoYangPackagePromotionRecord AS re WITH ( NOLOCK )
        INNER JOIN BaoYang..VipBaoYangPackageConfig AS co WITH ( NOLOCK ) ON re.PackageId = co.PKID
WHERE   re.BatchCode = @BatchCode;"))
            {
                cmd.Parameters.AddWithValue("@BatchCode", batchCode);
                return DbHelper.ExecuteFetch<SingleBaoYangPackage>(true, cmd);
            }
        }

        public static int SelectPromotionDetailsTotal(string batchCode)
        {
            using (var cmd = new SqlCommand(@"SELECT  COUNT(t.PKID)
FROM    BaoYang..VipBaoYangPackagePromotionDetail AS t WITH ( NOLOCK )
WHERE   t.BatchCode = @BatchCode;"))
            {
                cmd.Parameters.AddWithValue("@BatchCode", batchCode);
                var returnValue = DbHelper.ExecuteScalar(true, cmd);
                var result = (int)returnValue;
                return result;
            }
        }

        public static int SelectPromotionDetailsCount(string batchCode)
        {
            using (var cmd = new SqlCommand(@"SELECT  COUNT(DISTINCT t.PromotionId)
FROM    BaoYang..VipBaoYangPackagePromotionDetail AS t WITH ( NOLOCK )
WHERE   t.BatchCode = @BatchCode
        AND t.Status = @Status;"))
            {
                cmd.Parameters.AddWithValue("@BatchCode", batchCode);
                cmd.Parameters.AddWithValue("@Status", "SUCCESS");
                var returnValue = DbHelper.ExecuteScalar(true, cmd);
                var result = (int)returnValue;
                return result;
            }
        }

        public static bool UpdateBaoYangRedemptionCodeOrderId(string redemptionCode, int orderId)
        {
            using (var cmd = new SqlCommand(@"UPDATE  BaoYang..VipBaoYangRedemptionCode
SET     OrderId = @OrderId
WHERE   RedemptionCode = @RedemptionCode;"))
            {
                cmd.Parameters.AddWithValue("@OrderId", orderId);
                cmd.Parameters.AddWithValue("@RedemptionCode", redemptionCode);
                var result = DbHelper.ExecuteNonQuery(cmd);
                return result > 0;
            }
        }

        /// <summary>
        /// 根据优惠券Id获取保养大客户2B买断订单
        /// </summary>
        /// <param name="promotionId"></param>
        /// <returns></returns>
        public static int GetPreSettled2BOrderIdByPromotionId(long promotionId)
        {
            var sql = @"SELECT c.OrderId AS ToBOrder 
                        FROM BaoYang..VipBaoYangGiftPackCouponDetail
                          AS s WITH ( NOLOCK )
                          INNER JOIN BaoYang..VipBaoYangRedemptionCode AS c WITH ( NOLOCK )
                            ON s.RedemptionCode = c.RedemptionCode
                        WHERE s.PromotionId = @PromotionId
                        Union All
                        SELECT r.ToBOrder
                        FROM BaoYang..VipBaoYangPackagePromotionDetail AS d WITH ( NOLOCK )
                          INNER JOIN BaoYang..VipBaoYangPackagePromotionRecord AS r WITH ( NOLOCK )
                            ON d.BatchCode = r.BatchCode
                        WHERE d.PromotionId = @PromotionId;";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@PromotionId", promotionId);
                return Convert.ToInt32(DbHelper.ExecuteScalar(cmd));
            }
        }

    }
}
