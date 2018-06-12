using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.Job.Sitemap.BLL;

namespace Tuhu.C.Job.Sitemap
{
    [DisallowConcurrentExecution]
    public class WeeklySitemapJob:IJob
    {
        public static readonly ILog Logger = LogManager.GetLogger(typeof(WeeklySitemapJob));
        public static bool isFull = Boolean.Parse(ConfigurationManager.AppSettings["isFull"]);

        public void Execute(IJobExecutionContext context)
        {
            Logger.Info("WeeklySitemapJob启动");
            try
            {
                if (!Boolean.Parse(ConfigurationManager.AppSettings["SitemapJobIsClose"]))
                {
                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();

                    bool result = URLBusiness.GenerateTireUrlFiles();
                    /*
                      bool result=URLBusiness.CreateFiles();
                    bool result1 = URLBusiness.InsertWeeklyURLToDB(isFull);
                    bool result2= URLBusiness.GenerateSitemapFile(isFull);
                    */
                    stopWatch.Stop();
                    TimeSpan ts = stopWatch.Elapsed;
                    if (result)
                        Logger.Info($"WeeklySitemapJob执行成功");
                    else
                        Logger.Info($"WeeklySitemapJob执行失败");
                    Logger.Info(string.Format("耗时：{0}s", ts.TotalSeconds));
                }
                else
                {
                    Logger.Info($"WeeklySitemapJob已关闭");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"WeeklySitemapJob异常：{ex.ToString()}");
            }
        }
    }
}
