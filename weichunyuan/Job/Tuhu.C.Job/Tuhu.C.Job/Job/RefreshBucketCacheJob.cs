using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using Tuhu.Service.Shop;

namespace Tuhu.C.Job.Job
{
    [DisallowConcurrentExecution]
    public class RefreshBucketCacheJob : IJob
    {
        private static readonly ILog shopLog = LogManager.GetLogger(typeof(RefreshBucketCacheJob));
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                List<int> shopIds;
                using (var cmd = new SqlCommand(@"SELECT S.PKID FROM Shops AS S WITH(NOLOCK)"))
                {
                    shopIds = DbHelper.ExecuteQuery(true, cmd, (dt) =>
                    {
                        return
                            Enumerable.Select(dt.AsEnumerable(),
                                row => !row.IsNull("PKID") ? Convert.ToInt32(row.GetValue("PKID")) : -1)
                                .Where(shopId => shopId > 0)
                                .ToList();
                    });
                }
                shopLog.Info("开始刷新门店缓存");
                if (shopIds != null && shopIds.Any())
                {
                    var shopIdArry = shopIds.ToArray();
                    var batchNum = (shopIdArry.Length + 127) / 128;
                    for (var index = 0; index < batchNum; index++)
                    {
                        var batchArry = shopIdArry.Skip(index * 128).Take(128).ToArray();
                        foreach (var shopId in batchArry)
                        {
                            try
                            {
                                TuhuNotification.SendNotification("DelayedShopChange", new Dictionary<string, object>()
                                {
                                    ["ShopId"] = shopId
                                });
                            }
                            catch (Exception ex)
                            {
                                shopLog.Info($"向刷门店mq发消息失败，ShopId:{shopId}，ErrorMsg:{ex.Message}");
                            }                                            
                        }
                        using (var client=new ShopClient())
                        {
                            foreach (var shopId in batchArry)
                            {
                                client.SendBeautyShopChangeQueue(shopId, 0);
                            }
                        }
                        shopLog.Info("成功向MQ发送第"+index+"批shopId");
                        Thread.Sleep(5000);
                    }
                }


                List<int> regionIds = null;
                using (var cmd = new SqlCommand(@"
                     SELECT R1.PKID FROM Gungnir..tbl_region R1 (NOLOCK) WHERE ParentID=0 OR EXISTS(SELECT 1 FROM Gungnir..tbl_region R2 (NOLOCK) WHERE R2.ParentID=R1.PKID)"))
                {
                    regionIds = DbHelper.ExecuteQuery(true, cmd, (dt) =>
                    {
                        return
                            Enumerable.Select(dt.AsEnumerable(),
                                row => !row.IsNull("PKID") ? Convert.ToInt32(row.GetValue("PKID")) : -1)
                                .Where(regionId => regionId > 0)
                                .ToList();
                    });
                }
                shopLog.Info("开始刷新地区门店缓存");
                if (regionIds != null && regionIds.Any())
                {
                    using (var client = new ShopClient())
                    {
                        foreach (var regionId in regionIds)
                        {
                            client.UpdateShopDetailsByRegionId(regionId);
                            shopLog.Info("成功刷新地区id为"+regionId+"门店缓存");
                            Thread.Sleep(10);
                        }
                    }
                }
                shopLog.Info("地区门店缓存刷新成功");
            }
            catch (Exception ex)
            {
                shopLog.Error("刷门店缓存异常"+ex);
            }                                                                    
        }
    }
}


