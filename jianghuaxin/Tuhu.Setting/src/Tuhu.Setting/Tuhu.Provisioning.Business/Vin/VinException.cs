using System;
using System.Runtime.Serialization;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business.Vin
{
    public class VinException : TuhuBizException
    {
        public VinException()
        { }
        public VinException(int errorCode, string message)
            : base(errorCode, message)
        { }
        public VinException(int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        { }
        public VinException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
