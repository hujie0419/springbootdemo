using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common.Models;
using Tuhu.Provisioning.DataAccess.DAO.Tire;
using Tuhu.Provisioning.DataAccess.Entity.Tire;

namespace Tuhu.Provisioning.Business.Tire
{
    public class InstallNowManager
    {
        public static IEnumerable<InstallNowModel> SelectList(InstallNowConditionModel request, PagerModel pager)
            => DalInstallNow.SelectList(request, pager);

        public static string FetchDisPlayNameByPID(string pid) => DalInstallNow.FetchDisPlayNameByPID(pid);

        public static ResultModel SaveInstallNow(string cityIds, IEnumerable<PidModel> pIDS) => DalInstallNow.SaveInstallNow(cityIds, pIDS);

        public static int DeleteInstallNow(int pkid) => DalInstallNow.DeleteInstallNow(pkid);

        public static IEnumerable<InstallNowModel> SelectInstallNowByPKIDS(string pKIDS) => DalInstallNow.SelectInstallNowByPKIDS(pKIDS);

        public static int BitchOff(string pkids) => DalInstallNow.BitchOff(pkids);

        public static int BitchOn(IEnumerable<InstallNowModel> list) => DalInstallNow.BitchOn(list);
    }
}