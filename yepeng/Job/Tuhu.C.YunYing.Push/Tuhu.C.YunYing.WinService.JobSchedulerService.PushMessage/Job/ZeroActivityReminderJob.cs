using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Quartz;
using Tuhu.Service.Push;
using Tuhu.Service.Push.Models;
using Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.DAL;
using System.Threading;
using Newtonsoft.Json;

namespace Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.Job
{
    [DisallowConcurrentExecution]
    public class ZeroActivityReminderJob : IJob
    {
        private static readonly ILog logger = LogManager.GetLogger<ZeroActivityReminderJob>();

        public void Execute(IJobExecutionContext context)
        {
            logger.Info("ZeroActivityReminderJob启动");
            try
            {
                var zeroActivityInfos = DalPushMessage.SelectUnnotifiedZeroActivityInfo();
                if (zeroActivityInfos != null && zeroActivityInfos.Any())
                {
                    var numsOfApplies = DalPushMessage.SelectSumsOfApplies(zeroActivityInfos.Select(_ => _.Period).ToList());
                    foreach (var zeroActivityInfo in zeroActivityInfos)
                    {
                        var userIdGroup = numsOfApplies.ContainsKey(zeroActivityInfo.Period) ? DalPushMessage.SelectUserIdsToNotify(zeroActivityInfo.Period, (int)Math.Floor(Convert.ToDouble(numsOfApplies[zeroActivityInfo.Period] - 1) / 100.00) + 1) : new List<IEnumerable<string>>();
                        if (userIdGroup.Any())
                        {
                            int i = 1;
                            foreach (var userIds in userIdGroup)
                            {
                                try
                                {
                                    using (var client = new TemplatePushClient())
                                    {
                                        logger.Info($"第{zeroActivityInfo.Period.ToString()}期众测活动第{i}批100条推送开始发送");
                                        var pushResult = client.PushByUserIDAndBatchID(userIds, 371, new Service.Push.Models.Push.PushTemplateLog
                                        {
                                            DeviceType = Service.Push.Models.Push.DeviceType.WeChat,
                                            Replacement = JsonConvert.SerializeObject(new Dictionary<string, string>()
                                                {
                                                    { "{{ios.productdisplayname}}", zeroActivityInfo.ProductDisplayName},
                                                    { "{{android.productdisplayname}}", zeroActivityInfo.ProductDisplayName},
                                                    { "{{messagebox.productdisplayname}}", zeroActivityInfo.ProductDisplayName},
                                                    { "{{ios.period}}", zeroActivityInfo.Period.ToString()},
                                                    { "{{android.period}}", zeroActivityInfo.Period.ToString()},
                                                    { "{{messagebox.period}}", zeroActivityInfo.Period.ToString()},
                                                    { "{{keyword1.DATA}}", $"您预约的途虎众测活动将在今天{zeroActivityInfo.ActivityStartTime.ToString("HH:mm")}开始，点击详情马上参与"},
                                                    { "{{keyword2.DATA}}", zeroActivityInfo.ProductDisplayName},
                                                    { "{{keyword3.DATA}}", zeroActivityInfo.ActivityStartTime.ToString("yyyy.MM.d HH:mm")},
                                                    {"{{Period}}", zeroActivityInfo.Period.ToString()},
                                                    {"{{PID}}", zeroActivityInfo.PID}
                                                })
                                        });
                                        if (!pushResult.Result)
                                            logger.Error($"第{zeroActivityInfo.Period.ToString()}期众测活动开测提醒消息推送失败" +
                                                             $"code:{pushResult.ErrorCode};" +
                                                             $"msg:{pushResult.ErrorMessage}");
                                        pushResult.ThrowIfException(true);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    logger.Error($"第{zeroActivityInfo.Period.ToString()}期活动消息推送失败=>放弃众测活动开测提醒推送逻辑=>" + ex.Message, ex);
                                }
                                Thread.Sleep(2000);
                                i++;
                            }
                        }
                        var updateResult = DalPushMessage.UpdateReminderTransmissionState(zeroActivityInfo.Period);
                        if (!updateResult)
                            logger.Error($"更新第{zeroActivityInfo.Period.ToString()}期众测活动开测提醒发送状态失败");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("ZeroActivityReminderJob", ex);
            }
            logger.Info("ZeroActivityReminderJob结束");
        }
    }
}
