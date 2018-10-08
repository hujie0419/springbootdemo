using System;
using System.Collections.Generic;
using System.Configuration;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.EmployeeManagement
{
    public class EmployeeManager
    {
        #region Private Fields
        static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static readonly IConnectionManager connectionManager = new ConnectionManager(SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn);

        private static readonly ILog logger = LoggerFactory.GetLogger("Employee");
        private static readonly IDBScopeManager DbScopeManager = new DBScopeManager(connectionManager);
        private static readonly EmployeeHandler handler = new EmployeeHandler(DbScopeManager);
        #endregion
      

        public List<WMSUserObject> SelectShopsEmployeeByEmailAddress(string emailAddress)
        {
            try
            {
                return handler.SelectShopsEmployeeByEmailAddress(emailAddress);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new EmployeeException(BizErrorCode.SystemError, "查询人员所在仓库失败", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in getting WMSUserObject by emailAddress.");

                throw exception;
            }
        }

        public bool IsActiveByGroups(string groups, string userName)
        {
            try
            {
                return handler.IsActiveByGroups(groups, userName);
            }
            catch (Exception ex)
            {
                var exception = new EmployeeException(0, "根据传入组别判断是否有此人！", ex);
                logger.Log(Level.Error, exception, "Error in  HrEmployee by Groups.");
                throw exception;
            }
        }

        public List<UserGroups> GetAllUserGroups()
        {
            try
            {
                return handler.GetAllUserGroups();
            }
            catch (Exception ex)
            {
                var exception = new EmployeeException(0, "查询所有组别的的所有员工！", ex);
                logger.Log(Level.Error, exception, "Error in  HrEmployee by Groups.");
                throw exception;
            }
        }
    }
}
