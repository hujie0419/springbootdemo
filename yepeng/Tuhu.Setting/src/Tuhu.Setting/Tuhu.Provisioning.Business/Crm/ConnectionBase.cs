using System;
using System.Data;
using System.Data.SqlClient;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business.Crm
{
    /// <summary>
    ///数据连接基类
    /// </summary>
    public class ConnectionBase
    {
        #region 数据库连接
        private string _connectionStr = null;
        public string connectionStr
        {
            get
            {
                try
                {
                    if (_connectionStr == null && string.IsNullOrEmpty(_connectionStr))
                        return _connectionStr = System.Configuration.ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
                    else
                        return _connectionStr;
                }
                catch (Exception ex)
                {
                    return _connectionStr = string.Format("数据库Key：{0} 不存在；错误信息：{1}", _connectionStr, ex.ToString());
                }
            }
        }

        private SqlConnection _SqlConn = null;
        public SqlConnection SqlConnectionOpen
        {
            get
            {
                try
                {
                    if (_SqlConn == null)
                    {
                        _SqlConn = new SqlConnection(connectionStr);
                        if (_SqlConn.State != ConnectionState.Open)
                            _SqlConn.Open();
                        return _SqlConn;
                    }
                    else
                    {
                        if (_SqlConn.State != ConnectionState.Open)
                            _SqlConn.Open();
                        return _SqlConn;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        #endregion

        #region 日志初始化
        protected static readonly ILog logger = LoggerFactory.GetLogger("CRM");
        #endregion
    }
}
