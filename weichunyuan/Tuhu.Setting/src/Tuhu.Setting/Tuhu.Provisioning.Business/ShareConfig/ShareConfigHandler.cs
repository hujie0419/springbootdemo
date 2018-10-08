using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;


namespace Tuhu.Provisioning.Business.ShareConfig
{
    public class ShareConfigHandler
    {
        private readonly IDBScopeManager dbManager;

        public ShareConfigHandler(IDBScopeManager dbManager)
        {
            this.dbManager = dbManager;
        }
        public List<ShareConfigSource> SelectShareConfig(ShareConfigQuery query)
        {
            Func<SqlConnection, List<ShareConfigSource>> action = (connection) => DalShareConfig.QueryShareConfigs(connection, query);
            return dbManager.Execute(action);
        }
        public int SelectPKIdByLocation(string location, string specialParam = null)
        {
            Func<SqlConnection, int> action = (connection) => DalShareConfig.SelectPKIdByLocation(connection, location, specialParam);
            return dbManager.Execute(action);
        }
        public string SelectLocationByPKId(int pkid)
        {
            Func<SqlConnection, string> action = (connection) => DalShareConfig.SelectLocationByPKId(connection, pkid);
            return dbManager.Execute(action);
        }
        public List<ShareSupervisionConfig> SelectShareSConfigByJumpId(int jumpId)
        {
            Func<SqlConnection, List<ShareSupervisionConfig>> action = (connection) => DalShareConfig.QueryShareSConfigByJumpId(connection, jumpId);
            return dbManager.Execute(action);
        }
        public List<ShareConfigLog> SelectShareConfigLogById(int id)
        {
            Func<SqlConnection, List<ShareConfigLog>> action = (connection) => DalShareConfig.QueryShareConfigLogById(connection, id);
            return dbManager.Execute(action);
        }
        public bool UpdateShareConfig(ShareConfigSource scs)
        {
            Func<SqlConnection, bool> action = (connection) => DalShareConfig.UpdateShareConfig(connection, scs);
            return dbManager.Execute(action);
        }
       
        public int InsertShareConfig(ShareConfigSource scs)
        {
            Func<SqlConnection, int> action = (connection) => DalShareConfig.InsertShareConfig(connection, scs);
            return dbManager.Execute(action);
        }
        public ShareSupervisionConfig SelectShareSConfigById(int id)
        {
            Func<SqlConnection, ShareSupervisionConfig> action = (connection) => DalShareConfig.QueryShareSConfigById(connection, id);
            return dbManager.Execute(action);
        }
        public bool InsertShareSConfig(ShareSupervisionConfig ssc)
        {
            Func<SqlConnection, bool> action = (connection) => DalShareConfig.InsertShareSConfig(connection, ssc);
            return dbManager.Execute(action);
        }
        public bool UpdateShareSConfig(ShareSupervisionConfig ssc)
        {
            Func<SqlConnection, bool> action = (connection) => DalShareConfig.UpdateShareSConfig(connection, ssc);
            return dbManager.Execute(action);
        }
        public bool DeleteShareSConfigById(int id)
        {
            Func<SqlConnection, bool> action = (connection) => DalShareConfig.DeleteShareSConfigById(connection, id);
            return dbManager.Execute(action);
        }
        public bool DeleteShareConfigByLocation(string location, string specialParam)
        {
            Func<SqlConnection, bool> action = (connection) => DalShareConfig.DeleteShareConfigByLocation(connection, location, specialParam);
            return dbManager.Execute(action);
        }
        public bool DeleteShareSConfigByJumpId(int jumpId)
        {
            Func<SqlConnection, bool> action = (connection) => DalShareConfig.DeleteShareSConfigByJumpId(connection, jumpId);
            return dbManager.Execute(action);
        }
        public bool InsertShareConfigLog(ShareConfigLog scl)
        {
            Func<SqlConnection, bool> action = (connection) => DalShareConfig.InsertShareConfigLog(connection, scl);
            return dbManager.Execute(action);
        }
        public bool DeleteShareConfigLog(int id)
        {
            Func<SqlConnection, bool> action = (connection) => DalShareConfig.DeleteShareConfigLog(connection, id);
            return dbManager.Execute(action);
        }
    }
}