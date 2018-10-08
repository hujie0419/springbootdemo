using Common.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tuhu.Service.Shop;
using Tuhu.Service.ThirdParty;
using Tuhu.Service.ThirdParty.Models;

namespace Tuhu.C.Job.PushAdInfo
{
    [DisallowConcurrentExecution]
    public class PushAdInfoJob : IJob
    {
        public static readonly ILog Logger = LogManager.GetLogger(typeof(PushAdInfoJob));
        public void Execute(IJobExecutionContext context)
        {
            Logger.Info("PushAdInfoJob开始执行");
            try
            {                    
                 PushAdInfoToGoogle();
            }
            catch (Exception ex)
            {
                Logger.Error($"PushAdInfoJob error occur：{ex.ToString()}");
            }
        }

        public async void PushAdInfoToGoogle()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            long topPKid = 0;
            long pkid = 0;
            using (var client = new ThirdPartyClient())
            {
                var response=client.GetTopPKID();
                if (response != null && response.Result > 0)
                {
                    topPKid = response.Result;
                }
                while (pkid <= topPKid)
                {
                    var listAdInforesponse=client.FetchAdInfosByPKID(pkid, 5000);
                    if (listAdInforesponse != null && listAdInforesponse.Result != null)
                    {
                        var andriodList = listAdInforesponse.Result.Where(p => p.TerminalType == Service.ThirdParty.Models.OSType.Android && !string.IsNullOrWhiteSpace(p.AndroidID)).ToList();

                        foreach (var andriodItem in andriodList)
                        {
                          string result=  await ProcessAndroidAdInfo(andriodItem);
                        }
                        pkid = listAdInforesponse.Result.LastOrDefault().PKID;
                        //下一次遍历的初始Pkid
                        pkid += 1;
                    }
                    
                }
            }
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            Logger.Info(string.Format("耗时：{0}s", ts.TotalSeconds));
            //return true;
        }
    
        public async Task<string> ProcessAndroidAdInfo(AdInfo adInfo)
        {
            string result = string.Empty;
            try
            {
                var eventType = GetEventType(adInfo.Type.ToString());
                var androidId = adInfo.AndroidID;
                var appversion = adInfo.AppVersion;
                var osversion = adInfo.OSVersion.Trim().Replace("Android", "").Trim();
                var unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                string url = string.Format(@"http://www.googleadservices.com/pagead/conversion/app/1.0?dev_token=NldQwKEyu5kkEqIKQxz65A&link_id=392487AFBBAC7BD1A3D53D4CF6166499&app_event_type={0}&pdid={1}&id_type=ssaid&lat=0&app_version={2}&os_version={3}&sdk_version={2}&timestamp={4}", eventType, androidId, appversion, osversion, unixTimestamp);
                 result = await Get(url);
                JObject obj = JsonConvert.DeserializeObject(result) as JObject;
                // var flag = obj["errors"].Where(p=>!string.IsNullOrWhiteSpace(p));
                var jarray = JArray.Parse(obj["errors"].ToString());
                var flag = jarray.Count <= 0;
                using (var client = new ThirdPartyClient())
                {
                    if (flag)
                    {
                        client.UpdateAdInfo(adInfo.PKID, StatusType.SuccessTransfer, "");
                    }
                    else
                    {
                        var errormsg = string.Empty;
                        foreach (var arrayitem in jarray)
                        {
                            errormsg += arrayitem;
                            errormsg += "_";
                        }
                        errormsg=errormsg.TrimEnd(new char[] { '_' });
                        client.UpdateAdInfo(adInfo.PKID, StatusType.FailTransfer, errormsg);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Process Android AdInfo error occur in pkid:{adInfo.PKID}",ex);
            }
            return result;
        }

        public string GetEventType(string eventType)
        {
            string eventtype = "session_start";
            switch (eventType)
            {
                case "FirstOpen":
                    eventtype = "first_open";
                    break;
                case "SessionStart":
                    eventtype = "session_start";
                    break;
                case "Registration":
                    eventtype = "custom&app_event_name=registration";
                    break;
                case "Purchase":
                    eventtype = "in_app_purchase";
                    break;
            }
            return eventtype;
        }

        public async Task<string> Get(string url)
        {
            string result = string.Empty;

            try
            {
                using (var client = new HttpClient())
                {
                    var getResult = await client.GetAsync(url);
                    result = await getResult.Content.ReadAsStringAsync();

                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Get请求出错，url：{url}", ex);
            }

            return result;
        }
    }
}
