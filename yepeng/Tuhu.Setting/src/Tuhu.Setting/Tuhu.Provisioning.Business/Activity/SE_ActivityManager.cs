using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.Business.Logger;
using Newtonsoft.Json;

namespace Tuhu.Provisioning.Business.Activity
{
  public static  class SE_ActivityManager
    {
        public static int Save(SE_Activity model)
        {
            int result = 0;
            if (model.ID.ToString() == "00000000-0000-0000-0000-000000000000")
            {
                if (DALSE_Activity.Add(model))
                {
                    result = 1;
                }
            }
            else
            {
                if (DALSE_Activity.Update(model))
                    result = 1;
            }
            return result;
        }


        public static int Delete(string id)
        {
            return DALSE_Activity.Delete(id) == true ? 1 : 0;
        }

        public static IEnumerable<SE_Activity> GetList(string whereStr, int pageSize, int pageIndex,out int rowCount)
        {
            return DALSE_Activity.GetList(whereStr, pageSize, pageIndex, out rowCount).ConvertTo<SE_Activity>();
        }


        public static SE_Activity GetEntity(string id)
        {
            return DALSE_Activity.GetEntity(id);
        }

    }
}
