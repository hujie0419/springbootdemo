using System.Collections.Generic;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.EmployeeManagement
{
    internal class EmployeeHandler
    {
        #region Private Fields

        private readonly IConnectionManager connectionManager;
        private readonly IDBScopeManager DbScopeManager;

        #endregion

        #region Ctor

        internal EmployeeHandler(IConnectionManager connectionManager)
        {
            this.connectionManager = connectionManager;
        }

        internal EmployeeHandler(IDBScopeManager _DbScopeManager)
        {
            this.DbScopeManager = _DbScopeManager;
        }

        #endregion

        public List<WMSUserObject> SelectShopsEmployeeByEmailAddress(string emailAddress)
        {
            ParameterChecker.CheckNull(emailAddress, "emailAddress");
            return DbScopeManager.Execute(conn => DalEmployee.SelectShopsEmployeeByEmailAddress(conn, emailAddress));
        }

        public bool IsActiveByGroups(string groups, string userName)
        {
            return DbScopeManager.Execute(conn => DalEmployee.IsActiveByGroups(conn, groups, userName));
        }

        public List<UserGroups> GetAllUserGroups()
        {
            return DbScopeManager.Execute(connection => DalEmployee.GetAllUserGroups(connection));
        }
    }
}
