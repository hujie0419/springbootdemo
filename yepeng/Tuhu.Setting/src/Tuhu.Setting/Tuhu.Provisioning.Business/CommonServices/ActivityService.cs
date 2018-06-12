using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Activity;
using Tuhu.Service.PinTuan;

namespace Tuhu.Provisioning.Business.CommonServices
{
    public class ActivityService
    {
        private static readonly Common.Logging.ILog logger = LogManager.GetLogger(typeof(ActivityService));

        public static bool RefrestPTCache(string groupId)
        {
            var result = false;
            try
            {
                using (var client = new PinTuanClient())
                {
                    var getResult = client.RefreshCache(groupId??string.Empty);
                    getResult.ThrowIfException(true);
                    result = getResult.Result.Code == 1;
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
