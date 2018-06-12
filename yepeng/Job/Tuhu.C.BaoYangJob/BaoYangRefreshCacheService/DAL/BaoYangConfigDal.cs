using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu;

namespace BaoYangRefreshCacheService.DAL
{
    public static class BaoYangConfigDal
    {
        /// <summary>
        /// 根据configName查找Config
        /// </summary>
        /// <param name="configName"></param>
        /// <returns></returns>
        public static string GetConfigByConfigName(string configName)
        {
            var parameter = new SqlParameter("@ConfigName", configName);
            var sql = @"SELECT  Config
                        FROM    BaoYang..BaoYangConfig WITH ( NOLOCK )
                        WHERE   ConfigName = @ConfigName;";
            var result = (string)DbHelper.ExecuteScalar(true, sql, CommandType.Text, parameter);
            return result;
        }
    }
}
