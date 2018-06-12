using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Tuhu.C.Job.Activity.Dal;
using Tuhu.C.Job.Activity.Models;

namespace Tuhu.C.Job.Activity.Job
{
    [DisallowConcurrentExecution]
    public class LuckyWheelSynchronousDataJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<LuckyWheelSynchronousDataJob>();
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                var needSynchronousDatas = DalLuckyWheel.GetBiLuckyWheelUserlotteryCount().ToList();
                //var bidateTime = needSynchronousDatas.Select(r => r.CreateDateTime).FirstOrDefault();
                //var logdateTime = DalLuckyWheel.GetLogLuckyWheelUserlotteryCreateDateTime();
                //if (bidateTime > logdateTime.AddHours(1))
                //{
                var listInsertData = new List<LuckyWheelUserlotteryCountModel>();
                var biIds = new List<int>();
                if (!needSynchronousDatas.Any())
                {
                    Logger.Info($"bi库查询大翻盘数据结果为空时间{DateTime.Now}");
                    return;
                }
                var watcher = new Stopwatch();
                watcher.Start();
                Logger.Info("从bi库同步大翻盘数据开始");
                foreach (var data in needSynchronousDatas)
                {
                    int isExitData = 0;
                    if (string.IsNullOrWhiteSpace(data.HashKey))
                    {
                        isExitData = DalLuckyWheel.GetLogLuckyWheelUserlotteryCount(data.UserId, data.UserGroup);
                    }
                    else
                    {
                        isExitData = DalLuckyWheel.GetLogLuckyWheelUserlotteryCount(data.UserId, data.HashKey);
                    }
                    if (isExitData > 0)
                    {
                        var result = DalLuckyWheel.UpdateLogLuckyWheelUserlotteryCount(data);
                        if (result <= 0)
                        {
                            Logger.Error($"数据同步到log表失败UserId{data.UserId}UserGroup{data.UserGroup}");
                        }
                        else
                        {
                            biIds.Add(data.Pkid);
                        }
                    }
                    else
                    {
                        listInsertData.Add(data);
                    }

                }
                if (listInsertData.Any())
                {
                    var listResult = DalLuckyWheel.BatchInsertLogLuckyWheelUserlotteryCount(listInsertData);
                    if (!listResult)
                    {
                        Logger.Error($"数据同步到log表批量插入执行失败");
                    }
                    else
                    {
                        biIds.AddRange(listInsertData.Select(r => r.Pkid));
                    }
                }
                if (biIds.Any())
                {
                    var delResult = DalLuckyWheel.BatchDeleteLogLuckyWheelUserlotteryCount(biIds);
                    if (delResult > 0)
                    {
                        Logger.Info($"从bi库删除数据成功, 耗时：{watcher.ElapsedMilliseconds} ms");
                    }
                    else
                    {
                        Logger.Error($"从bi库删除数据失败, 耗时：{watcher.ElapsedMilliseconds} ms");
                    }
                }
                watcher.Stop();
                Logger.Info($"从bi库同步大翻盘数据结束, 耗时：{watcher.ElapsedMilliseconds} ms");
                //}
            }
            catch (Exception e)
            {
                Logger.Error($"同步大翻盘job执行失败{e.Message + e.InnerException}");
            }

        }
    }
}
