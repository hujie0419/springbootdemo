using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.YunYing.WinService.JobSchedulerService.BLL;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.Job
{
    [DisallowConcurrentExecution]
    public class CreateVipPromotion2BOrderJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CreateVipPromotion2BOrderJob));

        public void Execute(IJobExecutionContext context)
        {
            Logger.Info("CreateVipPromotion2BOrderJob Start");

            var resList = VipPromotionOrderBusiness.GetSplitOrders();
            foreach (var res in resList)
            {
                VipPromotionOrderBusiness.ProcessBatchOrders(res?.Item1, res?.Item2?.SplitOrders);
            }
            Logger.Info("CreateVipPromotion2BOrderJob End");
        }

    }
}
