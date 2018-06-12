using System;
using Common.Logging;
using Quartz;
using Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.BLL;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.Job
{
    /// <summary> 路通 已过期，生产配置不存在</summary>
    [DisallowConcurrentExecution]
    public class NonBaoYangUserCouponJob : IJob
    {
        private static readonly ILog CouponLog = LogManager.GetLogger(typeof(NonBaoYangUserCouponJob));

        public void Execute(IJobExecutionContext context)
        {
            CouponLog.Info("非保养用户优惠券发放服务启动");
            try
            {
                var result = Business.InsertNonBaoYangPromotionData();

                CouponLog.Info(result ? "非保养用户优惠券发放成功" : "非保养用户优惠券发放失败");
            }
            catch (Exception ex)
            {
                CouponLog.Error(ex.Message, ex);
            }

            CouponLog.Info("非保养用户优惠券发放服务结束");

        }

    }
}
