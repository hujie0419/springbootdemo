using System;
using System.Runtime.Serialization;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business
{
    public class CouponActivityConfigException : TuhuBizException
    {

        public CouponActivityConfigException()
        { }

        public CouponActivityConfigException(int errorCode, string message)
            : base(errorCode, message)
        { }

        public CouponActivityConfigException(int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        { }

        protected CouponActivityConfigException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

    }
}
