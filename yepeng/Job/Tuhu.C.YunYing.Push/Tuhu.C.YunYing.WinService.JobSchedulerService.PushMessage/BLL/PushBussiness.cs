using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using RestSharp;
using Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.UMApi.Core;
using Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.DAL;
using Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.Model;
using Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.UMApi;
using Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.UMApi.Base;
using Common.Logging;

namespace Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.BLL
{
    public class PushBussiness
    {
        private static readonly ILog Logger = LogManager.GetLogger<PushBussiness>();

        private static string andriodAppKey = ConfigurationManager.AppSettings["UmAndroidappkey"];
        private static string andriodAppSecret = ConfigurationManager.AppSettings["UmAndroidapp_master_secret"];
        private static string iosAppKey = ConfigurationManager.AppSettings["UmiOSappkey"];
        private static string iosAppSecret = ConfigurationManager.AppSettings["UmiOSapp_master_secret"];
        private static string receiptUrl = ConfigurationManager.AppSettings["UM:ReceiptUrl"];
        private static string receiptType = ConfigurationManager.AppSettings["UM:ReceiptType"];
        private static string broadcastSwitch = ConfigurationManager.AppSettings["UM:BroadcastSwitch"];
        private static string PushMessageReceiptQueue = "PushMessageReceipt";

        private static string AndroidRichTextImageKey = "RichTextImage";

        private static int MaxSendNum = Convert.ToInt32(ConfigurationManager.AppSettings["MaxSendNum"]);
        /// <summary>
        /// Android单播 （别名）
        /// </summary>
        /// <param name="messages"></param>
        internal static void SendAndroidMessageByAlias(IList<MessageModel> messages)
        {
            if (messages != null && messages.Any())
            {
                foreach (var message in messages)
                {
                    if (message != null)
                    {
                        // 有手机号的用户需要判断通知开关是否打开
                        if (!string.IsNullOrWhiteSpace(message.PhoneNumber)
                            && ((message.CenterMsgType == Service.Push.Models.Constants.MessageType_Common && !message.Notice) ||
                                 (message.CenterMsgType == Service.Push.Models.Constants.MessageType_Broadcast && !message.Radio) ||
                                 (message.CenterMsgType == Service.Push.Models.Constants.MessageType_Logistics && !message.Logistics) ||
                                 (message.CenterMsgType == Service.Push.Models.Constants.MessageType_Message && !message.Privateletter)
                                ))
                        {
                            message.Status = "Close";
                            message.Note = "推送开关关闭";
                            TuhuNotification.SendNotification(PushMessageReceiptQueue, message);
                            continue;
                        }

                        UMengMessagePush umAndriodPush = new UMengMessagePush(andriodAppKey, andriodAppSecret);
                        PostUMengAndroidJson postJson = new PostUMengAndroidJson();
                        postJson.type = "customizedcast";
                        postJson.thirdparty_id = Guid.NewGuid().ToString();
                        postJson.receipt_type = Convert.ToInt32(receiptType);
                        postJson.receipt_url = receiptUrl;
                        postJson.alias = message.PhoneNumber;
                        postJson.description = message.Description;
                        postJson.alias_type = "TUHU";
                        postJson.payload = new AndroidPayload();
                        postJson.payload.body = new ContentBody();
                        postJson.production_mode = "true";
                        postJson.payload.extra = new SerializableDictionary<string, string>();

                        if (message.MessageType == 1)
                        // 推送消息
                        {
                            postJson.payload.display_type = "message";
                            postJson.payload.body.custom = message.Body;
                        }
                        else
                        // 推送通知
                        {
                            postJson.payload.display_type = "notification";
                            postJson.payload.body.ticker = message.Subject;
                            postJson.payload.body.title = message.Subject;
                            postJson.payload.body.text = message.Body;

                            if (message.AfterOpen == "GoApp")
                            // 打开app
                            {
                                postJson.payload.body.after_open = "go_app";
                            }
                            else
                            // 自定义打开行为
                            {
                                postJson.payload.body.after_open = "go_activity";
                                postJson.payload.body.activity = message.AppActivity;
                            }
                        }

                        #region 设置自定义参数
                        if (!string.IsNullOrWhiteSpace(message.ExKey1))
                        {
                            postJson.payload.extra[message.ExKey1] = message.ExValue1;
                        }
                        if (!string.IsNullOrWhiteSpace(message.ExKey2))
                        {
                            postJson.payload.extra[message.ExKey2] = message.ExValue2;
                        }
                        if (!string.IsNullOrWhiteSpace(message.RichTextImage))
                        {
                            postJson.payload.extra[AndroidRichTextImageKey] = message.RichTextImage;
                        }
                        #endregion

                        if (message.ExpiredTimd.HasValue)
                        // 设置过期时间
                        {
                            postJson.policy = new Policy();
                            postJson.policy.expire_time = message.ExpiredTimd.Value.ToString("yyyy-MM-dd HH:mm:ss");
                        }

                        ReturnJsonClass result = umAndriodPush.SendMessage(postJson);
                        message.ActualSendTime = DateTime.Now;
                        message.TuhuId = postJson.thirdparty_id;

                        #region 处理推送结果
                        if (!string.IsNullOrEmpty(result?.ret)
                            && result.ret.Equals("SUCCESS", StringComparison.InvariantCultureIgnoreCase))
                        {
                            message.Status = "Success";
                            message.UMMessageId = result.data?.msg_id;
                        }
                        else
                        {
                            message.Status = "Fail";
                            message.Note = SimpleJson.SerializeObject(result);
                        }

                        TuhuNotification.SendNotification(PushMessageReceiptQueue, message);
                        //DalPushMessage.UpdateMessageModel(message);
                        #endregion
                    }
                }
            }
        }

        /// <summary>
        /// Android单播
        /// </summary>
        /// <param name="messages"></param>
        internal static void SendAndroidMessage(IList<MessageModel> messages)
        {
            if (messages != null && messages.Any())
            {
                foreach (var message in messages)
                {
                    if (message != null)
                    {
                        // 有手机号的用户需要判断通知开关是否打开
                        if (!string.IsNullOrWhiteSpace(message.PhoneNumber)
                            && ((message.CenterMsgType == Service.Push.Models.Constants.MessageType_Common && !message.Notice) ||
                                 (message.CenterMsgType == Service.Push.Models.Constants.MessageType_Broadcast && !message.Radio) ||
                                 (message.CenterMsgType == Service.Push.Models.Constants.MessageType_Logistics && !message.Logistics) ||
                                 (message.CenterMsgType == Service.Push.Models.Constants.MessageType_Message && !message.Privateletter)
                                ))
                        {
                            message.Status = "Close";
                            message.Note = "推送开关关闭";
                            TuhuNotification.SendNotification(PushMessageReceiptQueue, message);
                            continue;
                        }

                        UMengMessagePush umAndriodPush = new UMengMessagePush(andriodAppKey, andriodAppSecret);
                        PostUMengAndroidJson postJson = new PostUMengAndroidJson();
                        postJson.type = "unicast";
                        postJson.thirdparty_id = Guid.NewGuid().ToString();
                        postJson.receipt_type = Convert.ToInt32(receiptType);
                        postJson.device_tokens = message.DeviceToken;
                        postJson.receipt_url = receiptUrl;
                        postJson.description = message.Description;
                        postJson.payload = new AndroidPayload();
                        postJson.payload.body = new ContentBody();
                        postJson.production_mode = "true";
                        postJson.payload.extra = new SerializableDictionary<string, string>();

                        if (message.MessageType == 1)
                        // 推送消息
                        {
                            postJson.payload.display_type = "message";
                            postJson.payload.body.custom = message.Body;
                        }
                        else
                        // 推送通知
                        {
                            postJson.payload.display_type = "notification";
                            postJson.payload.body.ticker = message.Subject;
                            postJson.payload.body.title = message.Subject;
                            postJson.payload.body.text = message.Body;

                            if (message.AfterOpen == "GoApp")
                            // 打开app
                            {
                                postJson.payload.body.after_open = "go_app";
                            }
                            else
                            // 自定义打开行为
                            {
                                postJson.payload.body.after_open = "go_activity";
                                postJson.payload.body.activity = message.AppActivity;
                            }
                        }

                        #region 设置自定义参数
                        if (!string.IsNullOrWhiteSpace(message.ExKey1))
                        {
                            postJson.payload.extra[message.ExKey1] = message.ExValue1;
                        }
                        if (!string.IsNullOrWhiteSpace(message.ExKey2))
                        {
                            postJson.payload.extra[message.ExKey2] = message.ExValue2;
                        }
                        if (!string.IsNullOrWhiteSpace(message.RichTextImage))
                        {
                            postJson.payload.extra[AndroidRichTextImageKey] = message.RichTextImage;
                        }
                        #endregion

                        if (message.ExpiredTimd.HasValue)
                        // 设置过期时间
                        {
                            postJson.policy = new Policy();
                            postJson.policy.expire_time = message.ExpiredTimd.Value.ToString("yyyy-MM-dd HH:mm:ss");
                        }

                        ReturnJsonClass result = umAndriodPush.SendMessage(postJson);
                        message.ActualSendTime = DateTime.Now;
                        message.TuhuId = postJson.thirdparty_id;

                        #region 处理推送结果
                        if (!string.IsNullOrEmpty(result?.ret)
                            && result.ret.Equals("SUCCESS", StringComparison.InvariantCultureIgnoreCase))
                        {
                            message.Status = "Success";
                            message.UMMessageId = result.data?.msg_id;
                        }
                        else
                        {
                            message.Status = "Fail";
                            message.Note = SimpleJson.SerializeObject(result);
                        }

                        TuhuNotification.SendNotification(PushMessageReceiptQueue, message);
                        //DalPushMessage.UpdateMessageModel(message);
                        #endregion
                    }
                }
            }
        }

        /// <summary>
        /// IOS单播
        /// </summary>
        /// <param name="messages"></param>
        internal static void SendIOSMessage(IList<MessageModel> messages)
        {
            if (messages.Any())
            {
                foreach (var message in messages)
                {
                    if (message != null)
                    {
                        // 有手机号的用户需要判断通知开关是否打开
                        if (!string.IsNullOrWhiteSpace(message.PhoneNumber)
                            && ((message.CenterMsgType == Service.Push.Models.Constants.MessageType_Common && !message.Notice) ||
                                 (message.CenterMsgType == Service.Push.Models.Constants.MessageType_Broadcast && !message.Radio) ||
                                 (message.CenterMsgType == Service.Push.Models.Constants.MessageType_Logistics && !message.Logistics) ||
                                 (message.CenterMsgType == Service.Push.Models.Constants.MessageType_Message && !message.Privateletter)
                                ))
                        {
                            message.Status = "Close";
                            message.Note = "推送开关关闭";
                            TuhuNotification.SendNotification(PushMessageReceiptQueue, message);
                            continue;
                        }

                        UMengMessagePush umIosPush = new UMengMessagePush(iosAppKey, iosAppSecret);
                        PostUMIosJson postJson = new PostUMIosJson();
                        postJson.type = "unicast";
                        postJson.thirdparty_id = Guid.NewGuid().ToString();
                        postJson.device_tokens = message.DeviceToken;
                        postJson.receipt_type = Convert.ToInt32(receiptType);
                        postJson.receipt_url = receiptUrl;
                        postJson.description = message.Description;
                        postJson.production_mode = "true";
                        postJson.payload = new JsonObject();
                        var apsJson = new JsonObject();
                        //IOS10 推送
                        if (!string.IsNullOrEmpty(message.IOSMainTitle) || !string.IsNullOrEmpty(message.IOSTitle))
                        {
                            apsJson.Add("alert", new
                            {
                                title = message.IOSMainTitle,
                                subtitle = message.IOSTitle,
                                body = message.Body,
                            });
                        }
                        else
                        {
                            apsJson.Add("alert", string.IsNullOrWhiteSpace(message.Subject) ? message.Body
                                : message.Subject + message.Body);
                        }

                        if (message.IOSShowBadge.HasValue && message.IOSShowBadge.Value)
                        {
                            apsJson.Add("badge", 1);// 设置角标，目前数量没有维护，统一设置为1
                        }

                        //ios10富文本图片
                        if (!string.IsNullOrEmpty(message.RichTextImage))
                        {
                            apsJson.Add("mutable-content", "1");
                            apsJson.Add("image", (message.RichTextImage));
                        }

                        postJson.payload.Add("aps", apsJson);
                        
                        #region 设置自定义参数 
                        //todo:增加验证 IOS KEY "d","p"为友盟保留字段
                        if (!string.IsNullOrWhiteSpace(message.ExKey1))
                        {
                            postJson.payload.Add(message.ExKey1, message.ExValue1);
                        }
                        if (!string.IsNullOrWhiteSpace(message.ExKey2))
                        {
                            postJson.payload.Add(message.ExKey2, message.ExValue2);
                        }
                        if (!string.IsNullOrWhiteSpace(message.ExKey3))
                        {
                            postJson.payload.Add(message.ExKey3, message.ExValue3);
                        }
                        #endregion

                        if (message.ExpiredTimd.HasValue)
                        // 设置过期时间
                        {
                            postJson.policy = new Policy();
                            postJson.policy.expire_time = message.ExpiredTimd.Value.ToString("yyyy-MM-dd HH:mm:ss");
                        }

                        ReturnJsonClass result = umIosPush.SendMessage(postJson);
                        message.ActualSendTime = DateTime.Now;
                        message.TuhuId = postJson.thirdparty_id;

                        #region 处理推送结果
                        if (!string.IsNullOrEmpty(result?.ret)
                            && result.ret.Equals("SUCCESS", StringComparison.InvariantCultureIgnoreCase))
                        {
                            message.Status = "Success";
                            message.UMMessageId = result.data?.msg_id;
                        }
                        else
                        {
                            message.Status = "Fail";
                            message.Note = SimpleJson.SerializeObject(result);
                        }

                        TuhuNotification.SendNotification(PushMessageReceiptQueue, message);
                        //DalPushMessage.UpdateMessageModel(message);
                        #endregion
                    }
                }
            }
        }

        /// <summary>
        /// Android组播
        /// </summary>
        internal static void SendAndroidGroupcastMessage(IList<MessageModel> messages)
        {
            if (messages != null && messages.Any())
            {
                foreach (var message in messages)
                {
                    if (message != null)
                    {
                        UMengMessagePush umAndriodPush = new UMengMessagePush(andriodAppKey, andriodAppSecret);
                        PostUMengAndroidJson postJson = new PostUMengAndroidJson();
                        postJson.type = "groupcast";
                        postJson.thirdparty_id = Guid.NewGuid().ToString();
                        postJson.description = message.Description;
                        postJson.payload = new AndroidPayload();
                        postJson.payload.body = new ContentBody();
                        postJson.production_mode = broadcastSwitch;
                        //postJson.production_mode = "true";
                        postJson.payload.extra = new SerializableDictionary<string, string>();

                        postJson.payload.display_type = "notification";
                        postJson.payload.body.ticker = message.Subject;
                        postJson.payload.body.title = message.Subject;
                        postJson.payload.body.text = message.Body;

                        if (message.AfterOpen == "GoApp")
                        // 打开app
                        {
                            postJson.payload.body.after_open = "go_app";
                        }
                        else
                        // 自定义打开行为
                        {
                            postJson.payload.body.after_open = "go_activity";
                            postJson.payload.body.activity = message.AppActivity;
                        }

                        #region 设置自定义参数
                        if (!string.IsNullOrWhiteSpace(message.ExKey1))
                        {
                            postJson.payload.extra[message.ExKey1] = message.ExValue1;
                        }
                        if (!string.IsNullOrWhiteSpace(message.ExKey2))
                        {
                            postJson.payload.extra[message.ExKey2] = message.ExValue2;
                        }
                        if (!string.IsNullOrWhiteSpace(message.RichTextImage))
                        {
                            postJson.payload.extra[AndroidRichTextImageKey] = message.RichTextImage;
                        }
                        #endregion

                        if (message.TaskType == "AppGroupcast")
                        // 组播
                        {
                            postJson.filter = new JsonObject();
                            var tags = message.Tags.Split(',').ToList();
                            tags = tags.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

                            if (tags.Any())
                            {
                                postJson.filter = new JsonObject();
                                var tagsJson = new List<object>(tags.Count);
                                foreach (var item in tags)
                                {
                                    var tag = new JsonObject();
                                    tag.Add("tag", item);
                                    tagsJson.Add(tag);
                                }

                                var f2 = new JsonObject[1];
                                f2[0] = new JsonObject();
                                f2[0].Add("or", tagsJson);
                                var f3 = new JsonObject();
                                f3.Add("and", f2);
                                postJson.filter.Add("where", f3);
                            }
                            else
                            {
                                message.Status = "Fail";
                                message.Note = "no tags";
                                DalPushMessage.UpdateTaskMessageModel(message);
                                Logger.Error("PushUMApi=>" + message.Id + ":" + message.Note);
                                return;
                            }
                        }
                        else
                        // 广播
                        {
                            #region 组装标签 (排除关闭广播的用户)
                            postJson.filter = new JsonObject();
                            var tag = new JsonObject();
                            tag.Add("tag", "Not_Accept_Advertising");
                            var f1 = new JsonObject[1];
                            f1[0] = new JsonObject();
                            f1[0].Add("not", tag);
                            var f2 = new JsonObject[1];
                            f2[0] = new JsonObject();
                            f2[0].Add("or", f1);
                            var f3 = new JsonObject();
                            f3.Add("and", f2);
                            postJson.filter.Add("where", f3);
                            #endregion
                        }

                        if (message.ExpiredTimd.HasValue)
                        // 设置过期时间
                        {
                            postJson.policy = new Policy();
                            postJson.policy.expire_time = message.ExpiredTimd.Value.ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        if (postJson.policy == null)
                        {
                            postJson.policy = new Policy() { max_send_num = MaxSendNum };
                        }
                        else
                        {
                            postJson.policy.max_send_num = MaxSendNum;
                        }
                        ReturnJsonClass result = umAndriodPush.SendMessage(postJson);
                        message.ActualSendTime = DateTime.Now;
                        message.TuhuId = postJson.thirdparty_id;

                        #region 处理推送结果
                        if (!string.IsNullOrEmpty(result?.ret)
                            && result.ret.Equals("SUCCESS", StringComparison.InvariantCultureIgnoreCase))
                        {
                            message.Status = "Success";
                            message.UMTaskId = result.data?.task_id;
                        }
                        else
                        {
                            message.Status = "Fail";
                            message.Note = SimpleJson.SerializeObject(result);
                            Logger.Error("PushUMApi=>" + message.Id + ":" + message.Note);
                        }

                        DalPushMessage.UpdateTaskMessageModel(message);
                        #endregion
                    }
                }
            }
        }

        /// <summary>
        /// IOS组播
        /// </summary>
        /// <param name="messages"></param>
        internal static void SendIOSGroupcastMessage(IList<MessageModel> messages)
        {
            if (messages.Any())
            {
                foreach (var message in messages)
                {
                    if (message != null)
                    {
                        UMengMessagePush umIosPush = new UMengMessagePush(iosAppKey, iosAppSecret);
                        PostUMIosJson postJson = new PostUMIosJson();
                        postJson.type = "groupcast";
                        postJson.thirdparty_id = Guid.NewGuid().ToString();
                        postJson.description = message.Description;
                        postJson.production_mode = broadcastSwitch;
                        //postJson.production_mode = "true";
                        postJson.payload = new JsonObject();
                        var apsJson = new JsonObject();
                        //IOS10 推送
                        if (!string.IsNullOrEmpty(message.IOSMainTitle) || !string.IsNullOrEmpty(message.IOSTitle))
                        {
                            apsJson.Add("alert", new
                            {
                                title = message.IOSMainTitle,
                                subtitle = message.IOSTitle,
                                body = message.Body,
                            });
                        }
                        else
                        {
                            apsJson.Add("alert", string.IsNullOrWhiteSpace(message.Subject) ? message.Body
                                : message.Subject + message.Body);
                        }

                        if (message.IOSShowBadge.HasValue && message.IOSShowBadge.Value)
                        {
                            apsJson.Add("badge", 1);// 设置角标，目前数量没有维护，统一设置为1
                        }

                        //ios10富文本图片
                        if (!string.IsNullOrEmpty(message.RichTextImage))
                        {
                            apsJson.Add("mutable-content", "1");
                            apsJson.Add("image", (message.RichTextImage));
                        }

                        postJson.payload.Add("aps", apsJson);

                        if (message.TaskType == "AppGroupcast")
                        // 组播
                        {
                            postJson.filter = new JsonObject();
                            var tags = message.Tags.Split(',').ToList();
                            tags = tags.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

                            if (tags.Any())
                            {
                                postJson.filter = new JsonObject();
                                var tagsJson = new List<object>(tags.Count);
                                foreach (var item in tags)
                                {
                                    var tag = new JsonObject();
                                    tag.Add("tag", item);
                                    tagsJson.Add(tag);
                                }

                                var f2 = new JsonObject[1];
                                f2[0] = new JsonObject();
                                f2[0].Add("or", tagsJson);
                                var f3 = new JsonObject();
                                f3.Add("and", f2);
                                postJson.filter.Add("where", f3);
                            }
                            else
                            {
                                message.Status = "Fail";
                                message.Note = "no tags";
                                DalPushMessage.UpdateTaskMessageModel(message);
                                Logger.Error("PushUMApi=>" + message.Id + ":" + message.Note);
                                return;
                            }
                        }
                        else
                        // 广播
                        {
                            #region 组装标签 (排除关闭广播的用户)
                            postJson.filter = new JsonObject();
                            var tag = new JsonObject();
                            tag.Add("tag", "Not_Accept_Advertising");
                            var f1 = new JsonObject[1];
                            f1[0] = new JsonObject();
                            f1[0].Add("not", tag);
                            var f2 = new JsonObject[1];
                            f2[0] = new JsonObject();
                            f2[0].Add("or", f1);
                            var f3 = new JsonObject();
                            f3.Add("and", f2);
                            postJson.filter.Add("where", f3);
                            #endregion
                        }

                        #region 设置自定义参数
                        if (!string.IsNullOrWhiteSpace(message.ExKey1))
                        {
                            postJson.payload.Add(message.ExKey1, message.ExValue1);
                        }
                        if (!string.IsNullOrWhiteSpace(message.ExKey2))
                        {
                            postJson.payload.Add(message.ExKey2, message.ExValue2);
                        }
                        if (!string.IsNullOrWhiteSpace(message.ExKey3))
                        {
                            postJson.payload.Add(message.ExKey3, message.ExValue3);
                        }

                        #endregion

                        if (message.ExpiredTimd.HasValue)
                        // 设置过期时间
                        {
                            postJson.policy = new Policy();
                            postJson.policy.expire_time = message.ExpiredTimd.Value.ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        if (postJson.policy == null)
                        {
                            postJson.policy = new Policy() { max_send_num = MaxSendNum };
                        }
                        else
                        {
                            postJson.policy.max_send_num = MaxSendNum;
                        }
                        ReturnJsonClass result = umIosPush.SendMessage(postJson);
                        message.ActualSendTime = DateTime.Now;
                        message.TuhuId = postJson.thirdparty_id;

                        #region 处理推送结果
                        if (!string.IsNullOrEmpty(result?.ret)
                            && result.ret.Equals("SUCCESS", StringComparison.InvariantCultureIgnoreCase))
                        {
                            message.Status = "Success";
                            message.UMTaskId = result.data?.task_id;
                        }
                        else
                        {
                            message.Status = "Fail";
                            message.Note = SimpleJson.SerializeObject(result);
                            Logger.Error("PushUMApi=>" + message.Id + ":" + message.Note);
                        }
                        DalPushMessage.UpdateTaskMessageModel(message);
                        #endregion
                    }
                }
            }
        }

        /// <summary>
        /// 查询任务类型消息的发送状态
        /// </summary>
        /// <param name="taskId"></param>
        internal static void QueryTaskStatus(string taskId)
        {
            UMengMessagePush umIosPush = new UMengMessagePush(iosAppKey, iosAppSecret, false);
            var query = new TaskQueryModel
            {
                appkey = iosAppKey,
                task_id = taskId
            };
            var result = umIosPush.QueryTaskStatus(query);

            if (!string.IsNullOrEmpty(result?.ret)
                            && result.ret.Equals("SUCCESS", StringComparison.InvariantCultureIgnoreCase))
            {
                var data = result.data;
            }
            else
            {
                TaskQueryResult errResult = new TaskQueryResult
                {
                    ret = "Fail",
                    data = new StatusInfo { error_code = result?.data?.error_code }
                };
            }
        }
        internal static  bool CheckIsOpenByNameFromCache(string name)
        {
            var result = TuhuMemoryCache.Instance.GetOrSet($"runtimeswitch/{name}", () => DAL.DalTemplatePush.CheckIsOpenByName(name), TimeSpan.FromMinutes(1));
            return result;
        }
    }
}
