using Common.Logging;
using Quartz;
using System;
using Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.BLL;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.Job
{
    /// <summary> 路通 已过期，生产配置不存在</summary>
    [DisallowConcurrentExecution]
    public class BaoYangUserCouponJob : IJob
    {
        private static readonly ILog CouponLog = LogManager.GetLogger(typeof(BaoYangUserCouponJob));
        public void Execute(IJobExecutionContext context)
        {
            CouponLog.Info("保养用户优惠券发放服务开始");

            try
            {
                var result = Business.InsertBaoYangPromotionData();

                CouponLog.Info(result ? "保养用户优惠券发放成功" : "保养用户优惠券发放失败");
            }
            catch (Exception ex)
            {
                CouponLog.Error(ex.Message, ex);
            }

            CouponLog.Info("保养用户优惠券发放服务结束");

        }
    }
}
