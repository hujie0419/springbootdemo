using System;
using System.Runtime.Serialization;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business.Idx
{   
    public class IdxException : TuhuBizException
    {
        public IdxException()
        { }
        public IdxException(int errorCode, string message)
            : base(errorCode, message)
        { }
        public IdxException(int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        { }
        public IdxException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
