using Common.Logging;
using Quartz;
using Tuhu.C.ActivityJob.Bll.ActivityPage;

namespace Tuhu.C.ActivityJob.Job.ActivityPage
{
    /// <summary>
    /// 描述：根据区域自动通过活动申请
    /// 创建人：wpf
    /// 创建时间：2019-11-28
    /// </summary>
    public class AutoPassUserActivityApplyJob : IJob
    {
        protected static readonly ILog Logger = LogManager.GetLogger(typeof(AutoPassUserActivityApplyJob));


        public void Execute(IJobExecutionContext context)
        {
            Logger.Info($"AutoPassUserActivityApplyJOB开始执行");

            AutoPassUserActivityApplyBll.AutoPassUserActivityApplyJOB();

            Logger.Info($"AutoPassUserActivityApplyJOB 执行结束执行");
        }
    }
}
