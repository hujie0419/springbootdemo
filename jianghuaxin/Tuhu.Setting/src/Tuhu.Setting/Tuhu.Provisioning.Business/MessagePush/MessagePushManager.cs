using System.Collections.Generic;
using System.Configuration;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.MessagePushManagement;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public class MessagePushManager : IMessagePushManager
	{
		#region Private Fields
        static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static readonly IConnectionManager connectionManager = new ConnectionManager(SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn);

		private static readonly IDBScopeManager DbScopeManager = new DBScopeManager(connectionManager);
		private static readonly ILog logger = LoggerFactory.GetLogger("Coupon");

		private MessagePushHandler handler = null;

		#endregion
		public MessagePushManager()
		{
			handler = new MessagePushHandler(DbScopeManager);
		}


		public List<MessagePush> GetAllMessagePush()
		{
			return handler.GetAllMessagePush();
		}

		public void Delete(int PKID)
		{
			handler.Delete(PKID);
		}
		public void Add(MessagePush messagePush)
		{
			handler.Add(messagePush);
		}
		public void Update(MessagePush messagePush)
		{
			handler.Update(messagePush);
		}
		public MessagePush GetMessagePushByID(int PKID)
		{
			return handler.GetMessagePushByID(PKID);
		}
		public MessagePush GetMessagePushByEnID(string EnID)
		{
			return handler.GetMessagePushByEnID(EnID);
		}



        public List<MessagePush> GetAppMessagePush()
        {
            return handler.GetAppMessagePush();
        }
        public void DeleteAppMessagePush(int PKID)
        {
            handler.DeleteAppMessagePush(PKID);
        }
        public void AddAppMessagePush(MessagePush messagePush)
        {
            handler.AddAppMessagePush(messagePush);
        }
        public void UpdateAppMessagePush(MessagePush messagePush)
        {
            handler.UpdateAppMessagePush(messagePush);
        }
        public MessagePush GetAppMessagePushByID(int PKID)
        {
            return handler.GetAppMessagePushByID(PKID);
        }
        public MessagePush GetAppMessagePushByEnID(string EnID)
        {
            return handler.GetAppMessagePushByEnID(EnID);
        }

	}
}
