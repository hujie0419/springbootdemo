using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Tuhu.Service.Push;
using Tuhu.Service.Push.Models.Push;
using Tuhu.C.Job.Activity.Dal;
using Tuhu.Service.UserAccount;

namespace Tuhu.C.Job.Activity.Job
{
    [DisallowConcurrentExecution]
    public class GroupBuyingFreeCouponPushMessageJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<GroupBuyingFreeCouponPushMessageJob>();

        public void Execute(IJobExecutionContext context)
        {
            var watcher = new Stopwatch();
            watcher.Start();
            Logger.Info("拼团免单券提示推送Job开始执行");
            try
            {
                DoJob();
            }
            catch (Exception ex)
            {
                Logger.Warn("拼团免单券提示推送Job出现异常", ex);
            }

            watcher.Stop();
            Logger.Info($"拼团免单券提示推送Job完成,用时{watcher.ElapsedMilliseconds}毫秒");
        }

        private void DoJob()
        {
            var startTime = DateTime.Now.Date.AddDays(1);
            var endTime = startTime.AddDays(1);
            var count = DalGroupBuying.GetExpiringFreeCouponCount(startTime, endTime);
            if (count == 0)
            {
                Logger.Warn($"待过期的拼团免单券数量为{count}");
                return;
            }

            const int step = 1000;
            var start = 0;
            while (start < count)
            {
                var userList = DalGroupBuying.GetExpiringFreeCouponList(startTime, endTime, start, step);
                try
                {
                    PushMessage(2195, userList);
                    Logger.Info($"待过期拼团免单券推送成功，第{start / step + 1}批，共{count / step + 1}批");
                }
                catch (Exception ex)
                {
                    Logger.Warn(
                        $"待过期拼团免单券推送失败，第{start / step + 1}批，共{count / step + 1}批,{ex.Message}/{ex.InnerException}");
                }

                start += step;
            }
        }

        private void PushMessage(int batchId, List<Guid> userList)
        {
            //var target = userList.Select(g => g.ToString("D")).ToList();
            var nickNameList = GetGroupBuyingUserName(userList);
            if (nickNameList.Any())
            {
                using (var client = new TemplatePushClient())
                {
                    foreach (var item in nickNameList)
                    {
                        var target = new List<string> {item.Item1.ToString("D")};
                        var nickName = item.Item2;
                        var result =
                            client.PushByUserIDAndBatchID(target, batchId, new PushTemplateLog
                            {
                                Replacement = JsonConvert.SerializeObject(new Dictionary<string, string>
                                {
                                    {"{{nickname}}", nickName}
                                })
                            });
                        if (!(result.Success && result.Result))
                        {
                            Logger.Warn(
                                $"向{item.Item1:d}/{item.Item2}用户推送信息{batchId}失败-->{result.Exception}");
                        }
                    }
                }
            }
        }

        private static List<Tuple<Guid, string>> GetGroupBuyingUserName(List<Guid> userLists)
        {
            var result = new List<Tuple<Guid, string>>();
            using (var client = new UserAccountClient())
            {
                var searchResule = client.GetUsersByIds(userLists);
                if (searchResule.Success && searchResule.Result.Any())
                {
                    foreach (var item in searchResule.Result)
                    {
                        result.Add(new Tuple<Guid, string>(item.UserId, item.Profile.NickName));
                    }
                }
                else
                {
                    Logger.Warn($"GetGroupBuyingUserName==>{string.Join(",", userLists)}");
                }

                return result;
            }
        }

    }
}
