using System;
using Tuhu.Models;

namespace Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.Model
{
    public class MessageModel
    {
        [Column("PKID")]
        public int Id { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public string Description { get; set; }

        public int MessageType { get; set; }

        public DateTime? BeginSendTime { get; set; }

        public DateTime? ExpiredTimd { get; set; }

        public string PhoneNumber { get; set; }

        public string Status { get; set; }

        public DateTime? ActualSendTime { get; set; }

        public string Channel { get; set; }

        public string Note { get; set; }

        [Column("Device_Tokens")]
        public string DeviceToken { get; set; }

        public string TuhuId { get; set; }

        public string UMMessageId { get; set; }

        public string UMTaskId { get; set; }

        public string AfterOpen { get; set; }

        public string AppActivity { get; set; }

        public string ExKey1 { get; set; }
        public string ExKey2 { get; set; }
        public string ExValue1 { get; set; }
        public string ExValue2 { get; set; }

        public string ExKey3 { get; set; }
        public string ExValue3 { get; set; }

        /// <summary>
        /// 系统通知
        /// </summary>
        public bool Notice { get; set; }

        /// <summary>
        /// 物流通知
        /// </summary>
        public bool Logistics { get; set; }

        /// <summary>
        /// 优惠活动（广播）通知
        /// </summary>
        public bool Radio { get; set; }

        /// <summary>
        /// 发现社区通知（私信）
        /// </summary>
        public bool Privateletter { get; set; }

        /// <summary>
        /// 推送开关
        /// </summary>
        public bool TimeSet { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        public string CenterMsgType { get; set; }

        /// <summary>
        /// 是否设置角标
        /// </summary>
        public bool? IOSShowBadge { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// 任务类型
        /// AppBroadcast： 广播
        /// AppGroupcast： 组播
        /// </summary>
        public string TaskType { get; set; }

        /// <summary>
        /// IOS10 主标题
        /// </summary>
        public string IOSMainTitle { get; set; }

        /// <summary>
        /// IOS10 副标题
        /// </summary>
        public string IOSTitle { get; set; }
        /// <summary>
        /// 富文本图片
        /// </summary>
        public string RichTextImage { get; set; }
    }

    public class ZeroActivityInfoModel{
        /// <summary>
        /// 期数
        /// </summary>
        public int Period { get; set; }
        /// <summary>
        /// 活动开始时间
        /// </summary>
        [Column("StartDateTime")]
        public DateTime ActivityStartTime { get; set; }
        /// <summary>
        /// PID
        /// </summary>
        public string PID { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        [Column("DisplayName")]
        public string ProductDisplayName { get; set; }
    }
}
