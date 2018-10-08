using Common.Logging;
using Quartz;
using System;
using System.Diagnostics;
using Tuhu.C.Job.Activity.Dal;
using Tuhu.Nosql;
using Tuhu.Service.PinTuan;
using Tuhu.Service.PinTuan.Models;

namespace Tuhu.C.Job.Activity.Job
{

    /// <summary>
    ///     拼团 - 订单JOB 
    /// </summary>
    [DisallowConcurrentExecution]
    public class GroupBuyingOrderJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<GroupBuyingOrderJob>();
        public void Execute(IJobExecutionContext context)
        {
            var watcher = new Stopwatch();
            watcher.Start();
            Logger.Info("拼团订单状态Job开始执行");
            try
            {
                DoJob();
            }
            catch (Exception ex)
            {
                Logger.Warn("拼团订单状态Job出现异常", ex);
            }
            watcher.Stop();
            Logger.Info($"拼团订单状态Job完成,用时{watcher.ElapsedMilliseconds}毫秒");
        }

        private void DoJob()
        {
            var data = DalGroupBuying.GetExpiringGroupInfo();
            Logger.Info($"当前有{data.Count}个团处于拼团中状态");
            var span1 = TimeSpan.FromMinutes(160);
            var span2 = TimeSpan.FromMinutes(190);
            var span3 = TimeSpan.FromMinutes(50);
            var span4 = TimeSpan.FromMinutes(80);
            foreach (var item in data)
            {
                if (item.EndTime > DateTime.Now + span1 && item.EndTime < DateTime.Now + span2 &&
                    !GetExpiringCache(item.GroupId, 3))
                {
                    Logger.Info($"团号为{item.GroupId:D}拼团将在三个小时候结束，为相关用户推送提醒");
                    PushMessage(item.GroupId, 1651, 3);
                    SetExpiringCache(item.GroupId, 3);
                }

                else if (item.EndTime > DateTime.Now + span3 && item.EndTime < DateTime.Now + span4 &&
                         !GetExpiringCache(item.GroupId, 1))
                {
                    Logger.Info($"团号为{item.GroupId:D}拼团将在一个小时候结束，为相关用户推送提醒");
                    PushMessage(item.GroupId, 1651, 1);
                    SetExpiringCache(item.GroupId, 1);
                }

                else if (item.EndTime < DateTime.Now)
                {
                    using (var client = new PinTuanClient())
                    {
                        var pinTuanAutoFinishResult = client.PinTuanAutoFinish(new PinTuanAutoFinishRequest()
                        {
                            GroupId = item.GroupId
                        });
                        // 判断返回值
                        if (!pinTuanAutoFinishResult.Result)
                        {
                            var result = client.ExpireGroupBuying(item.GroupId);
                            if (result.Success && result.Result.Code == 1)
                            {
                                Logger.Info($"团{item.GroupId}过期，取消成功");
                            }
                            else
                            {
                                Logger.Warn($"团{item.GroupId}过期,取消失败，{result.Exception?.Message},{result.Result?.Info}");
                            }
                        }
                        else
                        {
                            Logger.Info($" GroupBuyingOrderJob -> DoJob -> {item.GroupId} auto finished ");
                        }
                    }
                }
            }
        }

        private async void PushMessage(Guid groupId, int batchId, int flag)
        {
            using (var client = new PinTuanClient())
            {
                var result = await client.PushPinTuanMessageAsync(groupId, batchId);
                if (!result.Success && result.Result)
                {
                    Logger.Warn(
                        $"GroupBuyingPushMessage==>{flag}小时推送==>fail==>{groupId}/{batchId}==>{result.Exception}");
                }
            }
        }

        private bool GetExpiringCache(Guid groupId, int flag)
        {
            using (var client = CacheHelper.CreateCacheClient("GroupBuyingMessageName"))
            {
                var result = client.Get<bool>($"GroupBuyingKey/{flag}/{groupId}");
                if (!result.Success && result.Exception != null)
                {
                    string ex = result.Exception.Message;
                    Logger.Warn($"获取GroupBuyingKey/{groupId}缓存失败：{ex}");
                }

                return result.Value;
            }
        }

        private void SetExpiringCache(Guid groupId, int flag)
        {
            using (var client = CacheHelper.CreateCacheClient("GroupBuyingMessageName"))
            {
                var result = client.Set($"GroupBuyingKey/{flag}/{groupId}", true, TimeSpan.FromMinutes(60));
                if (!result.Success && result.Exception != null)
                {
                    string ex = result.Exception.Message;
                    Logger.Warn($"设置GroupBuyingKey/{groupId}缓存失败：{ex}");
                }
            }
        }

    }
}
