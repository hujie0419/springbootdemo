using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity.UserManager;
using Tuhu.Provisioning.DataAccess.Entity;
using Microsoft.ApplicationBlocks.Data;

namespace Tuhu.Provisioning.DataAccess.DAO.UserManager
{
    public class DalUserManager
    {
        public static List<UserGrowthSchedule> GetUserGrowthScheduleList(SqlConnection conn, DateTime st, DateTime et)
        {
            string sql = @"select * from Tuhu_profiles..UserGrowthSchedule
                            where DayString>=@StartTime and DayString<=@EndTime
                            order by DayString desc";
            var sqlParam = new[]
            {
                new SqlParameter("@StartTime",st),
                new SqlParameter("@EndTime",et)
            };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParam)
                .ConvertTo<UserGrowthSchedule>()
                .ToList();
        }
    }
}
