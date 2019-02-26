using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess;
using Tuhu.Provisioning.DataAccess.DAO;

namespace Tuhu.Provisioning.Business.DictionariesManagement
{
    public class DictionariesManager 
    {
        private static readonly IConnectionManager connectionManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString.Decrypt());
        private static readonly IConnectionManager ReadOnlyConnectionManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString);

        private static readonly IDBScopeManager DbScopeManager = new DBScopeManager(connectionManager);
        private static readonly IDBScopeManager ReadOnlyDbScopeManager = new DBScopeManager(ReadOnlyConnectionManager);
        private static readonly ILog logger = LoggerFactory.GetLogger("ProductInfomation");

        public DictionariesManager()
        {
        }

        public DataTable GetAllFinanceMark()
        {
            return ReadOnlyDbScopeManager.Execute(connection => DalDictionary.GetAllFinanceMarks(connection));
        }

        public List<Dictionary> SelectDeliveryType(string dicType)
        {
            return ReadOnlyDbScopeManager.Execute(connection => DalDictionary.SelectDeliveryType(connection, dicType));
        }
    }
}
