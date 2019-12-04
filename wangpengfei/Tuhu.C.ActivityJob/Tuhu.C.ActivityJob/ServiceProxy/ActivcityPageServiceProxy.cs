using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.ActivityPage;

namespace Tuhu.C.ActivityJob.ServiceProxy
{
    /// <summary>
    /// 活动页服务代理
    /// </summary>
    public class ActivcityPageServiceProxy
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ActivcityPageServiceProxy));

        /// <summary>
        /// 活动页关键字搜索数据刷新
        /// </summary>
        /// <returns></returns>
        public static bool RefreshActivityKeyword()
        {
            using (var client = new ActivityPageCacheClient())
            {
                var clientResult = client.RefreshActivityKeyword();

                if (!(clientResult.Success && clientResult.Result))
                {
                    Logger.Warn($"RefreshActivityKeyword,调用活动页刷新关键字搜索接口失败,message:{clientResult.ErrorMessage}");
                    return false;
                }
                else
                {
                    return clientResult.Result;
                }
            }
        }
    }
}
