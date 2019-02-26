using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;
using System.Data;

namespace Tuhu.Provisioning.Business
{
  public  class ActivityHomeBll
    {

        public static bool Add(List<ActivityHome> model)
        {
            DALActivityHome.Delete(Convert.ToInt32(model[0].ActivityID));
            foreach (var item in model)
            {
                DALActivityHome.Add(item);
            }
            return true;
        }


        public static DataTable GetList(string activityID)
        {
            return DALActivityHome.GetList(Convert.ToInt32(activityID));
        }


    }
}
