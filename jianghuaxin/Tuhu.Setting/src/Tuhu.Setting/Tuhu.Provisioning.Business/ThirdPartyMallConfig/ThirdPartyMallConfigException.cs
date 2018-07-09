using System;
using System.Runtime.Serialization;
using Tuhu.Component.Framework;


namespace Tuhu.Provisioning.Business.ThirdPartyMallConfig
{
    public class ThirdPartyMallConfigException : TuhuBizException
    {

        public ThirdPartyMallConfigException()
        { }

        public ThirdPartyMallConfigException(int errorCode, string message)
            : base(errorCode, message)
        { }

        public ThirdPartyMallConfigException(int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        { }

        protected ThirdPartyMallConfigException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

    }
}
