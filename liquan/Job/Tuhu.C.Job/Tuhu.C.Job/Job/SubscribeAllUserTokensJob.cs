using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Tuhu.Service.Push.Models;
using Tuhu.Service.Push.Models.Push;

namespace Tuhu.C.Job.Job
{
    public class SubscribeAllUserTokensJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<SubscribeAllUserTokensJob>();

        public void Execute(IJobExecutionContext context)
        {
            string jobname = "SubscribeAllUserTokens";
            var checkresult = HandleExpiredMessageBoxJob.CheckIsOpenWithDescription(jobname);
            if (!checkresult.Item1)
            {
                Logger.Info("开关已关 return");
            }
            int count = 0;

            while (true)
            {
                count++;
                Logger.Info($"开始刷新第{count}批次");
                checkresult = HandleExpiredMessageBoxJob.CheckIsOpenWithDescription(jobname);
                if (!checkresult.Item1)
                {
                    Logger.Info("开关已关 return");
                }
                int maxpkid = 0;
                int.TryParse(checkresult.Item2, out maxpkid);
                var results = SelectDeviceInfos(maxpkid);
                var topicnames = SelectAllTopicNames();
                if (results != null && results.Any())
                {
                    var tokens = ParseTargets(results, 900);
                    Parallel.ForEach(tokens, new ParallelOptions()
                    {
                        MaxDegreeOfParallelism = 4,
                        TaskScheduler = TaskScheduler.Default
                    }, t =>
                    {
                        try
                        {
                            var iostokens = t.Where(x =>
                                string.Equals(x.Platform, "ios", StringComparison.OrdinalIgnoreCase));
                            var androidtokens = t.Where(x =>
                                 string.Equals(x.Platform, "android", StringComparison.OrdinalIgnoreCase));

                            foreach (var topicname in topicnames)
                            {
                                if (iostokens.Any())
                                {
                                    using (var client = new Tuhu.Service.Push.TemplatePushClient())
                                    {
                                        var result = client.SubscribeByRegids(iostokens.Select(x => x.Token), topicname, DeviceType.iOS);
                                        Logger.Info($"第{count}批次 ios结果{result.Result}");
                                    }
                                }
                                if (androidtokens.Any())
                                {
                                    using (var client = new Tuhu.Service.Push.TemplatePushClient())
                                    {
                                        var result = client.SubscribeByRegids(androidtokens.Select(x => x.Token), topicname, DeviceType.Android);
                                        Logger.Info($"第{count}批次 android结果{result.Result}");
                                    }
                                }
                            }

                        }
                        catch (System.Exception ex)
                        {
                            Logger.Warn("HandleUserMessageSettingJob ex=>" + ex);
                        }
                    });
                    HandleExpiredMessageBoxJob.UpdateRunTimeSwitchDescription(jobname, results.Max(x => x.PKID).ToString());
                    Logger.Info($"结束刷新第{count}批次");
                }
                else
                {
                    break;
                }
            }
            Logger.Info($"刷新结束");

        }

        public IEnumerable<string> SelectAllTopicNames()
        {
            //var props = typeof(UserMessageSetting).GetProperties().Where(x => x.PropertyType == typeof(bool?));
            //return props.Select(x => x.Name);
            using (var client = new Tuhu.Service.Push.MessageBoxPushClient())
            {
                var result = client.SelectAllMessageNavigationType();
                result.ThrowIfException(true);
                return result.Result.Select(x => x.PushAlias);
            }
        }
        public static IEnumerable<IPushDeviceInfo> SelectDeviceInfos(int maxpkid)
        {
            string sql =
                $"SELECT TOP 10000 * FROM Tuhu_profiles..Push_XiaoMiDeviceInfo WITH ( NOLOCK) WHERE PKID>{maxpkid} and vendor='xiaomi' ";

            DataTable table;
            using (var cmd = new SqlCommand(sql))
                table = DbHelper.ExecuteQuery(true, cmd, _ => _);

            if (table != null || table.Rows.Count > 0)
            {
                return table.AsEnumerable().AsParallel().Select(r => new IPushDeviceInfo()
                {
                    DeviceId = r["DeviceId"]?.ToString(),
                    Platform = r["Platform"]?.ToString(),
                    Token = r["Token"]?.ToString(),
                    UserId = r["UserId"]?.ToString(),
                    Vendor = r["Vendor"]?.ToString(),
                    PushVendor = r["PushVendor"]?.ToString(),
                    PKID = Convert.ToInt32(r["PKID"].ToString())
                });
            }
            else
            {
                return new List<IPushDeviceInfo>();
            }
        }
        public static IEnumerable<IEnumerable<T>> ParseTargets<T>(IEnumerable<T> targets, int maxcount)
        {
            if (targets != null && targets.Any())
            {
                int TotalCount = targets.Count();
                int pages = TotalCount / maxcount;
                for (int i = 0; i <= pages; i++)
                {
                    var temps = targets.Skip(i * maxcount).Take(maxcount);
                    yield return temps;
                }
            }
            else
            {
                yield break;
            }
        }
        public class IPushDeviceInfo : PushDeviceInfo
        {
            public int PKID { get; set; }
        }
    }
}
