using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class Dictionary
    {
        public string DicKey { get; set; }
        public string DicValue { get; set; }
        public string DicType { get; set; }

        public string DicPayMethod { get; set; }
        public string ParentKey { get; set; }
    }
    public class ConnectionHelper
    {
        /// <summary>
        /// Gungnir 数据库连接字符串
        /// </summary>
        public static readonly string Gungnir = GetConnectionString("Gungnir");

        /// <summary>
        /// 通过config文件中配置的key获取连接字符串
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        private static string GetConnectionString(string Key)
        {
            try
            {
                return ConfigurationManager.ConnectionStrings[Key].ConnectionString;
            }
            catch (Exception ex)
            {
                throw new Exception("连接字符串初始化异常..", ex);
            }
        }
        public static string GetDecryptConn(string Key)
        {
            try
            {
                var connstr = ConfigurationManager.ConnectionStrings[Key].ConnectionString;
                if (SecurityHelp.IsBase64Formatted(connstr))
                    connstr = SecurityHelp.DecryptAES(connstr);
                return connstr;
            }
            catch (Exception ex)
            {
                throw new Exception("连接字符串初始化异常..", ex);
            }

        }
    }
}
