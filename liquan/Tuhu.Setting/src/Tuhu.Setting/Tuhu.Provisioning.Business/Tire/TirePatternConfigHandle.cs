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
    public class TirePatternConfigHandle
    {
        #region
        private readonly IDBScopeManager dbManager;
        public TirePatternConfigHandle(IDBScopeManager dbManager)
        {
            this.dbManager = dbManager;
        }
        #endregion

        public List<TirePatternChangeLog> SelectTirePatternChangeLog(string pattern)
        {
            Func<SqlConnection, List<TirePatternChangeLog>> action =
                (connection) => DalTirePatternConfig.SelectTirePatternChangeLog(connection, pattern);
            return dbManager.Execute(action);
        }

        public List<TirePatternConfig> QueryTirePatternConfig(TirePatternConfigQuery query)
        {
            Func<SqlConnection, List<TirePatternConfig>> action = (connection) => DalTirePatternConfig.QueryTirePatternConfig(connection, query);
            return dbManager.Execute(action);
        }

        public List<TirePatternConfig> GetConfigByPattern(string pattern)
        {
            Func<SqlConnection, List<TirePatternConfig>> action = (connection) => DalTirePatternConfig.GetConfigByPattern(connection, pattern);
            return dbManager.Execute(action);
        }

        public List<TirePatternConfig> GetConfigByPKID(int pkid)
        {
            Func<SqlConnection, List<TirePatternConfig>> action = (connection) => DalTirePatternConfig.GetConfigByPKID(connection, pkid);
            return dbManager.Execute(action);
        }

        public List<string> GetALlPattern()
        {
            Func<SqlConnection, List<string>> action =
                (connection) => DalTirePatternConfig.GetALlPattern(connection);
            return dbManager.Execute(action);
        }

        public bool InsertTirePatternConfig(TirePatternConfig config)
        {
            Func<SqlConnection, bool> action = (connecttion) => DalTirePatternConfig.InsertTirePatternConfig(connecttion, config);
            return dbManager.Execute(action);
        }

        public bool InsertTirePatternChangeLog(TirePatternChangeLog log)
        {
            Func<SqlConnection, bool> action = (connecttion) => DalTirePatternConfig.InsertTirePatternChangeLog(connecttion, log);
            return dbManager.Execute(action);
        }

        public bool UpdateTirePatternConfigExpectPattern(TirePatternConfig config)
        {
            Func<SqlConnection, bool> action = (connecttion) => DalTirePatternConfig.UpdateTirePatternConfigExpectPattern(connecttion, config);
            return dbManager.Execute(action);
        }

        public bool UpdateTirePatternConfig(TirePatternConfig oldConfig, TirePatternConfig newConfig)
        {
            Func<SqlConnection, bool> action = (connecttion) => DalTirePatternConfig.UpdateTirePatternConfig(connecttion, oldConfig, newConfig);
            return dbManager.Execute(action);
        }

        public List<string> GetAffectedPids(string pattern)
        {
            Func<SqlConnection, List<string>> action =
                (connection) => DalTirePatternConfig.GetAffectedPids(connection, pattern);
            return dbManager.Execute(action);
        }
    }
}
