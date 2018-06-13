using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Activity.Server.Manager;

namespace Tuhu.Service.Activity.Server
{
    public class ActivityService : IActivityService
    {
        public Task<OperationResult<TireActivityModel>> SelectTireActivityAsync(string vehicleId, string tireSize) =>
            OperationResult.FromResultAsync(ActivityManager.SelectTireActivityAsync(vehicleId, tireSize));
    }
}
