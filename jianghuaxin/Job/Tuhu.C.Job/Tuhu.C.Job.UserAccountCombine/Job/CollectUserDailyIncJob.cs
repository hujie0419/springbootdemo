using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using System.Linq;
using System.Collections.Generic;
using Tuhu.C.Job.UserAccountCombine.Model;
using Tuhu.C.Job.UserAccountCombine.BLL;
using Quartz;
using Tuhu.MessageQueue;
using Tuhu;

namespace Tuhu.C.Job.UserAccountCombine.Job
{
    [DisallowConcurrentExecution]
    public class CollectUserDailyIncJob : IJob
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(CollectUserDailyIncJob));
        private const string runtimeSwitch = "BatchCollectUserDailyIncJob";

        public void Execute(IJobExecutionContext context)
        {
            _logger.Info("CollectUserDailyIncJob开始执行：" + DateTime.Now.ToString());
            if (UACManager.CheckSwitch(runtimeSwitch))
            {
                DateTime startTime = DateTime.Now.Date.AddDays(-30);
                DateTime endTime = DateTime.Now.Date.AddDays(-1);
                List<string> dateList = new List<string>();

                for (DateTime date = startTime; date <= endTime; date = date.AddDays(1))
                {
                    dateList.Add(date.ToString("yyyy-MM-dd"));
                }

                #region 多线程，处理批量日期
                //try
                //{
                //    Dictionary<int, List<Task>> taskDic = new Dictionary<int, List<Task>>();

                //    for (int index = 0; index < dateList.Count; index++)
                //    {
                //        var task = new Task(() =>
                //        {
                //            UACManager.CollectUserTableDailyInc(dateList[index]);
                //            System.Threading.Thread.Sleep(1000);
                //        });
                //        var num = index / 10;
                //        if (taskDic.ContainsKey(num) && taskDic[num] != null)
                //        {
                //            taskDic[num].Add(task);
                //        }
                //        else
                //        {
                //            taskDic[num] = new List<Task>() { task };
                //        }
                //    }

                //    foreach (var tasks in taskDic.Values)
                //    {
                //        foreach (var item in tasks)
                //        {
                //            item.Start();
                //        }
                //        Task.WaitAll(tasks.ToArray());
                //    }
                //}
                //catch (AggregateException ex)
                //{
                //    foreach (Exception inner in ex.InnerExceptions)
                //    {
                //        _logger.Info($"CollectUserDailyIncJob：运行异常=》{ex}");
                //    }
                //}
                #endregion
                #region 单线程，处理批量日期
                try
                {
                    foreach (var date in dateList)
                    {
                        UACManager.CollectUserTableDailyInc(date);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Info($"CollectUserDailyIncJob：运行异常=》{ex}");
                    Tuhu.TuhuMessage.SendEmail("【用户日增长Warning】批量执行从" +
                        startTime.ToString("yyyy-MM-dd") + "到" +
                        endTime.ToString("yyyy-MM-dd") + "时，脚本异常EOM",
                                "zhangchen3@tuhu.cn;liuchao1@tuhu.cn",
                                "请检查数据和日志." + ex);
                }
                #endregion

            }
            else
            {
                //单线程，及时收集前一天数据
                try
                {
                    DateTime collectTime = DateTime.Now.Date.AddDays(-1);
                    UACManager.CollectUserTableDailyInc(collectTime.ToString("yyyy-MM-dd"));
                }
                catch (Exception ex)
                {
                    _logger.Info($"CollectUserDailyIncJob：运行异常=》{ex}");
                    Tuhu.TuhuMessage.SendEmail("【用户日增长Warning】日期" + DateTime.Now.Date.AddDays(-1).ToString("yyyy-MM-dd") + "脚本执行异常EOM",
                                "zhangchen3@tuhu.cn;liuchao1@tuhu.cn",
                                "请检查数据和日志." + ex);
                }
            }
            _logger.Info("CollectUserDailyIncJob执行结束");
        }
    }
}
