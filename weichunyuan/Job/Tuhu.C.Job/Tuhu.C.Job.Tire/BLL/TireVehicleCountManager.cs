using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.Job.Tire.DAL;
using Tuhu.C.Job.Tire.Model;

namespace Tuhu.C.Job.Tire.BLL
{
    public class TireVehicleCountManager
    {
        public static void DoJob(ILog logger)
        {
            var dat = new List<TireMatchVehicleModel>();
            DalTireVehicleCount.GetVehicleData().ForEach(g => dat.AddRange(g.TiresMatch?.Split(';').Where(t=>!string.IsNullOrWhiteSpace(t)).Select(t => new TireMatchVehicleModel { Tire = t, Vehicle = g.ProductID })));
            var data = dat.GroupBy(g => g.Tire).Select(g => new TireMatchVehicleCount
            {
                PID = g.Key,
                VehicleCount = g.Count()
            }).ToList();
            DalTireVehicleCount.SetTireMatchVehicle(data,logger);
        }
    }
}
