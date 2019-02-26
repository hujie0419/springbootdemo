using System;
using System.Runtime.Serialization;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business
{
    public class BaoYangActivitySettingException : TuhuBizException
    {    

        public BaoYangActivitySettingException()
        { }

        public BaoYangActivitySettingException(int errorCode, string message)
            : base(errorCode, message)
        { }

        public BaoYangActivitySettingException(int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        { }

        protected BaoYangActivitySettingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

    }
}
