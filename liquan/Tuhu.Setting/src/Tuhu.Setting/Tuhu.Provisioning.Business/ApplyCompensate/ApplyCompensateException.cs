using System;
using System.Runtime.Serialization;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business
{
    public class ApplyCompensateException : TuhuBizException
    {

        public ApplyCompensateException()
        { }

        public ApplyCompensateException(int errorCode, string message)
            : base(errorCode, message)
        { }

        public ApplyCompensateException(int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        { }

        protected ApplyCompensateException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

    }
}
