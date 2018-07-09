using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Tuhu.C.Job.Initialization.Dal;

namespace Tuhu.C.Job.Initialization.Job
{
    [DisallowConcurrentExecution]
    public class InitPinTuanProductJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<InitPinTuanProductJob>();
        public void Execute(IJobExecutionContext context)
        {
            var watcher = new Stopwatch();
            watcher.Start();
            Logger.Info("拼团产品批量同步Job开始执行");
            try
            {
                DoJob();
            }
            catch (Exception ex)
            {
                Logger.Warn("拼团产品批量同步出现异常", ex);
            }
            watcher.Stop();
            Logger.Info($"拼团产品批量同步完成,用时{watcher.ElapsedMilliseconds}毫秒");
        }
        public void DoJob()
        {
            var data = DalPinTuan.GetOriginalPinTuanProduct();
            var key = data.Select(g => g.ProductGroupId).Distinct().ToList();
            if (key.Any())
            {
                foreach(var item in key)
                {
                    var parametrs = data.Where(g => g.ProductGroupId == item).ToList();
                    if (parametrs.Any())
                    {
                        var addCount = DalPinTuan.AddPinTuanProductInfo(parametrs, item);
                        Logger.Info($"{item}产品组中添加{addCount}个产品");
                    }
                }
            }
        }
    }
}
