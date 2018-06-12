using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Quartz;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using Tuhu.Service.Order;
using Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.DAL;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.Job
{
    [DisallowConcurrentExecution]
    public class BaoYangFreeQuan0901Job : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BaoYangFreeQuan0901Job));
        public void Execute(IJobExecutionContext context)
        {
            if (DateTime.Now < new DateTime(2017, 9, 1))
            {
                Logger.Info("2017-9-1开始发放指定机油到店保养券");
                return;
            }
            if (DateTime.Now > new DateTime(2017, 10, 16))
            {
                Logger.Info("2017-10-15结束发放指定机油到店保养券");
                return;
            }
            try
            {
                Logger.Info($"0901机油到店保养券job启动");
                Guid id = Guid.NewGuid();
                DataTable dt = DAL.DalCoupon.GetJiYouFanQuanOrders();
                if (dt != null && dt.Rows.Count > 0)
                {
                    var dic = dt.Rows.OfType<DataRow>()
                        .Select(x => new
                        {
                            UserID = x["UserID"].ToString(),
                            SumMoney = x["SumMoney"].ToString(),
                            UserTel = x["UserTel"].ToString(),
                            RuleID = x["RuleID"].ToString(),
                            OrderID = x["OrderID"].ToString()
                        }).Distinct().ToList();
                    Logger.Info($"获取到{dic.Count}个");
                    foreach (var i in dic)
                    {
                        using (var orderClient = new OrderQueryClient())
                        {
                            if (!string.IsNullOrWhiteSpace(i.SumMoney))
                            {
                                List<string> orderIds = new List<string> { i.OrderID };
                                //获取拆单数据
                                int orderid = Convert.ToInt32(i.OrderID);
                                var splitOrderIds =
                                    orderClient.GetRelatedSplitOrderIDsAsync(orderid,
                                        Service.Order.Enum.SplitQueryType.Full);
                                if (splitOrderIds != null && splitOrderIds.Result != null &&
                                    splitOrderIds.Result.Result != null && splitOrderIds.Result.Result.Any())
                                {
                                    //属于拆单订单
                                    var splitOrders =
                                        DalCoupon.SelectSplitOrderListByOrderIds(splitOrderIds.Result.Result);
                                    if (splitOrders != null && splitOrders.Any())
                                    {
                                        //一个订单已经安装了，另外一个还未安装------等两个订单都安装完成后返券
                                        if (splitOrders.Where(s => s.Status != "7Canceled" && s.Status != "3Installed")
                                                .FirstOrDefault() != null)
                                        {
                                            Logger.Info(id + $"订单{i.OrderID}还存在其他未完成安装的关联订单，暂不返券");
                                            continue;
                                        }
                                        else
                                        {
                                            orderIds.AddRange(splitOrders.Where(s => s.Status == "3Installed")
                                                .Select(s => s.PKID.ToString()));
                                        }
                                        //拆的单如果取消了怎么办-----那就只返没取消并且已安装的那个订单的金额
                                    }
                                }

                                Logger.Info(id + $"塞券{i.OrderID}-订单总金额{i.SumMoney}");
                                var items = DalCoupon.SelectOrderListByOrderIdForJiYouFanQuan(orderIds);
                                try
                                {
                                    if (items != null && items.Any())
                                    {
                                        Logger.Info(
                                            id +
                                            $"塞券关联订单{string.Join(",", orderIds)}-关联订单总金额{items.Sum(l => l.TotalPrice)}");
                                        int money = 0;
                                        var oilitems = items.Where(x => x.Pid.LastIndexOf("OL-") == 0).ToList();
                                        var oiltotalmoney = (int)oilitems.Sum(x => x.TotalPrice);
                                        money = oiltotalmoney >= 485 ? 485 : oiltotalmoney;
                                        int result = CreateOrYzPromotion(i.UserID, money, i.OrderID);
                                        if (result > 0)
                                        {
                                            string userTel = DalCoupon.GetUserPhoneByUserId(new Guid(i.UserID));
                                            BLL.Business.SendMarketingSms(userTel, "途虎8月机油买一送一");
                                        }
                                    }
                                    else
                                    {
                                        Logger.Error("OrderID:" + i.OrderID + "查询订单列表数据出错,数据为空");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(
                                        "OrderID:" + i.OrderID + "查询订单列表数据出错,数据:" + JsonConvert.SerializeObject(items),
                                        ex);
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Logger.Error(ex);
            }
            Logger.Info($"0901机油到店保养券job结束");
        }

        public static int CreateOrYzPromotion(string userId, int money, string orderID)
        {
            try
            {
                var promotionCodeModel = new PromotionCodeModel();
                promotionCodeModel.OrderId = 0;
                promotionCodeModel.Status = 0;
                promotionCodeModel.Description = "8月机油买一送一活动返券，仅限胜牌/海湾/出光品牌机油到店保养使用";
                promotionCodeModel.Discount = money;
                promotionCodeModel.MinMoney = money;
                promotionCodeModel.EndTime = new DateTime(2017, 12, 31).Date;
                promotionCodeModel.StartTime = new DateTime(2017, 9, 1).Date;
                promotionCodeModel.UserId = new Guid(userId);
                promotionCodeModel.CodeChannel = "指定机油到店保养券";
                promotionCodeModel.BatchId = Convert.ToInt32(orderID);
                promotionCodeModel.RuleId = 2006;
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
