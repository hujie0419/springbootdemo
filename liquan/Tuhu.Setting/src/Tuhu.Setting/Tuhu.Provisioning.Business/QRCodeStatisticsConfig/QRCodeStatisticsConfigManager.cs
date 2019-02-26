using System;
using System.Collections.Generic;
using System.Configuration;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.QRCodeStatisticsConfig
{
    public class QRCodeStatisticsConfigManager
    {
        #region Private Fields
        static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static readonly IConnectionManager connectionManager = new ConnectionManager(SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn);
        private static readonly IDBScopeManager DbScopeManager = new DBScopeManager(connectionManager);
        private static readonly ILog logger = LoggerFactory.GetLogger("QRCodeStatisticsConfig");
        private QRCodeStatisticsConfigHandler handler = null;
        #endregion

        public QRCodeStatisticsConfigManager()
        {
            handler = new QRCodeStatisticsConfigHandler(DbScopeManager);
        }

        /// <summary>
        /// 获取微信二维码扫描事件统计
        /// </summary>
        /// <param name="sqlconn"></param>
        /// <param name="queryName"></param>
        /// <returns></returns>
        public IEnumerable<QRCodeStatisticsConfigModel> GetListByPage(string queryName)
        {
            try
            {
                return handler.GetListByPage(queryName);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex.ToString());
                return null;
            }
        }
    }
}
