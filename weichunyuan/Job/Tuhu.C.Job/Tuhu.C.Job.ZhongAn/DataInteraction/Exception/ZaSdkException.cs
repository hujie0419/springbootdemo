using System;
using System.Runtime.Serialization;

namespace Zhongan.DI.ZhonganSdkException
{
    /// <summary>
    /// 客户端异常。
    /// </summary>
    public class ZaSdkException : Exception
    {
        private string errorCode;
        private string errorMsg;

        public ZaSdkException()
            : base()
        {
        }

        public ZaSdkException(string message)
            : base(message)
        {
        }

        protected ZaSdkException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public ZaSdkException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public ZaSdkException(string errorCode, string errorMsg)
            : base(errorCode + ":" + errorMsg)
        {
            this.errorCode = errorCode;
            this.errorMsg = errorMsg;
        }

        public string ErrorCode
        {
            get { return this.errorCode; }
        }

        public string ErrorMsg
        {
            get { return this.errorMsg; }
        }
    }
}
