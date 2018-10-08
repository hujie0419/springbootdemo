using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.MyCenterNews
{
    public class MyCenterNewsManager
    {
        #region Private Fields
        static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static readonly IConnectionManager connectionManager = new ConnectionManager(SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn);
        private static readonly IDBScopeManager DbScopeManager = new DBScopeManager(connectionManager);
        private static readonly ILog logger = LoggerFactory.GetLogger("MyCenterNewsManager");
        private MyCenterNewsHandler handler = null;
        #endregion

        public MyCenterNewsManager()
        {
            handler = new MyCenterNewsHandler(DbScopeManager);
        }

        public bool Insert(MyCenterNewsModel model)
        {
            try
            {
                return handler.Insert(model);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, string.Format("方法:{0},错误:{1}", "MyCenterNewsManager.Insert", ex.Message), model);
                return false;
            }
        }
    }
}
