using System;
using System.Diagnostics;
using System.Threading;
using Common.Logging;
using Quartz;
using Tuhu.Service.PinTuan;
using Tuhu.Service.PinTuan.Models;

namespace Tuhu.C.Job.Job
{
    /// <summary>
    ///     拼团车型认证服务补足 Job
    /// </summary>
    [DisallowConcurrentExecution]
    public class PinTuanCarCertificationRepairJob : IJob
    {

        private static readonly ILog Logger = LogManager.GetLogger<PinTuanCarCertificationRepairJob>();

        /// <summary>
        ///     触发执行Job
        /// </summary>
        /// <param name="context"></param>
        public void Execute(IJobExecutionContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            Logger.Info("拼团车型认证服务补足Job 开始执行");
            try
            {
                Run();
            }
            catch (Exception e)
            {
                Logger.Error("拼团车型认证服务补足Job -> Error", e.InnerException ?? e);
            }
            stopwatch.Stop();
            Logger.Info($"拼团车型认证服务补足Job 结束执行,用时{stopwatch.Elapsed.Seconds}秒");

        }

        /// <summary>
        ///     开始执行
        /// </summary>
        private void Run()
        {
            using (var pinTuanCarCertificationClient = new PinTuanCarCertificationClient())
            {
                while (true)
                {
                    // 获取异常数据
                    var noFinishDataListResult = pinTuanCarCertificationClient.SearchReceiveWarningData();
                    noFinishDataListResult?.ThrowIfException(true);
                    var noFinishDataList = noFinishDataListResult?.Result;
                    if (noFinishDataList == null || noFinishDataList.Count == 0)
                    {
                        return;
                    }

                    // 循环
                    foreach (var pinTuanCertificationModel in noFinishDataList)
                    {
                        // 日志
                        Logger.Info($" 拼团车型认证福利补足数据 {pinTuanCertificationModel.OrderId} {pinTuanCertificationModel.UserId} {pinTuanCertificationModel.PKID} ");
                        //调用服务
                        try
                        {
                            pinTuanCarCertificationClient.CarCertificationReceiveGive(
                                new CarCertificationReceiveGiveRequest()
                                {
                                    CerificationReceiveId = pinTuanCertificationModel.PKID

                                });
                        }
                        catch (Exception e)
                        {
                            Logger.Error($"拼团车型认证服务补足Job 补足失败 ", e);
                        }
                    }

                    Thread.Sleep(TimeSpan.FromSeconds(5));

                }
            }
        }


    }
}
