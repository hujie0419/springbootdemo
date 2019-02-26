using System;
using System.Runtime.Serialization;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business
{
    public class OrderAreaException : TuhuBizException
    {
        public OrderAreaException()
        { }
        public OrderAreaException(int errorCode, string message)
            : base(errorCode, message)
        { }
        public OrderAreaException(int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        { }
        public OrderAreaException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
