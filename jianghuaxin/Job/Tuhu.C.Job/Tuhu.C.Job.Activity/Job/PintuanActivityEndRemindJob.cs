using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Tuhu.C.Job.Activity.Dal;
using Tuhu.C.Job.Activity.Models;
using Tuhu.Nosql;
using Tuhu.Service.Product;

namespace Tuhu.C.Job.Activity.Job
{
    public class PintuanActivityEndRemindJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<PintuanActivityEndRemindJob>();
        private const string PreGroupBuyingInfoCacheName = "preGroupBuyingInfo";

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                Logger.Info("拼团活动过期提醒Job开始执行");
                stopwatch.Start();
                Exec();
                stopwatch.Stop();
                Logger.Info($"拼团活动过期提醒Job执行结束，耗时{stopwatch.ElapsedMilliseconds}ms");
            }
            catch (Exception ex)
            {
                Logger.ErrorException("拼团活动过期提醒Job执行发生异常", ex);
            }
        }

        private static void Exec()
        {
            // 1. GetNeedSendEmailMetadata
            var expiredInfo = DalGroupBuying.GetYiwuStockOutGroupBuyingInfo().ToList();
            expiredInfo.AddRange(DalGroupBuying.GetExpiredGroupBuyingInfo());
            expiredInfo.AddRange(GetNotExpiredRemindGroupBuyingInfos());

            // 2. SendEmail
            SendEmail(expiredInfo.Distinct(new ExpiredGroupBuyingInfoEqualityComparer()).ToArray());
        }

        /// <summary>
        /// 获取未过期要提醒的团信息
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<ExpiredGroupBuyingInfo> GetNotExpiredRemindGroupBuyingInfos()
        {
            var preList = new List<ExpiredGroupBuyingInfo>();
            var list = DalGroupBuying.GetActiveGroupBuyingInfo().ToList();
            using (var client = new ProductClient())
            {
                var result = client.SelectSkuProductListByPids(list.Select(_ => _.PID).Distinct().ToList());
                if (result.Success && result.Result.Any())
                {
                    foreach (var info in list)
                    {
                        info.IsActive = result.Result.FirstOrDefault(_ => _.Pid.Equals(info.PID))?.Onsale ?? false;
                    }
                }
            }
            using (var cacheClient = CacheHelper.CreateCacheClient())
            {
                var result = cacheClient.Get<List<ExpiredGroupBuyingInfo>>(PreGroupBuyingInfoCacheName);
                if (result.Success && result.Value?.FirstOrDefault() != null)
                {
                    preList.AddRange(result.Value);
                }
                else
                {
                    Logger.Warn($"获取缓存失败，{result.Message}", result.Exception);
                }
                cacheClient.Set(PreGroupBuyingInfoCacheName, list, TimeSpan.FromHours(24.2));
            }

            var infos = new List<ExpiredGroupBuyingInfo>();
            // 库存为0的情况
            foreach (var info in list.Where(_ => _.CurrentSoldCount >= _.TotalStockCount))
            {
                if (preList.Any(_ => _.TotalStockCount > _.CurrentSoldCount && _.ProductGroupId.Equals(info.ProductGroupId) && _.PID.Equals(info.PID)))
                {
                    info.TriggerType = "商品库存不足";
                    infos.Add(info);
                }
            }
            // 产品库商品下架
            foreach (var info in list.Where(_ => !_.IsActive))
            {
                if (preList.Any(_ => _.IsActive && _.ProductGroupId.Equals(info.ProductGroupId) && _.PID.Equals(info.PID)))
                {
                    info.TriggerType = "产品库商品下架";
                    infos.Add(info);
                }
            }
            return infos;
        }

        /// <summary>
        /// 发送邮件提醒
        /// </summary>
        /// <param name="expiredInfo">要提醒的活动信息</param>
        private static void SendEmail(IReadOnlyCollection<ExpiredGroupBuyingInfo> expiredInfo)
        {
            if (expiredInfo == null || expiredInfo.Count <= 0)
            {
                Logger.Info("没有数据，无需发送提醒邮件");
                return;
            }
            var htmlBuilder = new StringBuilder();
            htmlBuilder.AppendLine(@"
<table style=""font-size:14px;border:1px solid rgb(0, 92, 182);border-collapse:collapse;"">
    <thead  style=""background-color:rgb(102, 171, 239);"">
        <tr>
            <td style=""border:1px solid rgb(0, 92, 182);padding: 8px;"">GroupId</td>
            <td style=""border:1px solid rgb(0, 92, 182);padding: 8px;"">PID</td>
            <td style=""border:1px solid rgb(0, 92, 182);padding: 8px;"">商品名称</td>
            <td style=""border:1px solid rgb(0, 92, 182);padding: 8px;"">商品售价</td>
            <td style=""border:1px solid rgb(0, 92, 182);padding: 8px;"">商品活动价</td>
            <td style=""border:1px solid rgb(0, 92, 182);padding: 8px;"">商品团长价</td>
            <td style=""border:1px solid rgb(0, 92, 182);padding: 8px;"">标签</td>
            <td style=""border:1px solid rgb(0, 92, 182);padding: 8px;"">创建人</td>
            <td style=""border:1px solid rgb(0, 92, 182);padding: 8px;"">活动开始时间</td>
            <td style=""border:1px solid rgb(0, 92, 182);padding: 8px;"">活动结束时间</td>
            <td style=""border:1px solid rgb(0, 92, 182);padding: 8px;"">触发条件</td>
        </tr>
    </thead>
    <tbody>");
            foreach (var info in expiredInfo)
            {
                htmlBuilder.AppendLine("\t\t<tr>");
                htmlBuilder.AppendLine($"\t\t\t<td style=\"border:1px solid rgb(0, 92, 182);padding: 8px;\">{info.ProductGroupId}</td>");
                htmlBuilder.AppendLine($"\t\t\t<td style=\"border:1px solid rgb(0, 92, 182);padding: 8px;\">{info.PID}</td>");
                htmlBuilder.AppendLine($"\t\t\t<td style=\"border:1px solid rgb(0, 92, 182);padding: 8px;\">{info.ProductName}</td>");
                htmlBuilder.AppendLine($"\t\t\t<td style=\"border:1px solid rgb(0, 92, 182);padding: 8px;\">{info.OriginalPrice:0.00}</td>");
                htmlBuilder.AppendLine($"\t\t\t<td style=\"border:1px solid rgb(0, 92, 182);padding: 8px;\">{info.FinalPrice:0.00}</td>");
                htmlBuilder.AppendLine($"\t\t\t<td style=\"border:1px solid rgb(0, 92, 182);padding: 8px;\">{info.SpecialPrice:0.00}</td>");
                htmlBuilder.AppendLine($"\t\t\t<td style=\"border:1px solid rgb(0, 92, 182);padding: 8px;\">{string.Join(", ", info.Label?.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries) ?? Enumerable.Empty<string>())}</td>");
                htmlBuilder.AppendLine($"\t\t\t<td style=\"border:1px solid rgb(0, 92, 182);padding: 8px;\">{info.Creator}</td>");
                htmlBuilder.AppendLine($"\t\t\t<td style=\"border:1px solid rgb(0, 92, 182);padding: 8px;\">{info.BeginTime:yyyy-MM-dd HH:mm:ss}</td>");
                htmlBuilder.AppendLine($"\t\t\t<td style=\"border:1px solid rgb(0, 92, 182);padding: 8px;\">{info.EndTime:yyyy-MM-dd HH:mm:ss}</td>");
                htmlBuilder.AppendLine($"\t\t\t<td  style=\"border:1px solid rgb(0, 92, 182);padding: 8px;\">{info.TriggerType}</td>");
                htmlBuilder.AppendLine("\t\t</tr>");
            }
            htmlBuilder.AppendLine("\t</tbody>");
            htmlBuilder.AppendLine("</table>");
#if DEBUG
            TuhuMessage.SendEmail($"{DateTime.Today:yyyy-MM-dd}拼团活动结束提醒", "李卫涵<liweihan@tuhu.cn>;", htmlBuilder.ToString());
#else
            TuhuMessage.SendEmail($"{DateTime.Today:yyyy-MM-dd}拼团活动结束提醒",
                System.Configuration.ConfigurationManager.AppSettings["PintuanActivityEndRemindJob:To"],
                htmlBuilder.ToString());
#endif
            Logger.Info("邮件提醒已发送");
        }
    }
}
