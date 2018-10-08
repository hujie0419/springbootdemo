using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.ThirdParty;
using Tuhu.C.Job.DailyShopSyncJob.DAL;
using Tuhu.C.Job.DailyShopSyncJob.Models;

namespace Tuhu.C.Job.DailyShopSyncJob
{
    [DisallowConcurrentExecution]
    public class TuhuDailyShopSyncJob: IJob
    {

        public static readonly ILog Logger = LogManager.GetLogger(typeof(TuhuDailyShopSyncJob));
        public void Execute(IJobExecutionContext context)
        {
            Logger.Info("TuhuDailyShopSyncJob开始执行");
            try
            {
                if (ShopChangeDAL.CheckSwitch("DailyAlipayShopSync"))
                {
                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();
                    bool result = SyncDailyShopChange();
                    stopWatch.Stop();
                    TimeSpan ts = stopWatch.Elapsed;
                    if (result)
                        Logger.Info($"TuhuDailyShopSyncJob执行成功");
                    else
                        Logger.Info($"TuhuDailyShopSyncJob执行失败");
                    Logger.Info(string.Format("耗时：{0}s", ts.TotalSeconds));
                }
                else {
                    Logger.Info($"TuhuDailyShopSyncJob已关闭");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"TuhuDailyShopSyncJob异常：{ex.ToString()}");
            }
        }

        public bool SyncDailyShopChange()
        {
            bool isupdated = true;
            try
            {
                List<ShopChange> shopChangeList = new List<ShopChange>();
                shopChangeList = ShopChangeDAL.GetYesterdayShopChangeList();
                if (shopChangeList != null && shopChangeList.Any())
                {
                    using (var client = new AliPayServiceClient())
                    {
                        foreach (var shopchange in shopChangeList.OrderBy(q => q.CreateDateTime))
                        {
                            if (shopchange.ShopIsInActive ||shopchange.ShopHoliday)
                            {
                                var deleteshopresponse = client.DeleteShopsFromCheZhuPlatform(new List<int> { shopchange.ShopId }, null);
                                if (deleteshopresponse != null && deleteshopresponse.Result != null)
                                {
                                    if (deleteshopresponse.Result.code == "10000")
                                    {
                                        Logger.Info($"删除门店{shopchange.ShopId}成功");
                                    }
                                    else
                                    {
                                        Logger.Info($"删除门店{shopchange.ShopId}失败,原因:{deleteshopresponse.Result.msg}");
                                    }
                                }

                            }


                            if (shopchange.ShopService)
                            {
                                client.DeleteShopsFromCheZhuPlatform(new List<int>() { shopchange.ShopId }, null);
                                client.InputShopsToCheZhuPlatform(new List<int>() { shopchange.ShopId }, null);
                                client.InputShopServicesToCheZhuPlatform(new List<int>() { shopchange.ShopId }, null);
                                Logger.Info($"门店{shopchange.ShopId}服务已更新");
                            }
                            else
                            {
                                if (shopchange.ShopDetail)
                                {
                                    client.DeleteShopsFromCheZhuPlatform(new List<int>() { shopchange.ShopId }, null);
                                    client.InputShopsToCheZhuPlatform(new List<int>() { shopchange.ShopId }, null);
                                    client.InputShopServicesToCheZhuPlatform(new List<int>() { shopchange.ShopId }, null);
                                    Logger.Info($"门店{shopchange.ShopId}已更新");
                                    //  client.InputShopServicesToCheZhuPlatform(new List<int>() { shopchange.ShopId }, null);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                isupdated = false;
                Logger.Error("SyncDailyShopChange异常", ex);
            }
            return isupdated;
        }
    }
}
