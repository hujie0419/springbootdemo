using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business.Sale
{
    public class SaleException : TuhuBizException
    {
           public SaleException()
        { }
        public SaleException(int errorCode, string message)
			: base(errorCode, message)
        { }
        public SaleException(int errorCode, string message, Exception innerException)
			: base(errorCode, message, innerException)
        { }
        public SaleException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
