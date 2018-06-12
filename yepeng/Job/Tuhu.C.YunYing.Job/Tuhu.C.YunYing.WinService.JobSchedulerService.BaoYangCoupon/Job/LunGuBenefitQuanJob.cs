using Common.Logging;
using Quartz;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.Job
{
    /// <summary>买轮胎送轮毂优惠券 huhengxing 2016-8-24 已过期</summary>
    [DisallowConcurrentExecution]
    public class LunGuBenefitQuanJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LunGuBenefitQuanJob));
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                if (DateTime.Now < new DateTime(2016, 8, 26))
                {
                    Logger.Info("2016-8-26开始发放轮毂优惠券");
                    return;
                }

                Logger.Info("启动任务");
                using (var cmd = new SqlCommand(@" 		SELECT	
		                                                O.UserID,
		                                                O.UserTel,
		                                                O.SumMoney,
		                                                O.PKID AS OrderID
                                                FROM	Gungnir..tbl_Order AS O WITH ( NOLOCK )
                                                WHERE   O.Status <> N'7Canceled'
												AND ((O.OrderDatetime>'2016-08-26' AND O.OrderDatetime<'2016-08-26 12:00:00') OR O.OrderDateTime > DATEADD(MINUTE,-35, GETDATE()) )
                                                AND O.OrderChannel IN (SELECT ChannelKey AS OrderChannel FROM Gungnir..tbl_ChannelDictionaries where (ChannelType=N'自有渠道' or ChannelType=N'H5合作渠道') and ChannelKey not like N'%汽车超人%')
												AND EXISTS (SELECT 1 FROM Gungnir..tbl_OrderList AS OL WITH(NOLOCK) WHERE OL.OrderID=O.PKID and OL.PID LIKE N'TR-%')
                                                AND NOT EXISTS ( SELECT	1
						                                            FROM	Gungnir..tbl_PromotionCode AS PC WITH ( NOLOCK )
						                                            WHERE	PC.UserId = O.UserID
								                                        AND PC.BatchID = O.PKID
								                                        AND PC.CodeChannel = N'买轮胎送轮毂券活动' )
												AND  (SELECT COUNT(1) FROM Gungnir..tbl_PromotionCode AS PC WITH ( NOLOCK )
						                                            WHERE	PC.UserId = O.UserID
								                                        AND PC.CodeChannel = N'买轮胎送轮毂券活动') < 5		                                        
                "))
                {
                    cmd.CommandType = CommandType.Text;
                    var dt = Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(true, cmd);
                    var dic = dt.Rows.OfType<DataRow>().Select(x => new { UserID = x["UserID"].ToString(), SumMoney = x["SumMoney"].ToString(), UserTel = x["UserTel"].ToString(), OrderID = x["OrderID"].ToString() }).ToList();
                    foreach (var i in dic)
                    {
                        if (!string.IsNullOrWhiteSpace(i.SumMoney))
                        {
                            int result = CreateOrYzPromotion(i.UserID, 30, i.OrderID,652, 1137, "买轮胎送轮毂券");
                            if (result < 1)
                            {
                                Logger.Error(new Exception("买轮胎送轮毂券发券失败"));
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

        public static int CreateOrYzPromotion(string userId, int money, string orderId,int getRuleId, int ruleId, string description)
        {
            try
            {
                var promotionCodeModel = new PromotionCodeModel();
                promotionCodeModel.OrderId = 0;
                promotionCodeModel.Status = 0;
                promotionCodeModel.Description = description;
                promotionCodeModel.Discount = money;
                promotionCodeModel.MinMoney = (money + 1);
                promotionCodeModel.EndTime = DateTime.Now.AddDays(5).Date;
                promotionCodeModel.StartTime = DateTime.Now.Date;
                promotionCodeModel.UserId = new Guid(userId);
                promotionCodeModel.CodeChannel = "买轮胎送轮毂券活动";
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
