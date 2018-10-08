using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.Monitor;
using Tuhu.Provisioning.DataAccess.DAO.Push;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.Push;
using Tuhu.Service.Push;
using Tuhu.Service.Push.Models;
using AfterOpenEnum = Tuhu.Service.Push.Models.AfterOpenEnum;
using AndriodModel = Tuhu.Service.Push.Models.AndriodModel;
using IOSModel = Tuhu.Service.Push.Models.IOSModel;


namespace Tuhu.Provisioning.Business.Push
{
    public class PushHandler
    {
        private const int Push_Broadcast = 1;
        private const int Push_Groupcast = 3;
        private const int Push_Single = 2;
        private static readonly ILog logger = LoggerFactory.GetLogger("PushHandler");
        private static readonly string PushSourceName = "yewu.tuhu.cn";
        public static bool PushMessage(PushMessageViewModel message, IList<string> phones)
        {
            var result = false;
            var pushMessage = ConverToMessageModel(message);
            pushMessage.Type = MessageType.AppNotification;
            pushMessage.PhoneNumbers = phones;

            try
            {
                using (var client = new PushClient())
                {
                    var pushResult = client.PushMessages(pushMessage);
                    if (pushResult != null)
                    {
                        if (!string.IsNullOrEmpty(pushResult.ErrorMessage))
                        {
                            logger.Log(Level.Error, pushResult.ErrorMessage, "Error occurred in PushMessage");
                        }
                        else
                        {
                            result = pushResult.Result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "Error occurred in PushMessage");
            }

            return result;
        }

        public static bool PushTaskMessage(PushMessageViewModel message)
        {
            var result = false;
            var pushMessage = ConverToMessageModel(message);

            pushMessage.Type = message.PushType == Push_Groupcast ? MessageType.AppGroupcast : MessageType.AppBroadcast;
            if (message.PushType == Push_Groupcast)
            // 组播，添加tag的逻辑
            {
                pushMessage.Tags = message.Tags?.Split(';').ToList();
            }

            try
            {
                using (var client = new PushClient())
                {
                    var pushResult = client.PushTaskMessage(pushMessage);
                    if (pushResult != null)
                    {
                        if (!string.IsNullOrEmpty(pushResult.ErrorMessage))
                        {
                            logger.Log(Level.Error, pushResult.ErrorMessage, "Error occurred in PushTaskMessage");
                        }
                        else
                        {
                            result = pushResult.Result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex, "Error occurred in PushTaskMessage");
            }

            return result;
        }

        public static List<PushMessageViewModel> SearchPushHis(int pageIndex, int pageSize, out int count)
        {
            count = 0;
            List<PushMessageViewModel> result = null;
            try
            {
                result = DalPush.SearchPushHis(pageIndex, pageSize, out count);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex.Message);
            }

            return result;
        }

        public IList<string> GetPushMsgPersonConfig(SqlConnection connection)
        {
            IList<string> result = null;
            try
            {
                result = DalPush.GetPushMsgPersonConfig(connection);
            }
            catch (Exception exception)
            {
                throw new Exception("GetPushMsgPersonConfig:" + exception.Message);
            }
            return result;
        }

        public List<TagConfigModel> GetAllPushTag(SqlConnection connection)
        {
            List<TagConfigModel> result = null;

            try
            {
                result = DalPush.GetAllPushTag(connection);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex.Message);
                ExceptionMonitor.AddNewMonitor("Push", ex, "GetAllPushTag", MonitorLevel.Error);
            }

            return result;
        }

        public static bool DeleteMessages(int messageId, string operUser, out string errmsg)
        {
            errmsg = "";
            using (var client = new PushClient())
            {
                var pushResult = client.DeletePushMessage(messageId, operUser);

                if (pushResult != null && pushResult.Result == 1)
                    return true;
                //异常
                if (pushResult == null || pushResult.ErrorCode == "98")
                    logger.Log(Level.Error, pushResult.ErrorMessage, "Error occurred in PushTaskMessage");

                switch (pushResult.Result)
                {
                    case 2:
                        errmsg = "删除失败！超出发送时间不能删除";
                        break;
                    case 3:
                        errmsg = "删除失败！只有新建的推送消息才可以删除";
                        break;
                    case 4:
                        errmsg = "消息已经推送,不能删除";
                        break;
                    default:
                        errmsg = "删除失败！";
                        break;
                }
                return false;
            }
        }
        public static bool UpdateAPPExpireTime(int messageId, DateTime appExpireTime, out string errmsg)
        {
            errmsg = "";
            using (var client = new PushClient())
            {
                var pushResult = client.UpdateAPPExpireTime(messageId, appExpireTime);

                if (pushResult != null && pushResult.Result == 1)
                {
                    errmsg = "更新成功";
                    return true;
                }
                //异常
                if (pushResult == null || pushResult.ErrorCode == "98")
                    logger.Log(Level.Error, pushResult.ErrorMessage, "Error occurred in PushTaskMessage");

                errmsg = "删除失败！";
                return false;
            }
        }

        private static PushMessageModel ConverToMessageModel(PushMessageViewModel message)
        {
            PushMessageModel pushMessage = new PushMessageModel();

            if (message != null)
            {
                pushMessage.Description = message.Description;
                pushMessage.Title = message.Title;
                pushMessage.Content = message.Content;
                pushMessage.BeginSendTime = message.SendTime.HasValue ? message.SendTime : DateTime.Now;
                pushMessage.ExpireTime = message.ExpireTime.HasValue
                    ? message.ExpireTime
                    : pushMessage.BeginSendTime.Value.AddDays(2);
                pushMessage.AppExpireTime = message.APPExpireTime.HasValue
                    ? message.APPExpireTime
                    : pushMessage.BeginSendTime.Value.AddDays(7);
                pushMessage.RichTextImage = message.RichTextImage?.Replace("http://image.tuhu.cn", "https://img1.tuhu.org");
                pushMessage.AndriodModel = new AndriodModel
                {
                    AfterOpen = message.AfterOpen == "0"
                        ? AfterOpenEnum.GoApp
                        : AfterOpenEnum.GoActivity,
                    AppActivity = message.AppActivity,
                    ExKey1 = message.AndriodKey1,
                    ExValue1 = message.AndriodValue1,
                    ExKey2 = message.AndriodKey2,
                    ExValue2 = message.AndriodValue2
                };
                switch (message.AppMsgType)
                {
                    case "activity":
                        pushMessage.CenterMsgType = Constants.MessageType_Broadcast;
                        pushMessage.HeadImageUrl = "http://image.tuhu.cn/news/activity.png";
                        break;
                    case "discovery":
                        pushMessage.CenterMsgType = Constants.MessageType_Message;
                        pushMessage.HeadImageUrl = "http://image.tuhu.cn/news/faxian.png";
                        break;
                    case "system":
                        pushMessage.CenterMsgType = Constants.MessageType_Common;
                        pushMessage.HeadImageUrl = "http://image.tuhu.cn/news/wuliu.png";
                        break;
                    case "logistics":
                        pushMessage.CenterMsgType = Constants.MessageType_Logistics;
                        pushMessage.HeadImageUrl = "http://image.tuhu.cn/news/logistics.png";
                        break;
                    default:
                        pushMessage.CenterMsgType = Constants.MessageType_Broadcast;
                        pushMessage.HeadImageUrl = "http://image.tuhu.cn/news/activity.png";
                        break;
                }

                pushMessage.BigImagePath = message.BigImagePath;
                pushMessage.IOSModel = new IOSModel()
                {
                    ExKey1 = message.IOSKey1,
                    ExValue1 = message.IOSValue1,
                    ExKey2 = message.IOSKey2,
                    ExValue2 = message.IOSValue2,
                    ExKey3 = message.IOSKey3,
                    ExValue3 = message.IOSValue3,
                    ShowBadge = message.IOSShowBadge,
                    IOSMainTitle = message.IOSMainTitle,
                    IOSTitle = message.IOSTitle
                };

                pushMessage.SourceName = PushSourceName;
                pushMessage.OperUser = message.OperUser;
            }

            return pushMessage;
        }
    }
}
