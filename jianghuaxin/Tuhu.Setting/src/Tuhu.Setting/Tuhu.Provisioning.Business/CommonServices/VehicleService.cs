using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Vehicle;

namespace Tuhu.Provisioning.Business.CommonServices
{
    public class VehicleService
    {
        private static readonly Common.Logging.ILog logger = LogManager.GetLogger(typeof(VehicleService));
        public static IEnumerable<string> GetTidsByVIN(string vin)
        {
            var result = null as IEnumerable<string>;
            vin = vin?.Replace(" ", "");
            if (!string.IsNullOrEmpty(vin))
            {
                using (var client = new VehicleClient())
                {
                    var serviceResult = client.GetTidsByVIN(vin, "setting");
                    serviceResult.ThrowIfException(true);
                    var resultModel = serviceResult.Result;
                    result = resultModel.Data;
                }
            }
            return result ?? new List<string>();
        }
    }
}
