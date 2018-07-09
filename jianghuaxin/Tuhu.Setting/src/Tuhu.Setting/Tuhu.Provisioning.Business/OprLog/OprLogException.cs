using System;
using System.Runtime.Serialization;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business.OprLogManagement
{
    public class OprLogException : TuhuBizException
    {
        #region Ctor

        public OprLogException()
        { }

        public OprLogException(int errorCode, string message)
            : base(errorCode, message)
        { }

        public OprLogException(int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        { }

        protected OprLogException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        #endregion
    }
}
