using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.DAO;

namespace Tuhu.Provisioning.Business
{
   public class SE_GroupBuyingManager
    {
        public static bool Add(SE_GroupBuyingConfig model)
        {
            using (var connection = ProcessConnection.OpenConfiguration)
            {
                return DALSE_GroupBuying.Add(connection, model);
            }

        }


        public static bool Update(SE_GroupBuyingConfig model)
        {
            using (var connection = ProcessConnection.OpenConfiguration)
            {
                return DALSE_GroupBuying.Update(connection, model);
            }
        }


        public static bool Delete(int  id)
        {
            using (var connection = ProcessConnection.OpenConfiguration)
            {
                return DALSE_GroupBuying.Delete(connection, id);
            }
        }

        public static SE_GroupBuyingConfig GetEntity(int id)
        {
            using (var connection = ProcessConnection.OpenConfigurationReadOnly)
            {
                return DALSE_GroupBuying.GetEntity(connection, id);
            }
        }

        public static IEnumerable<SE_GroupBuyingConfig> GetList()
        {
            using (var connection = ProcessConnection.OpenConfigurationReadOnly)
            {
                return DALSE_GroupBuying.GetList(connection);
            }
        }



    }
}
