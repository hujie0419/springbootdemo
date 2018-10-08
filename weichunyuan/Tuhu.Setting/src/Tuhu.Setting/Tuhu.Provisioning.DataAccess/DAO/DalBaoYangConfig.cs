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
        /// <summary>
        /// 修改保养配置文件
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="configXml"></param>
        /// <param name="configName"></param>
        /// <returns></returns>
        public static bool UpdateConfig(SqlConnection conn, string configXml, string configName)
        {
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@ConfigName", configName),
                new SqlParameter("@ConfigXml",configXml),
            };
            var sql = @"UPDATE  BaoYang.dbo.BaoYangConfig
                        SET     Config = @ConfigXml ,
                                UpdatedTime = GETDATE()
                        WHERE   ConfigName = @ConfigName; ";
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
        }
        /// <summary>
        /// 添加配置文件
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="configXml"></param>
        /// <param name="configName"></param>
        /// <returns></returns>
        public static bool InsertConfig(SqlConnection conn, string configXml, string configName)
        {
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@ConfigName", configName),
                new SqlParameter("@ConfigXml",configXml),
            };
            var sql = @"INSERT INTO BaoYang.dbo.BaoYangConfig
                        ( ConfigName, Config, CreatedTime, UpdatedTime)
                        VALUES 
                        (@ConfigName, @ConfigXml, GETDATE(), GETDATE())";
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
        }
        /// <summary>
        /// 判断是否有配置项
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="configName"></param>
        /// <returns></returns>
        public static bool IsExistsConfig(SqlConnection conn, string configName)
        {
            var result = false;
            var sql = @"Select  count(1) from 
                        BaoYang..BaoYangConfig 
                        where configName = @configName";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@ConfigName", configName)
            };
            var obj = SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters);
            if (obj != null && Convert.ToInt32(obj) > 0)
            {
                result = true;
            }
            return result;
        }
    }
}
