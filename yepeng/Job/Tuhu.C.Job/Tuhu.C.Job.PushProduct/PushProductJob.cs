using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Quartz;
using System.Diagnostics;
using Tuhu.C.Job.PushProduct.BLL;

namespace Tuhu.C.Job.PushProduct
{
    [DisallowConcurrentExecution]
    public class PushProductJob: IJob
    {
        public static readonly ILog Logger = LogManager.GetLogger(typeof(PushProductJob));

        public void Execute(IJobExecutionContext context)
        {
            Logger.Info("PushProductJob启动");
            try
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                ProductBusiness.GenerateProductFile();
                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;
                if (true)
                    Logger.Info($"PushProductJob执行成功");
                else
                    Logger.Info($"PushProductJob执行失败");
                Logger.Info(string.Format("耗时：{0}s", ts.TotalSeconds));
            }
            catch (Exception ex)
            {
                Logger.Error($"PushProductJob异常：{ex.ToString()}");
            }
        }
    }
}
