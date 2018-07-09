using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.DAO;
using System.Data;


namespace Tuhu.Provisioning.Business
{
   public class ActivityMenuBll
    {

     

       

        public static  bool Add(List<ActivityMenu> list)
        {
            Delete(list[0].ActivityID.ToString());
            foreach (var model in list)
            {
                DALActivityMenu.Add(model);
            }
            return true;
        }

        public static bool Update(ActivityMenu model)
        {
            return DALActivityMenu.Update(model);
        }


        private static bool Delete(string id)
        {
            int temp = 0;
            if (int.TryParse(id,out temp))
            {
                return DALActivityMenu.Delete(temp);
            }
            else
                return false;
        }

        public static DataTable GetList(string activityID)
        {
            DataTable dt = DALActivityMenu.GetTable(activityID);
            return dt;
        }

        public static ActivityMenu GetModel(string id)
        {
            int primarykey = 0;
            if (int.TryParse(id, out primarykey))
            {
                return new ActivityMenu(DALActivityMenu.GetDataRow(primarykey));
            }
            else
                return null;
        }

    }
}
