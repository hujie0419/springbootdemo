using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Nosql;
using Tuhu.Service.Activity.Server.Manager.FlashSaleSystem;
using Tuhu.Service.BaoYang;
using Tuhu.Service.BaoYang.Models.Activity;
using FixedPriceActivityRoundResponse = Tuhu.Service.Activity.Models.FixedPriceActivityRoundResponse;

namespace Tuhu.Service.Activity.Server.FlashSaleSystem
{
    public class ActivityCache
    {
        private static Nosql.CacheClient GetCacheCliet()
        {
            return CacheHelper.CreateCacheClient("Activity", 10);
        }

        public static async Task<bool> UpdateBaoYangActivityConfig(Guid activityId)
        {
            bool result = false;
            const string ActivityKey = "Activitiy";
            using (var client = new BaoYangClient())
            {
                var serviceResult = await client.GetFixedPriceActivityConfigByActivityIdAsync(activityId);

                if (serviceResult.Success)
                {
                    var cacheResult = await GetCacheCliet().SetAsync($"{ActivityKey}/{activityId}", serviceResult.Result);
                    result = cacheResult.Success;
                }
            }
            return result;           
        }

        public static async Task<FixedPriceActivityConfig> GetBaoYangActivityConfig(Guid activityId)
        {
            const string ActivityKey = "Activitiy";

            var cacheResult = await GetCacheCliet().GetOrSetAsync($"{ActivityKey}/{activityId}", async () =>
            {
                using (var client = new BaoYangClient())
                {
                    var serviceResult = await client.GetFixedPriceActivityConfigByActivityIdAsync(activityId);

                    if (serviceResult.Success)
                    {
                        return serviceResult.Result;
                    }
                    else
                    {
                        throw serviceResult.Exception;
                    }
                }
            }, TimeSpan.FromMinutes(10));

            if (cacheResult.Success)
            {
                return cacheResult.Value;
            }
            else
            {
                throw cacheResult.Exception;
            }
        }

        public static async Task<bool> UpdateBaoYangPurchaseCount(Guid activityId)
        {
            bool success = false;
            var activityConfig = await GetBaoYangActivityConfig(activityId);
            var round = ActivityValidator.GetCurrentRoundConfig(activityConfig, DateTime.Now);
            if (round != null)
            {
                var counter = new BaoYangCounter(activityId, round.LimitedQuantity, activityConfig.ItemQuantityPerUser, round);
                success = await counter.ResetPurchaseCount();
            }
            return success;
        }

        public static async Task<FixedPriceActivityRoundResponse> GetFixedPriceActivityRoundCache(Guid activityId)
        {
            FixedPriceActivityRoundResponse result = null;
            var activityConfig = await GetBaoYangActivityConfig(activityId);
            var round = ActivityValidator.GetCurrentRoundConfig(activityConfig, DateTime.Now);
            if (round != null)
            {
                var counter = new BaoYangCounter(activityId, round.LimitedQuantity, activityConfig.ItemQuantityPerUser, round);
                var purchaseCount = await counter.GetCurrentActivityPurchaseCount();
                var orderInfos = await counter.GetOrderHistory();
                result = new FixedPriceActivityRoundResponse
                {
                    ActivityId = activityConfig.ActivityId,
                    ActivityName = activityConfig.ActivityName,
                    UserLimitedQuantity = activityConfig.ItemQuantityPerUser,
                    LimitedQuantity = round.LimitedQuantity,
                    StartTime = round.StartTime,
                    EndTime = round.EndTime,
                    CurrentPurchaseCount = purchaseCount,
                    OrderInfos = orderInfos,
                };
            }
            return result;
        }

    }
}
