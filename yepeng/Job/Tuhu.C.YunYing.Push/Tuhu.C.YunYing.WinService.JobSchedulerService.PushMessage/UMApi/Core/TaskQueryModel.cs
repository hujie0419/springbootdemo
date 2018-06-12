namespace Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.UMApi.Core
{
    public class TaskQueryModel
    {
        /// <summary>
        /// 必填 应用唯一标识
        /// </summary>
        public string appkey { get; set; }
        /// <summary>
        /// 注意：该值由UMengMessagePush自动生成，无需主动赋值
        /// 
        /// 必填 时间戳，10位或者13位均可，时间戳有效期为10分钟 
        /// </summary>
        public string timestamp { get; set; }
        /// <summary>
        /// 必填 消息发送时，从返回消息中获取的task_id
        /// </summary>
        public string task_id { get; set; }
    }
}
