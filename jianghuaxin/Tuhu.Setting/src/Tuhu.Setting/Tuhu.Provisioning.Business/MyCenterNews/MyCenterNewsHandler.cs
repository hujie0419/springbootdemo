using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Framework;
using System.Data.SqlClient;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.DAO;

namespace Tuhu.Provisioning.Business.MyCenterNews
{
    public class MyCenterNewsHandler
    {
        private readonly IDBScopeManager dbManager;
        public MyCenterNewsHandler(IDBScopeManager dbManager)
        {
            this.dbManager = dbManager;
        }

        public bool Insert(MyCenterNewsModel model)
        {
            Func<SqlConnection, bool> action = (connection) => MyCenterNewsDAL.Insert(connection, model);
            return dbManager.Execute(action);
        }
    }
}