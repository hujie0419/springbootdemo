using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using Quartz;
using Tuhu.Service.Push.Models;
using Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.BLL;
using Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.DAL;
using Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.Model;
using Constants = Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.Model.Constants;

namespace Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.Job
{
    /// <summary> 刘阳阳 </summary>
    [DisallowConcurrentExecution]
    public class PushTaskMassageJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<PushTaskMassageJob>();

        public void Execute(IJobExecutionContext context)
        {
            Logger.Info("PushTaskMassageJob启动");

            try
            {
                IList<MessageModel> messages = DalPushMessage.SelectTaskMessage(
                    DateTime.Now, MessageType.AppBroadcast.ToString()).ToList();
                //更新状态   
                DalPushMessage.UpdateSendingStatusTask(messages);
                var iosMessage = messages.Where(x => !string.IsNullOrWhiteSpace(x.Channel)
                                                        &&
                                                        x.Channel.Equals(Constants.ChannelIOS,
                                                            StringComparison.InvariantCultureIgnoreCase)).ToList();
                var androidMessage = messages.Where(x => !string.IsNullOrWhiteSpace(x.Channel)
                                                        &&
                                                        x.Channel.Equals(Constants.ChannelAndriod,
                                                            StringComparison.InvariantCultureIgnoreCase)).ToList();

                PushBussiness.SendAndroidGroupcastMessage(androidMessage);
                PushBussiness.SendIOSGroupcastMessage(iosMessage);
            }
            catch (Exception ex)
            {
                Logger.Error("PushTaskMassageJob", ex);
            }

            Logger.Info("PushTaskMassageJobs结束");
        }
    }
}
