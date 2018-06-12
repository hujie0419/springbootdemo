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
    public class PushTimelyAndroidMessageJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<PushTimelyAndroidMessageJob>();

        public void Execute(IJobExecutionContext context)
        {
            Logger.Info("PushTimelyAndroidMessageJob启动");

            try
            {
                    IList<MessageModel> messages = DalPushMessage.SelectPushMessage(Constants.ChannelAndriod,
                        DateTime.Now, true).ToList();
                    messages =
                        messages.GroupBy(p => new { p.Id, p.PhoneNumber }).Select(g => g.First()).ToList(); // 去重
                    //更新状态   
                    DalPushMessage.UpdateSendingStatusPush(messages);
                    if (messages.Any())
                    {
                        PushBussiness.SendAndroidMessageByAlias(messages);
                    }
            }
            catch (Exception ex)
            {
                Logger.Error("PushTimelyAndroidMessageJob", ex);
            }

            Logger.Info("PushTimelyAndroidMessageJob结束");
        }
    }
}
