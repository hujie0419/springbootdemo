using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.BlackListConfig
{
    public class BlackListConfigHandler
    {
        private readonly IDBScopeManager dbManager;
        public BlackListConfigHandler(IDBScopeManager dbManager)
        {
            this.dbManager = dbManager;
        }

        public IEnumerable<BlackListConfigModel> GetList(string userId)
        {
            Func<SqlConnection, IEnumerable<BlackListConfigModel>> action = (connection) => BlackListConfigDAL.GetList(connection, userId);
            return dbManager.Execute(action);
        }

        public bool Add(string userId)
        {
            Func<SqlConnection, bool> action = (connection) => BlackListConfigDAL.Add(connection, userId);
            return dbManager.Execute(action);
        }

        public bool AddOnlyUserID(string userId)
        {
            Func<SqlConnection, bool> action = (connection) => BlackListConfigDAL.AddOnlyUserID(connection, userId);
            return dbManager.Execute(action);
        }

        public bool Delete(int id)
        {
            Func<SqlConnection, bool> action = (connection) => BlackListConfigDAL.Delete(connection, id);
            return dbManager.Execute(action);
        }
    }
}
