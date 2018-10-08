using Common.Logging;
using Quartz;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Tuhu.C.Job.Job
{
    [DisallowConcurrentExecution]
    public class WeChatActivitySystemJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(WeChatActivitySystemJob));
        private static readonly string Env = ConfigurationManager.AppSettings["Env"];

        public void Execute(IJobExecutionContext context)
        {
            Logger.Info("启动任务");
            AsyncHelper.RunSync(TaskMethod);
            Logger.Info("结束任务");
        }


        public async Task TaskMethod()
        {
            var uri = Env == "pro" ? "https://wx.tuhu.cn/Activity/ReloadEntity?id=" : "http://wx.tuhu.dev/Activity/ReloadEntity?id=";
            var sql = Env == "pro" ? "SELECT id FROM Activity.dbo.ActivityBuild (NOLOCK)  where id>1500 AND  EndDate IS NOT NULL AND EndDate>dateadd(MINUTE,-10,GETDATE())"
                : "SELECT id FROM Activity.dbo.ActivityBuild (NOLOCK)  where id in (15156,11129)";

            using (var cmd = new SqlCommand(sql))
            using (var httpClient = new HttpClient())
            {
                foreach (var id in DbHelper.ExecuteQuery(true, cmd, dt => dt.ToList<int>()))
                {
                    try
                    {
                        using (var response = await httpClient.GetAsync(uri + id))
                        {
                            if (response.StatusCode != HttpStatusCode.OK)
                                continue;

                            var result = await response.Content.ReadAsStringAsync();
                        }
                    }
                    catch(Exception ex)
                    {
                        Logger.Warn($"WeChatActivitySystemJob:{id}-{uri}", ex);
                        break;
                    }
                }
            }
        }
    }
}
