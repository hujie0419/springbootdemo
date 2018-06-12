using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Quartz;
using System.Data;
using System.Data.SqlClient;
using Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.DAL;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.Job
{
    [DisallowConcurrentExecution]
    public class TireGiftPromotionJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(TireGiftPromotionJob));

        private const string ShopIds =
                @"23667,24089,22495,22955,22958,23559,23179,22499,22344,21818,22388,22389,22446,22447,22448,22450,22497,22500,23065,23279,11727,12498,13775,14817,15021,17980,19043,19136,19224,19377,20039,20510,20957,21825,22308,22309,22506,22574,22956,22968,22969,23111,23188,23194,23454,23492,23540,23553,23565,23673,23677,23844,24008,24128,22449,22492,22096,15020,22498,19470";
        public void Execute(IJobExecutionContext context)
        {
            if (DateTime.Now.Date < new DateTime(2017, 8, 28) || DateTime.Now.Date > new DateTime(2018, 1, 31))
            {
                Logger.Info("2017年8月28 到 2018年1月31 才能买马牌MC5轮胎送礼品券");
                return;
            }
            try
            {
                Logger.Info("买马牌MC5轮胎送礼品券job启动");
                DataTable orders = DAL.DalCoupon.SelectMaPaiTireOrders(ShopIds);
                if (orders != null && orders.Rows.Count > 0)
                {
                    Logger.Info($"获取到{orders.Rows.Count}个");
                    foreach (DataRow row in orders.Rows)
                    {
                        string orderno = row["OrderNo"]?.ToString();
                        string orderid = row["PKID"]?.ToString();
                        string userid = row["UserID"]?.ToString();
                        int productcount = string.IsNullOrEmpty(row["count"]?.ToString())
                            ? 0
                            : Convert.ToInt32(row["count"].ToString());
                        Logger.Info($"开始发放券userid:{userid}.orderid:{orderid}.productcount:{productcount}");
                        if (productcount >= 2)
                        {
                            Logger.Info($"开始发放券userid:{userid}.orderid:{orderid}");
                            int result = 0;
                            if (productcount >= 2 && productcount <= 3)
                            {
                                result = CreateOrYzPromotion(userid, 69, orderid);
                            }
                            if (productcount >= 4)
                            {
                                result = CreateOrYzPromotion(userid, 129, orderid);
                            }
                            if (result > 0)
                            {
                                string userTel = DalCoupon.GetUserPhoneByUserId(new Guid(userid));
                                BLL.Business.SendMarketingSms(userTel, "《至“臻”安心，纵享宁静——买马牌MC5轮胎送礼品券》");
                            }
                            Logger.Info($"发放券userid:{userid}.orderid:{orderid}.productcount:{productcount}.result:{result}");
                        }

                    }
                }
            }
            catch (System.Exception ex)
            {
                Logger.Error(ex);
            }
            Logger.Info("买马牌MC5轮胎送礼品券job结束");
        }

        public static int CreateOrYzPromotion(string userId, int money, string orderID)
        {
            try
            {
                var promotionCodeModel = new PromotionCodeModel();
                promotionCodeModel.OrderId = 0;
                promotionCodeModel.Status = 0;
                promotionCodeModel.Description = "至“臻”安心，纵享宁静——买马牌MC5轮胎送礼品券";
                promotionCodeModel.Discount = money;
                promotionCodeModel.MinMoney = money;
                promotionCodeModel.EndTime = DateTime.Now.AddMonths(1).Date;
                promotionCodeModel.StartTime = DateTime.Now.Date;
                promotionCodeModel.UserId = new Guid(userId);
                promotionCodeModel.CodeChannel = "买马牌MC5轮胎送礼品券";
                promotionCodeModel.BatchId = Convert.ToInt32(orderID);
                promotionCodeModel.RuleId = 262;
                promotionCodeModel.Number = 1;
                using (var db = DbHelper.CreateDbHelper())
                {
                    var result = DalCoupon.CreatePromotionCode(promotionCodeModel, db, "TireGiftPromotionJob",
                        "大客户", "马牌员工胎");
                    return result;
                }
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
                    cmd.Parameters.Add(new SqlParameter()
                    {
                        ParameterName = "@Results",
                        DbType = DbType.Int32,
                        Direction = ParameterDirection.Output,
                        Value = 0
                    });
                    using (var helper = DbHelper.CreateDbHelper(false))
                    {
                        helper.ExecuteNonQuery(cmd);
                        var result = Convert.ToInt32(cmd.Parameters["@Results"].Value);
                        return result;
                    }
                    //DbHelper.ExecuteNonQuery(cmd);
                    //var result = Convert.ToInt32(cmd.Parameters["@Results"].Value);
                    //return result;
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
