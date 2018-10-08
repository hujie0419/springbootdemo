using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;

namespace Tuhu.Provisioning.Business
{
    /// <summary>
    /// 链接字符串处理
    /// </summary>
    public static class ProcessConnection
    {
        #region  Gungnir & Gungnir readOnly
        /// <summary>
        /// 读写 Gungnir
        /// </summary>
        public static SqlConnection OpenGungnir => OpenConnection(ProcessConnectionString("Gungnir", false));
        /// <summary>
        /// 只读库 Gungnir
        /// </summary>
        public static SqlConnection OpenGungnirReadOnly => OpenConnection(ProcessConnectionString("Gungnir", true));
        /// <summary>
        /// 延迟读取只读库(只读库延迟同步问题) Gungnir
        /// </summary>
        public static SqlConnection OpenGungnirReadOnlyForDelay => OpenConnection(ProcessConnectionString("Gungnir", true), true);
        #endregion

        #region Configuration & Configuration readOnly
        /// <summary>
        /// 读写 Configuration
        /// </summary>
        public static SqlConnection OpenConfiguration => OpenConnection(ProcessConnectionString("Configuration", false));
        /// <summary>
        /// 只读库 Configuration
        /// </summary>
        public static SqlConnection OpenConfigurationReadOnly => OpenConnection(ProcessConnectionString("Configuration", true));
        /// <summary>
        /// 延迟读取只读库(只读库延迟同步问题) Configuration
        /// </summary>
        public static SqlConnection OpenConfigurationReadOnlyForDelay => OpenConnection(ProcessConnectionString("Configuration", true), true);
        #endregion

        #region Tuhu_Groupon & Tuhu_Groupon_ReadOnly
        /// <summary>
        /// 读写 Tuhu_Groupon
        /// </summary>
        public static SqlConnection OpenTuhu_Groupon => OpenConnection(ProcessConnectionString("Tuhu_Groupon", false));
        /// <summary>
        /// 只读库 Tuhu_Groupon
        /// </summary>
        public static SqlConnection OpenTuhu_GrouponReadOnly => OpenConnection(ProcessConnectionString("Tuhu_Groupon", true));
        /// <summary>
        /// 延迟读取只读库(只读库延迟同步问题) Tuhu_Groupon
        /// </summary>
        public static SqlConnection OpenTuhu_GrouponReadOnlyForDelay => OpenConnection(ProcessConnectionString("Tuhu_Groupon", true), true);
        #endregion

        #region Tuhu_Discovery_Db
        public static SqlConnection OpenMarketing => OpenConnection(ProcessConnectionString("Tuhu_Discovery_Db", false));

        public static SqlConnection OpenMarketingReadOnly => OpenConnection(ProcessConnectionString("Tuhu_Discovery_Db", true));

        #endregion


        /// <summary>
        /// 创建只读连接字符串
        /// </summary>
        public static string ProcessConnectionString(string connectionStringName, bool readOnly = false)
        {
            var gungnir = ConfigurationManager.ConnectionStrings[connectionStringName];

            if (gungnir == null)
                return null;

            if (string.Compare(gungnir.ProviderName, "System.Data.SqlClient", StringComparison.OrdinalIgnoreCase) != 0)
                return gungnir.ConnectionString;

            var sb = new System.Data.SqlClient.SqlConnectionStringBuilder(gungnir.ConnectionString);

            if (readOnly)
                sb.ApplicationIntent = System.Data.SqlClient.ApplicationIntent.ReadOnly;

            sb.MultipleActiveResultSets = true;

            return sb.ToString();
        }

        /// <summary>
        /// 调用此方法时请使用 using 释放资源
        /// </summary>
        public static SqlConnection OpenConnection(string connectionString, bool isDelay = false)
        {
            if (isDelay == true)
                System.Threading.Thread.Sleep(1000);

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }
    }
}