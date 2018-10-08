using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.Job.Activity.Dal;

namespace Tuhu.C.Job.Activity.Job
{
    /// <summary>
    ///     拼团 - 优惠券补偿JOB
    /// </summary>
    [DisallowConcurrentExecution]
    public class PinTuanCouponCompensateJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<PinTuanCouponCompensateJob>();
        public void Execute(IJobExecutionContext context)
        {
            var watcher = new Stopwatch();
            watcher.Start();
            Logger.Info("优惠券拼团补偿Job开始执行");
            try
            {
                DoJob();
            }
            catch (Exception ex)
            {
                Logger.Warn("优惠券拼团补偿Job出现异常", ex);
            }
            watcher.Stop();
            Logger.Info($"优惠券拼团补偿Job完成,用时{watcher.ElapsedMilliseconds}毫秒");
        }

        private void DoJob()
        {
            var data = DalGroupBuying.GetGroupInfo();
            foreach(var item in data)
            {
                TuhuNotification.SendNotification("notification.GroupBuyingCreateCouponQueue", new Dictionary<string, object>
                {
                    ["GroupId"] = item.Item1,
                    ["ProductGroupId"] = item.Item2
                });
            }
        }
    }
}
