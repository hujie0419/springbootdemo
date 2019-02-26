using System;
using System.Runtime.Serialization;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business
{
    public class BusinessKeywordsConfigException : TuhuBizException
    {

        public BusinessKeywordsConfigException()
        { }

        public BusinessKeywordsConfigException(int errorCode, string message)
            : base(errorCode, message)
        { }

        public BusinessKeywordsConfigException(int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        { }

        protected BusinessKeywordsConfigException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

    }
}
