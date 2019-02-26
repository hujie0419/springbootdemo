using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Vehicle;
using Tuhu.Service.Vehicle.Model;

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

        /// <summary>
        /// 根据品牌获取二级车型
        /// </summary>
        /// <param name="para"></param>
        /// <param name="queryEnum"></param>
        /// <returns></returns>
        public static IEnumerable<VehicleBrand> GetVehicleInfoList(VehicleQueryCategoryParam para, VehicleQueryCategoryEnum queryEnum)
        {
            IEnumerable<VehicleBrand> result = null;
            try
            {
                using (var client = new VehicleClient())
                {
                    var serviceResult = client.GetVehicleInfoList(para, queryEnum);
                    serviceResult.ThrowIfException(true);
                    result = serviceResult.Result;
                }
            }
            catch (Exception ex)
            {
                logger.Error($"GetVehicleInfoList ->{para} -> {queryEnum}",ex);
            }
            return result;
        }
    }
}
