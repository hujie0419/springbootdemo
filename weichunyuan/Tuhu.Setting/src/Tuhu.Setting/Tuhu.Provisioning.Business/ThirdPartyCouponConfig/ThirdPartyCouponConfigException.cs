using System;
using System.Runtime.Serialization;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business.ThirdPartyCouponConfig
{
    public class ThirdPartyCouponConfigException : TuhuBizException
    {
        public ThirdPartyCouponConfigException()
        {

        }
        public ThirdPartyCouponConfigException(int errorCode, string message)
            : base(errorCode, message)
        { }

        public ThirdPartyCouponConfigException(int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        { }

        protected ThirdPartyCouponConfigException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
