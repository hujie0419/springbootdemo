using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Tuhu.C.Job.Activity.Dal;
using Tuhu.Service.Activity;
using Tuhu.Service.PinTuan;

namespace Tuhu.C.Job.Activity.Job
{
    [DisallowConcurrentExecution]
    public class UpdateAppHomeModuleInfoJob : IJob
    {
        private static ILog Logger = LogManager.GetLogger<UpdateAppHomeModuleInfoJob>();
        public void Execute(IJobExecutionContext context)
        {
            var watcher = new Stopwatch();
            watcher.Start();
            Logger.Info($"APP首页拼团/砍价模块数据刷新Job");
            try
            {
                DoJob();
            }
            catch (Exception ex)
            {
                Logger.Error($"APP首页拼团/砍价模块数据刷新Job异常：{ex}");
            }
            watcher.Stop();

            Logger.Info($"APP首页拼团/砍价模块数据刷新Job完成-->{watcher.ElapsedMilliseconds}毫秒");
        }
        public void DoJob()
        {
            using (var client = new PinTuanClient())
            {
                var result = client.RefreshCache();
                if (!(result.Success && result.Result.Code == 1))
                {
                    Logger.Warn("刷新拼团首页模块数据失败");
                }
                var data = DalGroupBuying.GetAllPinTuanProduct();
                foreach (var item in data)
                {
                    var resultItem = client.RefreshCache(item);
                    if (!(result.Success && result.Result.Code == 1))
                    {
                        Logger.Warn($"刷新拼团数据-->{item}--失败");
                    }
                }
            }
            using (var client = new ShareBargainClient())
            {
                var result = client.RefreshShareBargainCache();
                if (!(result.Success && result.Result))
                {
                    Logger.Warn("砍价首页模块数据失败");
                }
            }
        }
    }
}
