using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Common.Logging;
using Newtonsoft.Json;
using Quartz;
using Tuhu.C.Job.DAL;
using Tuhu.C.Job.Models;
using Tuhu.Service.Activity;
using Tuhu.Service.Activity.Models;

namespace Tuhu.C.Job.Job
{
    /// <summary>
    /// 统计大客户员工异常订单信息 并更新到数据库表[Activity].[dbo].[LargeCustomerInvalidOrder]
    /// </summary>
    public class LargeCustomerStaffInvalidOrderMonitorJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<LargeCustomerStaffInvalidOrderMonitorJob>();

        public void Execute(IJobExecutionContext context)
        {
            Logger.Info($"LargeCustomerStaffInvalidOrderMonitorJob开始执行");
            Stopwatch s = new Stopwatch();
            s.Start();

            LargeCustomerStaffInvalidOrder();

            Logger.Info($"LargeCustomerStaffInvalidOrderMonitorJob执行结束,耗时:{s.ElapsedMilliseconds}");
        }

        private void LargeCustomerStaffInvalidOrder()
        {
            var validList = new List<ActivityCustomerInvalidOrderResponse>();
            try
            {
                using (var client = new ActivityClient())
                {
                    //获取所有的异常订单信息
                    var clentResult = client.GetExceptionalCustomerOrderInfo();
                    if (clentResult.Success)
                    {
                        validList = clentResult.Result;
                    }
                    else
                    {
                        Logger.Warn($"LargeCustomerStaffInvalidOrder,获取所有的异常订单信息失败,ErrorMessage:{clentResult.ErrorMessage}");
                    }

                    if (!(bool)validList?.Any())
                    {
                        return;
                    }
                }

                //遍历异常信息
                foreach (var item in validList)
                {
                    try
                    {
                        //从数据库获取该数据，仅仅订单信息不相同就更新数据，没有userid、活动id、异常原因相同的数据就插入,
                        //都相同的数据不处理
                        var oldModel = LargeCustomerStaffInvalidOrderMonitorDal.GetLargeCustomerStaffInvalidOrder(item.UserId, item.ActivityId, item.InvalidType);
                        if (string.IsNullOrWhiteSpace(oldModel?.ActivityId))
                        {
                            //插入
                            var insertModel = new LargeCustomerInvalidOrderModel()
                            {
                                UserId = item.UserId,
                                ActivityId = item.ActivityId,
                                InvalidType = item.InvalidType,
                                OrderIDs = string.Join(",", item.OrderIDs),
                                DetailInfo = item.DetailInfo,
                                EmailSendCount = 0,
                                IsCouponDeleted = item.IsCouponDeleted,
                                Phone = item.Phone
                            };
                            var insertResult = LargeCustomerStaffInvalidOrderMonitorDal.InsertLargeCustomerStaffInvalidOrder(insertModel);
                            if (insertResult != 1)
                            {
                                var jsonModel = JsonConvert.SerializeObject(insertResult);
                                Logger.Warn($"LargeCustomerStaffInvalidOrder，插入数据失败,model:{jsonModel}");
                            }
                        }
                        else
                        {
                            //获取新的订单,有新的订单就更新
                            var oldOrders = oldModel.OrderIDs.Split(',');
                            var oldOrderList = new List<int>();
                            foreach (var id in oldOrders)
                            {
                                oldOrderList.Add(Convert.ToInt32(id));
                            }
                            var newOrders = oldOrderList.Except(item.OrderIDs);

                            if ((bool)newOrders?.Any())
                            {
                                //更新
                                var updateModel = new LargeCustomerInvalidOrderModel()
                                {
                                    UserId = item.UserId,
                                    ActivityId = item.ActivityId,
                                    InvalidType = item.InvalidType,
                                    OrderIDs = string.Join(",", item.OrderIDs),
                                    DetailInfo = item.DetailInfo,
                                    EmailSendCount = 0,
                                    IsCouponDeleted = item.IsCouponDeleted,
                                    Phone = item.Phone
                                };
                                var updateResult = LargeCustomerStaffInvalidOrderMonitorDal.UpdateLargeCustomerStaffInvalidOrder(updateModel);
                                if (updateResult != 1)
                                {
                                    var jsonModel = JsonConvert.SerializeObject(updateModel);
                                    Logger.Warn($"LargeCustomerStaffInvalidOrder，更新数据失败,model:{jsonModel}");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        var jsonModel = JsonConvert.SerializeObject(item);
                        Logger.Error($"遍历异常订单数据时异常,userID:{item.UserId},数据json:{jsonModel},ex{ex}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"LargeCustomerStaffInvalidOrder统计大客户员工异常订单信息异常,ex:{ex}");
            }
        }
    }
}
