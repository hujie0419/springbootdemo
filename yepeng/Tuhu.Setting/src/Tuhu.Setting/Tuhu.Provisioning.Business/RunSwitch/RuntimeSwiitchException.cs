using System;
using System.Runtime.Serialization;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business.RunSwitch
{
    public class RuntimeSwitchException : TuhuBizException
    {
        public RuntimeSwitchException()
        { }
        public RuntimeSwitchException(int errorCode, string message)
            : base(errorCode, message)
        { }
        public RuntimeSwitchException(int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        { }
        public RuntimeSwitchException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
