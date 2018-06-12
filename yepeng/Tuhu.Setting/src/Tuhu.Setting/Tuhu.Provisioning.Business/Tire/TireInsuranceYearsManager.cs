using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common.Models;
using Tuhu.Provisioning.DataAccess.Entity.Tire;
using Tuhu.Provisioning.DataAccess.DAO.Tire;
namespace Tuhu.Provisioning.Business.Tire
{
    public class TireInsuranceYearsManager
    {
        public static async Task<IEnumerable<TireInsuranceYearModel>> SelectInstallFeeListAsync(
            InstallFeeConditionModel condition, PagerModel pager)
        {
            var result = await DalTireInsuranceYears.SelectInstallFeeListAsync(condition, pager);
            return result;
        }

        public static async Task<IEnumerable<TireModifyConfigLog>> SelectLogsAsync(string pid,string type)
        {
            var result = await DalTireInsuranceYears.SelectLogsAsync(pid,type);
            return result;
        }
        public static async Task<bool> WriteLogsAsync(IEnumerable<TireModifyConfigLog> logs)
        {
            var result = await DalTireInsuranceYears.WriteLogsAsync(logs);
            return result;
        }
    }
}
