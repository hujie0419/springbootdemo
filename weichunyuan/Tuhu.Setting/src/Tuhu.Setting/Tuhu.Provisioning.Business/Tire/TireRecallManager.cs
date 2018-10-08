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
    public class TireRecallManager
    {
        public static IEnumerable<TireRecallModel> SelectList(TireRecallModel model, PagerModel pager) => DalTireRecall.SelectList(model, pager);

        public static IEnumerable<TireRecallModel> SelectList(TireRecallModel model) => DalTireRecall.SelectList(model);


        public static TireRecallModel FetchTireRecall(long pkid) => DalTireRecall.FetchTireRecall(pkid);
        public static bool UpdateStatus(long pkid, int status,string reason)
        {
            var detail  = DalTireRecall.FetchTireRecall(pkid);
            if (detail != null && detail.Status == 0)
            {
                return DalTireRecall.UpdateTireReallStatus(pkid, status, reason) >0;
            }
            return false;
        }

        public static Special_Bridgestone_Pidweekyear FetchSpecial_Bridgestone_Pidweekyear(int orderid) => DalTireRecall.FetchSpecial_Bridgestone_Pidweekyear(orderid);

        public static bool InsertProductRecallLog(TireRecallModel model) => DalTireRecall.InsertTireRecallLog(model);

        public static IEnumerable<TireRecallLog> GetTireRecallLog(long pkid) => DalTireRecall.FetchTireRecallLog(pkid);
    }
}
