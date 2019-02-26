using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.QRCodeStatisticsConfig
{
    public class QRCodeStatisticsConfigHandler
    {

        private readonly IDBScopeManager dbManager;
        public QRCodeStatisticsConfigHandler(IDBScopeManager dbManager)
        {
            this.dbManager = dbManager;
        }

        /// <summary>
        /// 获取微信二维码扫描事件统计
        /// </summary>
        /// <param name="sqlconn"></param>
        /// <param name="queryName"></param>
        /// <returns></returns>
        public IEnumerable<QRCodeStatisticsConfigModel> GetListByPage(string queryName)
        {
            Func<SqlConnection, IEnumerable<QRCodeStatisticsConfigModel>> action = (connection) => QRCodeStatisticsConfigDAL.GetListByPage(connection, queryName);
            return dbManager.Execute(action);
        }
    }
}
