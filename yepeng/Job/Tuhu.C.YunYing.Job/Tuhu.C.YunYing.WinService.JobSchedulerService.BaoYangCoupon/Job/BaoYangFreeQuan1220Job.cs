﻿using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Common.Logging;
using Quartz;
using Tuhu.WebSite.Component.SystemFramework;
using System.Collections.Generic;
using Newtonsoft.Json;
using Tuhu.YunYing.WinService.JobSchedulerService.BaoYangCoupon.DAL;

namespace Tuhu.YunYing.WinService.JobSchedulerService.BaoYangCoupon.Job
{
    /// <summary>保养免单券 huhengxing 2016-12-19 </summary>
    [DisallowConcurrentExecution]
    public class BaoYangFreeQuan1220Job : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BaoYangFreeQuan1220Job));
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                if (DateTime.Now < new DateTime(2016, 12, 20))
                {
                    Logger.Info("2016-12-20开始发放保养免单券");
                    return;
                }
                Guid id = Guid.NewGuid();
                Logger.Info("启动任务" + id);
                var dt = DalCoupon.Get1220BaoYangFreeOrderList();

                Logger.Info(id + "查出订单数量为" + dt.Rows.Count);
                var dic = dt.Rows.OfType<DataRow>().Select(x => new { UserID = x["UserID"].ToString(), SumMoney = x["SumMoney"].ToString(), UserTel = x["UserTel"].ToString(), RuleID = x["RuleID"].ToString(), OrderID = x["OrderID"].ToString() }).ToList();
                Logger.Info(id + "执行订单数量为" + dic.Count);
                foreach (var i in dic)
                {
                    if (!string.IsNullOrWhiteSpace(i.SumMoney))
                    {

                        Logger.Info(id + $"塞券{i.OrderID}-订单总金额{i.SumMoney}");
                        var items = DalCoupon.SelectOrderListByOrderId(i.OrderID);
                        try
                        {

                            if (items != null && items.Any())
                            {
                                //获取优惠券金额 机油上限300，总额上限500
                                var m = DalCoupon.GetOrderPromotionMoney(items.ToList(),300,500);
                                int result = CreateOrYzPromotion(i.UserID, m, i.OrderID);
                                if (result > 0)
                                {
                                    BLL.Business.SendMarketingSms(i.UserTel, "保养免单券", "感谢您参与途虎养车保养买一送一活动，活动返券已发至您的账户，可下载手机APP查看http://dwz.cn/Beul5。");
                                }
                            }
                            else
                            {
                                Logger.Error("OrderID:" + i.OrderID + "查询订单列表数据出错,数据为空");
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error("OrderID:" + i.OrderID + "查询订单列表数据出错,数据:" + JsonConvert.SerializeObject(items), ex);
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

        public static int CreateOrYzPromotion(string userId, int money, string orderID)
        {
            try
            {
                var promotionCodeModel = new PromotionCodeModel();
                promotionCodeModel.OrderId = 0;
                promotionCodeModel.Status = 0;
                promotionCodeModel.Description = "12.19保养活动返券";
                promotionCodeModel.Discount = money;
                promotionCodeModel.MinMoney = money;
                promotionCodeModel.EndTime = DateTime.Now.AddDays(180).Date;
                promotionCodeModel.StartTime = DateTime.Now.Date;
                promotionCodeModel.UserId = new Guid(userId);
                promotionCodeModel.CodeChannel = "保养买一送一返券活动2";
                promotionCodeModel.BatchId = Convert.ToInt32(orderID);
                promotionCodeModel.RuleId = 23;
                promotionCodeModel.Number = 1;
                var result = DalCoupon.CreatePromotionCode(promotionCodeModel);
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return -99;
            }
        }
    }
}
