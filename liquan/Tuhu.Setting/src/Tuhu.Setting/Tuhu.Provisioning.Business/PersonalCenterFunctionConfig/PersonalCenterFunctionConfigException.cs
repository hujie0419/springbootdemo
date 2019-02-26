using System;
using System.Runtime.Serialization;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business
{
    public class PersonalCenterFunctionConfigException : TuhuBizException
    {

        public PersonalCenterFunctionConfigException()
        { }

        public PersonalCenterFunctionConfigException(int errorCode, string message)
            : base(errorCode, message)
        { }

        public PersonalCenterFunctionConfigException(int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        { }

        protected PersonalCenterFunctionConfigException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

    }
}
