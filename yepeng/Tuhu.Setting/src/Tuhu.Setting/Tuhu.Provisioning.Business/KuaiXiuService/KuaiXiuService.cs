using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.KuaiXiu;
using Tuhu.Service.KuaiXiu.Models;

namespace Tuhu.Provisioning.Business.KuaiXiuService
{
    public class KuaiXiuService
    {
        public async Task<bool> RevertServiceCodes(IEnumerable<string> serviceCodes, string channel, string source)
        {
            var result = false;

            if (serviceCodes != null && serviceCodes.Any())
            {
                using (var client = new ServiceCodeClient())
                {
                    var request = new RevertServiceCodesRequest
                    {
                        Source = source,
                        Channel = channel,
                        ServiceCodes = serviceCodes.ToList()
                    };
                    var serviceResult = await client.RevertServiceCodesAsync(request);
                    result = serviceResult.Result;
                }
            }

            return result;
        }

        public async Task<IEnumerable<ServiceCode>> GetServiceCodeDetailsByCodes(IEnumerable<string> serviceCodes)
        {
            using (var client = new ServiceCodeClient())
            {
                var serviceResult = await client.SelectServiceCodeDetailsByCodesAsync(serviceCodes);
                serviceResult.ThrowIfException(true);
                return serviceResult.Result;
            }
        }
    }
}
