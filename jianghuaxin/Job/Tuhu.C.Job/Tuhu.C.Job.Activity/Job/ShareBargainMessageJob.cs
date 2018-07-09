using Common.Logging;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Nosql;
using Tuhu.Service.Push;
using Tuhu.Service.Push.Models.Push;
using Tuhu.C.Job.Activity.Dal;
using Tuhu.C.Job.Activity.Models;
using Tuhu.Service.UserAccount;

namespace Tuhu.C.Job.Activity.Job
{
    [DisallowConcurrentExecution]
    public class ShareBargainMessageJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<ShareBargainMessageJob>();
        public void Execute(IJobExecutionContext context)
        {
            var watcher = new Stopwatch();
            Logger.Info("砍价消息推送Job开始执行");
            try
            {
                DoJob();
            }
            catch(Exception ex)
            {
                Logger.Warn("砍价消息推送Job执行出现异常", ex);
            }
            watcher.Stop();
            Logger.Info($"砍价消息推送Job执行完成,用时{watcher.ElapsedMilliseconds}毫秒");
        }

        private void DoJob()
        {
            var dat = DalShareBargain.GetAllShareBargain();
            Logger.Info($"待选择数据{dat.Count()}条");
            var time = DateTime.Now;
            var span = TimeSpan.FromMinutes(30);
            var span2 = TimeSpan.FromMinutes(120);
            foreach (var item in dat)
            {
                int batchId = 0;
                if ((!item.IsOver) && (item.EndDateTime < time) && (time - item.EndDateTime <= span))
                {
                    batchId = 634;
                }
                else if ((!item.IsOver) && (item.EndDateTime - time < span2) && (item.EndDateTime - time > span2 - span))
                {
                    batchId = 632;
                }
                else if (item.IsOver && (item.EndDateTime + TimeSpan.FromDays(1) - time < span2 && (item.EndDateTime + TimeSpan.FromDays(1) - time > span2 - span)))
                {
                    batchId = 636;
                }
                if (batchId != 0 && !GetCache(item.IdKey, batchId))
                {
                    Logger.Info($"用户{item.OwnerId},产品名称{item.ProductName},IdKey是{item.IdKey},模板Id是{batchId}");
                    PushMessage(item.OwnerId, item.IdKey, batchId, item.ProductName, item.FinalPrice, item.PID, item.ActivityProductId, item.SimpleDisplayName, item.OriginalPrice);
                    SetCache(item.IdKey, batchId);
                }
            }
        }

        private void PushMessage(Guid userId,Guid idKey,int batchId,string ProductName,decimal FinalPrice,string pid,int apId,string simpleDisplayName,decimal originalPrice)
        {
            var target = new List<string>()
                {
                    userId.ToString("D")
                };
            var nickName = "";
            using (var client = new UserAccountClient())
            {
                var searchResule = client.GetUserById(userId);
                if (searchResule.Success)
                {
                    nickName = searchResule.Result?.Profile?.NickName ?? "";
                }
            }
            using (var client = new TemplatePushClient())
            {
                var result = client.PushByUserIDAndBatchID(target, batchId, new PushTemplateLog()
                {
                    Replacement = JsonConvert.SerializeObject(new Dictionary<string, string>()
                    {
                        ["{{IdKey}}"] = idKey.ToString("D"),
                        ["{{Pid}}"] = pid,
                        ["{{AcitvityProductId}}"] = apId.ToString(),
                        ["{{ProductName}}"] = ProductName,
                        ["{{NickName}}"] = nickName,
                        ["{{ProductBriefName}}"] = simpleDisplayName,
                        ["{{Price}}"] = originalPrice.ToString("#0.00"),
                        ["{{ActivityPrice}}"] = FinalPrice.ToString("#0.00")
                    }),
                });
                if (result.Success && result.Result)
                {
                    Logger.Info($"砍价消息推送成功,用户{userId},产品名称{ProductName},IdKey是{idKey},模板Id是{batchId}");
                }
                else
                {
                    Logger.Warn($"砍价消息推送失败,用户{userId},产品名称{ProductName},IdKey是{idKey},模板Id是{batchId}");
                }
            }
        }
        private void SetCache(Guid idKey,int batchId)
        {
            using (var client = CacheHelper.CreateCacheClient("ShareBargainMessageName"))
            {
                var result = client.Set($"ShareBargainKey/{idKey}/{batchId}", true, TimeSpan.FromMinutes(60));
                if (!result.Success && result.Exception != null)
                {
                    string ex = result.Exception.Message;
                    Logger.Warn($"设置ShareBargainKey/{idKey}/{batchId}缓存失败：{ex}");
                }
            }
        }

        private bool GetCache(Guid idKey, int batchId)
        {
            //Logger.Info($"{idKey}/{batchId}查询缓存");
            using (var client = CacheHelper.CreateCacheClient("ShareBargainMessageName"))
            {
                var result = client.Get<bool>($"ShareBargainKey/{idKey}/{batchId}");
                if (!result.Success && result.Exception != null)
                {
                    string ex = result.Exception.Message;
                    Logger.Warn($"获取ShareBargainKey/{idKey}/{batchId}缓存失败：{ex}");
                }
                return result.Value;
            }
        }
    }
}
