using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Config;

namespace Tuhu.Provisioning.Business.CommonServices
{
    public class ConfigService
    {
        private static readonly Common.Logging.ILog logger = LogManager.GetLogger(typeof(ConfigService));
        /// <summary>
        /// 刷新发现标签配置缓存
        /// </summary>
        /// <returns></returns>
        public static bool RefreshArticleTabConfigCache()
        {
            var result = false;
            try
            {
                using (var client = new ConfigClient())
                {
                    var getResult = client.RefreshArticleTabConfigCache();
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
