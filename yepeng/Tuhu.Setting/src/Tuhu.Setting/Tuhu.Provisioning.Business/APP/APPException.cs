using System;
using System.Runtime.Serialization;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business
{
    public class APPException : TuhuBizException
    {

        public APPException()
        { }

        public APPException(int errorCode, string message)
            : base(errorCode, message)
        { }

        public APPException(int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        { }

        protected APPException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

    }
}
