using System;
using System.Runtime.Serialization;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business.LogisticManagement
{
    public class LogisticException : TuhuBizException
    {
        #region Ctor

        public LogisticException()
        { }

        public LogisticException(int errorCode, string message)
            : base(errorCode, message)
        { }

        public LogisticException(int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        { }

        public LogisticException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        #endregion
    }
}
