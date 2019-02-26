using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.FAQManagement
{
    public class FAQHandler
    {
        #region Private Fields
        private readonly IDBScopeManager dbManager;
        #endregion

        #region Ctor
        public FAQHandler(IDBScopeManager dbManager)
        {
            this.dbManager = dbManager;
        }
        #endregion

        public List<FAQ> SelectBy(string Orderchannel, string CateOne, string CateTwo, string CateThree, string Question)
        {
            Func<SqlConnection, List<FAQ>> action = (connection) => DalFAQ.SelectBy(connection, Orderchannel, CateOne, CateTwo, CateThree, Question);
            return dbManager.Execute(action);
        }
        public List<FAQ> SelectAll()
        {
            Func<SqlConnection, List<FAQ>> action = (connection) => DalFAQ.SelectAll(connection);
            return dbManager.Execute(action);
        }
        public void Delete(int PKID)
        {
            Action<SqlConnection> action = (connection) => DalFAQ.Delete(connection, PKID);
            dbManager.Execute(action);
        }
        public void Add(FAQ fAQ)
        {
            Action<SqlConnection> action = (connection) => DalFAQ.Add(connection, fAQ);
            dbManager.Execute(action);
        }
        public void Update(FAQ fAQ)
        {
            Action<SqlConnection> action = (connection) => DalFAQ.Update(connection, fAQ);
            dbManager.Execute(action);
        }
        public FAQ GetByPKID(int PKID)
        {
            Func<SqlConnection, FAQ> action = (connection) => DalFAQ.GetByPKID(connection, PKID);
            return dbManager.Execute(action);
        }

        #region  投诉FAQ
        public List<FAQ> TousuSelectBy(string Orderchannel, string CateOne, string CateTwo, string CateThree, string Question)
        {
            Func<SqlConnection, List<FAQ>> action = (connection) => DalFAQ.TousuSelectBy(connection, Orderchannel, CateOne, CateTwo, CateThree, Question);
            return dbManager.Execute(action);
        }
        public List<FAQ> TousuSelectAll()
        {
            Func<SqlConnection, List<FAQ>> action = (connection) => DalFAQ.TousuSelectAll(connection);
            return dbManager.Execute(action);
        }
        public void TousuFaqDelete(int PKID)
        {
            Action<SqlConnection> action = (connection) => DalFAQ.TousuDelete(connection, PKID);
            dbManager.Execute(action);
        }
        public void TousuFAQAdd(FAQ fAQ)
        {
            Action<SqlConnection> action = (connection) => DalFAQ.TousuAdd(connection, fAQ);
            dbManager.Execute(action);
        }
        public void TousuFaqUpdate(FAQ fAQ)
        {
            Action<SqlConnection> action = (connection) => DalFAQ.TousuUpdate(connection, fAQ);
            dbManager.Execute(action);
        }
        public FAQ TousuGetByPKID(int PKID)
        {
            Func<SqlConnection, FAQ> action = (connection) => DalFAQ.TousuGetByPKID(connection, PKID);
            return dbManager.Execute(action);
        }
        #endregion
        public ActivityIntroductionModel GetActivityIntroductionById(int ID)
        {
            Func<SqlConnection, ActivityIntroductionModel> action = (connection) => DalActivityIntroduction.GetActivityIntroductionById(connection, ID);
            return dbManager.Execute(action);
        }

    }
}
