using System;
using System.Runtime.Serialization;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business
{
    public class PointsCenterConfigException : TuhuBizException
    {    

        public PointsCenterConfigException()
        { }

        public PointsCenterConfigException(int errorCode, string message)
            : base(errorCode, message)
        { }

        public PointsCenterConfigException(int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        { }

        protected PointsCenterConfigException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

    }
}
