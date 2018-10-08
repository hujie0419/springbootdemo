using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Common.Logging;
using Quartz;
using Tuhu.C.Job.Activity.Dal;
using Tuhu.Service.Order;
using Tuhu.Service.Order.Request;
using Tuhu.Service.PinTuan;

namespace Tuhu.C.Job.Activity.Job
{
    /// <summary>
    ///     拼团 - 拼团未付款自动取消JOB
    /// </summary>
    [DisallowConcurrentExecution]
    public class PinTuanOrderCancelJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<PinTuanOrderCancelJob>();

        public void Execute(IJobExecutionContext context)
        {
            var watcher = new Stopwatch();
            watcher.Start();
            Logger.Info("未付款拼团订单定期取消Job开始执行");
            try
            {
                DoJob();
            }
            catch (Exception ex)
            {
                Logger.Warn("未付款拼团订单定期取消Job出现异常", ex);
            }

            watcher.Stop();
            Logger.Info($"未付款拼团订单定期取消Job完成,用时{watcher.ElapsedMilliseconds}毫秒");
        }

        private void DoJob()
        {
            // 获取超时的订单
            var data = DalGroupBuying.GetExpiredUserList();
            if (data.Any())
            {
                using (var client = new OrderApiForCClient())
                using (var clientPinTuna = new PinTuanClient())
                {
                    foreach (var item in data)
                    {
                        var OrderData = client.FetchOrderByOrderId(item.OrderId);
                        var result = OrderData.Success && OrderData.Result?.Status == "7Canceled";
                        if (!result && OrderData.Result != null)
                        {
                            //判断状态能不能取消
                            if (new[] {"0NewPingTuan", "0New"}.Contains(OrderData.Result.Status))
                            {
                                result = CancelPinTuanOrder(item.UserId, item.OrderId);
                            }
                            else
                            {
                                result = true;
                                //调用补足服务，订单已取消 ，拼团状态未改变
                                clientPinTuna.RepairPinTuanOrderStatus(new List<int> {item.OrderId});
                            }

                            Logger.Info($"当前订单：{item.OrderId} {OrderData.Result.Status} ");
                        }

                        //if (result)
                        //{
                        //    var ChangeResult = DalGroupBuying.ChangeUserOrderStatus(item.OrderId);
                        //    if (ChangeResult == 0)
                        //    {
                        //        Logger.Warn($"修改拼团用户状态为订单取消失败/{item.OrderId}");
                        //    }
                        //}
                        if (!result)
                        {
                            Logger.Warn($"取消未付款订单{item.OrderId}失败");
                        }
                    }
                }
            }
        }

        private bool CancelPinTuanOrder(Guid UserId, int OrderId)
        {
            Logger.Info($"拼团失败，取消以下订单,{UserId:D}/{OrderId}");
            using (var client = new OrderOperationClient())
            {
                var request = new CancelOrderRequest
                {
                    OrderId = OrderId,
                    UserID = UserId,
                    Remark = "用户参与拼团，下单24小时内未付款,订单取消"
                };

                var result = client.CancelOrderForApp(request);
                if (!(result.Success && result.Result.IsSuccess))
                {
                    if (result.Exception != null)
                    {
                        Logger.Warn($"订单取消失败，/{result.Exception.Message}");
                    }
                    else if (result.Result != null && result.Result.RespCode != 1)
                    {
                        Logger.Warn($"订单取消失败,{result.Result.Message}");
                    }
                    else
                    {
                        Logger.Warn($"订单取消失败,{UserId}/{OrderId}");
                    }

                    return false;
                }

                return true;
            }
        }
    }
}
