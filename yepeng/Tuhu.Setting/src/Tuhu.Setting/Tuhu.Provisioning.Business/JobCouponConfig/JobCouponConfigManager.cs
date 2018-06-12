using System;
using System.Collections.Generic;
using System.Configuration;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.JobCouponConfig
{
    public class JobCouponConfigManager
    {
        #region Private Fields
        static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static readonly IConnectionManager connectionManager = new ConnectionManager(SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn);
        private static readonly IDBScopeManager DbScopeManager = new DBScopeManager(connectionManager);
        private static readonly ILog logger = LoggerFactory.GetLogger("JobCouponConfigManager");
        private JobCouponConfigHandler handler = null;
        #endregion

        public JobCouponConfigManager()
        {
            handler = new JobCouponConfigHandler(DbScopeManager);
        }

        public IEnumerable<JobCouponConfigModel> Select(int pageIndex = 1, int pageSieze = 10)
        {
            try
            {
                return handler.Select(pageIndex, pageSieze);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex.ToString());
                return null;
            }
        }

        public JobCouponConfigModel SelectById(int id)
        {
            try
            {
                return handler.SelectById(id);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex.ToString());
                return null;
            }
        }

        public bool Insert(JobCouponConfigModel model)
        {
            try
            {
                return handler.Insert(model);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex.ToString());
                return false;
            }
        }

        public bool Update(JobCouponConfigModel model)
        {
            try
            {
                return handler.Update(model);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex.ToString());
                return false;
            }
        }
    }
}