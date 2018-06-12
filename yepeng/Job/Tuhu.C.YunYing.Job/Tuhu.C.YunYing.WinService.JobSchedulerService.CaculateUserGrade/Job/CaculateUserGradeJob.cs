using Common.Logging;
using Quartz;
using System;
using System.Diagnostics;
using System.Linq;
using Tuhu.C.YunYing.WinService.JobSchedulerService.CaculateUserGrade.Dal;
using Tuhu.C.YunYing.WinService.JobSchedulerService.CaculateUserGrade.Proxy;
using System.Collections.Generic;
using System.Threading;
using Tuhu.C.YunYing.WinService.JobSchedulerService.CaculateUserGrade.Model;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.CaculateUserGrade.Job
{
    [DisallowConcurrentExecution]
    public class CaculateUserGradeJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CaculateUserGradeJob));
        private static string logStringIn = "CaculateUserGrade：ErrorMsg={0}";
        private static string logStringOut = "CaculateUserGrade：UserId={0}";
        private static string ExecuteTime = "CaculateUserGrade：查询第{0}次，耗时{1}ms";
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                Logger.Info("CaculateUserGradeJob开始执行：" + DateTime.Now.ToString());
                var dao = new UserGradeStatisticsDetailDal();
                int i = 1;
                while (i <= 2000)
                {
                    try
                    {
                        Stopwatch time = Stopwatch.StartNew();
                        var UserIdList = dao.GetUserIdByPage(i, 5000);
                        if (UserIdList != null && UserIdList.Any())
                        {
                            i++;
                            UserIdList.ForEach(f =>
                            {
                                if (!MemberProxy.CaculateUserGrade(f.UserId, Logger))
                                {
                                    Logger.Info(string.Format(logStringOut, f.UserId));
                                    Thread.Sleep(1000);
                                }
                                //System.Threading.Thread.Sleep(10);
                            });
                        }
                        else
                        {
                            break;
                        }
                        time.Stop();
                        long timeCount = time.ElapsedMilliseconds;
                        Logger.Info(string.Format(ExecuteTime, i - 1, timeCount));
                    }
                    catch (Exception ex)
                    {
                        Logger.Info(string.Format(logStringIn, ex.ToString()));
                        Thread.Sleep(1000);
                    }
                }
                Logger.Info("CaculateUserGradeJob执行结束：" + DateTime.Now.ToString());
            }
            catch (Exception ex)
            {
                Logger.Info($"CaculateUserGradeJob：异常=》{ex}");
            }

        }

    }
}
