using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Quartz;
using Tuhu.Service.Push.Models.Push;

namespace Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.Job
{
    [DisallowConcurrentExecution]
    public class CalculateMessageInfoJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<CalculateMessageInfoJob>();

        public void Execute(IJobExecutionContext context)
        {
            Logger.Info("开始统计消息");

            var templates = DAL.DalTemplatePush.SelectPushedTemplates();
            if (templates != null && templates.Any())
            {
                foreach (var pushTemplate in templates)
                {
                    try
                    {
                        if (pushTemplate.DeviceType == DeviceType.MessageBox)
                        {
                            continue;
                        }
                        Logger.Info($"开始统计{pushTemplate.PKID}");
                        using (var client = new Tuhu.Service.Push.TemplatePushClient())
                        {
                            var result = client.CalculateMessageInfo(pushTemplate.PKID, PushServiceType.XiaoMi);
                            result.ThrowIfException(true);
                            if (pushTemplate.DeviceType == DeviceType.Android)
                            {
                                var result2 = client.CalculateMessageInfo(pushTemplate.PKID, PushServiceType.HuaWei);
                                result2.ThrowIfException(true);
                                Logger.Info($"结束统计{pushTemplate.PKID}.huawei:result:{result2.Result}");

                                var result3 = client.CalculateMessageInfo(pushTemplate.PKID, PushServiceType.HuaWeiPro);
                                result3.ThrowIfException(true);
                                Logger.Info($"结束统计{pushTemplate.PKID}.huawei:result:{result3.Result}");
                            }
                            Logger.Info($"结束统计{pushTemplate.PKID}.xiaomi:result:{result.Result}");
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Logger.Warn($"统计消息数据job.templateid:{pushTemplate.PKID}.ex:{ex}");
                    }
                }
            }
            Logger.Info($"结束统计..");

        }
    }
}
