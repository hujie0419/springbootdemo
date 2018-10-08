using System;
using System.Runtime.Serialization;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business
{
    public class PointsRulesConfigException : TuhuBizException
    {
        public PointsRulesConfigException()
        { }

        public PointsRulesConfigException(int errorCode, string message)
            : base(errorCode, message)
        { }

        public PointsRulesConfigException(int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        { }

        protected PointsRulesConfigException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

    }
}
