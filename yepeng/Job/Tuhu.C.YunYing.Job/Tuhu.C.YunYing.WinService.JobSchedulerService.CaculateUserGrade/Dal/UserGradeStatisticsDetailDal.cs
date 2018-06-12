using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.YunYing.WinService.JobSchedulerService.CaculateUserGrade.Model;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.CaculateUserGrade.Dal
{
    public class UserGradeStatisticsDetailDal
    {
        internal IEnumerable<UserGradeStatisticsDetail> GetUserIdByPage(int pageIndex, int pageSize)
        {
            string sql = @"SELECT a.UserId
FROM (
SELECT UserId FROM Tuhu_profiles.dbo.UserGradeStatisticsDetail(NOLOCK)
GROUP BY UserId 
) AS a 
ORDER BY a.UserId
OFFSET @begin ROWS FETCH NEXT @pageSize ROWS ONLY";
            var cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@begin", (pageIndex - 1) * pageSize + 1);
            cmd.Parameters.AddWithValue("@pageSize", pageSize);
            var result = DbHelper.ExecuteSelect<UserGradeStatisticsDetail>(true, cmd);
            return result;
        }

    }
}
