using System;
using System.Collections.Generic;
using System.Configuration;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.Baishi
{
    public class BaishiManager
    {
        #region Private Fields
        static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static readonly IDBScopeManager DbScopeManager = new DBScopeManager(SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn);

        private static readonly ILog Logger = LoggerFactory.GetLogger("BaishiManager");

        private readonly BaishiHandler handler = null;

        #endregion

        public BaishiManager()
        {
            handler = new BaishiHandler(DbScopeManager);
        }



        public List<ExternalShop> SelectExternalShops()
        {
            try
            {
                return handler.SelectExternalShops();
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new BaishiException(BizErrorCode.SystemError, "查询百世外包仓出错", innerEx);
                Logger.Log(Level.Error, exception, "Error occurred in selecting baishi location.");

                throw exception;
            }
        }
    }
}
