using Common.Logging;
using Quartz;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Tuhu.C.Job.Job
{
    /// <summary>Sql代理</summary>
    [DisallowConcurrentExecution]
    public class WeiXinRecommendFriendJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(WeiXinRecommendFriendJob));
        public void Execute(IJobExecutionContext context)
        {
            Logger.Info("启动任务");
            using (var cmd = new SqlCommand(@"SELECT  DISTINCT
		RecommendID,
		Invitation_UserID
FROM	SystemLog.[dbo].[tbl_Recommend] AS R WITH ( NOLOCK )
INNER JOIN Tuhu_profiles..UserObject U WITH ( NOLOCK )
		ON '{' + R.W_Invitation_UserID + '}' = U.u_user_id
INNER JOIN Gungnir.[dbo].[tbl_Order] AS O WITH ( NOLOCK )
		ON O.UserID = U.UserID
INNER JOIN dbo.tbl_OrderList AS TOR WITH ( NOLOCK )
		ON O.PKID = TOR.OrderID
WHERE	R.IsGet = 0
		AND R.IsOld = 0
		AND ( ( O.Status = '3Installed'
				AND O.InstallStatus = '2Installed'
				AND O.InstallShopID > 0
				AND InstallType = '1ShopInstall' )
			  OR ( ISNULL(InstallShopID, 0) = 0
				   AND O.Status = '2Shipped'
				   AND DeliveryStatus IN ( '3.5Signed' ) ) )
		AND TOR.Category <> 'CarWashing';"))
            {
                try
                {
                    var dt = DbHelper.ExecuteQuery(true, cmd, _ => _);
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow item in dt.Rows)
                        {
                            ReturnCashCoupon((int)item["RecommendID"], item["Invitation_UserID"].ToString());
                        }
                    }
                }
                catch (Exception ex)
                {

                    Logger.Error(ex.Message, ex);
                }

            }
            Logger.Info("结束任务");
        }

        public void ReturnCashCoupon(int rid, string userId)
        {
            var cmd = new SqlCommand("[Gungnir].dbo.[PromotionCode_CreatePromotionCode]")
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Code", "");
            cmd.Parameters.AddWithValue("@UserId", new Guid(userId));
            cmd.Parameters.AddWithValue("@StartTime", DateTime.Now.Date);
            cmd.Parameters.AddWithValue("@EndTime", DateTime.Now.AddDays(30));
            cmd.Parameters.AddWithValue("@OrderId", 0);
            cmd.Parameters.AddWithValue("@Status", 0);
            cmd.Parameters.AddWithValue("@Type", 13);
            cmd.Parameters.AddWithValue("@Description", "途虎现金券");
            cmd.Parameters.AddWithValue("@Discount", 20);
            cmd.Parameters.AddWithValue("@MinMoney", 20);
            cmd.Parameters.AddWithValue("@DeviceID", "");
            cmd.Parameters.AddWithValue("@CodeChannel", "途虎");
            cmd.Parameters.AddWithValue("@RuleID", 38);
            cmd.Parameters.AddWithValue("@PromtionName", "途虎现金券");

            if (DbHelper.ExecuteNonQuery(cmd) > 0)
            {
                var cmd2 = new SqlCommand("UPDATE  SystemLog.[dbo].[tbl_Recommend] SET    IsGet = 1 , AwardTime = GETDATE() WHERE   RecommendID = @reID");
                cmd2.Parameters.AddWithValue("@reID", rid);
                DbHelper.ExecuteNonQuery(cmd2);
            }
        }
    }
}
