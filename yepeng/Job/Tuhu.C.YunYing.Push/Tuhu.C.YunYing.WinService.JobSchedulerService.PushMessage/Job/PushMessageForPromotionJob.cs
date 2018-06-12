using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Quartz;
using Tuhu.Service.Push.Models;
using Tuhu.Service.Push;
using System.Configuration;
using Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.DAL;
using Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.Model;

namespace Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.Job
{
    /// <summary> 王递杰 </summary>
    [DisallowConcurrentExecution]
    public class PushMessageForPromotionJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<PushMessageForPromotionJob>();
        private static string msgTitle = ConfigurationManager.AppSettings["Promotion:Title"];
        private static string msgContent = ConfigurationManager.AppSettings["Promotion:Content"];
        private static string msgDesc = ConfigurationManager.AppSettings["Promotion:Description"];

        public void Execute(IJobExecutionContext context)
        {
            Logger.Info("PushMessageForPromotionJob启动");

            try
            {
                //每次取1W条
                IList<MessageModel> messages = DalPushMessage.SelectPushMessageForPromotion().ToList();
                messages = messages.GroupBy(p => new { p.PhoneNumber }).Select(g => g.First()).ToList(); // 去重
                if (messages == null || messages.Count == 0) return;

                //更新状态sending 2→发送中
                DalPushMessage.UpdateSendStatePromotion(messages, 2);

                var messageModel = new PushMessageModel
                {
                    PhoneNumbers = messages.Select(p => p.PhoneNumber).ToList(),
                    Title = msgTitle,
                    Content = msgContent,
                    Description = msgDesc,
                    //过期时间
                    ExpireTime = null,
                    //推送消息类型
                    Type = MessageType.AppNotification,
                    //消息系统中专用的消息类型 (0回评,1普通,2广播,3私信)
                    CenterMsgType = Service.Push.Models.Constants.MessageType_Common,
                    //推送方名称（域名）
                    SourceName = "PushMessageForPromotionJob",
                    // 标识是否插入消息到App消息中心。默认为true;
                    InsertAppCenterMsg = false,
                    // App消息过期时间
                    AppExpireTime = null,
                    //操作人
                    OperUser = "PushMessageForPromotionJob"
                };
                //推送结果
                bool sendResult = true;

                using (var client = new PushClient())
                {
                    var pushResult = client.PushMessages(messageModel);
                    if (pushResult != null)
                    {
                        if (!string.IsNullOrEmpty(pushResult.ErrorMessage))
                        {
                            sendResult = false;
                            Logger.Error("PushMessageForPromotionJob:" + pushResult.ErrorMessage);
                        }
                        else
                        {
                            sendResult = pushResult.Result;
                        }
                    }
                }
                //更新状态
                if (sendResult)
                    DalPushMessage.UpdateSendStatePromotion(messages, 1);//已发送
                else
                    DalPushMessage.UpdateSendStatePromotion(messages, 3);//发送异常
            }
            catch (Exception ex)
            {
                Logger.Error("PushMessageForPromotionJob", ex);
            }

            Logger.Info("PushMessageForPromotionJob结束");
        }
    }
}
