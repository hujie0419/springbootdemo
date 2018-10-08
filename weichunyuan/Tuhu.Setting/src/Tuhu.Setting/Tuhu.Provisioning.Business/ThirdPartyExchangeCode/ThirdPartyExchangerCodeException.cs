using System;
using System.Runtime.Serialization;
using Tuhu.Component.Framework;


namespace Tuhu.Provisioning.Business.ThirdPartyExchangeCode
{
    public class ThirdPartyExchangerCodeException : TuhuBizException
    {

        public ThirdPartyExchangerCodeException()
        { }

        public ThirdPartyExchangerCodeException(int errorCode, string message)
            : base(errorCode, message)
        { }

        public ThirdPartyExchangerCodeException(int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        { }

        protected ThirdPartyExchangerCodeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

    }
}