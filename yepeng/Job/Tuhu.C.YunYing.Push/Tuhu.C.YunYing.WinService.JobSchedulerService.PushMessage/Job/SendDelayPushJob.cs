using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Quartz;
using Newtonsoft.Json;
using Tuhu.Service;
using Tuhu.Service.Push.Models.Push;

namespace Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.Job
{
    public class SendDelayPushJob : IJob
    {
        public static readonly ILog Logger = LogManager.GetLogger<SendDelayPushJob>();
        public void Execute(IJobExecutionContext context)
        {
            int count = 0;
            while (true)
            {
                var result = DAL.DalTemplatePush.SelectDelayPushLogs(1000);
                if (result != null && result.Any())
                {
                    Logger.Info($"开始推送第{count} 批次");
                    var pushitems = (from item in result.AsParallel()
                                     group item by item.SendBatchId
                                        into g
                                     select new
                                     {
                                         pushinfo = g
                                     }).ToList();
                    foreach (var pushitem in pushitems)
                    {
                        try
                        {
                            using (var client = new Tuhu.Service.Push.TemplatePushClient())
                            {
                                var pushtype = pushitem.pushinfo?.FirstOrDefault()?.PushType;

                                var pushinfo = pushitem.pushinfo?.FirstOrDefault();
                                var targets = pushitem.pushinfo?.Select(x => x.Targets)
                                    ?.Where(x => !string.IsNullOrEmpty(x));
                                OperationResult<bool> pushresult = null;

                                if (pushtype != null && pushinfo != null && targets != null && targets.Any())
                                {
                                    Logger.Info(
                                        $"开始推送:{pushtype.Value}.targets count:{targets.Count()}.batchid:{pushinfo.BatchId}");
                                    switch (pushtype.Value)
                                    {
                                        case PushType.UserID:
                                            pushresult = client.PushByUserIDAndBatchID(
                                                pushitem.pushinfo.Select(x => x.Targets),
                                                pushinfo.BatchId,
                                                JsonConvert.DeserializeObject<PushTemplateLog>(pushinfo.PushTemplateLog)
                                            );
                                            break;
                                        case PushType.DeviceID:
                                            pushresult = client.PushByDeviceIDAndBatchID(
                                                pushitem.pushinfo.Select(x => x.Targets),
                                                pushinfo.BatchId,
                                                JsonConvert.DeserializeObject<PushTemplateLog>(pushinfo.PushTemplateLog)
                                            );
                                            break;
                                        case PushType.Alias:
                                            pushresult = client.PushByBatchIDAndAlias(
                                                pushitem.pushinfo.Select(x => x.Targets),
                                                JsonConvert.DeserializeObject<PushTemplateLog>(pushinfo
                                                    .PushTemplateLog), pushinfo.BatchId
                                            );
                                            break;
                                        default:
                                            Logger.Warn($"未支持推送类型.{pushtype}");
                                            break;
                                    }
                                }

                                if (pushresult != null)
                                {
                                    Logger.Info($"结束推送第{count} 批次.result:{pushresult.Result}.batchid:{pushinfo.BatchId}");
                                }
                            }

                        }
                        catch (System.Exception ex)
                        {
                            Logger.Warn(ex);
                        }
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }
}
