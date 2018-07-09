using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Tuhu.Service.Order;
using Tuhu.Service.Order.Request;
using Tuhu.C.Job.Activity.Dal;
using Tuhu.C.Job.Activity.Models;

namespace Tuhu.C.Job.Activity.Job
{
    [DisallowConcurrentExecution]
    public class GroupBuyingCanceledJob : IJob
    {
        // 刷单批量订单取消（已废弃）
        private static readonly ILog Logger = LogManager.GetLogger<GroupBuyingCanceledJob>();
        public void Execute(IJobExecutionContext context)
        {
            var watcher = new Stopwatch();
            watcher.Start();
            Logger.Info("拼团订单取消Job开始执行");
            try
            {
                DoJob();
            }
            catch (Exception ex)
            {
                Logger.Warn("拼团订单取消Job出现异常", ex);
            }
            watcher.Stop();
            Logger.Info($"拼团订单取消Job完成,用时{watcher.ElapsedMilliseconds}毫秒");
        }
        private void DoJob()
        {
            var data = DalGroupBuying.GetTerribleOrderList();
            if (data.Count < 1)
            {
                Logger.Warn("GroupBuyingCanceledJob==>待取消订单为O");
                return;
            }
            CanceledOrder(data);
        }

        private void CanceledOrder(List<UserOrderJobModel> dataList)
        {
            using (var client = new OrderOperationClient())
            {
                foreach (var item in dataList)
                {
                    var request = new CancelOrderRequest
                    {
                        OrderId = item.OrderId,
                        UserID = item.UserId,
                        Remark = "拼团刷单订单取消",
                        FirstMenu = "拼团刷单订单取消",
                        SecondMenu = "拼团刷单订单取消"
                    };

                    var result = client.CancelOrderForApp(request);
                    if (!(result.Success && result.Result.IsSuccess))
                    {
                        if (result.Exception != null)
                        {
                            Logger.Warn($"订单取消失败，{item.UserId}/{item.OrderId}", result.Exception);
                        }
                        else if (result.Result != null && result.Result.RespCode != 1)
                        {
                            Logger.Warn($"订单取消失败,{result.Result.Message}");
                        }
                    }
                }
            }
        }
    }
}
