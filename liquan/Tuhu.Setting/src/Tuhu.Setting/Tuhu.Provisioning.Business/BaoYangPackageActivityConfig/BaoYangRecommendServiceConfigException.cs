using System;
using System.Runtime.Serialization;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business
{
    public class BaoYangPackageActivityConfigException : TuhuBizException
    {

        public BaoYangPackageActivityConfigException()
        { }

        public BaoYangPackageActivityConfigException(int errorCode, string message)
            : base(errorCode, message)
        { }

        public BaoYangPackageActivityConfigException(int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        { }

        protected BaoYangPackageActivityConfigException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

    }
}
