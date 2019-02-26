using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Purchase;
using Tuhu.Service.Purchase.Models;
using Tuhu.Service.Purchase.Request;

namespace Tuhu.Provisioning.Business.CommonServices
{
    public static class PurchaseService
    {
        private static readonly Common.Logging.ILog logger =  LogManager.GetLogger(typeof(PurchaseService));

        public static IEnumerable<CarPriceManagementResponse> SelectPurchaseInfoByPID(List<string> pids)
        {
            IEnumerable<CarPriceManagementResponse> result = new List<CarPriceManagementResponse>();
            try
            {
                if (pids != null && pids.Any())
                {
                    List<CarPriceManagementRequest> pidList = new List<CarPriceManagementRequest>();
                    pids.ForEach(x => pidList.Add(new CarPriceManagementRequest() { PID = x }));
                    using (var client = new PurchaseClient())
                    {
                        var getResult = client.SelectPurchaseInfoByPID(pidList);
                        getResult.ThrowIfException(true);
                        result = getResult.Result;
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
