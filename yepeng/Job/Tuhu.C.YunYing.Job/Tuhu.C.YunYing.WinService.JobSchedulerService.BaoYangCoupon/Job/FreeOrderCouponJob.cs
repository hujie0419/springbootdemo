using System;
using Common.Logging;
using Quartz;
using Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.BLL;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.Job
{
    /// <summary>刘超 已过期，生产配置不存在</summary>
    [DisallowConcurrentExecution]
    public class FreeOrderCouponJob : IJob
    {
        private static readonly ILog CouponLog = LogManager.GetLogger(typeof(FreeOrderCouponJob));
        public void Execute(IJobExecutionContext context)
        {
            if (DateTime.Now > new DateTime(2016, 5, 6))
            {
                return;
            }

            CouponLog.Info("免单优惠券发放服务开始");

            try
            {
                Business.InsertFreeOrderCoupon();
            }
            catch (Exception ex)
            {
                CouponLog.Error(ex.Message, ex);
            }

            CouponLog.Info("免单优惠券发放服务结束");
        }
    }
}
