using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.Job.Activity.Models;

namespace Tuhu.C.Job.Activity.Dal
{
    public static class DalShareBargain
    {
        public static IEnumerable<ShareBargainModel> GetAllShareBargain()
        {
            string sqlStr = @"
select TA.FinalPrice,
       TA.OwnerId,
       TA.idKey as IdKey,
       TB.productName as ProductName,
       TB.EndDateTime,
       TA.IsOver,
       TB.PID,
       TA.ActivityProductId,
       TB.OriginalPrice,
       TB.SimpleDisplayName
from Activity..BargainOwnerAction as TA with (nolock)
    inner join Configuration..BargainProduct as TB with (nolock)
        on TA.ActivityProductId = TB.PKID
where TB.EndDateTime > DATEADD(day, -1, GETDATE())
      and TA.IsPurchased = 0
      and TA.Status = 1;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                return DbHelper.ExecuteSelect<ShareBargainModel>(true, cmd);
            }
        }
    }
}
