using System;
using System.Runtime.Serialization;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business
{
    public class BaoYangPackageImageConfigException : TuhuBizException
    {

        public BaoYangPackageImageConfigException()
        { }

        public BaoYangPackageImageConfigException(int errorCode, string message)
            : base(errorCode, message)
        { }

        public BaoYangPackageImageConfigException(int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        { }

        protected BaoYangPackageImageConfigException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

    }
}
