using Common.Logging;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.C.Job.ThirdPart.Dal;
using Tuhu.C.Job.ThirdPart.Manager.GFCard;
using Tuhu.C.Job.ThirdPart.Model;
using Tuhu.C.Job.ThirdPart.Model.Enum;
using Tuhu.C.Job.ThirdPart.Model.GFCard;
using Tuhu.C.Job.ThirdPart.Proxy;
using Tuhu.Service.Member.Models;

namespace Tuhu.C.Job.ThirdPart.Manager
{
    /// <summary>
    /// 调度
    /// </summary>
    class GFJobExecutor
    {
        private static readonly int _taskDaysAgo = 10;//执行近N天的任务
        private static readonly int _smsTaskDaysAgo = 1;
        private static readonly int _pageSize = 300;
        private static readonly int _failedTaskRetryTimes = 1;
        private const int _smsStartHour = 8;
        private const int _smsEndHour = 20;
        private GFDal _gfDal = GFDal.GetInstance();
        /// <summary>
        /// 执行
        /// </summary>
        internal void Execute()
        {
            JobLogger.GFLogger.Info("开始从sftp加载文件");
            this.LoadFileThenCreateTasks();
            JobLogger.GFLogger.Info("从sftp服务加载文件并创建任务结束");
            Thread.Sleep(10000);//读取发券任务时只读库延迟
            JobLogger.GFLogger.Info("开始执行发券任务");
            AsyncHelper.RunSync(async () => await this.ExecutePromotionTasks());//执行发券任务
            AsyncHelper.RunSync(async () => await this.ExecuteRedemptionTasks());//执行发兑换码任务
            AsyncHelper.RunSync(async () => await this.ExecuteSmsTasks());//执行发短信任务
            var nowHour = DateTime.Now.Hour;
            if (nowHour >= 8 && nowHour < 10)
            {
                AsyncHelper.RunSync(async () => await this.ExecuteSmsFailedTasks());//执行发短信失败的任务
            }
        }
        /// <summary>
        /// 从ftp载入文件并创建任务
        /// </summary>
        private void LoadFileThenCreateTasks()
        {
            var fileManager = new GFFileHandler();
            fileManager.LoadFileThenCreateTasks();
        }
        /// <summary>
        /// 执行发券任务
        /// </summary>
        /// <returns></returns>
        private async Task ExecutePromotionTasks()
        {
            var startTime = DateTime.Now.AddDays(-_taskDaysAgo);
            var failedTaskRetryTime = new Dictionary<int, int>();
            for (var pageIndex = 1; pageIndex < 1000; pageIndex++)
            {
                var promotionTasks = await _gfDal.SelectCreatedAndFailedGFBankPromotionTask(startTime, _pageSize, 1);
                CumulateRetryTimes(failedTaskRetryTime, promotionTasks.Where(t => string.Equals(t.Status, nameof(GFTaskStatus.Failed))));
                var notRetryTaskIds = failedTaskRetryTime.Where(s => s.Value > _failedTaskRetryTimes).Select(s => s.Key);
                var avaiableTasks = promotionTasks.Where(s => !notRetryTaskIds.Contains(s.PKID));
                if (avaiableTasks.Any())
                {
                    JobLogger.GFLogger.Info($"开始执行第{pageIndex}批发券任务(每批{_pageSize}个)");
                    foreach (var task in avaiableTasks)//单线程执行任务，如果慢可改成多线程
                    {
                        var gfTaskManager = new GFPromotionTaskExecutor(task);
                        await gfTaskManager.ExecuteTask();
                    }
                    Thread.Sleep(3000);
                }
                else
                {
                    break;
                }
                JobLogger.GFLogger.Info($"第{pageIndex}批发券任务成功结束(每批{_pageSize}个)");
            }
        }

        private async Task ExecuteRedemptionTasks()
        {

            var startTime = DateTime.Now.AddDays(-_taskDaysAgo);
            var failedTaskRetryTime = new Dictionary<int, int>();
            for (var pageIndex = 1; pageIndex < 999999; pageIndex++)
            {
                var redemptionTasks = await _gfDal.SelectCreatedAndFailedGFBankRedemptionCodeTask(startTime, _pageSize, 1);
                CumulateRetryTimes(failedTaskRetryTime, redemptionTasks.Where(t => string.Equals(t.Status, nameof(GFTaskStatus.Failed))));
                var notRetryTaskIds = failedTaskRetryTime.Where(s => s.Value > _failedTaskRetryTimes).Select(s => s.Key);
                var avaiableTasks = redemptionTasks.Where(s => !notRetryTaskIds.Contains(s.PKID));
                if (avaiableTasks.Any())
                {                  
                    JobLogger.GFLogger.Info($"开始执行第{pageIndex}批发兑换码任务(每批{_pageSize}个)");
                    foreach (var task in avaiableTasks)//单线程执行任务，如果慢可改成多线程
                    {
                        var gfTaskManager = new GFRedemptionTaskExecutor(task);
                        await gfTaskManager.ExecuteTask();
                    }
                    Thread.Sleep(3000);
                }
                else
                {
                    break;
                }

                JobLogger.GFLogger.Info($"第{pageIndex}批发兑换码任务成功结束(每批{_pageSize}个)");
            }
        }
        /// <summary>
        /// 执行发短信任务
        /// </summary>
        /// <returns></returns>
        private async Task ExecuteSmsTasks()
        {
            var startTime = DateTime.Now.AddDays(-_smsTaskDaysAgo);
            for (var pageIndex = 1; pageIndex <= 99; pageIndex++)
            {
                var smsTasks = await _gfDal.SelectGFSmsTasks(startTime, _pageSize, 1, nameof(GFTaskStatus.Success));
                if (smsTasks.Any())
                {
                    JobLogger.GFLogger.Info($"开始执行第{pageIndex}批发短信任务(每批{_pageSize}个)");
                    var gfTaskManager = new GFSmsTaskExecutor(smsTasks);
                    await gfTaskManager.ExecuteTasks(SmsSendTime);
                    Thread.Sleep(3000);
                }
                else
                {
                    break;
                }
                JobLogger.GFLogger.Info($"第{pageIndex}批发短信任务结束(每批{_pageSize}个)");
            }

        }
        /// <summary>
        /// 执行发短信失败的任务
        /// </summary>
        /// <returns></returns>
        private async Task ExecuteSmsFailedTasks()
        {
            var startTime = DateTime.Now.AddDays(-_smsTaskDaysAgo);
            for (var pageIndex = 1; pageIndex <= 99; pageIndex++)
            {
                var smsTasks = await _gfDal.SelectGFSmsTasks(startTime, _pageSize, 1, nameof(GFTaskStatus.SmsFailed));
                if (smsTasks.Any())
                {
                    JobLogger.GFLogger.Info($"开始执行第{pageIndex}批发短信失败的任务(每批{_pageSize}个)");
                    var gfTaskManager = new GFSmsTaskExecutor(smsTasks);
                    await gfTaskManager.ExecuteSmsFailedTasks(SmsSendTime);
                    Thread.Sleep(3000);
                }
                else
                {
                    break;
                }
                JobLogger.GFLogger.Info($"第{pageIndex}批发短信失败的任务成功结束(每批{_pageSize}个)");
            }

        }
        /// <summary>
        /// 计算重试次数
        /// </summary>
        /// <param name="counter"></param>
        /// <param name="tasks"></param>
        private void CumulateRetryTimes(Dictionary<int, int> counter, IEnumerable<GFBankTaskBaseModel> tasks)
        {
            foreach (var item in tasks)
            {
                if (counter.ContainsKey(item.PKID))
                {
                    counter[item.PKID] += 1;
                }
                else
                {
                    counter[item.PKID] = 1;
                }
            }
        }
        /// <summary>
        /// 获取短信发送时间
        /// </summary>
        /// <returns></returns>
        private DateTime SmsSendTime
        {

            get
            {
                var now = DateTime.Now;
                var result = now;
                if (now.Hour < _smsStartHour)
                {
                    result = new DateTime(now.Year, now.Month, now.Day, 8, 0, 0);
                }
                if (now.Hour >= _smsEndHour)
                {
                    var tomorrow = now.AddDays(1);
                    result = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 8, 0, 0);
                }

                return result;
            }
        }
    }
}
