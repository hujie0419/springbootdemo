using System;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business.Monitor
{
    public class MonitorException : TuhuBizException
    {
        #region Ctor

        public MonitorException()
        { }

        public MonitorException(int errorCode, string message)
            : base(errorCode, message)
        { }

        public MonitorException(int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        { }

        #endregion
    }
}
