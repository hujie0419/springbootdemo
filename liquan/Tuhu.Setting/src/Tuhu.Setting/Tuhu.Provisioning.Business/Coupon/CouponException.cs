using System;
using System.Runtime.Serialization;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business.CouponManagement
{
    public class CouponException : TuhuBizException
    {
        public CouponException()
        { }

        public CouponException(int errorCode, string message)
            : base(errorCode, message)
        { }

        public CouponException(int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        { }

        protected CouponException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
