using System;
using System.Runtime.Serialization;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business
{
    public class FlashSaleProductsExtendException : TuhuBizException
    {    

        public FlashSaleProductsExtendException()
        { }

        public FlashSaleProductsExtendException(int errorCode, string message)
            : base(errorCode, message)
        { }

        public FlashSaleProductsExtendException(int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        { }

        protected FlashSaleProductsExtendException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

    }
}
