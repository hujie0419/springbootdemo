using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Quartz;
using Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.BLL;
using Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.DAL;
using Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.Model;

namespace Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.Job
{
    /// <summary> 刘阳阳 </summary>
    [DisallowConcurrentExecution]
    public class PushMessageForUnregisterJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<PushMessageForUnregisterJob>();

        public void Execute(IJobExecutionContext context)
        {
            Logger.Info("PushMessageForUnregister启动");

            try
            {
                IList<MessageModel> messages = DalPushMessage.SelectPushMessageForUnregister(
                    DateTime.Now).ToList();
                //更新状态   
                DalPushMessage.UpdateSendingStatusPush(messages);
                var iosMessage = messages.Where(x => !string.IsNullOrWhiteSpace(x.Channel)
                                                        &&
                                                        x.Channel.Equals(Constants.ChannelIOS,
                                                            StringComparison.InvariantCultureIgnoreCase)).ToList();
                var androidMessage = messages.Where(x => !string.IsNullOrWhiteSpace(x.Channel)
                                                        &&
                                                        x.Channel.Equals(Constants.ChannelAndriod,
                                                            StringComparison.InvariantCultureIgnoreCase)).ToList();

                PushBussiness.SendAndroidMessage(androidMessage);
                PushBussiness.SendIOSMessage(iosMessage);
            }
            catch (Exception ex)
            {
                Logger.Error("PushMessageForUnregister", ex);
            }

            Logger.Info("PushMessageForUnregister结束");
        }
    }
}
