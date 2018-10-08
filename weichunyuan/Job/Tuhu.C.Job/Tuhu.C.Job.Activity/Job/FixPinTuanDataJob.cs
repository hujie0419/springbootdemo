using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Quartz;
using Tuhu.C.Job.Activity.Dal;
using Tuhu.Service.Order;
using Tuhu.Service.Order.Enum;
using Tuhu.Service.Order.Request;

namespace Tuhu.C.Job.Activity.Job
{
    [DisallowConcurrentExecution]
    public class FixPinTuanDataJob:IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<FixPinTuanDataJob>();
        public void Execute(IJobExecutionContext context)
        {
            var watcher = new Stopwatch();
            Logger.Info("拼团异常数据补偿Job开始执行");
            try
            {
                DoJob();
            }
            catch (Exception ex)
            {
                Logger.Warn("拼团异常数据补偿Job执行出现异常", ex);
            }
            watcher.Stop();
            Logger.Info($"拼团异常数据补偿Job执行完成,用时{watcher.ElapsedMilliseconds}毫秒");
        }

        private void DoJob()
        {
            var orderList = DalGroupBuying.GetOrderList();

            using (var client = new OrderOperationClient())
            {
                foreach (var item in orderList)
                {
                    var setResult = client.ExecuteOrderProcess(new ExecuteOrderProcessRequest
                    {
                        OrderId = item.Item1,
                        CreateBy = item.Item2.ToString("D"),
                        OrderProcessEnum = OrderProcessEnum.PinTuanSuccess
                    });
                    if (!setResult.Success)
                    {
                        Logger.Warn($"{item.Item1}出现异常", setResult.Exception);
                    }
                }
            }

            //var groupInfo = DalGroupBuying.GetCouponList();
            //foreach(var item in groupInfo)
            //{
            //    TuhuNotification.SendNotification("notification.GroupBuyingCreateCouponQueue",
            //        new Dictionary<string, object>
            //        {
            //            ["GroupId"] = item.Item1,
            //            ["ProductGroupId"] = item.Item2
            //        }, 10000);
            //}
        }
    }
}
