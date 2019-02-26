using System;
using System.Runtime.Serialization;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business
{
    public class VIPAuthorizationExchangeCodeConfigException : TuhuBizException
    {

        public VIPAuthorizationExchangeCodeConfigException()
        { }

        public VIPAuthorizationExchangeCodeConfigException(int errorCode, string message)
            : base(errorCode, message)
        { }

        public VIPAuthorizationExchangeCodeConfigException(int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        { }

        protected VIPAuthorizationExchangeCodeConfigException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

    }
}
