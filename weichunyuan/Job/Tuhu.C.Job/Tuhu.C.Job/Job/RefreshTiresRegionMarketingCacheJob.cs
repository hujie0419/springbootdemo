using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Service.Config;
using Tuhu.Service.Shop;
using Tuhu.Service.Shop.Models.Region;

namespace Tuhu.C.Job.Job
{
    [DisallowConcurrentExecution]
    public class RefreshTiresRegionMarketingCacheJob : IJob
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(RefreshTiresRegionMarketingCacheJob));

        public void Execute(IJobExecutionContext context)
        {
            List<MiniRegion> miniRegion = new List<MiniRegion>();
            try
            {
                logger.Info("开始刷新轮胎区域活动缓存");

                var allActivityId = GetAllActivityId();

                if (allActivityId != null && allActivityId.Any())
                {
                    using (var client = new RegionClient())
                    {
                        var data = client.GetAllMiniRegion();
                        data.ThrowIfException(true, "获取所有地区服务失败");
                        miniRegion = data.Result.ToList();
                    }

                    if (miniRegion != null && miniRegion.Any())
                    {
                        foreach (var activityId in allActivityId)
                        {
                            foreach (var region in miniRegion)
                            {
                                using (var client = new ConfigClient())
                                {
                                    var result = client.RefreshRegionMarketingCache(activityId, region.RegionId);
                                    result.ThrowIfException(true);
                                    if (!result.Result)
                                        logger.Error("刷新轮胎区域活动失败");
                                }
                            }
                        }
                    }
                    else
                    {
                        logger.Info("未获取到地区");
                    }
                }
                else
                {
                    logger.Info("没有已配置的活动");
                }
            }
            catch (Exception ex)
            {
                logger.Error("刷新轮胎区域活动缓存Job异常", ex);
            }
            logger.Info("刷新轮胎区域活动缓存任务完成");
        }

        public List<Guid> GetAllActivityId()
        {
            List<Guid> result = new List<Guid>();
            using (var cmd = new SqlCommand(@"SELECT  t.ActivityId
            FROM    ( SELECT    rmc.ActivityId ,
                                CASE WHEN rmc.StartTime IS NOT NULL THEN rmc.StartTime
                                     ELSE tfs.StartDateTime
                                END AS StartTime ,
                                CASE WHEN rmc.EndTime IS NOT NULL THEN rmc.EndTime
                                     ELSE tfs.EndDateTime
                                END AS EndTime
                      FROM      Configuration..RegionMarketingConfig AS rmc WITH ( NOLOCK )
                                JOIN Activity.dbo.tbl_FlashSale AS tfs WITH ( NOLOCK ) ON rmc.ActivityId = tfs.ActivityID
                    ) AS t
            WHERE   t.StartTime <= GETDATE()
                    AND t.EndTime >= GETDATE();"))
            {
                result = DbHelper.ExecuteQuery(true, cmd, (dt) =>
                {
                    return
                        Enumerable.Select(dt.AsEnumerable(),
                            row => !row.IsNull("ActivityId") ? Guid.Parse(row.GetValue("ActivityId")) : Guid.Empty)
                            .Where(x => x != Guid.Empty)
                            .ToList();
                });
            }
            return result;
        }
    }
}
