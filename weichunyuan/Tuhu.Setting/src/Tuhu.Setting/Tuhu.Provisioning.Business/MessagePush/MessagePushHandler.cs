using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.DAO;

namespace Tuhu.Provisioning.Business.MessagePushManagement
{
    public class MessagePushHandler
    {
        #region Private Fields
        private readonly IDBScopeManager dbManager;
        #endregion

        #region Ctor
        public MessagePushHandler(IDBScopeManager dbManager)
        {
            this.dbManager = dbManager;
        }
        #endregion


        public List<MessagePush> GetAllMessagePush()
        {
            Func<SqlConnection, List<MessagePush>> action = (connection) => DalMessagePush.GetAllMessagePush(connection);
            return dbManager.Execute(action);
        }
        public void Delete(int PKID)
        {
            Action<SqlConnection> action = (connection) => DalMessagePush.Delete(connection, PKID);
            dbManager.Execute(action);
        }
        public void Add(MessagePush messagePush)
        {
            Action<SqlConnection> action = (connection) => DalMessagePush.Add(connection, messagePush);
            dbManager.Execute(action);
        }
        public void Update(MessagePush messagePush)
        {
            Action<SqlConnection> action = (connection) => DalMessagePush.Update(connection, messagePush);
            dbManager.Execute(action);
        }
        public MessagePush GetMessagePushByID(int PKID)
        {
            Func<SqlConnection, MessagePush> action = (connection) => DalMessagePush.GetMessagePushByID(connection, PKID);
            return dbManager.Execute(action);
        }
        public MessagePush GetMessagePushByEnID(string EnID)
        {
            Func<SqlConnection, MessagePush> action = (connection) => DalMessagePush.GetMessagePushByEnID(connection, EnID);
            return dbManager.Execute(action);
        }

        public List<MessagePush> GetAppMessagePush()
        {
            Func<SqlConnection, List<MessagePush>> action = (connection) => DalMessagePush.GetAppMessagePush(connection);
            return dbManager.Execute(action);
        }
        public void DeleteAppMessagePush(int PKID)
        {
            Action<SqlConnection> action = (connection) => DalMessagePush.DeleteAppMessagePush(connection, PKID);
            dbManager.Execute(action);
        }
        public void AddAppMessagePush(MessagePush messagePush)
        {
            Action<SqlConnection> action = (connection) => DalMessagePush.AddAppMessagePush(connection, messagePush);
            dbManager.Execute(action);
        }
        public void UpdateAppMessagePush(MessagePush messagePush)
        {
            Action<SqlConnection> action = (connection) => DalMessagePush.UpdateAppMessagePush(connection, messagePush);
            dbManager.Execute(action);
        }
        public MessagePush GetAppMessagePushByID(int PKID)
        {
            Func<SqlConnection, MessagePush> action = (connection) => DalMessagePush.GetAppMessagePushByID(connection, PKID);
            return dbManager.Execute(action);
        }
        public MessagePush GetAppMessagePushByEnID(string EnID)
        {
            Func<SqlConnection, MessagePush> action = (connection) => DalMessagePush.GetAppMessagePushByEnID(connection, EnID);
            return dbManager.Execute(action);
        }
    }
}
