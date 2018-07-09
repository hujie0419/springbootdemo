using Common.Logging;
using Quartz;
using System;
using System.Diagnostics;
using Tuhu.C.Job.Tire.BLL;

namespace Tuhu.C.Job.Tire.Job
{
    public class TireVehicleCountJob:IJob
    {
        private readonly ILog logger = LogManager.GetLogger(typeof(TireVehicleCountJob));
        public void Execute(IJobExecutionContext context)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            logger.Info($"轮胎适配车型数量统计开始");
            try
            {
                TireVehicleCountManager.DoJob(logger);
            }
            catch(Exception ex)
            {
                logger.Error("轮胎适配车型数量统计异常", ex);
            }
            sw.Stop();
            logger.Info($"轮胎适配车型数量统计完成，用时{sw.ElapsedMilliseconds}毫秒");
        }
    }
}
