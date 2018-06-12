using Common.Logging;
using Quartz;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.Job
{
    /// <summary>818送机油自动返券 huhengxing 2016-8-16 未上线不使用了</summary>
    [DisallowConcurrentExecution]
    public class JiYouFanQuanJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(JiYouFanQuanJob));

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                if (DateTime.Now < new DateTime(2016, 8, 25))
                {
                    Logger.Info("2016-8-25开始发放818送机油自动返券");
                    return;
                }
                if (DateTime.Now > new DateTime(2016, 9, 10))
                {
                    Logger.Info("2016-9-10后结束发放818送机油自动返券");
                    return;
                }
                Logger.Info("启动任务");
                using (var cmd = new SqlCommand(@" 		SELECT
		                                                O.UserID,
		                                                O.UserTel,
		                                                O.SumMoney,
		                                                O.PKID AS OrderID
                                                FROM	Gungnir..tbl_Order AS O WITH ( NOLOCK )
                                                WHERE   (O.Status = N'3Installed' OR O.DeliveryStatus ='3Received' )
                                                AND O.OrderType !='12加油卡' AND O.OrderType !='11违章代缴'
                                                AND O.CreateDateTime>'2016-08-18' AND O.CreateDateTime<'2016-08-25 12:00:00'
                                                AND NOT EXISTS ( SELECT	1
						                                            FROM	Gungnir..tbl_PromotionCode AS PC WITH ( NOLOCK )
						                                            WHERE	PC.UserId = O.UserID
								                                        AND PC.BatchID = O.PKID
								                                        AND PC.CodeChannel = N'818送机油自动返券活动' )
		                                        AND O.PKID NOT IN (
				                                        SELECT T.PKID FROM Gungnir..tbl_Order AS T WITH ( NOLOCK )
				                                        JOIN Gungnir..tbl_OrderList AS L WITH ( NOLOCK ) ON T.PKID = L.OrderID
				                                        where (T.Status = N'3Installed' OR T.DeliveryStatus ='3Received' )
				                                        AND T.CreateDateTime>'2016-08-18' AND T.CreateDateTime<'2016-08-25 12:00:00'
				                                        AND T.OrderType !='12加油卡' AND T.OrderType !='11违章代缴'
				                                        AND L.Remark like N'%4e5b7862-85de-433e-a62e-d235425654a7%'
		                                        )
                "))
                {
                    cmd.CommandType = CommandType.Text;
                    var dt = Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(true, cmd);
                    var dic = dt.Rows.OfType<DataRow>().Select(x => new { UserID = x["UserID"].ToString(), SumMoney = x["SumMoney"].ToString(), UserTel = x["UserTel"].ToString(), OrderID = x["OrderID"].ToString() }).ToList();
                    foreach (var i in dic)
                    {
                        if (!string.IsNullOrWhiteSpace(i.SumMoney))
                        {
                            int result = 0;
                            int m = 0;
                            if (Convert.ToDecimal(i.SumMoney) >= 1499)
                            {
                                result = CreateOrYzPromotion(i.UserID, 308, i.OrderID, 607, 1020, "0817 送机油 308元胜牌机油券");
                                m = 308;
                            }
                            else if (Convert.ToDecimal(i.SumMoney) >= 999)
                            {
                                result = CreateOrYzPromotion(i.UserID, 208, i.OrderID, 606, 1020, "0817 送机油 208元胜牌机油券");
                                m = 208;
                            }
                            else if (Convert.ToDecimal(i.SumMoney) >= 699)
                            {
                                result = CreateOrYzPromotion(i.UserID, 138, i.OrderID, 605, 1020, "0817 送机油 138元胜牌机油券");
                                m = 138;
                            }

                            if (result > 0 && m > 0)
                            {
                                BLL.Business.SendMarketingSms(i.UserTel, "途虎送机油", m, "胜牌机油专用券");
                            }
                        }
                    }
                }

                Logger.Info("结束任务");
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
        }

        public static int CreateOrYzPromotion(string userId, int money, string orderId, int getRuleId, int ruleId, string description)
        {
            try
            {
                var promotionCodeModel = new PromotionCodeModel();
                promotionCodeModel.OrderId = 0;
                promotionCodeModel.Status = 0;
                promotionCodeModel.Description = description;
                promotionCodeModel.Discount = money;
                promotionCodeModel.MinMoney = (money + 1);
                promotionCodeModel.EndTime = DateTime.Now.AddDays(90).Date;
                promotionCodeModel.StartTime = DateTime.Now.Date;
                promotionCodeModel.UserId = new Guid(userId);
                promotionCodeModel.CodeChannel = "818送机油自动返券活动";
                promotionCodeModel.BatchId = Convert.ToInt32(orderId);
                promotionCodeModel.GetRuleId = getRuleId;
                promotionCodeModel.RuleId = ruleId;
                promotionCodeModel.Number = 1;
                var result = CreateMultiplePromotionCode(promotionCodeModel);
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return -99;
            }
        }

        public static int CreateMultiplePromotionCode(PromotionCodeModel promotionCodeModel)
        {
            try
            {
                using (var cmd = new SqlCommand("[Gungnir].[dbo].[Beautify_CreatePromotionCode]"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@StartTime", promotionCodeModel.StartTime.ToString());
                    cmd.Parameters.AddWithValue("@EndDateTime", promotionCodeModel.EndTime.ToString());
                    cmd.Parameters.AddWithValue("@Type", promotionCodeModel.Type);
                    cmd.Parameters.AddWithValue("@Description", promotionCodeModel.Description);
                    cmd.Parameters.AddWithValue("@Discount", promotionCodeModel.Discount);
                    cmd.Parameters.AddWithValue("@MinMoney", promotionCodeModel.MinMoney);
                    cmd.Parameters.AddWithValue("@Number", promotionCodeModel.Number);
                    cmd.Parameters.AddWithValue("@CodeChannel", promotionCodeModel.CodeChannel);
                    cmd.Parameters.AddWithValue("@UserID", promotionCodeModel.UserId);
                    cmd.Parameters.AddWithValue("@BatchID", promotionCodeModel.BatchId == null ? 0 : promotionCodeModel.BatchId.Value);
                    cmd.Parameters.AddWithValue("@RuleID", promotionCodeModel.RuleId);
                    cmd.Parameters.AddWithValue("@GetRuleID", promotionCodeModel.GetRuleId);
                    cmd.Parameters.Add(new SqlParameter()
                    {
                        ParameterName = "@Results",
                        DbType = DbType.Int32,
                        Direction = ParameterDirection.Output,
                        Value = 0
                    });

                    DbHelper.ExecuteNonQuery(cmd);
                    var result = Convert.ToInt32(cmd.Parameters["@Results"].Value);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return -99;
            }
        }
    }
}