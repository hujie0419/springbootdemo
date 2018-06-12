using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Quartz;
using Tuhu.Service.Push.Models.Push;
using Newtonsoft.Json;
using Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.BLL;

namespace Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.Job
{
    [DisallowConcurrentExecution]
    public class CalculateSinglePushJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<CalculateSinglePushJob>();

        public void Execute(IJobExecutionContext context)
        {

            Logger.Info("单播统计开始");
            List<int> deviceTypes = Enum.GetValues(typeof(DeviceType)).Cast<DeviceType>()
                .Where(x => x != DeviceType.None && x != DeviceType.MessageBox).Select(x => (int)x).ToList();
            foreach (var deviceType in deviceTypes)
            {
                try
                {
                    var isopen = PushBussiness.CheckIsOpenByNameFromCache("countsinglepush");
                    if (!isopen)
                    {
                        Logger.Info("开关已关 return ");
                        return;
                    }
                    var delivered = DAL.DalTemplatePush.CountSinglePushDeliveredByDeviceType(deviceType);
                    var resolved = DAL.DalTemplatePush.CountSinglePushResolvedByDeviceType(deviceType);
                    foreach (var r in resolved)
                    {
                        isopen = PushBussiness.CheckIsOpenByNameFromCache("countsinglepush");
                        if (!isopen)
                        {
                            Logger.Info("开关已关 return ");
                            return;
                        }
                        Logger.Info($"单播统计开始{r.TemplateInfo}");
                        var d = delivered.FirstOrDefault(x => x.TemplateInfo == r.TemplateInfo &&
                                                              x.DeviceType == r.DeviceType && r.PushTime == x.PushTime);
                        Func<DeviceType, PushServiceType> GetPushServiceType = devicetype =>
                        {
                            if (devicetype == DeviceType.Android)
                                return PushServiceType.XiaoMi;
                            else if (devicetype == DeviceType.iOS)
                                return PushServiceType.XiaoMi;
                            else if (devicetype == DeviceType.HuaWeiAndroid)
                                return PushServiceType.HuaWei;
                            else if (devicetype == DeviceType.WeChat)
                                return PushServiceType.WeiXin;
                            else if (devicetype == DeviceType.WeiXinApp)
                                return PushServiceType.WeiXinApp;
                            else
                                return PushServiceType.XiaoMi;
                        };

                        var m = new CalculateMessageInfo()
                        {
                            Delivered = d?.SendCount.ToString() ?? "",
                            DeliveryRate = "",
                            Devicetype = r.DeviceType,
                            MessageID = "",
                            Click = "",
                            ClickRate = "",
                            PushServiceType = GetPushServiceType(r.DeviceType),
                            Resolved = r.SendCount.ToString(),
                            Templateid = r.TemplateInfo
                        };
                        Logger.Info($"单播统计结束{JsonConvert.SerializeObject(m)}.sendtime:{r.PushTime}");
                        DAL.DalTemplatePush.CreateOrUpdateMessageStatistics(m, r.PushTime);

                    }
                }
                catch (System.Exception ex)
                {
                    Logger.Warn($"deviceType:{deviceType},ex:{ex}");
                }
            }


            Logger.Info("单播统计结束");
        }
    }
}
