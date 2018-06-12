using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.DAL;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.Job
{
    /// <summary>618福利保养免单券 huhengxing 2016-6-13 已过期</summary>
    [DisallowConcurrentExecution]
    public class BaoYangFreeQuanJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BaoYangFreeQuanJob));

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                if (DateTime.Now < new DateTime(2016, 6, 14))
                {
                    Logger.Info("2016-6-14开始发放618福利日保养免单券");
                    return;
                }
                if (DateTime.Now > new DateTime(2016, 8, 3))
                {
                    Logger.Info("2016-8-3后结束发放618福利日保养免单券");
                    return;
                }
                Logger.Info("启动任务");
                using (var cmd = new SqlCommand(@"[Gungnir].[dbo].[ProMotionCode_BaoYangFreeQuan]"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    var dt = Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(true, cmd);
                    var dic = dt.Rows.OfType<DataRow>().Select(x => new { UserID = x["UserID"].ToString(), SumMoney = x["SumMoney"].ToString(), UserTel = x["UserTel"].ToString(), RuleID = x["RuleID"].ToString(), OrderID = x["OrderID"].ToString() }).ToList();
                    foreach (var i in dic)
                    {
                        if (!string.IsNullOrWhiteSpace(i.SumMoney))
                        {
                            var items = SelectOrderListByOrderId(i.OrderID);
                            var oil = items.Where(x => x.Pid.LastIndexOf("OL-") == 0).ToList();
                            var other = items.Where(x => x.Pid.LastIndexOf("OL-") != 0).ToList();
                            var oilMoney = oil.Select(x => x.TotalPrice).AsQueryable().Sum();
                            if (oilMoney > 300)
                                oilMoney = 300;

                            var otherMoney = other.Select(x => x.TotalPrice).AsQueryable().Sum();
                            var m = Convert.ToInt32((otherMoney + oilMoney).ToString("0"));
                            if (m > 800)
                                m = 800;

                            int result = CreateOrYzPromotion(i.UserID, m, i.OrderID);
                            if (result > 0)
                            {
                                string userTel = DalCoupon.GetUserPhoneByUserId(new Guid(i.UserID));
                                BLL.Business.SendMarketingSms(userTel, "途虎保养免单券", m, "到店保养券");
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

        public static IEnumerable<OrderItem> SelectOrderListByOrderId(string orderId)
        {
            using (var cmd = new SqlCommand(@"SELECT
                                                PKID,OrderID,OrderNo,PID,Poid,Category,Name,
                                                Size,Remark,CarCode,CarName,Num,MarkedPrice,Discount,Price,TotalDiscount,
                                                TotalPrice,LastUpdateTime,PurchasePrice,Cost,InstallFee,Vendor,IsPaid,PaidVia,
                                                InstockDate,PaidDate,PurchaseStatus,IsInstallFeePaid,InstallFeePaidDate,Deleted,
                                                CreateDate,Commission,HCNum,OrigProdId,ProductType,ParentID,WeekYear,RefID,
                                                FUPID,PromotionCode,PromotionMoney,MatchedProducts,ExtCol,IsDaiFa,NodeNo,
                                                TotalManualDiscount,ListPrice,PayPrice,TuhuCost
                                                FROM Gungnir..tbl_OrderList WITH ( NOLOCK ) WHERE OrderID=@OrderID AND Deleted = 0 "))
            {
                cmd.Parameters.AddWithValue("@OrderID", orderId);
                var dt = Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(true, cmd);
                if (dt != null && dt.Rows.Count > 0)
                {
                    return dt.Rows.OfType<DataRow>().Select(x => new OrderItem(x)).ToList();
                }
                return null;
            }
        }

        public static int CreateOrYzPromotion(string userId, int money, string orderID)
        {
            try
            {
                if (money > 800)
                    money = 800;

                var promotionCodeModel = new PromotionCodeModel();
                promotionCodeModel.OrderId = 0;
                promotionCodeModel.Status = 0;
                promotionCodeModel.Description = "618保养免单返券";
                promotionCodeModel.Discount = money;
                promotionCodeModel.MinMoney = (money + 1);
                promotionCodeModel.EndTime = DateTime.Now.AddDays(180).Date;
                promotionCodeModel.StartTime = DateTime.Now.Date;
                promotionCodeModel.UserId = new Guid(userId);
                promotionCodeModel.CodeChannel = "618福利日保养免单券活动";
                promotionCodeModel.BatchId = Convert.ToInt32(orderID);
                promotionCodeModel.RuleId = 23;
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
