using System;
using System.Runtime.Serialization;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business.EmployeeManagement
{
    /// <summary>
    /// Base exception for employee module.
    /// </summary>
    public class EmployeeException : TuhuBizException
    {
        #region Ctor

        public EmployeeException()
        { }

        public EmployeeException(int errorCode, string message)
            : base(errorCode, message)
        { }

        public EmployeeException(int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        { }

        protected EmployeeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        #endregion
    }
}
