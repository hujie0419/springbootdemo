using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.ActivityJob.Dal.Rebate;
using Tuhu.C.ActivityJob.ServiceProxy;
using Tuhu.Service.Order.Enum;

namespace Tuhu.C.ActivityJob.Job.Rebate
{
    /// <summary>
    /// 描述：朋友圈返现 - 16元返现车品订单历史数据关联安装门店JOB
    /// 创建人：姜华鑫
    /// 创建时间：2019-05-16
    /// </summary>
    public class RebateRepairHistoricalDataJob : IJob
    {
        protected static readonly ILog Logger = LogManager.GetLogger(typeof(RebateRepairHistoricalDataJob));

        public void Execute(IJobExecutionContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            Logger.Info("朋友圈返现 - 16元返现车品订单历史数据关联安装门店JOB 开始执行");
            Run();
            stopwatch.Stop();
            Logger.Info($"朋友圈返现 - 16元返现车品订单历史数据关联安装门店JOB 结束执行,用时{stopwatch.Elapsed}");
        }

        private void Run()
        {
            try
            {
                var vehicleProduct = RebateDal.GetRebateApplyConfigList();
                int dataCount = vehicleProduct.Count();
                int updateCount = 0;//实际更新数据条数

                Logger.Info($"朋友圈返现 - 16元返现车品订单历史数据关联安装门店JOB，需要关联安装门店的16元返现数据有{dataCount}条");

                //关联安装门店
                Parallel.ForEach(vehicleProduct, new ParallelOptions() { MaxDegreeOfParallelism = 4 }, (item) =>
                 {
                     var order = OrderServiceProxy.FetchOrderInfoByID(item.OrderId);
                     if (order == null || order.PKID <= 0)
                     {
                         Logger.Error($"RebateRepairHistoricalDataJob，订单号：{item.OrderId}不存在");
                     }
                     else
                     {
                         var orderType = OrderServiceProxy.CheckOrderProductTypeByOrderId(item.OrderId);
                         int installShopId = 0;
                         if (orderType.Contains("车品订单"))
                         {
                             var relatedOrderIds = OrderServiceProxy.GetRelatedSplitOrderIDs(item.OrderId, SplitQueryType.Full);
                             relatedOrderIds.RemoveAll(q => q == item.OrderId);
                             foreach (var orderItem in relatedOrderIds)
                             {
                                 var relateOrderType = OrderServiceProxy.CheckOrderProductTypeByOrderId(orderItem);
                                 if (relateOrderType.Contains("服务订单"))
                                 {
                                     installShopId = OrderServiceProxy.FetchOrderInfoByID(orderItem)?.InstallShopId ?? 0;
                                 }
                                 if (installShopId != 0)
                                 {
                                     break;
                                 }
                             }
                         }
                         else
                         {
                             installShopId= order.InstallShopId ?? 0;
                         }
                         item.InstallShopId = installShopId;
                     }
                 });

                //批量更新安装门店
                updateCount = RebateDal.BatchUpdateInstallShopId(vehicleProduct.Split(200));

                Logger.Info($"朋友圈返现 - 16元返现车品订单历史数据关联安装门店JOB，实际关联安装门店的16元返现数据有{updateCount}条");
            }
            catch(Exception ex)
            {
                Logger.Error($"RebateRepairHistoricalDataJob -> Run -> error ,异常消息：{ex.Message}，堆栈信息：{ex.StackTrace}");
            }
        }
    }
}
