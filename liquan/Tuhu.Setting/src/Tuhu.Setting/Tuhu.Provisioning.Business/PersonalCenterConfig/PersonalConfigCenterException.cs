using System;
using System.Runtime.Serialization;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business
{
    public class PersonalCenterConfigException : TuhuBizException
    {    

        public PersonalCenterConfigException()
        { }

        public PersonalCenterConfigException(int errorCode, string message)
            : base(errorCode, message)
        { }

        public PersonalCenterConfigException(int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        { }

        protected PersonalCenterConfigException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

    }
}
