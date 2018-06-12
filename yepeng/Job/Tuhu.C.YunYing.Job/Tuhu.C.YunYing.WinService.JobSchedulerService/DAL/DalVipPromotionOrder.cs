using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.C.YunYing.WinService.JobSchedulerService.Model;
using static Tuhu.C.YunYing.WinService.JobSchedulerService.Model.VipPromotionOrder;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.DAL
{
   public class DalVipPromotionOrder
    {
        /// <summary>
        /// 获取订单
        /// </summary>
        /// <param name="orderTypes"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static IEnumerable<VipPromotionOrderModel> GetOrderIds(List<string> orderType, DateTime startTime, DateTime endTime)
        {
            var sql = @"SELECT  o.OrderType ,
                                o.PKID AS OrderId
                        FROM    Gungnir..tbl_Order AS o WITH ( NOLOCK )
                        WHERE   o.LastUpdateTime < @EndTime
                                AND o.LastUpdateTime >= @StartTime
                                AND o.OrderType IN (SELECT *
                                                    FROM Gungnir..SplitString(@OrderType, N',', 1));";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@OrderType", string.Join(",", orderType));
                cmd.Parameters.AddWithValue("@StartTime", startTime);
                cmd.Parameters.AddWithValue("@EndTime", endTime);
                return DbHelper.ExecuteSelect<VipPromotionOrderModel>(true, cmd);
            }
        }

        /// <summary>
        /// 筛选传入订单中指定类型的订单
        /// </summary>
        /// <param name="orderIds"></param>
        /// <param name="orderType"></param>
        /// <returns></returns>
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
    }
}
