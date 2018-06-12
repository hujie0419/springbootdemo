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
    /// <summary>五周年店庆保养免单券 huhengxing 2016-10-8 </summary>
    [DisallowConcurrentExecution]
    public class FiveYearBaoYangFreeQuanJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(FiveYearBaoYangFreeQuanJob));

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                if (DateTime.Now < new DateTime(2016, 10, 10))
                {
                    Logger.Info("2016-10-10开始发放五周年店庆保养免单券");
                    return;
                }
                Guid id = Guid.NewGuid();
                Logger.Info("启动任务" + id);
                using (var cmd = new SqlCommand(@" 		SELECT	P.RuleID,
		                                                        O.UserID,
		                                                        O.UserTel,
		                                                        O.SumMoney,
		                                                        O.PKID AS OrderID
                                                        FROM	Gungnir..tbl_Order AS O WITH ( NOLOCK )
                                                        JOIN	Gungnir..tbl_PromotionCode AS P WITH ( NOLOCK )
		                                                        ON O.PKID = P.OrderId
                                                        WHERE	P.GetRuleID=904
		                                                        AND  O.Status = N'3Installed' AND  O.InstallDatetime < (GETDATE() - 1)
                                                                AND NOT EXISTS ( SELECT	1
						                                                            FROM	Gungnir..tbl_PromotionCode AS PC WITH ( NOLOCK )
						                                                            WHERE	PC.UserId = O.UserID
								                                                        AND PC.RuleID = 23
								                                                        AND PC.BatchID = O.PKID
								                                                        AND PC.CodeChannel = N'五周年店庆保养免单券活动'
                                                                                        AND (PC.Status <> 3 OR (PC.Status = 3 AND PC.UsedTime IS NOT NULL))
                                                                                         )
                "))
                {
                    cmd.CommandType = CommandType.Text;
                    var dt = Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(true, cmd);
                    Logger.Info(id + "查出订单数量为" + dt.Rows.Count);
                    var dic = dt.Rows.OfType<DataRow>().Select(x => new { UserID = x["UserID"].ToString(), SumMoney = x["SumMoney"].ToString(), UserTel = x["UserTel"].ToString(), RuleID = x["RuleID"].ToString(), OrderID = x["OrderID"].ToString() }).ToList();
                    Logger.Info(id + "执行订单数量为" + dic.Count);
                    foreach (var i in dic)
                    {
                        if (!string.IsNullOrWhiteSpace(i.SumMoney))
                        {
                            Logger.Info(id + $"塞券{i.OrderID}-订单总金额{i.SumMoney}");
                            var items = SelectOrderListByOrderId(i.OrderID);
                            var oil = items.Where(x => x.Pid.LastIndexOf("OL-") == 0).ToList();
                            var other = items.Where(x => x.Pid.LastIndexOf("OL-") != 0).ToList();
                            var oilMoney = oil.Select(x => x.TotalPrice).AsQueryable().Sum();
                            if (oilMoney > 300)
                                oilMoney = 300;

                            var otherMoney = other.Select(x => x.TotalPrice).AsQueryable().Sum();
                            var m = Convert.ToInt32((otherMoney + oilMoney).ToString("0"));
                            if (m > 500)
                                m = 500;

                            int result = CreateOrYzPromotion(i.UserID, m, i.OrderID);
                            if (result > 0)
                            {
                                string userTel = DalCoupon.GetUserPhoneByUserId(new Guid(i.UserID));
                                BLL.Business.SendMarketingSms(userTel, "途虎养车网五周年店庆保养5折", m, "保养现金券");
                            }
                        }
                    }
                }

                Logger.Info("结束任务" + id);
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
                                                FROM Gungnir..tbl_OrderList WITH ( NOLOCK ) WHERE OrderID=@OrderID AND Deleted = 0 AND  ( ProductType IS NULL OR ProductType & 32 <>32 )  "))
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
                var promotionCodeModel = new PromotionCodeModel();
                promotionCodeModel.OrderId = 0;
                promotionCodeModel.Status = 0;
                promotionCodeModel.Description = "途虎养车五周年店庆活动返券，限到店保养使用";
                promotionCodeModel.Discount = money;
                promotionCodeModel.MinMoney = money;
                promotionCodeModel.EndTime = DateTime.Now.AddDays(180).Date;
                promotionCodeModel.StartTime = DateTime.Now.Date;
                promotionCodeModel.UserId = new Guid(userId);
                promotionCodeModel.CodeChannel = "五周年店庆保养免单券活动";
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
