using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.BlackListConfig
{
    public class BlackListConfigManage
    {
        #region Private Fields
        static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static readonly IConnectionManager connectionManager = new ConnectionManager(SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn);
        private static readonly IDBScopeManager DbScopeManager = new DBScopeManager(connectionManager);
        private static readonly ILog logger = LoggerFactory.GetLogger("BlackListConfigManage");
        private BlackListConfigHandler handler = null;
        #endregion

        public BlackListConfigManage()
        {
            handler = new BlackListConfigHandler(DbScopeManager);
        }

        public IEnumerable<BlackListConfigModel> GetList(string userId)
        {
            try
            {
                return handler.GetList(userId);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex.ToString());
                return null;
            }
        }

        public bool Add(string userId)
        {
            try
            {
                return handler.Add(userId);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex.ToString());
                return false;
            }
        }

        public bool AddOnlyUserID(string userId)
        {
            try
            {
                return handler.AddOnlyUserID(userId);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex.ToString());
                return false;
            }
        }

        public bool Delete(int id)
        {
            try
            {
                return handler.Delete(id);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex.ToString());
                return false;
            }
        }
    }
}
