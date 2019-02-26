using System;
using System.Runtime.Serialization;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business
{
    public class BaoYangPackageBrandPriorityConfigException : TuhuBizException
    {

        public BaoYangPackageBrandPriorityConfigException()
        { }

        public BaoYangPackageBrandPriorityConfigException(int errorCode, string message)
            : base(errorCode, message)
        { }

        public BaoYangPackageBrandPriorityConfigException(int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        { }

        protected BaoYangPackageBrandPriorityConfigException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

    }
}
