using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.ActivityJob.Models.CarveupPoint;

namespace Tuhu.C.ActivityJob.Dal.CarveupPoint
{
    public class CarveupPointDal
    {
        public static int GetExpiredGroupCount()
        {
            using (var cmd = new SqlCommand(@"
                SELECT COUNT(1)
                FROM [Activity].[dbo].[CarveupPointGroupInfo] WITH (NOLOCK)
                WHERE GETDATE() > EndTime
                      AND GroupState = 'Ongoing'
                      AND IsDeleted = 0;"))
            {
                cmd.CommandType = CommandType.Text;
                return Convert.ToInt32(DbHelper.ExecuteScalar(true, cmd));
            }
        }

        public static List<CarveupPointGroupModel> GetExpiredGroups(int pageSize, ref int maxPkid)
        {
            using (var cmd = new SqlCommand($@"
                SELECT TOP {pageSize}
                       [PKID],
                       [GroupId],
                       [ActivityId]
                FROM [Activity].[dbo].[CarveupPointGroupInfo] WITH (NOLOCK)
                WHERE PKID > {maxPkid}
                      AND GETDATE() > EndTime
                      AND GroupState = 'Ongoing'
                      AND IsDeleted = 0
                ORDER BY PKID;"))
            {
                cmd.CommandType = CommandType.Text;
                var result = DbHelper.ExecuteSelect<CarveupPointGroupModel>(cmd)
                    ?? new List<CarveupPointGroupModel>();

                maxPkid = result.Any() ? result.Max(x => x.PKID) : maxPkid;
                return result.ToList();
            }
        }
    }
}
