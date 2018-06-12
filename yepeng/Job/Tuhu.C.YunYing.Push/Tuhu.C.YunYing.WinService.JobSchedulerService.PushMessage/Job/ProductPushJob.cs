using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Newtonsoft.Json;
using Quartz;
using Tuhu.Service.Product.Models;
using Tuhu.Service.Push.Models.Push;
using Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.BLL;

namespace Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.Job
{
    [DisallowConcurrentExecution]
    public class ProductPushJob : IJob
    {

        private static readonly ILog Logger = LogManager.GetLogger<ProductPushJob>();
      
        public void Execute(IJobExecutionContext context)
        {
            Logger.Info("开始产品推送job");
            var isopen = PushBussiness.CheckIsOpenByNameFromCache("productpush");
            if (!isopen)
            {
                Logger.Info("开关已关 return");
                return;
            }
            var pid = "TR-CT-C2-MC5|17";
            var activityid = "2878";
            var models = DAL.DalTemplatePush.SelectProductPushModels();
            if (models != null && models.Any())
            {
                var result = from m in models
                             group m by m?.DeviceID
                             into g
                             select new
                             {
                                 deviceid = g.Key,
                                 models = g
                             };
                if (result.Any())
                {
                    Logger.Info($"开始推送,共{result.Count()}个deviceid");
                    var productinfos = SelectProductsByPids(new List<string>() { pid });
                    foreach (var item in result)
                    {
                        try
                        {
                            isopen =  PushBussiness.CheckIsOpenByNameFromCache("productpush"); 
                            if (!isopen)
                            {
                                Logger.Info("开关已关 return");
                                return;
                            }
                            var deviceid = item.deviceid;
                            //var pid = item.models.OrderByDescending(x => x?.ClickTime).FirstOrDefault()?.PID;
                            //pid = RecordPids.FirstOrDefault(x => !string.Equals(pid, x, StringComparison.OrdinalIgnoreCase));

                            if (!string.IsNullOrEmpty(deviceid) && !string.IsNullOrEmpty(pid))
                            {
                                var product = productinfos.FirstOrDefault();
                                if (product != null)
                                {
                                    var tiresize = product.Size.ToString();
                                    var productname = product.DisplayName;
                                    //var price = product.Price.ToString(".00");
                                    var price = "459";
                                    using (var client = new Tuhu.Service.Push.TemplatePushClient())
                                    {
                                        var pushresult =
                                            client.PushByDeviceIDAndBatchID(new List<string>() { deviceid }, 652,
                                                new PushTemplateLog()
                                                {
                                                    Replacement =
                                                        JsonConvert.SerializeObject(new Dictionary<string, string>()
                                                        {
                                                                {"{{ios.tyresize}}", tiresize},
                                                                {"{{ios.activityID}}",activityid},
                                                                {"{{ios.productname}}", productname},
                                                                {"{{android.activityID}}",activityid },
                                                                {"{{activityID}}", activityid},
                                                                {"{{android.tyresize}}", tiresize},
                                                                {"{{android.productname}}", productname},
                                                                {"{{messagebox.tyresize}}", tiresize},
                                                                {"{{messagebox.productname}}", productname},
                                                                {"{{messagebox.activityID}}", activityid},
                                                                {"{{ios.price}}", price},
                                                                {"{{android.price}}", price},
                                                                {"{{messagebox.price}}", price},
                                                        }),
                                                    DeviceType = DeviceType.Android
                                                });
                                        pushresult.ThrowIfException(true);
                                        Logger.Info($"结束推送{deviceid}.result:{pushresult.Result}");
                                        DAL.DalTemplatePush.UpdateProductPushPushResult(deviceid);
                                    }

                                }
                            }
                        }
                        catch (System.Exception ex)
                        {
                            Logger.Warn(ex);
                        }
                    }
                }
            }
            Logger.Info("结束推送");
            Logger.Info("开始推送短信");
            var smsmodels = DAL.DalTemplatePush.SelectProductSendSmsModels();
            if (smsmodels != null && smsmodels.Any())
            {
                var result = from m in smsmodels
                             group m by m?.DeviceID
                             into g
                             select new
                             {
                                 deviceid = g.Key,
                                 models = g
                             };
                if (result.Any())
                {
                    var userids = smsmodels.Select(x => x?.UserID)?.Distinct()?.Where(x => !string.IsNullOrEmpty(x));
                    if (userids != null && userids.Any())
                    {
                        var mobiledist = DAL.DalTemplatePush.SelectMobilesByUserIDAsync(userids);
                        var productinfos = SelectProductsByPids(new List<string>() { pid });
                        Logger.Info($"开始推送,共{userids.Count()}个userid");
                        foreach (var item in result)
                        {
                            try
                            {

                                isopen = PushBussiness.CheckIsOpenByNameFromCache("productpush");
                                if (!isopen)
                                {
                                    Logger.Info("开关已关 return");
                                    return;
                                }
                                //var pid = item?.models?.OrderByDescending(x => x?.ClickTime).FirstOrDefault()?.PID;
                                //pid = RecordPids.FirstOrDefault(x => !string.Equals(pid, x, StringComparison.OrdinalIgnoreCase));
                                var userid = item?.models?.OrderByDescending(x => x?.ClickTime).FirstOrDefault()?.UserID;
                                var product = productinfos.FirstOrDefault();
                                if (!string.IsNullOrEmpty(userid) && mobiledist.ContainsKey(userid) && product != null)
                                {
                                    var tiresize = product.Size.ToString();
                                    var productname = product.DisplayName;
                                    //var price = product.Price.ToString(".00");
                                    var price = "459";
                                    string shorturl =
                                        GetShortUrl($"https://wx.tuhu.cn/staticpage/activity/list.html?id={activityid}");
                                    using (var client = new Tuhu.Service.Utility.SmsClient())
                                    {
                                        var smsresult = client.SendSms(mobiledist[userid], 93, shorturl, tiresize, productname, price);
                                        smsresult.ThrowIfException(true);
                                        Logger.Info(
                                            $"结束推送短信.{userid},Success:{smsresult.Success}.result:{smsresult.Result}");
                                        DAL.DalTemplatePush.UpdateProductPushPushSmsResult(userid);
                                    }
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
            Logger.Info("全部结束");
        }

        public string GetShortUrl(string url)
        {
            using (var client = new Tuhu.Service.Utility.UtilityClient())
            {
                var result = client.GetTuhuDwz(url, "pushjob");
                result.ThrowIfException(true);
                return result.Result;
            }
        }
        public IEnumerable<ProductModel> SelectProductsByPids(IEnumerable<string> pids)
        {
            if (pids != null && pids.Any())
            {
                using (var client = new Tuhu.Service.Product.ProductClient())
                {
                    var result = client.SelectProduct(pids);
                    result.ThrowIfException(true);
                    return result.Result;
                }
            }
            return new List<ProductModel>();
        }
    }
}
