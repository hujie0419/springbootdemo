using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.DAO.Tire;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.Tire;

namespace Tuhu.Provisioning.Business.Tire
{
    public class TireSpecParamsConfigHandle
    {
        #region
        private readonly IDBScopeManager dbManager;
        public TireSpecParamsConfigHandle(IDBScopeManager dbManager)
        {
            this.dbManager = dbManager;
        }
        #endregion

        public List<TireSpecParamsEditLog> SelectTireSpecParamsEditLog(string pid)
        {
            Func<SqlConnection, List<TireSpecParamsEditLog>> action =
                (connection) => DalTireSpecParamsConfig.SelectTireSpecParamsEditLog(connection, pid);
            return dbManager.Execute(action);
        }

        public List<TireSpecParamsModel> QueryTireSpecParamsModel(TireSpecParamsConfigQuery query)
        {
            Func<SqlConnection, List<TireSpecParamsModel>> action = (connection) => DalTireSpecParamsConfig.QueryTireSpecParamsModel(connection, query);
            return dbManager.Execute(action);
        }

        public TireSpecParamsModel SelectTireSpecParamsModelByPid(string pid)
        {
            Func<SqlConnection, TireSpecParamsModel> action =
                (connection) => DalTireSpecParamsConfig.SelectTireSpecParamsModelByPid(connection, pid);
            return dbManager.Execute(action);
        }

        public bool InsertTireSpecParamsConfig(TireSpecParamsConfig config)
        {
            Func<SqlConnection, bool> action = (connection) => DalTireSpecParamsConfig.InsertTireSpecParamsConfig(connection, config);
            return dbManager.Execute(action);
        }

        public bool UpdateTireSpecParamsConfig(TireSpecParamsConfig config)
        {
            Func<SqlConnection, bool> action = (connection) => DalTireSpecParamsConfig.UpdateTireSpecParamsConfig(connection, config);
            return dbManager.Execute(action);
        }

        public bool InsertTireSpecParamsEditLog(TireSpecParamsEditLog log)
        {
            Func<SqlConnection, bool> action = (connection) => DalTireSpecParamsConfig.InsertTireSpecParamsEditLog(connection, log);
            return dbManager.Execute(action);
        }

        public bool CheckPidExist(string pid)
        {
            Func<SqlConnection, bool> action = (connection) => DalTireSpecParamsConfig.CheckPidExist(connection, pid);
            return dbManager.Execute(action);
        }
    }
}
