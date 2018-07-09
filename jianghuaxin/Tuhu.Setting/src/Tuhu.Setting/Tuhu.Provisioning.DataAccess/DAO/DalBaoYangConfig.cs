using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ApplicationBlocks.Data;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public static class DalBaoYangConfig
    {
        private const string ProcSelectConfig = "BaoYang..BaoYangConfig_SelectConfig";

        public static string SelectConfig(SqlConnection conn, string configName)
        {
            var result = string.Empty;

            var parameters = new[]
            {
                new SqlParameter("@ConfigName", configName)
            };

            using (var reader = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, ProcSelectConfig, parameters))
            {
                while (reader.Read())
                {
                    result = reader.IsDBNull(0) ? string.Empty : reader.GetString(0).Trim();
                    break;
                }
            }

            return result;
        }
    }
}
