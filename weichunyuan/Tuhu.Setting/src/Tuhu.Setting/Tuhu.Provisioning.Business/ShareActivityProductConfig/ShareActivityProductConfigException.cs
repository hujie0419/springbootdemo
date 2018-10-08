using System;
using System.Runtime.Serialization;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business
{
    public class ShareActivityProductConfigException : TuhuBizException
    {

        public ShareActivityProductConfigException()
        { }

        public ShareActivityProductConfigException(int errorCode, string message)
            : base(errorCode, message)
        { }

        public ShareActivityProductConfigException(int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        { }

        protected ShareActivityProductConfigException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

    }
}
