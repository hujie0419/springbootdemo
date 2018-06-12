using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net.Mail;
using System.Linq;
using Common.Logging;
using Quartz;
using Tuhu.C.Job.Activity.Dal;
using Tuhu.Service.Utility.Request;

namespace Tuhu.C.Job.Activity.Job
{
    [DisallowConcurrentExecution]
    public class SeckillConfigMonitorJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<SeckillConfigMonitorJob>();
    
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                Logger.Info("SeckillConfigMonitorJob开始执行");
                var sw = new Stopwatch();
                sw.Start();
                var data = GetActivityConfigInfo(DateTime.Now);
                if (string.IsNullOrEmpty(data))
                {
                    sw.Stop();
                    Logger.Info($"SeckillConfigMonitorJob在时间：{DateTime.Now}，下一个场次秒杀活动已配置，job执行结束耗时{sw.ElapsedMilliseconds}");
                }
                else
                {
                    var sentStr = ConfigurationManager.AppSettings["SeckillPhoneSend"];
                    foreach (var item in sentStr.Split(','))
                    {
                        using (var client = new Tuhu.Service.Utility.SmsClient())
                        {
                            Logger.Info($"秒杀活动开场前检查活动没有配置发送短信通知CellPhone:{sentStr}");
                            var sendresponse = client.SendSms(new SendTemplateSmsRequest()
                            {
                                Cellphone = item,
                                TemplateId = 226,
                                TemplateArguments = new[] { DateTime.Now.ToString("yyyy年MM月dd日") + data }
                            });
                            if (sendresponse != null)
                            {
                                if (sendresponse.Success && sendresponse.Result == 1)
                                {
                                    Logger.Info($"秒杀活动开场前检查活动没有配置给{item}发送短信通知成功");
                                }
                                else
                                {
                                    Logger.Error($"秒杀活动开场前检查活动没有配置给{item}发送短信失败", sendresponse.Exception);
                                }
                            }
                        }
                    }
                }
                sw.Stop();
                Logger.Info($"SeckillConfigMonitorJob执行结束耗时{sw.ElapsedMilliseconds}");
            }
            catch (Exception e)
            {
                Logger.Error($"秒杀活动开场前检查活动没有配置发送短信失败", e);
            }
        }

        private static string GetActivityConfigInfo(DateTime dt)
        {
            var schedule = ConvertToSchedule(dt.Hour);
            #region 过期的待审核的数据在这里删掉
            var expiredDatas = DalActivity.SelectExpiredScheduleActivity(schedule);
            foreach (var activity in expiredDatas)
            {
                Logger.Info($"{activity}活动已经结束了，还没审核通过直接删掉了");
                DalActivity.DeleteFlashSaleTempByAcid(activity);
                DalActivity.DeleteFlashSaleProductsTempByAcid(activity);
            }
            #endregion
            var activityId = DalActivity.SelectNextScheduleActivity(schedule);
            if (string.IsNullOrEmpty(activityId))
            {
                return null;

            }
            else
            {
                return schedule;
            }
        }

        public static string ConvertToSchedule(int hour)
        {
            var s = "";
            if (hour >= 20 && hour <= 24)
            {
                s = "0点场";
            }
            if (hour >= 0 && hour <= 9)
            {
                s = "10点场";
            }
            if (hour >= 10 && hour <= 12)
            {
                s = "13点场";
            }
            if (hour >= 13 && hour <= 15)
            {
                s = "16点场";
            }
            if (hour >= 16 && hour <= 19)
            {
                s = "20点场";
            }
            return s;
        }
    }
}
