using System;
using System.Runtime.Serialization;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business
{
    public class RulesDictionaryConfigException : TuhuBizException
    {

        public RulesDictionaryConfigException()
        { }

        public RulesDictionaryConfigException(int errorCode, string message)
            : base(errorCode, message)
        { }

        public RulesDictionaryConfigException(int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        { }

        protected RulesDictionaryConfigException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

    }
}
