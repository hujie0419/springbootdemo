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
  public static  class DecorativePatternManager
    {
        public static bool Save(SE_DecorativePattern model)
        {
            if (model.ID.ToString() == "00000000-0000-0000-0000-000000000000")
            {
                return DALDecorativePattern.Add(model);
            }
            else
            {
                return DALDecorativePattern.Update(model);
            }
        }

        public static SE_DecorativePattern GetEntity(string id)
        {
            return DALDecorativePattern.GetEntity(id);
        }


        public static IEnumerable<SE_DecorativePattern> GetList()
        {
            return DALDecorativePattern.GetList();
        }

        public static IEnumerable<SE_DecorativePattern> GetList(string type,string name)
        {
            return DALDecorativePattern.GetList(type, name);
        }

        public static bool Delete(string id)
        {
            return DALDecorativePattern.Delete(id);
        }


        public static DataTable GetTriePatten(string tire)
        {
            return DALDecorativePattern.GetTirePatten(tire);
        }


    }
}
