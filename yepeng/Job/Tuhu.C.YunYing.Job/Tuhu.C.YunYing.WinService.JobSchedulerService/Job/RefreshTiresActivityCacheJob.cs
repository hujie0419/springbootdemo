using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Activity;
using Tuhu.Service.Config;
using Tuhu.Service.Shop;
using Tuhu.Service.Shop.Models.Region;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.Job
{
    [DisallowConcurrentExecution]
    public class RefreshTiresActivityCacheJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(RefreshTiresActivityCacheJob));

        public void Execute(IJobExecutionContext context)
        {
            List<MiniRegion> miniRegion = new List<MiniRegion>();
            Logger.Info("开始刷新轮胎楼层活动缓存-2");

            try
            {
                var allActivityId = GetAllActivityId();
                if (allActivityId != null && allActivityId.Any())
                {
                    using (var client = new RegionClient())
                    {
                        var data = client.GetAllMiniRegion();
                        data.ThrowIfException(true, "获取所有地区服务失败-2");
                        miniRegion = data.Result.ToList();
                    }
                    var municipalities = new List<string>() { "上海市", "北京市", "重庆市", "天津市" };
                    if (miniRegion != null && miniRegion.Any())
                    {
                        foreach (var activityId in allActivityId)
                        {
                            foreach (var region in miniRegion)
                            {
                                if (municipalities.Contains(region.RegionName))
                                    RefreshRegionTiresActivityCache(activityId, region.RegionId);
                                if (!municipalities.Contains(region.RegionName))
                                {
                                    foreach (var child in region.ChildRegions)
                                    {
                                        RefreshRegionTiresActivityCache(activityId, child.RegionId);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Logger.Info("未获取到地区-2");
                    }
                }
                else
                {
                    Logger.Info("没有已配置的活动-2");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("刷新轮胎楼层活动缓存Job异常-2", ex);
            }
            Logger.Info("刷新轮胎楼层活动缓存完成-2");
        }

        public bool RefreshRegionTiresActivityCache(Guid activityId, int regionId)
        {
            var result = false;

            try
            {
                using (var client = new ActivityClient())
                {
                    var getResult = client.RefreshRegionTiresActivityCache(activityId, regionId);
                    getResult.ThrowIfException(true);
                    result = getResult.Result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("轮胎区域活动刷新缓存接口异常-2", ex);
            }

            return result;
        }
        public List<Guid> GetAllActivityId()
        {
            List<Guid> result = new List<Guid>();
            using (var cmd = new SqlCommand(@" SELECT ActivityId FROM   Configuration..TiresActivityConfig WITH ( NOLOCK )  WHERE StartTime<=GETDATE() AND EndTime >=GETDATE();"))
            {
                var dt = Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(true, cmd);
                result = dt.Rows.OfType<DataRow>().Select(x => Guid.Parse(x["ActivityId"].ToString())).ToList();
            }
            return result;
        }
    }
}
