using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Common.Logging;
using Quartz;
using Tuhu.Service.Activity.Models.Requests;
using Tuhu.Service.Activity.Models.Response;
using Tuhu.C.Job.ActivityMonitor.BLL;

namespace Tuhu.C.Job.Activity.Job
{
    public class TuhuActivitySaleOutQuantityMonitorJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<TuhuActivitySaleOutQuantityMonitorJob>();

        public void Execute(IJobExecutionContext context)
        {
            var sw = new Stopwatch();
            sw.Start();
            Logger.Info("监控限时抢购销售数量job开始执行");
            try
            {
                FirstCheckWrongData();

                SecondCheckWrongData();
            }
            catch (Exception ex)
            {
                Logger.Error("监控限时抢购销售数量job", ex);
            }
            sw.Stop();
            Logger.Info($"监控限时抢购销售数量job执行完成,用时{sw.ElapsedMilliseconds}毫秒");
        }

        #region 检查错误数据，这两个有部分重复代码 todo 合并

        private Tuple<List<FlashSaleWrongCacheResponse>, List<FlashSaleWrongCacheResponse>, List<FlashSaleWrongCacheResponse>>
            GetResult(List<FlashSaleWrongCacheResponse> request)
        {
            var logResults = new List<FlashSaleWrongCacheResponse>();
            var dbResults = new List<FlashSaleWrongCacheResponse>();
            var cacheResults = new List<FlashSaleWrongCacheResponse>();
            foreach (var item in request)
            {
                var logResult = new FlashSaleWrongCacheResponse()
                {
                    ActiivtyId = item.ActiivtyId,
                    CacheProducts = new List<FlashSaleCacheProduct>()
                };
                var dbResult = new FlashSaleWrongCacheResponse()
                {
                    ActiivtyId = item.ActiivtyId,
                    CacheProducts = new List<FlashSaleCacheProduct>()
                };
                var cacheResult = new FlashSaleWrongCacheResponse()
                {
                    ActiivtyId = item.ActiivtyId,
                    CacheProducts = new List<FlashSaleCacheProduct>()
                };
                foreach (var product in item.CacheProducts)
                {
                    if (product.StrType == "ConfigInconsistent")
                    {
                        logResult.CacheProducts.Add(product);
                    }
                    if (product.StrType == "CacheInconsistent")
                    {
                        cacheResult.CacheProducts.Add(product);
                    }
                    if (product.StrType == "SaleOver")
                    {
                        dbResult.CacheProducts.Add(product);
                    }
                }
                if (logResult.CacheProducts != null && logResult.CacheProducts.Any())
                    logResults.Add(logResult);
                if (dbResult.CacheProducts != null && dbResult.CacheProducts.Any())
                    dbResults.Add(dbResult);
                if (cacheResult.CacheProducts != null && cacheResult.CacheProducts.Any())
                    cacheResults.Add(cacheResult);
            }
            return new Tuple<List<FlashSaleWrongCacheResponse>, List<FlashSaleWrongCacheResponse>, List<FlashSaleWrongCacheResponse>>(logResults, dbResults, cacheResults);
        }
        private void SecondCheckWrongData()
        {
            using (var client = new Service.Activity.FlashSaleClient())
            {
                var clientResult = client.SelectFlashSaleWrongCache();
                var result = clientResult.Result;
                if (result.Any())
                {
                    var getResult = GetResult(result);
                    var logResults = getResult.Item1;
                    var dbResults = getResult.Item2;
                    var cacheResults = getResult.Item3;
                    if (logResults.Any())
                    {
                        var htmlStringdb = GenerateHtmlString(logResults);
                        MailService.SendMail("配置表的销量跟历史纪录表不一致=》有问题活动产品2", "wanghaoxing@tuhu.cn", htmlStringdb);
                    }
                    if (cacheResults.Any())
                    {
                        var htmlStringcache = GenerateHtmlString(cacheResults);
                        MailService.SendMail("缓存跟数据库记录的销量不符=》有问题活动产品2", "wanghaoxing@tuhu.cn", htmlStringcache);
                    }
                    if (dbResults.Any())
                    {
                        var htmlStringdb = GenerateHtmlString(dbResults);
                        MailService.SendMail("配置的总量小于了已经售出的产品数量=》有问题活动产品2", "wanghaoxing@tuhu.cn", htmlStringdb);
                    }
                    else
                    {
                        Logger.Info("无有问题活动产品,故无需邮件发送");
                    }
                }
            }
        }

        private void FirstCheckWrongData()
        {
            using (var client = new Service.Activity.FlashSaleClient())
            {
                var clientResult = client.SelectFlashSaleWrongCache();
                var result = clientResult.Result;
                var getResult = GetResult(result);
                var logResults = getResult.Item1;
                var dbResults = getResult.Item2;
                var cacheResults = getResult.Item3;
                if (result.Any())
                {
                    if (logResults.Any())
                    {
                        var htmlStringdb = GenerateHtmlString(logResults);
                        MailService.SendMail("配置表的销量跟历史纪录表不一致=》有问题活动产品", "wanghaoxing@tuhu.cn", htmlStringdb);
                        var models = new List<UpdateQuantityProductModel>();
                        foreach (var log in logResults)
                        {
                            foreach (var cp in log.CacheProducts)
                            {
                                models.Add(new UpdateQuantityProductModel()
                                {
                                    ActivityId = new Guid(log.ActiivtyId),
                                    Pid = cp.Pid
                                });
                            }
                        }
                        var updateClient =
                            client.UpdateConfigSaleoutQuantityFromLog(new UpdateConfigSaleoutQuantityRequest()
                            {
                                RefreshType = RefreshType.RefreshByPid,
                                ProductModels = models
                            });
                        if (!updateClient.Success)
                        {
                            Logger.Error("更新限时抢购配置数据失败，请排查日志查找原因");
                        }
                    }
                    if (cacheResults.Any())
                    {
                        var htmlStringcache = GenerateHtmlString(cacheResults);
                        MailService.SendMail("缓存跟数据库记录的销量不符=》有问题活动产品", "wanghaoxing@tuhu.cn", htmlStringcache);
                        var cacheclient =
                            client.RefreshFlashSaleHashCount(
                                cacheResults.Select(r => r.ActiivtyId.ToString()).ToList(), false);
                        if (!cacheclient.Success)
                        {
                            Logger.Error("更新限时抢购缓存数据失败，请排查日志查找原因");
                        }
                    }
                    if (dbResults.Any())
                    {
                        var htmlStringdb = GenerateHtmlString(dbResults);
                        MailService.SendMail("配置的总量小于了已经售出的产品数量=》有问题活动产品", "wanghaoxing@tuhu.cn", htmlStringdb);
                    }
                    else
                    {
                        Logger.Info("无有问题活动产品,故无需邮件发送");
                    }
                }
            }
        }

        #endregion

        private string GenerateHtmlString(List<FlashSaleWrongCacheResponse> result)
        {
            StringBuilder sb = new StringBuilder();
            sb = sb.Append($@"<label> 共有{result.Count}个活动有问题，活动ID分别是{string.Join(";", result.Select(r => r.ActiivtyId))}</label> ");
            sb = sb.Append(@"<table border=1" + " align=center" + " width =1000 " + "><tr bgcolor='#4da6ff'><td align=center><b>活动Id</b></td><td align=center><b>PID</b></td> <td align=center> <b> 售出数量</b> </td> <td align=center> <b> 缓存数量</b> </td><td align=center> <b> 配置总库存</b> </td><td align=center> <b> 日志表销量记录</b> </td><td align=center> <b> 问题</b> </td> </tr>");
            foreach (var item in result)
            {
                foreach (var product in item.CacheProducts)
                {
                    sb.AppendFormat("<tr style=\"background-color:" + "red" + "\">\n");

                    sb.AppendFormat("<td style=\"text-align:center\">");
                    sb.AppendFormat(item.ActiivtyId);
                    sb.AppendFormat("</td>");
                    sb.AppendFormat("<td style=\"text-align:center\">");
                    sb.AppendFormat(product.Pid ?? "");
                    sb.AppendFormat("</td>");
                    sb.AppendFormat("<td style=\"text-align:center\">");
                    sb.AppendFormat(product.SaleOutQuantity.ToString() ?? "");
                    sb.AppendFormat("</td>");
                    sb.AppendFormat("<td style=\"text-align:center\">");
                    sb.AppendFormat(product.CacheQuantity.ToString() ?? "");
                    sb.AppendFormat("</td>");
                    sb.AppendFormat("<td style=\"text-align:center\">");
                    sb.AppendFormat(product.TotalQuantity.ToString() ?? "");
                    sb.AppendFormat("</td>");
                    sb.AppendFormat("<td style=\"text-align:center\">");
                    sb.AppendFormat(product.LogRecordQuantity.ToString() ?? "");
                    sb.AppendFormat("</td>");
                    sb.AppendFormat("<td style=\"text-align:center\">");
                    sb.AppendFormat(product.StrType.ToString() ?? "");
                    sb.AppendFormat("</td>");
                    sb.AppendFormat("</tr>\n");
                }

            }
            sb.AppendFormat("</table>");

            return sb.ToString();
        }
    }
}
