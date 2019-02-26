using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common.Models;
using Tuhu.Provisioning.DataAccess.DAO.Tire;
using Tuhu.Provisioning.DataAccess.Entity.Tire;
using Newtonsoft.Json;
namespace Tuhu.Provisioning.Business.Tire
{
    public class InstallFeeManager
    {
        public static IEnumerable<InstallFeeModel> SelectInstallFeeList(InstallFeeConditionModel condition, PagerModel pager)
              => DalInstallFee.SelectInstallFeeList(condition, pager);

        public static int InstallFeeEdit(string pid, decimal addPrice, bool isEdit)
        {
            int result = -99;
           
             if (addPrice <= 0)
                result=DalInstallFee.Delete(pid);
            else if (isEdit)
                result = DalInstallFee.Edit(pid, addPrice);
            else
                result = DalInstallFee.Insert(pid, addPrice);
            return result;
        }

        public static List<string> SelectPackagePIDs(IEnumerable<string> pids)
=> DalInstallFee.SelectPackagePIDs(pids);
    }
}