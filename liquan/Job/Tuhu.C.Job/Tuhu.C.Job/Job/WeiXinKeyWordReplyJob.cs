using Common.Logging;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.C.Job.DAL;

namespace Tuhu.C.Job.Job
{
    [DisallowConcurrentExecution]
    public class WeiXinKeyWordReplyJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(WeiXinKeyWordReplyJob));
        private  readonly int _platform = 9;
        Tuhu_log_tbl_WxUserMessageInfoDAL _Tuhu_log_tbl_WxUserMessageInfoDAL = new Tuhu_log_tbl_WxUserMessageInfoDAL(Logger);
        private const string URL_CUSTOM_SEND = "https://api.weixin.qq.com/cgi-bin/message/custom/send?";
        private const string replyConetent = "您好，欢迎登陆www.tuhu.cn，或致电途虎咨询热线400-111-8868，我们将竭诚为您服务";

        public async void Execute(IJobExecutionContext context)
        {
            Logger.Info("启动任务 WeiXinKeyWordReplyJob");
            try
            {
                #region 对于 符合 新增的关键字模板   服务号内  24小时内    未进行回复的信息 进行 回复
                //List<string> keywords = new List<string>() { "机油", "轮胎", "店" };
                //1.获取所有  符合 新增的关键字模板 的信息 根据 openID 进行分组 避免重复发送
                List<string> openids = _Tuhu_log_tbl_WxUserMessageInfoDAL.GetMessageOpenids("gh_155524e3c1f5", DateTime.Parse("2018-04-03 16:00:00"));

                //2.调用微信 客服接口 发送 消息
                foreach (var openid in openids)
                {
                    string jsonData = JsonConvert.SerializeObject(new
                    {
                        touser = openid,
                        msgtype = "text",
                        text =
                             new
                             {
                                 content = replyConetent
                             }
                    });
                    string msg = PushCustomMessage(jsonData).GetAwaiter().GetResult();
                }
                #endregion
            }
            catch (Exception ex)
            {
                Logger.Info(" WeiXinKeyWordReplyJob 异常 ex："+ ex.Message);
            }
            
        }

        #region 通过 openid 发送 信息

        public static async Task<string> ProxyHttpRequest(Func<HttpClient, Task<HttpResponseMessage>> func)
        {
            var result = string.Empty;
            using (var client = new HttpClient())
            {
                HttpResponseMessage response = null;
                var tryCount = 3;//错误尝试次数
                try
                {
                    do
                    {
                        try
                        {
                            response = await func(client);
                        }
                        catch
                        {
                            Thread.Sleep(100);
                        }
                    } while (response == null && --tryCount > 0);

                    if (response != null)
                    {
                        response.EnsureSuccessStatusCode();
                        result = await response.Content.ReadAsStringAsync();
                    }
                }
                catch (AggregateException ex)
                {
                    Logger.Error(ex);
                }
                finally
                {
                    if (response != null)
                    {
                        response.Dispose();
                    }
                }
                return result;
            }
        }

        public string GetWeiXinAccessToken()
        {
            using (var client = new Tuhu.Service.ThirdParty.WeiXinServiceClient())
            {
                var response = client.GetAccessTokenByPlatform(this._platform);
                //await  Task.Delay(10000);
                if (response.Success && response.Result != null)
                {
                    return response.Result.access_token;
                }
                return null;
            }
        }

        public async Task<string> PushCustomMessage(string jsonData)
        {
            var token = GetWeiXinAccessToken();
            //校验token是否合法或者有效
            //token = await CheckOrUpdateWeiXinAccess(token);

            var result = await ProxyHttpRequest(async (client) =>
                await client.PostAsync(new Uri($"{URL_CUSTOM_SEND}access_token={token}"), new StringContent(jsonData, Encoding.UTF8, "application/json")));
            //await Task.Delay(10000);
            Logger.Info(string.Format("openid={0} 执行结果=）{1}", jsonData, result));
            //if (!await CheckResultCode(result, token)) return null;
            return result;
        }
        #endregion

    }
}
