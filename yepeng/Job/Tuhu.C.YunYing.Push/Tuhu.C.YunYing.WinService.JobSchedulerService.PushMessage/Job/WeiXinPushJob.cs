using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Quartz;
using Tuhu.Service.Push.Models.Push;
using Newtonsoft.Json;
using Tuhu.Service.Push.Models.WeiXinPush;

namespace Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.Job
{
    [DisallowConcurrentExecution]
    public class WeiXinPushJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<WeiXinPushJob>();

        public void Execute(IJobExecutionContext context)
        {
            Logger.Info("开始推送微信模版消息");
            var logs = DAL.DalWeiXin.SelectPushLogs();
            if (logs != null && logs.Any())
            {
                try
                {
                    Logger.Info($"开始推送{logs.Count()}个记录");
                    PushTemplate template = null;
                    using (var pushclient = new Tuhu.Service.Push.TemplatePushClient())
                    {
                        var templateresult = pushclient.SelectTemplateByBatchID(971);
                        templateresult.ThrowIfException(true);
                        if (templateresult.Result != null && templateresult.Result.Any())
                        {
                            template = templateresult.Result.FirstOrDefault(x => x.DeviceType == DeviceType.WeChat);
                        }
                    }
                    if (template != null)
                    {
                        foreach (var log in logs)
                        {
                            if (!string.IsNullOrEmpty(log?.OpenId))
                            {
                                try
                                {
                                    using (var client = new Tuhu.Service.Push.WeiXinPushClient())
                                    {
                                        var pushresult = client.PushByTemplate(template, new PushTemplateLog()
                                        {
                                            Replacement = JsonConvert.SerializeObject(new Dictionary<string, string>()
                                            {
                                                {"{{first.DATA}}", "金秋出游好时节,提醒您检查并养护刹车系统,为安全加码!"},
                                                {"{{keyword1.DATA}}", "￥99(含产品、工时费）"},
                                                {"{{keyword2.DATA}}", "到店检查全车刹车系统,并对刹车盘、片进行养护，保证其工作稳定。"},
                                                {"{{keyword3.DATA}}", "所有途虎合作门店"},
                                                {"{{remark.DATA}}", "温馨提示：成为途虎认证车主,可享多重车主专属权益。更有洗车福利、油卡福利,每月惠不停！"},
                                            }),
                                            Target = log.OpenId,
                                            DeviceType = DeviceType.WeChat
                                        });
                                        pushresult.ThrowIfException(true);
                                        UpdateWxAuthPushResult(log.OpenId, pushresult.Result);
                                        Logger.Info($"pushresult:{pushresult.Result}.openid:{log.OpenId}");
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    Logger.Warn(ex);
                                }
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Logger.Warn(ex);
                }
            }

            Logger.Info("结束推送微信模版消息");
        }
        public static int UpdateWxAuthPushResult(string openid, bool issuccess)
        {
            string sql = $" update Tuhu_notification..WXUserAuth  WITH(ROWLOCK)  set PushTime=GETDATE(),IsSuccess={(issuccess ? 1 : 0)} where openid='{openid}' ";
            using (var dbhelper = DbHelper.CreateLogDbHelper(false))
            {
                var result = dbhelper.ExecuteNonQuery(sql);
                return result;
            }
        }
    }
}
