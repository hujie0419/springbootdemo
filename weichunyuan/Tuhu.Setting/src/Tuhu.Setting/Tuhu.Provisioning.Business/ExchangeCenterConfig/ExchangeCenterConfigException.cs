using System;
using System.Runtime.Serialization;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business
{
    public class ExchangeCenterConfigException : TuhuBizException
    {    

        public ExchangeCenterConfigException()
        { }

        public ExchangeCenterConfigException(int errorCode, string message)
            : base(errorCode, message)
        { }

        public ExchangeCenterConfigException(int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        { }

        protected ExchangeCenterConfigException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

    }
}
