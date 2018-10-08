using Common.Logging;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Tuhu.Nosql;
using Tuhu.Service.Push;
using Tuhu.Service.Push.Models.Push;
using Tuhu.Service.UserAccount;
using Tuhu.C.Job.Activity.Dal;

namespace Tuhu.C.Job.Activity.Job
{
    [DisallowConcurrentExecution]
    public class GroupsPacketsPushJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<GroupsPacketsPushJob>();
        private static readonly int MemberNumber = 4;

        public void Execute(IJobExecutionContext context)
        {
            var watcher = new Stopwatch();
            watcher.Start();
            Logger.Info("GroupsPacketsPushJob开始执行");
            try
            {
                DoJob();
            }
            catch (Exception ex)
            {
                Logger.Warn("GroupsPacketsPushJob出现异常", ex);
            }

            watcher.Stop();
            Logger.Info($"GroupsPacketsPushJob完成,用时{watcher.ElapsedMilliseconds}毫秒");
        }

        private void DoJob()
        {
            const int step = 1000;
            var span1 = TimeSpan.FromMinutes(190);
            var span2 = TimeSpan.FromMinutes(160);
            var index = 0;
            var groupCount = DalGroupPackets.GetGroupCount();
            Logger.Info($"当前未满员的团有{groupCount}个");
            if (groupCount == 0) return;
            while (index * step < groupCount)
            {
                var groupInfo = DalGroupPackets.GetGroupInfo(index, step);
                index++;
                if (!groupInfo.Any()) continue;
                foreach (var item in groupInfo)
                {
                    if (item.Item2 - DateTime.Now > span2 && item.Item2 - DateTime.Now < span1 &&
                        !GetPacketsPushCache(item.Item1))
                    {
                        var userList = DalGroupPackets.GetUserInfo(item.Item1);
                        Logger.Info($"团{item.Item1:D}三小时后过期，给对应用户推送消息");
                        if (!userList.Any() || userList.Count == MemberNumber) continue;
                        var leftNumber = MemberNumber - userList.Count;
                        var nickList = GetUserNickName(userList);
                        foreach (var userItem in userList)
                        {
                            var nickName = nickList.Where(h => h.Item1 == userItem).Select(g => g.Item2)
                                               .FirstOrDefault() ?? "";
                            PushMessage(item.Item1, nickName, leftNumber, userItem);
                        }

                        SetPacketsPushCache(item.Item1);
                    }
                }
            }
        }


        private void SetPacketsPushCache(Guid id)
        {
            using (var cacheClient = CacheHelper.CreateCacheClient("GroupsPacketsPushJob"))
            {
                var result = cacheClient.Set($"PacketsPushKey/{id:D}", true, TimeSpan.FromHours(1));
                if (!result.Success && result.Exception != null)
                {
                    string ex = result.Exception.Message;
                    Logger.Warn($"设置PacketsPushKey/{id:D}缓存失败：{ex}");
                }
            }
        }

        private bool GetPacketsPushCache(Guid id)
        {
            using (var cacheClient = CacheHelper.CreateCacheClient("GroupsPacketsPushJob"))
            {
                var result = cacheClient.Get<bool>($"PacketsPushKey/{id:D}");
                if (!result.Success && result.Exception != null)
                {
                    string ex = result.Exception.Message;
                    Logger.Warn($"获取PacketsPushKey/{id:D}缓存失败：{ex}");
                }

                return result.Value;
            }
        }

        private List<Tuple<Guid, string>> GetUserNickName(List<Guid> userList)
        {
            var result = new List<Tuple<Guid, string>>();
            using (var client = new UserAccountClient())
            {
                var searchResule = client.GetUsersByIds(userList);
                if (searchResule.Success && searchResule.Result.Any())
                {
                    foreach (var item in searchResule.Result)
                    {
                        result.Add(new Tuple<Guid, string>(item.UserId, item.Profile.NickName));
                    }
                }
                else
                {
                    Logger.Warn($"GetGroupBuyingUserName==>{string.Join(",", userList)}");
                }

                return result;
            }
        }


        private void PushMessage(Guid packetGroupNo, string nickName, int count, Guid userId)
        {
            var templateLog = new PushTemplateLog
            {
                Replacement = JsonConvert.SerializeObject(new Dictionary<string, string>
                {
                    {"{{leftnumber}}", count.ToString()},
                    {"{{packetgroupno}}", packetGroupNo.ToString("D")},
                    {"{{nickname}}", nickName}
                })
            };

            var client = new TemplatePushClient();
            int batchId = 1642;
            var target = userId.ToString("D");
            try
            {
                var result =
                    client.PushByUserIDAndBatchID(new List<string> {target.ToLower()}, batchId, templateLog);
                result.ThrowIfException(true);
                if (!(result.Success && result.Result))
                {
                    Logger.Warn(
                        $"向用户{JsonConvert.SerializeObject(target)}推送信息{JsonConvert.SerializeObject(templateLog)},推送失败",
                        result.Exception);
                }
            }
            catch (Exception ex)
            {
                Logger.Warn($"向用户{JsonConvert.SerializeObject(target)}推送信息{batchId}出现异常", ex);
            }
            finally
            {
                client.Dispose();
            }
        }
    }
}
