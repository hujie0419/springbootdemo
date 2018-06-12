using System;
using System.Runtime.Serialization;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business
{
    public class AnnouncementConfigException : TuhuBizException
    {

        public AnnouncementConfigException()
        { }

        public AnnouncementConfigException(int errorCode, string message)
            : base(errorCode, message)
        { }

        public AnnouncementConfigException(int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        { }

        protected AnnouncementConfigException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

    }
}
