using System;
using System.Runtime.Serialization;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business
{
    public class VIPAuthorizationRuleConfigException : TuhuBizException
    {

        public VIPAuthorizationRuleConfigException()
        { }

        public VIPAuthorizationRuleConfigException(int errorCode, string message)
            : base(errorCode, message)
        { }

        public VIPAuthorizationRuleConfigException(int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        { }

        protected VIPAuthorizationRuleConfigException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

    }
}
