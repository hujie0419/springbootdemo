using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Quartz;
using Tuhu.Service.Push.Models.Push;
using Newtonsoft.Json;
using Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.DAL;

namespace Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.Job
{
    [DisallowConcurrentExecution]
    public class TemplatePushMessageJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<TemplatePushMessageJob>();
        public void Execute(IJobExecutionContext context)
        {
            var templates = DAL.DalTemplatePush.SelectPushTemplates();
            if (templates != null && templates.Count() > 0)
            {
                foreach (var pushTemplate in templates)
                {
                    bool result = false;
                    try
                    {
                        using (var client = new Tuhu.Service.Push.TemplatePushClient())
                        {
                            Logger.Info($"开始推送{JsonConvert.SerializeObject(pushTemplate)}");
                            DAL.DalTemplatePush.UpdateTemplateStatus(pushTemplate.PKID, PushStatus.Pushing);
                            var pushresult = client.PushByTemplateID(pushTemplate.PKID);
                            pushresult.ThrowIfException(true);
                            if (!pushresult.Result)
                            {
                                Logger.Error($"{pushTemplate.PKID} 推送失败");
                            }
                            result = pushresult.Result;
                            Logger.Info($"结束推送{JsonConvert.SerializeObject(pushTemplate)},result:{pushresult.Result}");
                        }
                    }
                    catch (System.Exception ex)
                    {
#if DEBUG
                        throw;
#else
                        Logger.Error($"{pushTemplate.PKID}.error:{ex}");
#endif
                    }
                    finally
                    {
                        DAL.DalTemplatePush.UpdateTemplateStatus(pushTemplate.PKID, result ? PushStatus.Success : PushStatus.Failed);
                    }
                }
                if (templates.Any(x => x.SendType == SendType.Broadcast && x.DeviceType == DeviceType.Android))
                {
                    var template = templates.FirstOrDefault(x => x.SendType == SendType.Broadcast && x.DeviceType == DeviceType.Android);
                    if (template != null)
                    {
                        var vendors = new List<string>() { "huawei", "huaweipro" };
                        foreach (var vendor in vendors)
                        {
                            var totalcount = DalHuaWeiPush.SelectHuaWeiDeviceTokenCounts(vendor);
                            Logger.Info($"华为总数:{totalcount}. ,模版id:{template.PKID}.vendor:{vendor}");
                            if (totalcount > 0)
                            {
                                int maxsize = 1000;
                                int pagecount = (totalcount + maxsize - 1) / maxsize;
                                for (int i = 1; i <= pagecount; i++)
                                {
                                    Logger.Info($"华为总数:{totalcount}. ,模版id:{template.PKID} 开始发送消息 index:{i}");
                                    TuhuNotification.SendNotification("HuaWeiBroadcastPushQueue123", new Dictionary<string, string>()
                                {
                                    {"Pageindex",i.ToString() },
                                    {"TemplateId",template.PKID.ToString() },
                                    {"MaxSize",maxsize.ToString() },
                                    {"vendor",vendor }
                                });
                                    System.Threading.Thread.Sleep(1000);
                                }
                            }
                        }

                    }
                }
            }
        }
    }
}
