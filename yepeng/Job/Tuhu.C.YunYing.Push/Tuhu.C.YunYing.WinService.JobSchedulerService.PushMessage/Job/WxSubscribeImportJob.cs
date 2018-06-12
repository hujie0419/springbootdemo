using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Quartz;
using Tuhu.Service.Push.Models.WeiXinPush;
using Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.BLL;
using Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.DAL;

namespace Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.Job
{
    [DisallowConcurrentExecution]
    public class WxSubscribeImportJob:IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<WxSubscribeImportJob>();
        public static Dictionary<string, int> Platform = new Dictionary<string, int>()
        {
            ["gh_14d1902086bc"] = 0, //途虎养车
            ["gh_155524e3c1f5"] = 9, //新汽车志
            ["gh_f45d2fdb7813"] = 10 //养车开发版
        };

        private static int platformId = Platform["gh_155524e3c1f5"];
        public void Execute(IJobExecutionContext context)
        {
            Logger.Info("开始记录微信openid");
            var j = 0;
            var total = 0;
            do
            {
                var maxopenid = DalWeiXin.GetMaxJobOpenId(platformId);
                var keyValue = WeiXinBLL.GetWeiXinOpenID(maxopenid, platformId);
                if (keyValue == null || keyValue.Item2 == null)
                {
                    Logger.Info("未获取到数据,return");
                    return;
                }
                total = keyValue.Item3;

                var openids = keyValue.Item2;
                var count = keyValue.Item4;

                if (openids != null && openids.Any())
                {
                    Logger.Info($"记录微信openid,获得{openids.Count()}个");
                    var i = 0;

                    foreach (var openid in openids)
                    {
                        i++;
                        j++;
                        try
                        {
                            var isopen = DalTemplatePush.CheckIsOpenByName("logwxuser" + platformId);
                            if (!isopen)
                            {
                                Logger.Info($"开关已关");
                                return;
                            }
                            using (var client = new Tuhu.Service.Push.WeiXinPushClient())
                            {
                                var response = client.LogWxUserOpenIDWithChannel(openid, true, "WX_OA_NewCarMagazine");
                            }
                            if (i == 100)
                            {
                                DalTemplatePush.UpdateRunTimeSwitchDescription("logwxuser" + platformId, openid);
                                i = 0;
                                Logger.Info($"已经导入{j}/{count}/{total}个");
                            }
                        }
                        catch (System.Exception ex)
                        {
                            Logger.Info(ex);
                        }
                    }
                    if (!string.IsNullOrEmpty(keyValue.Item1))
                    {
                        DalTemplatePush.UpdateRunTimeSwitchDescription("logwxuser" + platformId, keyValue.Item1);
                    }
                }
                

            } while (j < total);

            Logger.Info("结束记录微信openid");
        }
    }
}
