using System;
using System.Runtime.Serialization;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business
{
    public class DefaultSearchConfigException : TuhuBizException
    {

        public DefaultSearchConfigException()
        { }

        public DefaultSearchConfigException(int errorCode, string message)
            : base(errorCode, message)
        { }

        public DefaultSearchConfigException(int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        { }

        protected DefaultSearchConfigException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

    }
}
