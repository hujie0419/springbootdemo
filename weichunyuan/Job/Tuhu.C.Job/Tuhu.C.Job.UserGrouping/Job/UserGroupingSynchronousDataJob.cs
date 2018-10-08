using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Common.Logging;
using Quartz;
using Tuhu.C.Job.UserGrouping.Dal;
using Tuhu.C.Job.UserGrouping.Models;

namespace Tuhu.C.Job.UserGrouping.Job
{
    [DisallowConcurrentExecution]
    public class UserGroupingSynchronousDataJob :IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<UserGroupingSynchronousDataJob>();
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                var needSynchronousDatas = DalUserGrouping.GetUserGroupingModel().ToList();
                if (!needSynchronousDatas.Any())
                {
                    Logger.Info($"bi库查询用户需要分组数据结果为空时间{DateTime.Now}");
                    return;
                }
                var watcher = new Stopwatch();
                watcher.Start();
                Logger.Info("从bi库同步用户分组数据开始");
                var needInserlist = new List<UserGroupingModel>();
                foreach (var item in needSynchronousDatas)
                {
                    var logUserGrouping = DalUserGrouping.GetLogUserGroupingModel(item);
                    if (logUserGrouping == null)
                    {
                        needInserlist.Add(item);
                    }
                    else
                    {
                        if (logUserGrouping.EndDateTime != item.EndDateTime ||logUserGrouping.StartDateTime != item.StartDateTime)
                        {
                            var updateResult = DalUserGrouping.UpdatetblUserStatisticsGrouping(item);
                            if (updateResult <= 0)
                            {
                               Logger.Error($"从bi库同步用户分组数据到log表失败UserId{item.UserId}Tag{item.Tag}DeviceId{item.DeviceId}");
                            }
                        }
                    }
                }
                if(needInserlist.Any())
                {
                    var listResult = DalUserGrouping.BatchInserttblUserStatisticsGrouping(needInserlist);
                    if (!listResult)
                    {
                        Logger.Error("数据同步从bi库同步用户分组数据到log表失败");
                    }
                }
                watcher.Stop();
                Logger.Info($"数据同步从bi库同步用户分组数据到log表结束, 耗时：{watcher.ElapsedMilliseconds} ms");
            }
            catch (Exception e)
            {
                Logger.Error($"数据同步从bi库同步用户分组数据到log表失败{e.Message + e.InnerException}");
            }   
        }
    }
}
