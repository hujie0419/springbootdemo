using System.Collections.Generic;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public interface IMessagePushManager
    {
        List<MessagePush> GetAllMessagePush();
        void Delete(int PKID);
        void Add(MessagePush messagePush);
        void Update(MessagePush messagePush);
        MessagePush GetMessagePushByID(int PKID);
        MessagePush GetMessagePushByEnID(string EnID);

        List<MessagePush> GetAppMessagePush();
        void DeleteAppMessagePush(int PKID);
        void AddAppMessagePush(MessagePush messagePush);
        void UpdateAppMessagePush(MessagePush messagePush);
        MessagePush GetAppMessagePushByID(int PKID);
        MessagePush GetAppMessagePushByEnID(string EnID);
    }
}
