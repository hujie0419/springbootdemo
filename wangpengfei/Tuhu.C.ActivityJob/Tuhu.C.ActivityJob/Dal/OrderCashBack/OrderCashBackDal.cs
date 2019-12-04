using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.ActivityJob.Models.OrderCashBack;

namespace Tuhu.C.ActivityJob.Dal.OrderCashBack
{
    public class OrderCashBackDal
    {
        /// <summary>
        /// 获取要推送的用户数据: 未推送&未返现&订单时间超过7天
        /// </summary>
        /// <returns></returns>
        public static List<OrderCashBackOwnerModel> GetFailPushOwnerList() 
        {
            string sql = @"Select TOP 40000
                                   PKID,OrderId,ShareId,UserId
                            FROM [Activity].[dbo].[OrderCashBackOwner] WITH (NOLOCK)
                            WHERE IsFailPush = 0
                                  AND OrderCreateTime < @date
                                  AND Status < 3;";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@date", DateTime.Now.AddDays(-7));
                var dnResult = DbHelper.ExecuteSelect<OrderCashBackOwnerModel>(cmd);
                return dnResult?.ToList();
            }
        }

        /// <summary>
        /// 失败推送后修改推送状态
        /// </summary>
        /// <param name="PKIDs"></param>
        /// <returns></returns>
        public static int SetIsFailPush(List<int> PKIDs) 
        {
            string sql = @"UPDATE a
                            SET a.IsFailPush = 1,
                                a.LastUpdateDateTime = GETDATE()
                            FROM [Activity].[dbo].[OrderCashBackOwner] AS a WITH (ROWLOCK)
                                JOIN Activity..SplitString(@PKIDs, ',', 1) AS b
                                    ON a.PKID = b.Item
                            WHERE a.IsFailPush = 0;";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@PKIDs", string.Join(",", PKIDs));
                var dnResult = DbHelper.ExecuteNonQuery(cmd);
                int.TryParse(dnResult.ToString(), out int count);
                return count;
            }
        }

    }
}
