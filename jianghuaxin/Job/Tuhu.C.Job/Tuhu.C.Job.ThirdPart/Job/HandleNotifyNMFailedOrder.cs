using Common.Logging;
using Quartz;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Tuhu.Service.Order.Models;
using Tuhu.C.Job.ThirdPart.Dal;
using Newtonsoft.Json;
namespace Tuhu.C.Job.ThirdPart.Job
{
    [DisallowConcurrentExecution]
    public class HandleNotifyNMFailedOrder : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<HandleNotifyNMFailedOrder>();
        public void Execute(IJobExecutionContext context)
        {
            var notComplatelogs = NuoMiDal.GetNotComplatelogs();
            using (var client = new Tuhu.Service.ThirdParty.NuoMiServiceClient())
            {
                try
                {
                    if (notComplatelogs != null && notComplatelogs.Any())
                    {
                        Logger.Info($"获得糯米未完成订单,订单号:{string.Join(",", notComplatelogs.Select(x => x.THOrderID))}");
                        var orders = GetTuHuOrders(notComplatelogs.Select(x => x.THOrderID));
                        foreach (var order in orders)
                        {
                            var log = notComplatelogs.FirstOrDefault(x => x.THOrderID == order.OrderId.ToString());
                            if (log != null)
                            {
                                var result = client.OrderPaidSyncStatus(order, log.OrderType);
                                result.ThrowIfException(true);
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
#if DEBUG
                    throw;
#else
                    Logger.Error($"糯米同步未完成订单出错:logs:{JsonConvert.SerializeObject(notComplatelogs)},ex:{ex}");
#endif
                }

                var failedLogs = NuoMiDal.GetNotifyFailedLogs();
                try
                {
                    if (failedLogs != null && failedLogs.Count() > 0)
                    {
                        Logger.Info($"获得糯米通知失败订单,订单号:{string.Join(",", failedLogs.Select(x => x.THOrderID))}");
                        var orders = GetTuHuOrders(failedLogs.Select(x => x.THOrderID));
                        foreach (var order in orders)
                        {
                            var log = failedLogs.FirstOrDefault(x => x.THOrderID == order.OrderId.ToString());
                            if (log != null)
                            {
                                var result = client.UpdateFailedOrderStatus(order, log.OrderType);
                                result.ThrowIfException(true);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    throw;
#else
                    Logger.Error($"糯米同步失败订单状态出错:logs:{JsonConvert.SerializeObject(notComplatelogs)},ex:{ex}");
#endif
                }
            }
        }

        private IEnumerable<OrderModel> GetTuHuOrders(IEnumerable<string> ids)
        {
            using (var orderclient = new Tuhu.Service.Order.OrderQueryClient())
            {
                foreach (var id in ids)
                {
                    var result = orderclient.FetchOrderFullInfoByID(Convert.ToInt32(id));
                    result.ThrowIfException(true);
                    yield return result.Result;
                }
            }
        }

    }
}
