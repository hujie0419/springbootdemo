using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.KuaiXiu;
using Tuhu.Service.KuaiXiu.Models;

namespace Tuhu.Provisioning.Business.CommonServices
{
    public class KuaiXiuService
    {
        private static readonly Common.Logging.ILog logger = LogManager.GetLogger(typeof(KuaiXiuService));

        public static IEnumerable<ServiceCode> GetServiceCodeDetailsByCodes(IEnumerable<string> serviceCodes)
        {
            IEnumerable<ServiceCode> result = null;
            try
            {
                using (var client = new ServiceCodeClient())
                {
                    var serviceResult = client.SelectServiceCodeDetailsByCodes(serviceCodes);
                    serviceResult.ThrowIfException(true);
                    result = serviceResult.Result;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result ?? new List<ServiceCode>();
        }
    }
}
