using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Quartz;
using Newtonsoft.Json;
using Tuhu.Service.Push.Models.Push;
using Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.BLL;
using Tuhu.MessageQueue;

namespace Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.Job
{
    [DisallowConcurrentExecution]
    public class BiTemplatePushJob : IJob
    {
        public static readonly ILog Logger = LogManager.GetLogger<BiTemplatePushJob>();
        private int MqSendCount = 0;
        protected const string DefaultExchangeName = "direct.defaultExchange";
        public const string BiWxPushNotificationQueueName = "notification.BiWxPushNotificationQueue";
       
        public void Execute(IJobExecutionContext context)
        {
            Logger.Info("开始推送BI");
            var isopen = DAL.DalTemplatePush.CheckIsOpen();
            if (!isopen.Item1 || isopen.Item2 <= 0)
            {
                Logger.Info("开关已关 return ");
                return;
            }
            var templates = DAL.DalTemplatePush.SelectBiTemplatePushLogs(isopen.Item2);
            if (templates != null && templates.Any())
            {
                var groupresults = (from item in templates.AsParallel()
                                    group item by new
                                    {
                                        item.Replacement,
                                        item.BatchID
                                    }
                                     into g
                                    select new
                                    {
                                        key = g.Key,
                                        userids = g.Select(x => x?.UserID)?.Distinct(),
                                        deviceids = g.Select(x => x?.DeviceID)?.Distinct(),
                                        openids = g.Select(x => x?.OpenID)?.Distinct(),
                                        wxopenids = g.Select(x => x?.WxOpenID)?.Distinct()
                                    }).ToList();
                Logger.Info($"开始推送.共{groupresults.Count}批次");
                int count = 1;
                foreach (var groupresult in groupresults)
                {
                    isopen = DAL.DalTemplatePush.CheckIsOpen();
                    if (!isopen.Item1)
                    {
                        Logger.Info("开关已关 return ");
                        return;
                    }
                    try
                    {
                        Logger.Info($"开始推送第{count}批次.deviceidcount:{groupresult.deviceids?.Count()}.useridcount:{groupresult.userids?.Count()}");
                        count++;
                        using (var client = new Tuhu.Service.Push.TemplatePushClient())
                        {
                            var pushlog = new PushTemplateLog()
                            {
                                Replacement = groupresult.key.Replacement,
                                DeviceType = DeviceType.Android
                            };
                              
                            if (groupresult.userids != null && groupresult.userids.Any())
                            {
                                var temp = groupresult.userids.Where(x => !string.IsNullOrEmpty(x));
                                if (temp.Any())
                                {
                                    Logger.Info($"开始推送{groupresult.key.BatchID},count:{temp.Count()}");
                                    var result =
                                        client.PushByUserIDAndBatchID(temp, groupresult.key.BatchID,
                                            pushlog);
                                    Logger.Info($"结束推送{groupresult.key.BatchID}.PushByUserIDAndBatchID.result:{result.Result}");
                                }
                            }

                            if (groupresult.deviceids != null && groupresult.deviceids.Any())
                            {
                                var temp = groupresult.deviceids.Where(x => !string.IsNullOrEmpty(x));
                                if (temp.Any())
                                {
                                    Logger.Info($"开始推送{groupresult.key.BatchID},count:{temp.Count()}");
                                    var result =
                                        client.PushByDeviceIDAndBatchID(temp, groupresult.key.BatchID,
                                            pushlog);
                                    Logger.Info($"结束推送{groupresult.key.BatchID}.PushByDeviceIDAndBatchID.result:{result.Result}");
                                }
                            }
                            if (groupresult.openids != null && groupresult.openids.Any())
                            {
                                var temp = groupresult.openids.Where(x => !string.IsNullOrEmpty(x));
                                if (temp.Any())
                                {
                                    Logger.Info($" SendWxPushByMq 开始推送{groupresult.key.BatchID},count:{temp.Count()}");
                                    var wxtemplate = SelectWxAppPushTemplate(groupresult.key.BatchID, DeviceType.WeiXinApp);
                                    if (wxtemplate != null)
                                    {
                                        if (wxtemplate.PushStatus != PushStatus.Suspend)
                                        {
                                            foreach (var openid in temp)
                                            {
                                                var logtemp = pushlog.DeepCopy();
                                                logtemp.Target = openid;
                                                logtemp.TemplateInfo = wxtemplate.PKID;
                                                SendWxPushByMq(wxtemplate, openid, logtemp);
                                            }
                                        }
                                        else
                                        {
                                            Logger.Warn($"{wxtemplate.BatchID}-{wxtemplate.PKID} 模版被暂停.");
                                        }
                                    }
                                    Logger.Info($" SendWxPushByMq 结束推送{groupresult.key.BatchID},count:{temp.Count()}");
                                }
                            }
                            if (groupresult.wxopenids != null && groupresult.wxopenids.Any())
                            {
                                var temp = groupresult.wxopenids.Where(x => !string.IsNullOrEmpty(x));
                                if (temp.Any())
                                {
                                    Logger.Info($"SendWxPushByMq 开始推送{groupresult.key.BatchID},count:{temp.Count()}");
                                    var wxtemplate = SelectWxAppPushTemplate(groupresult.key.BatchID, DeviceType.WeChat);
                                    if (wxtemplate != null)
                                    {
                                        if (wxtemplate.PushStatus != PushStatus.Suspend)
                                        {
                                            foreach (var openid in temp)
                                            {
                                                var logtemp = pushlog.DeepCopy();
                                                logtemp.Target = openid;
                                                logtemp.TemplateInfo = wxtemplate.PKID;
                                                SendWxPushByMq(wxtemplate, openid, logtemp);
                                            }
                                        }
                                        else
                                        {
                                            Logger.Warn($"{wxtemplate.BatchID}-{wxtemplate.PKID} 模版被暂停.");
                                        }
                                    }
                                }
                                Logger.Info($" SendWxPushByMq 结束推送{groupresult.key.BatchID},count:{temp.Count()}");
                            }
                        }
                        Logger.Info($"结束推送第{count}批次");
                    }
                    catch (System.Exception ex)
                    {
                        Logger.Warn(ex);
                    }
                }
            }
            Logger.Info("结束推送BI");
        }
        public bool SendWxPushByMq(PushTemplate wxtemplate, string openid, PushTemplateLog pushlog)
        {
            try
            {
                MqSendCount++;
                if (MqSendCount % 15 == 0)
                {
                    System.Threading.Thread.Sleep(300);
                }

                TuhuNotification.SendNotification(BiWxPushNotificationQueueName, new
                {
                    wxtemplate = wxtemplate,
                    openid = openid,
                    pushlog = pushlog
                });
            }
            catch (System.Exception ex)
            {
                Logger.Warn(ex);
            }
            return false;
        }

        public PushTemplate SelectWxAppPushTemplate(int batchid, DeviceType deviceType)
        {

            return TuhuMemoryCache.Instance.GetOrSet("pushtemplates" + batchid + deviceType, () =>
              {
                  var templates = DAL.DalTemplatePush.SelectPushTemplatesByBatchID(batchid);
                  if (templates != null && templates.Any())
                  {
                      return templates.FirstOrDefault(x => x.DeviceType == deviceType);
                  }
                  return null;
              }, TimeSpan.FromHours(2));
        }
    }
}
