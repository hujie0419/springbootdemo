using System;
using System.Runtime.Serialization;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business.Payment
{
    public class PaymentException : TuhuBizException
    {
           public PaymentException()
        { }
        public PaymentException(int errorCode, string message)
			: base(errorCode, message)
        { }
        public PaymentException(int errorCode, string message, Exception innerException)
			: base(errorCode, message, innerException)
        { }
        public PaymentException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
