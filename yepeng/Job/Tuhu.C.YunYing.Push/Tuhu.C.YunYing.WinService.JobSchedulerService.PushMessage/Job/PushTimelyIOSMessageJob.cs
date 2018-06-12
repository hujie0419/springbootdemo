using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using Quartz;
using Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.BLL;
using Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.DAL;
using Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.Model;

namespace Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.Job
{
    /// <summary> 刘阳阳 </summary>
    [DisallowConcurrentExecution]
    public class PushTimelyIOSMessageJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<PushTimelyIOSMessageJob>();
        public void Execute(IJobExecutionContext context)
        {
            Logger.Info("PushTimelyIOSMessageJob启动");

            try
            {
                IList<MessageModel> messages = DalPushMessage.SelectPushMessage(Constants.ChannelIOS,
                        DateTime.Now, true).ToList();
                messages =
                    messages.GroupBy(p => new { p.Id, p.DeviceToken, p.PhoneNumber }).Select(g => g.First()).ToList(); // 去重
                                                                                                                       //更新状态   
                DalPushMessage.UpdateSendingStatusPush(messages);
                if (messages.Any())
                {
                    Logger.Info("PushTimelyIOSMessageJob推送消息数量:" + messages.Count);
                    PushBussiness.SendIOSMessage(messages);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("PushTimelyIOSMessageJob", ex);
            }

            Logger.Info("PushTimelyIOSMessageJob结束");
        }
    }
}
