using System;
using System.Runtime.Serialization;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business
{
    public class ShareActivityRulesConfigException : TuhuBizException
    {

        public ShareActivityRulesConfigException()
        { }

        public ShareActivityRulesConfigException(int errorCode, string message)
            : base(errorCode, message)
        { }

        public ShareActivityRulesConfigException(int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        { }

        protected ShareActivityRulesConfigException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

    }
}
