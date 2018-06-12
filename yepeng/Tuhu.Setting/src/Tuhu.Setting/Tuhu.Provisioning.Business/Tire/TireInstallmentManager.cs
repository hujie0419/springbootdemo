using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.Business.Tire
{
  public  class TireInstallmentManager
    {
        public static IEnumerable<InstallNowModel> SelectList(InstallNowConditionModel request, PagerModel pager)
            => DalInstallNow.SelectList(request, pager);
    }
}
