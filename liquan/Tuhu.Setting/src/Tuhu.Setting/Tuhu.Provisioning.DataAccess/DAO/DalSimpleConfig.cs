using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalSimpleConfig
    {
        public static string SelectConfig(string configName)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Configuration_AlwaysOnRead")))
            {
                var sql = @"SELECT ConfigContent FROM Configuration..SimpleConfig (NOLOCK) WHERE ConfigName=@ConfigName";
                return dbHelper.ExecuteScalar(sql, CommandType.Text, new SqlParameter("@ConfigName",configName)).ToString();
            }
        }

        public static bool UpdateConfig(string configName, string xml)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Configuration")))
            {
                var sql = @"UPDATE Configuration..SimpleConfig
                            SET ConfigContent = @Xml
                            WHERE ConfigName = @ConfigName";
                DbParameter[] parameters = {
                    new SqlParameter("@Xml", xml),
                    new SqlParameter("@ConfigName", configName)
                };
                return dbHelper.ExecuteNonQuery(sql, CommandType.Text, parameters) > 0;
            }
        }
    }
}
