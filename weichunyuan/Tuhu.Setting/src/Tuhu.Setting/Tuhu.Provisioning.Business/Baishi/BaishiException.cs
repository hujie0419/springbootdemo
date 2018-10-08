using System;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business.Baishi
{
    public class BaishiException : TuhuBizException
    {
        #region Ctor

        public BaishiException()
        { }

        public BaishiException(int errorCode, string message)
            : base(errorCode, message)
        { }

        public BaishiException(int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        { }

        #endregion
    }
}
