using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Tuhu.Service.CashBackActivity;
using Tuhu.Service.CashBackActivity.Request;

namespace Tuhu.C.ActivityJob.ServiceProxy
{
    /// <summary>
    /// 瓜分积分服务代理
    /// </summary>
    public class CarveupPointServiceProxy
    {
        private static readonly ILog _logger = LogManager.GetLogger<CarveupPointServiceProxy>();

        /// <summary>
        /// 过期瓜分积分活动
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool ExpireCarveupPointGroup(ExpireCarveupPointGroupRequest request)
        {
            try
            {
                using (var carveupPointClient = new CarveupPointClient())
                {
                    var expireResult = AsyncHelper.RunSync(
                        () => carveupPointClient.ExpireCarveupPointGroupAsync(request));

                    expireResult.ThrowIfException(true);
                    return expireResult.Result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"过期瓜分积分活动失败：{request}", ex);
                return false;
            }
        }
    }
}
