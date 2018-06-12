using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Quartz;
using Tuhu.Service.Push.Models.Push;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tuhu.Service.Push.Models.WeiXinPush;
using Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.DAL;
using Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.BLL;

namespace Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.Job
{
    [DisallowConcurrentExecution]
    public class LogWeiXinNewUserJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<LogWeiXinNewUserJob>();
        private static readonly object _locker = new object();
        public void Execute(IJobExecutionContext context)
        {
            Logger.Info("开始记录微信openid");
            var maxopenid = DalWeiXin.GetMaxJobOpenId();
            var result = WeiXinBLL.GetWeiXinOpenID(maxopenid);
            if (result == null || result.Item2 == null)
            {
                Logger.Info("未获取到数据,return");
                return;
            }
            var openids = result.Item2;
            if (openids != null && openids.Any())
            {
                Logger.Info($"记录微信openid,获得{openids.Count()}个");
                var isopen = DalTemplatePush.CheckIsOpenByName("logwxuser");
                if (!isopen)
                {
                    Logger.Info($"开关已关");
                    return;
                }

                int count = 0;
                foreach (var openid in openids)
                {
                    count++;
                    if (count % 15 == 0)
                    {
                        System.Threading.Thread.Sleep(300);
                    }

                    TuhuNotification.SendNotification("notification.LogWxNewUserQueue", openid);
                }
                DalTemplatePush.UpdateRunTimeSwitchDescription("logwxuser", openids.Last());
            }
            Logger.Info("结束记录微信openid");
        }
    }
}
