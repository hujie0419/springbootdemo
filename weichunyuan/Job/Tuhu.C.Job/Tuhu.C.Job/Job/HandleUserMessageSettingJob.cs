using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Tuhu.Service.Push.Models.MessageBox;
using Tuhu.Service.Push.Models.Push;

namespace Tuhu.C.Job.Job
{
    public class HandleUserMessageSettingJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<HandleUserMessageSettingJob>();
        public void Execute(IJobExecutionContext context)
        {
            string jobname = "handleusermessagesetting";
            int maxpkid = 0;
            var checkresult = HandleExpiredMessageBoxJob.CheckIsOpenWithDescription(jobname);
            if (!checkresult.Item1)
            {
                Logger.Info("开关已关 return");
            }

            var navigations = SelectAllMessageNavigationTypesAsync();
            int.TryParse(checkresult.Item2, out maxpkid);
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
                int.TryParse(checkresult.Item2, out maxpkid);
                var results = SelectUserMessageSetting(maxpkid);
                if (results != null && results.Any())
                {
                    Parallel.ForEach(results, new ParallelOptions() { MaxDegreeOfParallelism = 3 }, setting =>
                          {
                              try
                              {
                                  string topname = navigations.FirstOrDefault(x => x.PkId == setting.MessageNavigationType)?.PushAlias;
                                  if (!string.IsNullOrEmpty(topname))
                                  {
                                      using (var client = new Tuhu.Service.Push.MessageBoxPushClient())
                                      {
                                          var result = client.SubscribeUserMessageSwitchInfo(setting);
                                          result.ThrowIfException(true);
                                      }
                                  }
                              }
                              catch (System.Exception ex)
                              {
                                  Logger.Warn("HandleUserMessageSettingJob ex=>" + ex);
                              }
                          });
                    HandleExpiredMessageBoxJob.UpdateRunTimeSwitchDescription(jobname,
                        results.Max(x => x.PkId).ToString());
                    Logger.Info($"结束刷新第{count}批次");
                }
                else
                {
                    break;
                }
            }
            Logger.Info($"刷新结束");
        }

        private IEnumerable<UserMessageSwitchInfo> SelectUserMessageSetting(int maxpkid)
        {
            string sql =
                $" SELECT TOP 1000 * FROM Tuhu_notification..UserMessageSwitchInfo WITH ( NOLOCK) where pkid>={maxpkid} order by pkid";
            using (var helper = DbHelper.CreateLogDbHelper(true))
            {
                var result = helper.ExecuteSelect<UserMessageSwitchInfo>(sql);
                return result ?? new List<UserMessageSwitchInfo>();
            }
        }

        public static IEnumerable<MessageNavigationType> SelectAllMessageNavigationTypesAsync()
        {
            string sql = @"select * from Tuhu_notification..MessageNavigationType WITH(nolock) ";
            using (var helper = DbHelper.CreateLogDbHelper())
            {
                var result = helper.ExecuteSelect<MessageNavigationType>(sql);
                return result;
            }
        }
    }
}
