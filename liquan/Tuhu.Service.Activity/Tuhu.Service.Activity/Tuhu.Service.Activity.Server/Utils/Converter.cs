using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.BaoYang.Models;
using Tuhu.Service.Order.Models;

namespace Tuhu.Service.Activity.Server.Utils
{
    /// <summary>
    /// Converter Utils.
    /// </summary>
    public static class Converter
    {
        /// <summary>
        /// convert order car to vehicle request model.
        /// </summary>
        /// <param name="car"></param>
        /// <returns></returns>
        public static VehicleRequestModel Convert(OrderCarModel car)
        {
            VehicleRequestModel result = null;

            if (car != null)
            {
                List<BaoYangVehicleFivePropertyModel> propertiesList = null;
                if (car.ExtCol != null &&
                    car.ExtCol.ContainsKey("Properties")
                    && car.ExtCol["Properties"] != null)
                {
                    string properties = car.ExtCol["Properties"].ToString();
                    List<dynamic> list = JsonConvert.DeserializeObject<List<dynamic>>(properties);
                    propertiesList = list.Select(o => new BaoYangVehicleFivePropertyModel()
                    {
                        Property = o.propertyKey,
                        PropertyValue = o.propertyValue
                    }).ToList();
                }

                DateTime? onRoadTime = null;
                if (car.OnRoadYear > 0 && car.OnRoadYear < 9999 &&
                    car.OnRoadMonth > 0 && car.OnRoadMonth <= 12)
                {
                    onRoadTime = new DateTime(car.OnRoadYear.Value, car.OnRoadMonth.Value, 1);
                }

                result = new VehicleRequestModel()
                {
                    VehicleId = car.VehicleId,
                    PaiLiang = car.PaiLiang,
                    Nian = car.Nian,
                    Tid = car.Tid,
                    Properties = propertiesList,
                    OnRoadTime = onRoadTime,
                    Distance = car.Distance
                };
            }

            return result;
        }
    }
}
