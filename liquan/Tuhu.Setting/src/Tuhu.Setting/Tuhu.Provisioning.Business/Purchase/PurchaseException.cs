using System;

using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business.PurchaseManagement
{
    public class PurchaseException : TuhuBizException
    {
        public PurchaseException(int errorCode, string message, Exception innException)
            : base(errorCode, message, innException)
        { }

        public PurchaseException(int errorCode, string message)
            : base(errorCode, message)
        { }
    }
}
