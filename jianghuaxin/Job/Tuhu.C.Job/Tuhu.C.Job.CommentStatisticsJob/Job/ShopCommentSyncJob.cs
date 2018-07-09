using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Tuhu.C.Job.CommentStatisticsJob.Dal;

namespace Tuhu.C.Job.CommentStatisticsJob.Job
{
    /// <summary>
    /// MQ中的门店评论信息刷到ES（MQ出问题时执行）
    /// </summary>
    [DisallowConcurrentExecution]
    public class ShopCommentSyncJob : IJob
    {
        private static ILog Logger = LogManager.GetLogger<ShopCommentSyncJob>();
        public void Execute(IJobExecutionContext context)
        {
            Logger.Info("门店评论审核通过,数据刷新Job");
            try
            {
                var dataMap = context.JobDetail.JobDataMap;
                var isDay = dataMap.GetBoolean("IsDay");
                var startTime = dataMap.GetString("StartTime");
                var endTime = dataMap.GetString("EndTime");
                DoJob(isDay, startTime, endTime);
            }
            catch (Exception ex)
            {
                Logger.Error($"门店评论审核通过,数据刷新Job运行异常：{ex}");
            }
            Logger.Info("门店评论审核通过,数据刷新Job完成");
        }

        private static void DoJob(bool isDay, string startTime, string endTime)
        {
            var watcher = new Stopwatch();
            watcher.Start();

            if ((string.IsNullOrEmpty(startTime) || string.IsNullOrEmpty(endTime)) && isDay)
            {
                Logger.Warn("配置时间读取失败");
                return;
            }

            var start = 21000;
            if (isDay)
            {
                startTime = (DateTime.Now - TimeSpan.FromDays(3)).ToLocalTime().ToString("yyyy-MM-dd");
                endTime = DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd");
                start = 0;
            }

            var step = 1000;
            var commentCount = DalShopCommentSync.GetShopCommentCount(startTime, endTime);

            if (commentCount <= 0)
            {
                Logger.Warn("门店评论审核数据同步,待同步数据==>0条");
                return;
            }

            Logger.Info($"门店评论审核数据同步,待同步数据==>{commentCount}条,共{(commentCount / step) + 1}批");
            var sw = new Stopwatch();
            while (start < commentCount)
            {
                try
                {
                    sw.Start();
                    var commentIds = DalShopCommentSync.GetShopCommentId(startTime, endTime, start, step);
                    start += step;
                    foreach (var commentId in commentIds)
                    {
                        TuhuNotification.SendNotification("ShopCommentApproved", new Dictionary<string, object>
                        {
                            ["type"] = "shop",
                            ["id"] = commentId
                        });
                        Thread.Sleep(200);
                    }

                    sw.Stop();
                    Logger.Info(
                        $"门店评论审核数据同步,第{(start / step) + 1}批数据,共{(commentCount / step) + 1}批数据,用时{sw.ElapsedMilliseconds}毫秒");
                    sw.Reset();
                }
                catch (Exception ex)
                {
                    Logger.Warn($"第{start / step + 1}批数据异常/{ex.Message}/{ex.InnerException}/{ex.Source}");
                    Thread.Sleep(20000);
                    //start -= step;
                }
            }

            watcher.Stop();
            Logger.Info($"门店评论审核数据同步结束,从{startTime}到{endTime},共{commentCount}条数据,用时{watcher.ElapsedMilliseconds}毫秒");
        }
    }
}
