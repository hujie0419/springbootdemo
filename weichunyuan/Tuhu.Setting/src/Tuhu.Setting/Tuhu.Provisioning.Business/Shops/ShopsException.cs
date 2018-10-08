using Tuhu.Component.Framework;
using System.Runtime.Serialization;
using System;

namespace Tuhu.Provisioning.Business.ShopsManagement
{
    public class ShopsException: TuhuBizException
    {
        #region Ctor

        public ShopsException()
        { }

        public ShopsException(int errorCode, string message)
            : base(errorCode, message)
        { }

        public ShopsException(int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        { }

        protected ShopsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        #endregion
    }
}
