using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.Job;

namespace Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.BLL
{
    public class WeiXinBLL
    {
        private static readonly ILog Logger = LogManager.GetLogger<WeiXinBLL>();

        public static string GetWeiXinAccessToken(int platform=0)
        {

            using (var client = new WebClient())
            {
                client.Headers.Add("access_token", "access_token");
                string url = "https://wx.tuhu.cn/packet/test?platform=" + platform;
                var result = client.DownloadString(url);
                var obj = JsonConvert.DeserializeObject(result) as JObject;
                string accesstoken = obj != null && !string.IsNullOrEmpty(obj["access_token"]?.ToString())
                    ? obj["access_token"].ToString()
                    : "";
                if (string.IsNullOrEmpty(accesstoken))
                {
                    Logger.Info($"获取accesstoken失败.rsp:{result}");
                }
                return accesstoken;
            }
        }

        public static Tuple<string,IEnumerable<string>,int,int> GetWeiXinOpenID(string nextopenid = null,int platform=0)
        {
            string token = GetWeiXinAccessToken(platform);
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }
            string url =
                $"https://api.weixin.qq.com/cgi-bin/user/get?access_token={token}&next_openid={(string.IsNullOrEmpty(nextopenid) ? "" : nextopenid)}";
            using (var client = new WebClient())
            {
                var result = client.DownloadString(url);
                var obj = JsonConvert.DeserializeObject(result) as JObject;
                try
                {
                    if (obj["data"] != null && obj["data"]["openid"] != null)
                    {
                        var openids =
                            JsonConvert.DeserializeObject<IEnumerable<string>>(obj["data"]["openid"].ToString());
                        return Tuple.Create(obj["next_openid"].ToString(),
                            openids,obj.Value<int>("total"),obj.Value<int>("count"));
                    }
                }
                catch (System.Exception ex)
                {
                    Logger.Error(ex);
                    return null;
                }
            }
            return null;
        }

        public static Guid? GetUserIDByUnionID(string unionid)
        {
            try
            {
                using (var client = new Tuhu.Service.UserAccount.UserAuthAccountClient())
                {
                    var result =
                         client.GetUserWithUinonId(unionid, Service.UserAccount.Enums.AuthChannelEnumIn.WeiXinOpen);
                    result.ThrowIfException(true);
                    if (result.Success && result.Result != null)
                    {
                        return result.Result.UserId;
                    }

                }
            }
            catch (System.Exception ex)
            {
                Logger.Error(ex);
            }
            return null;
        }
    }


}

