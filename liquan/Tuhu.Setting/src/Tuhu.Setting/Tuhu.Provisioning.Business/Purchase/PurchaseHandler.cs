using System.Collections.Generic;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business.Purchase
{
    public class PurchaseHandler
    {
        #region Private Fields

        private readonly IConnectionManager connectionManager;
        private readonly IDBScopeManager dbManager;
        private readonly ILog logger;

        #endregion

        #region Ctor

        public PurchaseHandler(IConnectionManager connectionManager, IDBScopeManager dbManager, ILog logger)
        {
            this.connectionManager = connectionManager;
            this.dbManager = dbManager;
            this.logger = logger;
        }

        #endregion
        /// <summary>
        /// 获取审核人
        /// </summary>
        /// <param name="auditType">审核类型</param>
        /// <returns></returns>
        public List<string> GetBatchPurchaseAuditor(string auditType)
        {
            return dbManager.Execute(conn => DalPurchase.GetBatchPurchaseAuditor(conn, auditType));
        }

    }
}
