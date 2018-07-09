using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.Job.DailySitemap.BLL;

namespace Tuhu.C.Job.DailySitemap
{
    public class DailySitemapJob:IJob
    {
        public static readonly ILog Logger = LogManager.GetLogger(typeof(DailySitemapJob));
        public static bool isFull = Boolean.Parse(ConfigurationManager.AppSettings["isFull"]);

        public void Execute(IJobExecutionContext context)
        {
            Logger.Info("DailySitemapJob启动");
            try
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                bool result = URLBusiness.InsertDailyURLToDB(isFull);
                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;
                if (result)
                    Logger.Info($"DailySitemapJob执行成功");
                else
                    Logger.Info($"DailySitemapJob执行失败");
                Logger.Info(string.Format("耗时：{0}s", ts.TotalSeconds));
            }
            catch (Exception ex)
            {
                Logger.Error($"DailySitemapJob异常：{ex.ToString()}");
            }
        }
    }
}
