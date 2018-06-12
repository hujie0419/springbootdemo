using System;

namespace PddSdk.Response
{
    [Serializable]
    public abstract class PddResponse
    {
        /// <summary>
        /// 响应原始内容
        /// </summary>
        public virtual string Body { get; internal set; }

        /// <summary>
        /// 响应结果是否错误
        /// </summary>
        public bool IsError { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMsg { get; set; }

    }
}
