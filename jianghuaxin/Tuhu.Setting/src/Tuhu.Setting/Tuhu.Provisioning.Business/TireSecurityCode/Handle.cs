using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.DAO.TireSecurityCode;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.TireSecurityCode;

namespace Tuhu.Provisioning.Business.TireSecurityCode
{
    public class Handle
    {
        #region
        private readonly IDBScopeManager dbManager;
        public Handle(IDBScopeManager dbManager)
        {
            this.dbManager = dbManager;
        }
        #endregion

        public List<TireSecurityCodeConfig> QuerySecurityCodeConfigModel(TireSecurityCodeConfigQuery query)
        {
            Func<SqlConnection, List<TireSecurityCodeConfig>> action = (connection) => DalTireSecurityCodeConfig.QuerySecurityCodeConfigModel(connection, query);
            return dbManager.Execute(action);
        }

        public List<UploadSecurityCodeLog> QueryUploadSecurityCodeLogModel(LogSearchQuery query)
        {
            Func<SqlConnection, List<UploadSecurityCodeLog>> action = (connection) => DalTireSecurityCodeConfig.QueryUploadSecurityCodeLogModel(connection, query);
            return dbManager.Execute(action);
        }

        public List<UploadBarCodeLog> QueryUploadBarCodeLogModel(LogSearchQuery query)
        {
            Func<SqlConnection, List<UploadBarCodeLog>> action = (connection) => DalTireSecurityCodeConfig.QueryUploadBarCodeLogModel(connection, query);
            return dbManager.Execute(action);
        }

        public bool InsertTireSecurityCodeConfig(List<TireSecurityCodeConfig> list)
        {
            return DalTireSecurityCodeConfig.InsertTireSecurityCodeConfig(list);
        }

        public int InsertBarCodeConfig(List<InputBarCode> list)
        {
            return DalTireSecurityCodeConfig.InsertBarCodeConfig(list);
        }

        public List<InputBarCode> QueryInputBarCodeByError(string error, List<InputBarCode> list)
        {
            return DalTireSecurityCodeConfig.QueryInputBarCodeByError(error, list);
        }

        public List<TireSecurityCodeConfig> QuerySecurityCodeConfigModelByBatchNum(string batchNum)
        {
            Func<SqlConnection, List<TireSecurityCodeConfig>> action = (connection) => DalTireSecurityCodeConfig.QuerySecurityCodeConfigModelByBatchNum(connection, batchNum);
            return dbManager.Execute(action);
        }

        public bool DeleleSecurityCodeConfigModelByBatchNum(string batchNum)
        {
            Func<SqlConnection, bool> action = (connection) => DalTireSecurityCodeConfig.DeleleSecurityCodeConfigModelByBatchNum(connection, batchNum);
            return dbManager.Execute(action);
        }

        public bool DeleleBarCodeByBatchNum(string batchNum)
        {
            Func<SqlConnection, bool> action = (connection) => DalTireSecurityCodeConfig.DeleleBarCodeByBatchNum(connection, batchNum);
            return dbManager.Execute(action);
        }

        public bool InsertUploadSecurityCodeLog(UploadSecurityCodeLog log)
        {
            Func<SqlConnection, bool> action = (connection) => DalTireSecurityCodeConfig.InsertUploadSecurityCodeLog(connection, log);
            return dbManager.Execute(action);
        }

        public bool InsertUploadBarCodeLog(UploadBarCodeLog log)
        {
            Func<SqlConnection, bool> action = (connection) => DalTireSecurityCodeConfig.InsertUploadBarCodeLog(connection, log);
            return dbManager.Execute(action);
        }
    }
}
