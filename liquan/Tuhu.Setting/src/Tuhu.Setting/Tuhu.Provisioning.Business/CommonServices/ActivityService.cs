using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Activity;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.PinTuan;

namespace Tuhu.Provisioning.Business.CommonServices
{
    public class ActivityService
    {
        private static readonly Common.Logging.ILog logger = LogManager.GetLogger(typeof(ActivityService));

        public static async Task<bool> RefrestPTCache(string groupId)
        {
            var result = false;
            try
            {
                // 延迟3秒执行  ->  （读写分离）
                {
                    await Task.Delay(TimeSpan.FromSeconds(3));

                    using (var client = new PinTuanClient())
                    {
                        var getResult = client.RefreshCache(groupId ?? string.Empty);
                        getResult.ThrowIfException(true);
                        result = getResult.Result.Code == 1;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// 刷新大客户活动配置缓存
        /// </summary>
        /// <param name="activityExclusiveId"></param>
        /// <returns></returns>

        public static bool RefreshRedisCacheCustomerSetting(string activityExclusiveId)
        {
            var result = false;
            try
            {
                using (var client = new CacheClient())
                {
                    var getResult = client.RefreshRedisCacheCustomerSetting(activityExclusiveId);
                    getResult.ThrowIfException(true);
                    result = getResult.Result;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// 获取返现申请页面配置
        /// </summary>
        /// <returns></returns>
        public static RebateApplyPageConfig SelectRebateApplyPageConfig()
        {
            RebateApplyPageConfig result = null;
            try
            {
                using (var client = new ActivityClient())
                {
                    var getResult = client.SelectRebateApplyPageConfig();
                    getResult.ThrowIfException(true);
                    result = getResult.Result;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }
    }
}
