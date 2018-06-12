namespace Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.UMApi.Core
{
    public class TaskQueryResult
    {
        /// <summary>
        /// 返回结果，"SUCCESS"或者"FAIL"
        /// </summary>
        public string ret { get; set; }
        /// <summary>
        /// 结果具体信息
        /// </summary>
        public StatusInfo data { get; set; }
    }

    public class StatusInfo
    {
        #region 当"ret"为"SUCCESS"时,包含如下参数:
        /// <summary>
        /// 任务Id
        /// </summary>
        public string task_id { get; set; }
        /// <summary>
        /// 消息状态: 0-排队中, 1-发送中，2-发送完成，3-发送失败，4-消息被撤销，
        /// 5-消息过期, 6-筛选结果为空，7-定时任务尚未开始处理
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 消息总数
        /// </summary>
        public string total_count { get; set; }
        /// <summary>
        /// 消息受理数
        /// </summary>
        public string accept_count { get; set; }
        /// <summary>
        /// 消息实际发送数
        /// </summary>
        public string sent_count { get; set; }
        /// <summary>
        /// 打开数
        /// </summary>
        public string open_count { get; set; }
        /// <summary>
        /// 忽略数
        /// </summary>
        public string dismiss_count { get; set; }

        #endregion

        #region 当"ret"为"FAIL"时，包含参数如下:

        /// <summary>
        /// 错误码
        /// </summary>
        public string error_code { get; set; }

        #endregion
    }
}
