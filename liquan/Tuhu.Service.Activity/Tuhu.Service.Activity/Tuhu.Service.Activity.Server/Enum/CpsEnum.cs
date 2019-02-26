using System.ComponentModel;

namespace Tuhu.Service.Activity.Server.Enum
{
    public static class CpsEnum
    {
        /// <summary>
        /// CPS流水状态
        /// </summary>
        public enum CpsRunningStatus
        {
            [Description("未发送")]
            NOTSENT,
            [Description("请求发送失败")]
            SENDFAILURE,
            [Description("处理中")]
            PROCESSING,
            [Description("成功")]
            SUCCESS,
            [Description("失败")]
            FAIL,
            [Description("回调执行成功")]
            CALLBACKSUCCESSFUL,
            [Description("回调执行失败")]
            CALLBACKFAILED

        }

        /// <summary>
        /// CPS流水类型
        /// </summary>
        public enum CpsRunningType
        {
            [Description("增值")]
            VALUEADDED,
            [Description("减值")]
            IMPAIRMENT
        }


        /// <summary>
        /// 金融退款类型
        /// </summary>
        public enum CpsRefundType
        {
            [Description("部分退款")]
            PART,
            [Description("全部退款")]
            NORMAL
        }

        /// <summary>
        /// 退款路径
        /// </summary>
        public enum CpsRefundRoute
        {
            [Description("原路退")]
            ORIGINAL,
            [Description("按指定资金源退款")]
            APPOINT
        }
    }
}
