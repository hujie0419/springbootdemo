using System;
using System.Runtime.Serialization;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business.OrderTrackingLog
{
    public class OrderTrackingException : TuhuBizException
    {
        #region Ctor

        public OrderTrackingException()
        { }

        public OrderTrackingException(int errorCode, string message)
            : base(errorCode, message)
        { }

        public OrderTrackingException(int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        { }

        protected OrderTrackingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        #endregion
    }
}
