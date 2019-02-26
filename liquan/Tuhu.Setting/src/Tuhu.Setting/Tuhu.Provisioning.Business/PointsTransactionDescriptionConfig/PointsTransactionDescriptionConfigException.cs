using System;
using System.Runtime.Serialization;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business
{
    public class PointsTransactionDescriptionConfigException : TuhuBizException
    {

        public PointsTransactionDescriptionConfigException()
        { }

        public PointsTransactionDescriptionConfigException(int errorCode, string message)
            : base(errorCode, message)
        { }

        public PointsTransactionDescriptionConfigException(int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        { }

        protected PointsTransactionDescriptionConfigException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

    }
}
