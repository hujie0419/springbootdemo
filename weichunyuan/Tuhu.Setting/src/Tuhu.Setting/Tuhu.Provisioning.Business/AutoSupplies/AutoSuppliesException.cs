using System;
using System.Runtime.Serialization;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business.AutoSuppliesManagement
{
    public class AutoSuppliesException : TuhuBizException
    {

        public AutoSuppliesException()
        { }

        public AutoSuppliesException(int errorCode, string message)
            : base(errorCode, message)
        { }

        public AutoSuppliesException(int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        { }

        protected AutoSuppliesException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

    }
}
