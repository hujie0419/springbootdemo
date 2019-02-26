using System;
using System.Runtime.Serialization;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business
{
    public class OrderAreaProductException : TuhuBizException
    {
        public OrderAreaProductException()
        { }
        public OrderAreaProductException(int errorCode, string message)
            : base(errorCode, message)
        { }
        public OrderAreaProductException(int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        { }
        public OrderAreaProductException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
