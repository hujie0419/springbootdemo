using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Common.Logging;
using Quartz;
using Tuhu.C.Job.Activity.Dal;
using Tuhu.Service.PinTuan;

namespace Tuhu.C.Job.Activity.Job
{
    [DisallowConcurrentExecution]
    public class UpdateAllGroupBuyingESJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<UpdateAllGroupBuyingESJob>();
        public void Execute(IJobExecutionContext context)
        {
            var watcher = new Stopwatch();
            watcher.Start();
            Logger.Info("全量刷新ES中拼团数据Job开始执行");
            try
            {
                DoJob();
            }
            catch (Exception ex)
            {
                Logger.Warn("全量刷新ES中拼团数据Job出现异常", ex);
            }

            watcher.Stop();
            Logger.Info($"全量刷新ES中拼团数据Job完成,用时{watcher.ElapsedMilliseconds}毫秒");
        }

        private void DoJob()
        {
            const int step = 10;
            var start = 0;
            var count = DalGroupBuying.GetGroupBuyingCount();
            Logger.Warn($"待刷新拼团数量为{count}个");
            while (start <= count)
            {
                var productGroupIds = DalGroupBuying.GetProductGroupList(start, step);
                if (productGroupIds.Any())
                {
                    using (var client = new PinTuanClient())
                    {
                        var result = client.UpdateGroupBuyingInfo(productGroupIds);
                        if (!(result.Success && result.Result))
                        {
                            Logger.Warn($"更新ES数据失败-->{result.Exception?.Message}");
                        }
                    }
                }

                start += step;
            }
        }
    }
}
