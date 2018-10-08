using System;
using System.Collections.Generic;
using System.Configuration;
using Tuhu.Provisioning.Business.PurchaseManagement;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business.Purchase
{
    public class PurchaseManager
    {
        private string emailCCPurchase = ConfigurationSettings.AppSettings["emailCCPurchase"];

        #region Private Fields
        private static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static readonly IConnectionManager ConnectionManager = new ConnectionManager(SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn);
        //private static readonly IConnectionManager ConnectionManager = new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);
        private static readonly IDBScopeManager DbScopeManager = new DBScopeManager(ConnectionManager);

        private readonly PurchaseHandler handler = null;

        private static readonly ILog Logger = LoggerFactory.GetLogger("PurchaseManager");

        #endregion

        #region Ctor

        public PurchaseManager()
        {
            handler = new PurchaseHandler(ConnectionManager, DbScopeManager, Logger);
        }

        #endregion


        /// <summary>
        /// 获取审核人
        /// </summary>
        /// <param name="auditType">审核类型</param>
        /// <returns></returns>
        public List<string> GetBatchPurchaseAuditor(string auditType)
        {
            try
            {
                return handler.GetBatchPurchaseAuditor(auditType);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exMessageStr = string.Format("获取审核人，出现错误");
                var exception = new PurchaseException(0, exMessageStr, ex);
                Logger.Log(Level.Error, exception, "Error occured in PurchaseManager=>GetBatchPurchaseAuditor");
                throw exception;
            }
        }

    }
}
