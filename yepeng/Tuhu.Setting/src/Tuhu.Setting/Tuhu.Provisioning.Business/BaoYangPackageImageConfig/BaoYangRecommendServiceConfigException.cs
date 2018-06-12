using System;
using System.Runtime.Serialization;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business
{
    public class BaoYangRecommendServiceConfigException : TuhuBizException
    {

        public BaoYangRecommendServiceConfigException()
        { }

        public BaoYangRecommendServiceConfigException(int errorCode, string message)
            : base(errorCode, message)
        { }

        public BaoYangRecommendServiceConfigException(int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        { }

        protected BaoYangRecommendServiceConfigException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

    }
}
