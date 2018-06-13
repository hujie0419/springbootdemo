using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Tuhu.Service.Activity.Models;

namespace Tuhu.Service.Activity.DataAccess
{
    public class DalActivity
    {
        public static async Task<IEnumerable<TireActivityModel>> SelectTireActivity(string vehicleId, string tireSize, bool flag = true)
        {
            return await DbHelper.ExecuteSelectAsync<TireActivityModel>(flag, @"SELECT	* FROM	Activity.dbo.tbl_TireListActivity AS TLA WITH (NOLOCK) WHERE	TLA.Status = 1 AND EndTime > GETDATE() AND VehicleID = @VehicleID AND TireSize = @TireSize",
                CommandType.Text,
                new SqlParameter("@VehicleID", vehicleId),
                new SqlParameter("@tireSize", tireSize));
        }
    }
}

