using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Nosql;

namespace Tuhu.Provisioning.Business.Cache
{
    public class CacheManager
    {
        private static readonly Common.Logging.ILog logger = LogManager.GetLogger(typeof(CacheManager));

        public static bool ClearCacheByKey(string defaultClientName, string key)
        {
            var result = false;

            try
            {
                if (!string.IsNullOrEmpty(defaultClientName) && !string.IsNullOrEmpty(key))
                {
                    using (var client = CacheHelper.CreateCacheClient(defaultClientName))
                    {
                        var getResult = client.Remove(key);
                        result = getResult.Success;
                    }
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
