using Common.Logging;
using Quartz;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.Job
{
    /// <summary>轮毂半价券 huhengxing 2016-5-31 已过期</summary>
    [DisallowConcurrentExecution]
    public class LunGuHalfPriceQuanJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LunGuHalfPriceQuanJob));

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                if (DateTime.Now < new DateTime(2016, 6, 6))
                {
                    Logger.Info("2016-6-6开始发放轮毂活动半价券");
                    return;
                }
                if (DateTime.Now > new DateTime(2016, 7, 1))
                {
                    Logger.Info("2016-6-30后结束发放轮毂活动半价券");
                    return;
                }
                Logger.Info("启动任务");
                using (var cmd = new SqlCommand(@"[Gungnir].[dbo].[ProMotionCode_LunGuHalfPriceQuan]"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    var dt = Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(true, cmd);
                    var dic = dt.Rows.OfType<DataRow>().Select(x => new { UserID = x["UserID"].ToString(), SumMoney = x["SumMoney"].ToString(), UserTel = x["UserTel"].ToString(), RuleID = x["RuleID"].ToString(), OrderID = x["OrderID"].ToString() }).ToList();
                    foreach (var i in dic)
                    {
                        if (!string.IsNullOrWhiteSpace(i.SumMoney))
                        {
                            decimal totalMoney = 0;
                            bool success = decimal.TryParse(i.SumMoney, out totalMoney);

                            if (success)
                            {
                                var m = (int)totalMoney / 2;
                                int result = CreateOrYzPromotion(i.UserID, m, i.OrderID);
                                if (result > 0)
                                {
                                    BLL.Business.SendMarketingSms(i.UserTel, "途虎轮毂半价", m, "途虎现金券");
                                }
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

        public static int CreateOrYzPromotion(string userId, int money, string orderID)
        {
            try
            {
                var promotionCodeModel = new PromotionCodeModel();
                promotionCodeModel.OrderId = 0;
                promotionCodeModel.Status = 0;

                promotionCodeModel.Description = "轮毂半价活动返券，途虎全网可用(不支持加油卡/美容服务)";
                promotionCodeModel.Discount = money;
                promotionCodeModel.MinMoney = (money + 1);
                promotionCodeModel.EndTime = DateTime.Now.AddDays(30).Date;
                promotionCodeModel.StartTime = DateTime.Now.Date;
                promotionCodeModel.UserId = new Guid(userId);
                promotionCodeModel.CodeChannel = "轮毂活动半价券";
                promotionCodeModel.BatchId = Convert.ToInt32(orderID);
                promotionCodeModel.RuleId = 562;
                var result = CreateMultiplePromotionCode(promotionCodeModel, 1);
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return -99;
            }
        }

        public static int CreateMultiplePromotionCode(PromotionCodeModel promotionCodeModel, int num)
        {
            if (num == -1)
                num = promotionCodeModel.Number;

            return CreateMultiplePromotionCode2(promotionCodeModel, num);
        }

        public static int CreateMultiplePromotionCode2(PromotionCodeModel promotionCodeModel, int num)
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
                    cmd.Parameters.AddWithValue("@Number", num);
                    cmd.Parameters.AddWithValue("@CodeChannel", promotionCodeModel.CodeChannel);
                    cmd.Parameters.AddWithValue("@UserID", promotionCodeModel.UserId);
                    cmd.Parameters.AddWithValue("@BatchID", promotionCodeModel.BatchId == null ? 0 : promotionCodeModel.BatchId.Value);
                    cmd.Parameters.AddWithValue("@RuleID", promotionCodeModel.RuleId);
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