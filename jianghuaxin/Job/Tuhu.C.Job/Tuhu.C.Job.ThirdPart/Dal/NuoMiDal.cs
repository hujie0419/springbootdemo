using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.ThirdParty.Enums;
using Tuhu.Service.ThirdParty.Models;

namespace Tuhu.C.Job.ThirdPart.Dal
{
    public class NuoMiDal
    {
        public static IEnumerable<BaiDuNMOrderLog> GetNotifyFailedLogs()
        {
            IEnumerable<int> failedStatus = new List<int>() { 6, 11, 12, 13, 14, 15, 16 };

            string sql = string.Format("SELECT * FROM  [SystemLog].dbo.BaiDuNMOrderLog(NOLOCK) l WHERE l.OrderStatus IN ({0}) AND l.OrderType IS NOT NULL  AND l.CreateTime >= DATEADD(dd, -3, GETDATE()) ", string.Join(",", failedStatus));
            using (var dbhelper = DbHelper.CreateDbHelper())
            {
                var res = dbhelper.ExecuteSelect<BaiDuNMOrderLog>(sql);
                return res;
            }

        }

        public static IEnumerable<BaiDuNMOrderLog> GetNotComplatelogs()
        {
            string sql = string.Format(@"SELECT  * FROM    [SystemLog].[dbo].[BaiDuNMOrderLog](NOLOCK) l WHERE   l.OrderStatus = {0} AND OrderType IS NOT NULL and NMrefundBatchId is null AND l.CreateTime >= DATEADD(dd, -3, GETDATE());", (int)NuoMiLogOrderStatus.Paid);
            using (var dbhelper = DbHelper.CreateDbHelper())
            {
                return dbhelper.ExecuteSelect<BaiDuNMOrderLog>(sql);
            }

        }
    }
}
