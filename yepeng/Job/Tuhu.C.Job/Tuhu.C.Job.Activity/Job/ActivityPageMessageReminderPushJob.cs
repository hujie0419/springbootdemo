using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Common.Logging;
using Quartz;
using Tuhu.Service.Push.Models.Push;
using Tuhu.C.Job.Activity.Dal;
using Tuhu.C.Job.Activity.Models;

namespace Tuhu.C.Job.Activity.Job
{
    [DisallowConcurrentExecution]
    public class ActivityPageMessageReminderPushJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<ActivityPageMessageReminderPushJob>();
        public void Execute(IJobExecutionContext context)
        {
            var sw = new Stopwatch();
            sw.Start();
            Logger.Info("活动页消息推送Job开始执行");
            try
            {
                var datas = BeforeExecuteJob();
                if (!datas.Any())
                {
                    Logger.Info("活动页消息推送Job==>没有将要开始的活动商品");
                }
                else
                {
                    var boolPush = ExecuteJob(datas);
                    if (boolPush)
                    {
                        var pkids = datas.Select(r => r.Pkid).ToList();
                        AfterExecuteJob(pkids);
                    }


                }
            }
            catch (Exception ex)
            {
                Logger.Warn("活动页消息推送Job执行出现异常", ex);
            }
            sw.Stop();
            Logger.Info($"活动页消息推送Job执行完成,用时{sw.ElapsedMilliseconds}毫秒");
        }

        public bool ExecuteJob(List<ActivityMessageRemindModel> activityMessages)
        {
            if (activityMessages == null || !activityMessages.Any())
                return false;
            var pushMessages = activityMessages.GroupBy(r => new { r.ActivityId, r.UserId }).Select(g =>
              {
                  var model = g.FirstOrDefault<ActivityMessageRemindModel>();
                  if (model == null)
                      return new ActivityMessageRemindModel();
                  var result = new ActivityMessageRemindModel
                  {
                      ActivityId = g.Key.ActivityId,
                      Pid = model.Pid,
                      UserId = g.Key.UserId,
                      ProductName = model.ProductName,
                      Pkid = model.Pkid,
                      ActivityName = model.ActivityName,
                      IsMultiProducts = g.Count() > 1,
                      StartDtartTime = model.StartDtartTime,
                      
                  };
                  return result;
              });
            using (var client = new Service.Push.TemplatePushClient())
            {
                foreach (var pushMessage in pushMessages)
                {
                    if (pushMessage.IsMultiProducts)
                    {
                        var result = client.PushByUserIDAndBatchID(new List<string> { pushMessage.UserId.ToString() }, 676,
                            new PushTemplateLog()
                            {
                                Replacement = Newtonsoft.Json.JsonConvert.SerializeObject(new Dictionary<string, string>()
                                    {
                                        {"{{ios.productname}}", $"{pushMessage.ProductName}等商品"},
                                        {"{{ios.activitytime}}", pushMessage.StartDtartTime.ToString("F")},
                                        {"{{ios.activitypagehashid}}", pushMessage.ActivityId},
                                        {"{{android.productname}}", $"{pushMessage.ProductName}等商品"},
                                        {"{{android.activitytime}}", pushMessage.StartDtartTime.ToString("F")},
                                        {"{{android.activitypagehashid}}", pushMessage.ActivityId},
                                        {"{{messagebox.productname}}", $"{pushMessage.ProductName}等商品"},
                                        {"{{messagebox.activitytime}}", pushMessage.StartDtartTime.ToString("F")},
                                        {"{{messagebox.activitypagehashid}}", pushMessage.ActivityId}
                                    })
                            }
                        );
                        if (result.Success)
                        {
                            Logger.Info($"活动页推送消息成功Pid{pushMessage.Pid}ActivityId{pushMessage.ActivityId}UserId{pushMessage.UserId}");
                        }
                        else
                        {
                            Logger.Warn($"活动页推送消息失败Pid{pushMessage.Pid}ActivityId{pushMessage.ActivityId}UserId{pushMessage.UserId}");
                        }
                    }
                    else
                    {
                        var result = client.PushByUserIDAndBatchID(new List<string> { pushMessage.UserId.ToString() }, 676,
       new PushTemplateLog()
       {
           Replacement = Newtonsoft.Json.JsonConvert.SerializeObject(new Dictionary<string, string>()
               {
                                        {"{{ios.productname}}", $"{pushMessage.ProductName}"},
                                        {"{{ios.activitytime}}", pushMessage.StartDtartTime.ToString("F")},
                                        {"{{ios.activitypagehashid}}", pushMessage.ActivityId},
                                        {"{{android.productname}}", $"{pushMessage.ProductName}"},
                                        {"{{android.activitytime}}", pushMessage.StartDtartTime.ToString("F")},
                                        {"{{android.activitypagehashid}}", pushMessage.ActivityId},
                                        {"{{messagebox.productname}}", $"{pushMessage.ProductName}"},
                                        {"{{messagebox.activitytime}}",pushMessage.StartDtartTime.ToString("F")},
                                        {"{{messagebox.activitypagehashid}}", pushMessage.ActivityId}
               })
       });
                        if (result.Success)
                        {
                            Logger.Info($"活动页推送消息成功Pid{pushMessage.Pid}ActivityId{pushMessage.ActivityId}UserId{pushMessage.UserId}");
                        }
                        else
                        {
                            Logger.Warn($"活动页推送消息失败Pid{pushMessage.Pid}ActivityId{pushMessage.ActivityId}UserId{pushMessage.UserId}");
                        }
                    }
                }

            };
            return true;
        }



        public List<ActivityMessageRemindModel> BeforeExecuteJob()
        {
            var list = new List<ActivityMessageRemindModel>();
            var activtyProducts = DalActivity.SelectFlashSaleProductModels().ToList();

            if (activtyProducts.Any())
            {
                var pids = activtyProducts.Select(r => r.Pid);
                foreach (var enumerable in pids.Split(100).Select(_ => _.ToList()))
                {
                    var datas = (DalActivity.SelectActivityMessageRemindModel(enumerable)).ToList();
                    if (datas.Any())
                        list.AddRange(datas);
                };
                var result = (from l in list
                              join p in activtyProducts on l.Pid equals p.Pid
                              select new ActivityMessageRemindModel
                              {
                                  ActivityId = l.ActivityId,
                                  ActivityName = l.ActivityName,
                                  Pid = l.Pid,
                                  Pkid = l.Pkid,
                                  ProductName = l.ProductName,
                                  UserId = l.UserId,
                                  StartDtartTime = p.StartDateTime,
                              }).ToList();
                return result;
            }
            return list;
        }
        public void AfterExecuteJob(List<int> pkids)
        {
            foreach (var ints in pkids.Split(100).Select(_ => _.ToList()))
            {
                var enumerable = ints;
                var datas = (DalActivity.UpdateActivityMessageRemindModel(enumerable));
                if (datas <= 0)
                {
                    Logger.Warn("更新已经推送过的消息记录失败");
                }
                else
                {
                    Logger.Info($"更新已经推送过的消息记录成功共计{datas}条数据");
                }
            }
        }
    }
}
