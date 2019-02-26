using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business.Activity
{
    public class ActivityException : TuhuBizException
    {
             public ActivityException()
        { }
        public ActivityException(int errorCode, string message)
            : base(errorCode, message)
        { }
        public ActivityException(int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        { }
        public ActivityException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
