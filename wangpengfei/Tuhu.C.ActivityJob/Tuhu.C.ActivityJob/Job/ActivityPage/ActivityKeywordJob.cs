using Common.Logging;
using Quartz;
using Tuhu.C.ActivityJob.Bll.ActivityPage;

namespace Tuhu.C.ActivityJob.Job.ActivityPage
{
    /// <summary>
    /// 描述：活动页关键字搜索刷新job
    /// 创建人：花杨永
    /// 创建时间：2019-04-28
    /// </summary>
    public class ActivityKeywordJob : IJob
    {
        protected static readonly ILog Logger = LogManager.GetLogger(typeof(ActivityKeywordJob));


        public void Execute(IJobExecutionContext context)
        {
            Logger.Info($"ActivityKeywordJob开始执行");

            ActiviyPageBll.ActivityKeywordBll();

            Logger.Info($"ActivityKeywordJob 执行结束执行");
        }
    }
}
