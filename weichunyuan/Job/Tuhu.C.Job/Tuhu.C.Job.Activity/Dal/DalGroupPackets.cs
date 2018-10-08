using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.Activity.Dal
{
    public static class DalGroupPackets
    {
        public static int GetGroupCount()
        {
            const string sqlStr = @"
select COUNT(1)
from Tuhu_log.dbo.FightGroupsPacketsLog with (nolock)
where OrderBy = 4
      and UserId is null
      and CreateDateTime > GETDATE() - 1
      and PromotionPKID is null;";
            using (var dbHelper = DbHelper.CreateLogDbHelper(true))
            {
                using (var cmd = new SqlCommand(sqlStr))
                {
                    var result = dbHelper.ExecuteScalar(cmd);
                    int.TryParse(result?.ToString(), out var value);
                    return value;
                }
            }
        }

        public static List<Tuple<Guid, DateTime>> GetGroupInfo(int index, int step)
        {
            const string sqlStr = @"
select FightGroupsIdentity,
       CreateDateTime
from Tuhu_log.dbo.FightGroupsPacketsLog with (nolock)
where OrderBy = 4
      and UserId is null
      and CreateDateTime > GETDATE() - 1
      and PromotionPKID is null
order by PKID offset @start rows fetch next @step rows only;";
            using (var dbHelper = DbHelper.CreateLogDbHelper(true))
            {
                using (var cmd = new SqlCommand(sqlStr))
                {
                    cmd.Parameters.AddWithValue("@start", index * step);
                    cmd.Parameters.AddWithValue("@step", step);
                    return dbHelper.ExecuteQuery(cmd, dt =>
                    {
                        var result = new List<Tuple<Guid, DateTime>>();
                        if (dt == null || dt.Rows.Count == 0) return result;
                        for (var i = 0; i < dt.Rows.Count; i++)
                        {
                            var value1 = dt.Rows[i].GetValue<Guid>("FightGroupsIdentity");
                            var value2 = dt.Rows[i].GetValue<DateTime>("CreateDateTime") + TimeSpan.FromDays(1);
                            if (value1 != Guid.Empty) result.Add(new Tuple<Guid, DateTime>(value1, value2));
                        }

                        return result;
                    });
                }
            }
        }

        public static List<Guid> GetUserInfo(Guid groupId)
        {
            const string sqlStr = @"select UserId
from Tuhu_log.dbo.FightGroupsPacketsLog with (nolock)
where FightGroupsIdentity = @id
      and PromotionPKID is null
      and UserId is not null;";
            using (var dbHelper = DbHelper.CreateLogDbHelper(true))
            {
                using (var cmd = new SqlCommand(sqlStr))
                {
                    cmd.Parameters.AddWithValue("@id", groupId);
                    return dbHelper.ExecuteQuery(cmd, dt =>
                    {
                        var result = new List<Guid>();
                        if (dt == null || dt.Rows.Count == 0) return result;
                        for (var i = 0; i < dt.Rows.Count; i++)
                        {
                            var value = dt.Rows[i].GetValue<Guid>("UserId");
                            if (value != Guid.Empty) result.Add(value);
                        }

                        return result;
                    });
                }
            }
        }
    }
}
