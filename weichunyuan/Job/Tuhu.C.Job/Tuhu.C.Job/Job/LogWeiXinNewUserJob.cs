using Common.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Push.Models.WeiXinPush;

namespace Tuhu.C.Job.Job
{
    [DisallowConcurrentExecution]
    public class LogWeiXinNewUserJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<LogWeiXinNewUserJob>();
        public void Execute(IJobExecutionContext context)
        {
            Logger.Info("开始记录微信openid");
            var platform = 0;
            var tempplatform = GetSyncWxUserPlatform();
            if (!tempplatform.Item1)
            {
                Logger.Info($"开关已关");
                return;
            }
            if (!int.TryParse(tempplatform.Item2, out platform))
            {
                Logger.Warn($"{tempplatform}--platform非法");
                return;
            }
            try
            {
                var wxconfigs = SelectWxConfigs();
                var syncconfig = wxconfigs.FirstOrDefault(x => x.platform == platform);
                if (syncconfig == null)
                {
                    Logger.Warn($"{tempplatform}--配置未找到");
                    return;
                }
                var maxwxopenid = GetMaxJobOpenId(syncconfig.platform);
                var openids = GetWeiXinOpenID(syncconfig.channel, maxwxopenid);
                int count = 0;
                if (openids != null && openids.Item2 != null && openids.Item2.Any())
                {
                    Logger.Info($"{syncconfig.channel}记录微信openid,获得{openids.Item2.Count()}个");
                    bool iserror = false;
                    foreach (var openid in openids.Item2)
                    {
                        try
                        {
                            count += 1;
                            if (count % 100 == 0)
                            {
                                tempplatform = GetSyncWxUserPlatform();
                                if (!tempplatform.Item1)
                                {
                                    Logger.Info($"开关已关");
                                    return;
                                }
                            }
                            using (var client = new Tuhu.Service.Push.WeiXinPushClient())
                            {
                                var result = client.LogWxUserOpenIDWithChannel(openid, true, syncconfig.channel);
                                result.ThrowIfException(true);
                            }
                        }
                        catch (System.Exception ex)
                        {
                            iserror = true;
                            Logger.Warn($"同步微信用户ex:{ex}");
                        }
                    }
                    UpdateMaxJobOpenId(syncconfig.platform, openids.Item1);
                    if (!iserror)
                    {
                        Logger.Info($"成功结束同步{syncconfig.channel}."); 
                    }
                    Logger.Info($"结束同步{syncconfig.channel}.");
                }
            }
            catch (System.Exception ex)
            {
                Logger.Warn(ex);
            }
        }
        public static IEnumerable<WxConfig> SelectWxConfigs()
        {
            using (var client = new Tuhu.Service.Push.WeiXinPushClient())
            {
                var result = client.SelectWxConfigs();
                return result.Result;
            }
        }
        public static string GetWeiXinAccessToken(string channel)
        {
            using (var client = new Tuhu.Service.Push.WeiXinPushClient())
            {
                var result = client.SelectWxAccessToken(channel);
                result.ThrowIfException(true);
                return result.Result;
            }

        }
        public static bool UpdateMaxJobOpenId(int platform, string description)
        {
            string name = "logwxuserjob" + platform;
            string sql = $" update Gungnir..RuntimeSwitch with(rowlock) set description=N'{description}' where SwitchName=N'{name}' ";
            using (var helper = DbHelper.CreateDbHelper(false))
            {
                var result = helper.ExecuteNonQuery(sql);
                return result > 0;
            }
        }
        public static Tuple<string, IEnumerable<string>, int, int> GetWeiXinOpenID(string channel, string nextopenid = null)
        {
            string token = GetWeiXinAccessToken(channel);
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
                            openids, obj.Value<int>("total"), obj.Value<int>("count"));
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

        public static Tuple<bool, string> GetSyncWxUserPlatform()
        {
            var result = CheckIsOpenWithDescription("logwxuserjob");
            return result;
        }

        public static string GetMaxJobOpenId(int platform)
        {
            var result = CheckIsOpenWithDescription("logwxuserjob" + platform);
            return result?.Item2;
        }
        public static Tuple<bool, string> CheckIsOpenWithDescription(string name)
        {
            string sql = $"SELECT Value,Description FROM Gungnir..RuntimeSwitch WITH ( NOLOCK) WHERE SwitchName = N'{name}'";
            using (var helper = DbHelper.CreateDbHelper(true))
            using (var cmd = new SqlCommand(sql))
            {
                var result = helper.ExecuteQuery(cmd, dt =>
                {
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        var row = dt.Rows[0];
                        var isopen = string.Equals("true", row[0]?.ToString(), StringComparison.OrdinalIgnoreCase);
                        return Tuple.Create(isopen, row[1]?.ToString());
                    }
                    return Tuple.Create(false, "");
                });
                return result;
            }
        }
    }
}
