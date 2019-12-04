using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.LotteryDrawActivity;
using Tuhu.Service.LotteryDrawActivity.Request;

namespace Tuhu.C.ActivityJob.ServiceProxy
{
    /// <summary>
    /// 大翻盘服务
    /// </summary>
    public class BigBrandActivityAsyncService
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BigBrandActivityAsyncService));

        /// <summary>
        /// 大奖信息邮件推送
        /// </summary>
        /// <param name="userids"></param>
        /// <param name="batchid"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public static async Task<bool> GrandPrizeEmailPushAsync(GrandPrizeEmailPushRequest request)
        {
            try
            {
                using (var client = new BigBrandActivityAsyncClient())
                {
                    var pushResult = await client.GrandPrizeEmailPushAsync(request);
                    if (pushResult.Success && pushResult.Result)
                    {
                        return true;
                    }
                    else
                    {
                        Logger.Warn($"GrandPrizeEmailPushAsync失败{pushResult.ErrorCode + pushResult.ErrorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"GrandPrizeEmailPushAsync 异常,{ex.Message + ex.StackTrace}", ex);
            }

            return false;
        }
    }
}
